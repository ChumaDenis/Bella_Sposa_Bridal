import {
  Component, ChangeDetectionStrategy, OnInit, OnDestroy,
  inject, signal, HostListener, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { Subscription, forkJoin } from 'rxjs';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { DressCardComponent } from '../catalog/components/dress-card/dress-card';
import { DressService } from '../core/services/dress.service';
import { AtlierService } from '../core/services/atlier.service';
import { ViewedDressesService } from '../core/services/viewed-dresses.service';
import { LikedDressesService } from '../core/services/liked-dresses.service';
import { DressDetailDto, SILHOUETTE_LABELS } from '../core/models/dress.model';
import { AtlierInfoDto } from '../core/models/atlier.model';

interface MediaItem {
  kind: 'photo' | 'video';
  id: string;
  url: string;
  alt: string;
  thumbnailUrl?: string | null;
}

@Component({
  selector: 'app-dress-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, FooterComponent, DressCardComponent],
  templateUrl: './dress-detail.html',
  styleUrl: './dress-detail.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DressDetailComponent implements OnInit, OnDestroy {
  private route         = inject(ActivatedRoute);
  private dressService  = inject(DressService);
  private atlierService = inject(AtlierService);
  private viewedSvc     = inject(ViewedDressesService);
  private likedSvc      = inject(LikedDressesService);
  private router        = inject(Router);
  private cdr           = inject(ChangeDetectorRef);

  dress            = signal<DressDetailDto | null>(null);
  atlier           = signal<AtlierInfoDto | null>(null);
  loading          = signal(true);
  error            = signal(false);
  activeMediaIndex = signal(0);
  lightboxOpen     = signal(false);
  detailsOpen      = signal(false);
  liked            = signal(false);

  private observer!: IntersectionObserver;
  private routeSub!: Subscription;

  ngOnInit() {
    this.routeSub = this.route.paramMap.subscribe(params => {
      this.load(params.get('slug')!);
    });
  }

  ngOnDestroy() {
    this.observer?.disconnect();
    this.routeSub?.unsubscribe();
    document.body.style.overflow = '';
  }

  private load(slug: string) {
    this.loading.set(true);
    this.error.set(false);
    this.activeMediaIndex.set(0);
    this.lightboxOpen.set(false);
    this.detailsOpen.set(false);

    forkJoin({
      dress:  this.dressService.getBySlug(slug),
      atlier: this.atlierService.getInfo()
    }).subscribe({
      next: ({ dress, atlier }) => {
        if (dress) {
          dress.sizes = dress.sizes.slice().sort(
            (a, b) => (parseInt(a.replace(/\D/g, ''), 10) || 0) - (parseInt(b.replace(/\D/g, ''), 10) || 0)
          );
          this.liked.set(this.likedSvc.isLiked(dress.id));
          this.viewedSvc.add(dress.id);
        }
        this.dress.set(dress);
        this.atlier.set(atlier);
        this.loading.set(false);
        window.scrollTo({ top: 0 });
        setTimeout(() => this.initReveal(), 120);
        this.cdr.markForCheck();
      },
      error: () => {
        this.loading.set(false);
        this.error.set(true);
        this.cdr.markForCheck();
      }
    });
  }

  private initReveal() {
    this.observer?.disconnect();
    this.observer = new IntersectionObserver(entries => {
      entries.forEach(e => {
        if (e.isIntersecting) {
          e.target.classList.add('visible');
          this.observer.unobserve(e.target);
        }
      });
    }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });
    document.querySelectorAll('.reveal').forEach(el => this.observer.observe(el));
  }

  toggleLike() {
    const d = this.dress();
    if (!d) return;
    this.liked.set(this.likedSvc.toggle(d.id));
  }

  silhouetteLabel(n: number): string {
    return SILHOUETTE_LABELS[n] ?? 'Classic';
  }

  // ── Unified media (photos first, then videos) ────────────────────
  get mediaItems(): MediaItem[] {
    const d = this.dress();
    if (!d) return [];
    return [
      ...d.photos.map(p => ({ kind: 'photo' as const, id: p.id, url: p.url, alt: p.altText ?? d.name })),
      ...d.videos.map(v => ({ kind: 'video' as const, id: v.id, url: v.url, alt: d.name, thumbnailUrl: v.thumbnailUrl }))
    ];
  }

  get activeMedia(): MediaItem | null {
    return this.mediaItems[this.activeMediaIndex()] ?? null;
  }

  setMedia(index: number) { this.activeMediaIndex.set(index); }

  // ── Lightbox (photos only) ────────────────────────────────────────
  openLightbox() {
    if (!this.mediaItems.length) return;
    this.lightboxOpen.set(true);
    document.body.style.overflow = 'hidden';
  }

  openLightboxAt(index: number) {
    if (index < 0 || index >= this.mediaItems.length) return;
    this.activeMediaIndex.set(index);
    this.lightboxOpen.set(true);
    document.body.style.overflow = 'hidden';
  }

  closeLightbox() {
    this.lightboxOpen.set(false);
    document.body.style.overflow = '';
  }

  prevPhoto() {
    const len = this.mediaItems.length;
    if (!len) return;
    const i = this.activeMediaIndex();
    this.activeMediaIndex.set(i > 0 ? i - 1 : len - 1);
  }

  nextPhoto() {
    const len = this.mediaItems.length;
    if (!len) return;
    const i = this.activeMediaIndex();
    this.activeMediaIndex.set(i < len - 1 ? i + 1 : 0);
  }

  get lightboxPhotoNumber(): number {
    return this.activeMediaIndex() + 1;
  }

  @HostListener('document:keydown', ['$event'])
  onKey(e: KeyboardEvent) {
    if (!this.lightboxOpen()) return;
    if (e.key === 'Escape')     this.closeLightbox();
    if (e.key === 'ArrowLeft')  this.prevPhoto();
    if (e.key === 'ArrowRight') this.nextPhoto();
  }

  // ── Navigation ───────────────────────────────────────────────────
  goToAppointment() { this.router.navigate(['/appointment']); }
  goToCatalog()     { this.router.navigate(['/catalog']); }
}
