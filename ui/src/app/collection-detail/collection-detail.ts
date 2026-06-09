import {
  Component, ChangeDetectionStrategy, OnInit, inject,
  signal, computed, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { DressCardComponent } from '../catalog/components/dress-card/dress-card';
import { CollectionService } from '../core/services/collection.service';
import { DressService } from '../core/services/dress.service';
import { CollectionDto } from '../core/models/collection.model';
import { DressListDto } from '../core/models/dress.model';

@Component({
  selector: 'app-collection-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, FooterComponent, DressCardComponent],
  templateUrl: './collection-detail.html',
  styleUrl: './collection-detail.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CollectionDetailComponent implements OnInit {
  private route         = inject(ActivatedRoute);
  private collectionSvc = inject(CollectionService);
  private dressService  = inject(DressService);
  private router        = inject(Router);
  private cdr           = inject(ChangeDetectorRef);

  collection     = signal<CollectionDto | null>(null);
  allDresses     = signal<DressListDto[]>([]);
  loading        = signal(true);
  dressesLoading = signal(false);
  notFound       = signal(false);

  silhouetteFilter = signal<string>('all');
  colorFilter      = signal<string>('all');
  filtersOpen      = signal(false);

  filteredDresses = computed(() => {
    let list = this.allDresses();
    const sf = this.silhouetteFilter();
    const cf = this.colorFilter();
    if (sf !== 'all') list = list.filter(d => d.silhouetteName === sf);
    if (cf !== 'all') list = list.filter(d => d.color === cf);
    return list;
  });

  get availableSilhouettes(): string[] {
    const seen = new Set<string>();
    for (const d of this.allDresses()) { if (d.silhouetteName) seen.add(d.silhouetteName); }
    return [...seen].sort();
  }

  get availableColors(): string[] {
    const seen = new Set<string>();
    for (const d of this.allDresses()) { if (d.color) seen.add(d.color); }
    return [...seen].sort();
  }

  get hasActiveFilter(): boolean {
    return this.silhouetteFilter() !== 'all' || this.colorFilter() !== 'all';
  }

  ngOnInit() {
    const slug = this.route.snapshot.paramMap.get('slug');
    if (!slug) { this.router.navigate(['/collections']); return; }

    this.collectionSvc.getBySlug(slug).subscribe({
      next: col => {
        this.collection.set(col);
        this.loading.set(false);
        this.cdr.markForCheck();
        this.fetchDresses(col.id);
      },
      error: () => {
        this.loading.set(false);
        this.notFound.set(true);
        this.cdr.markForCheck();
      }
    });
  }

  private fetchDresses(collectionId: string) {
    this.dressesLoading.set(true);
    this.dressService.getByCollection(collectionId, 1, 200).subscribe({
      next: result => {
        this.allDresses.set(result.items);
        this.dressesLoading.set(false);
        this.cdr.markForCheck();
      },
      error: () => { this.dressesLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  setSilhouette(v: string) { this.silhouetteFilter.set(v); this.cdr.markForCheck(); }
  setColor(v: string)      { this.colorFilter.set(v);      this.cdr.markForCheck(); }
  clearFilters()           { this.silhouetteFilter.set('all'); this.colorFilter.set('all'); this.cdr.markForCheck(); }
  toggleFilters()          { this.filtersOpen.update(v => !v); this.cdr.markForCheck(); }

  goBack() { this.router.navigate(['/collections']); }
}
