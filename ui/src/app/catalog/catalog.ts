import {
  Component, ChangeDetectionStrategy, OnInit, OnDestroy, inject,
  signal, ChangeDetectorRef, AfterViewInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { DressCardComponent } from './components/dress-card/dress-card';
import { DressService } from '../core/services/dress.service';
import { CollectionService } from '../core/services/collection.service';
import { DressListDto, DressFilterMeta, SILHOUETTE_LABELS } from '../core/models/dress.model';
import { CollectionDto } from '../core/models/collection.model';

const ukNum = (s: string) => parseInt(s.replace(/\D/g, ''), 10) || 0;

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CommonModule, NavbarComponent, FooterComponent, DressCardComponent],
  templateUrl: './catalog.html',
  styleUrl: './catalog.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CatalogComponent implements OnInit, OnDestroy, AfterViewInit {
  private dressService   = inject(DressService);
  private collectionSvc  = inject(CollectionService);
  private cdr            = inject(ChangeDetectorRef);

  dresses          = signal<DressListDto[]>([]);
  filterMeta       = signal<DressFilterMeta | null>(null);
  collections      = signal<CollectionDto[]>([]);
  totalCount       = signal(0);
  hasMore          = signal(false);
  loadingMore      = signal(false);

  activeCollection = signal<string | null>(null);
  activeSilhouette = signal<number | null>(null);
  activeSize       = signal<string | null>(null);
  currentPage      = signal(1);
  loading          = signal(true);
  readonly pageSize = 12;

  readonly silhouetteLabels = SILHOUETTE_LABELS;

  private revealObserver!: IntersectionObserver;

  get silhouettes()    { return this.filterMeta()?.silhouettes ?? []; }
  get availableSizes() { return [...(this.filterMeta()?.sizes ?? [])].sort((a, b) => ukNum(a) - ukNum(b)); }

  private readonly onScroll = () => {
    if (document.documentElement.scrollHeight - window.scrollY - window.innerHeight < 400
        && this.hasMore() && !this.loadingMore()) {
      this.loadMore();
    }
  };

  ngOnInit() {
    forkJoin({
      meta: this.dressService.getMeta(),
      collections: this.collectionSvc.getAll()
    }).subscribe({
      next: ({ meta, collections }) => {
        this.filterMeta.set(meta);
        this.collections.set(collections);
        this.fetchDresses();
      },
      error: () => { this.loading.set(false); this.cdr.markForCheck(); }
    });
    window.addEventListener('scroll', this.onScroll, { passive: true });
  }

  ngOnDestroy() {
    window.removeEventListener('scroll', this.onScroll);
    this.revealObserver?.disconnect();
  }

  ngAfterViewInit() {
    setTimeout(() => this.initReveal(), 200);
  }

  private fetchDresses() {
    this.loading.set(true);
    this.currentPage.set(1);
    this.dresses.set([]);
    this.dressService.getAll({
      page: 1, pageSize: this.pageSize,
      collectionId: this.activeCollection() ?? undefined,
      silhouette: this.activeSilhouette() ?? undefined,
      size: this.activeSize() ?? undefined
    }).subscribe({
      next: result => {
        this.dresses.set(result.items);
        this.totalCount.set(result.totalCount);
        this.hasMore.set(1 < result.totalPages);
        this.loading.set(false);
        this.cdr.markForCheck();
        setTimeout(() => this.initReveal(), 100);
      },
      error: () => { this.loading.set(false); this.cdr.markForCheck(); }
    });
  }

  private loadMore() {
    const nextPage = this.currentPage() + 1;
    this.loadingMore.set(true);
    this.dressService.getAll({
      page: nextPage, pageSize: this.pageSize,
      collectionId: this.activeCollection() ?? undefined,
      silhouette: this.activeSilhouette() ?? undefined,
      size: this.activeSize() ?? undefined
    }).subscribe({
      next: result => {
        this.dresses.update(d => [...d, ...result.items]);
        this.currentPage.set(nextPage);
        this.hasMore.set(nextPage < result.totalPages);
        this.loadingMore.set(false);
        this.cdr.markForCheck();
        setTimeout(() => this.initReveal(), 100);
      },
      error: () => { this.loadingMore.set(false); this.cdr.markForCheck(); }
    });
  }

  initReveal() {
    if (this.revealObserver) this.revealObserver.disconnect();
    this.revealObserver = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('visible');
          this.revealObserver.unobserve(entry.target);
        }
      });
    }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });
    document.querySelectorAll('.reveal').forEach(el => this.revealObserver.observe(el));
  }

  selectCollection(id: string | null) {
    this.activeCollection.set(id);
    this.fetchDresses();
  }

  selectSilhouette(sil: number | null) {
    this.activeSilhouette.set(sil);
    this.fetchDresses();
  }

  selectSize(size: string | null) {
    this.activeSize.set(size);
    this.fetchDresses();
  }

  clearFilters() {
    this.activeCollection.set(null);
    this.activeSilhouette.set(null);
    this.activeSize.set(null);
    this.fetchDresses();
  }
}
