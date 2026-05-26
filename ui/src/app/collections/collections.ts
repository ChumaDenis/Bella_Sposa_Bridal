import {
  Component, ChangeDetectionStrategy, OnInit, OnDestroy, inject,
  signal, computed, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { CollectionService } from '../core/services/collection.service';
import { CollectionDto } from '../core/models/collection.model';

@Component({
  selector: 'app-collections',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, FooterComponent],
  templateUrl: './collections.html',
  styleUrl: './collections.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CollectionsComponent implements OnInit, OnDestroy {
  private collectionService = inject(CollectionService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  collections = signal<CollectionDto[]>([]);
  loading = signal(true);
  private visibleCount = signal(6);

  sortedCollections = computed(() =>
    [...this.collections()].sort(
      (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    )
  );

  get visibleCollections() { return this.sortedCollections().slice(0, this.visibleCount()); }
  get hasMore()            { return this.sortedCollections().length > this.visibleCount(); }

  private readonly onScroll = () => {
    if (!this.hasMore) return;
    if (document.documentElement.scrollHeight - window.scrollY - window.innerHeight < 400) {
      this.visibleCount.update(n => n + 6);
      this.cdr.markForCheck();
    }
  };

  ngOnInit() {
    this.collectionService.getAll().subscribe({
      next: cols => { this.collections.set(cols); this.loading.set(false); this.cdr.markForCheck(); },
      error: () => { this.loading.set(false); this.cdr.markForCheck(); }
    });
    window.addEventListener('scroll', this.onScroll, { passive: true });
  }

  ngOnDestroy() {
    window.removeEventListener('scroll', this.onScroll);
  }

  goToCollection(slug: string) {
    this.router.navigate(['/collections', slug]);
  }
}
