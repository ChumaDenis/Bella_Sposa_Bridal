import {
  Component, ChangeDetectionStrategy, OnInit, OnDestroy, inject,
  signal, ChangeDetectorRef
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
export class CollectionDetailComponent implements OnInit, OnDestroy {
  private route          = inject(ActivatedRoute);
  private collectionSvc  = inject(CollectionService);
  private dressService   = inject(DressService);
  private router         = inject(Router);
  private cdr            = inject(ChangeDetectorRef);

  collection     = signal<CollectionDto | null>(null);
  dresses        = signal<DressListDto[]>([]);
  loading        = signal(true);
  dressesLoading = signal(false);
  loadingMore    = signal(false);
  hasMore        = signal(false);
  notFound       = signal(false);
  currentPage    = signal(1);
  readonly pageSize = 12;

  private collectionId = '';

  private readonly onScroll = () => {
    if (document.documentElement.scrollHeight - window.scrollY - window.innerHeight < 400
        && this.hasMore() && !this.loadingMore()) {
      this.loadMore();
    }
  };

  ngOnInit() {
    const slug = this.route.snapshot.paramMap.get('slug');
    if (!slug) { this.router.navigate(['/collections']); return; }

    this.collectionSvc.getBySlug(slug).subscribe({
      next: col => {
        this.collectionId = col.id;
        this.collection.set(col);
        this.loading.set(false);
        this.cdr.markForCheck();
        this.fetchDresses();
      },
      error: () => {
        this.loading.set(false);
        this.notFound.set(true);
        this.cdr.markForCheck();
      }
    });

    window.addEventListener('scroll', this.onScroll, { passive: true });
  }

  ngOnDestroy() {
    window.removeEventListener('scroll', this.onScroll);
  }

  private fetchDresses() {
    this.dressesLoading.set(true);
    this.currentPage.set(1);
    this.dresses.set([]);
    this.dressService.getByCollection(this.collectionId, 1, this.pageSize).subscribe({
      next: result => {
        this.dresses.set(result.items);
        this.hasMore.set(1 < result.totalPages);
        this.dressesLoading.set(false);
        this.cdr.markForCheck();
      },
      error: () => { this.dressesLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  private loadMore() {
    const nextPage = this.currentPage() + 1;
    this.loadingMore.set(true);
    this.dressService.getByCollection(this.collectionId, nextPage, this.pageSize).subscribe({
      next: result => {
        this.dresses.update(d => [...d, ...result.items]);
        this.currentPage.set(nextPage);
        this.hasMore.set(nextPage < result.totalPages);
        this.loadingMore.set(false);
        this.cdr.markForCheck();
      },
      error: () => { this.loadingMore.set(false); this.cdr.markForCheck(); }
    });
  }

  goBack() {
    this.router.navigate(['/collections']);
  }
}
