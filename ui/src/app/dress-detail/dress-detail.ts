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
import { DressDetailDto, DressPhoto, SILHOUETTE_LABELS } from '../core/models/dress.model';
import { AtlierInfoDto } from '../core/models/atlier.model';

@Component({
  selector: 'app-dress-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, FooterComponent, DressCardComponent],
  templateUrl: './dress-detail.html',
  styleUrl: './dress-detail.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DressDetailComponent implements OnInit, OnDestroy {
  private route        = inject(ActivatedRoute);
  private dressService = inject(DressService);
  private atlierService = inject(AtlierService);
  private viewedSvc   = inject(ViewedDressesService);
  private likedSvc    = inject(LikedDressesService);
  private router      = inject(Router);
  private cdr         = inject(ChangeDetectorRef);

  dress            = signal<DressDetailDto | null>(null);
  atlier           = signal<AtlierInfoDto | null>(null);
  loading          = signal(true);
  error            = signal(false);
  activePhotoIndex = signal(0);
  lightboxOpen     = signal(false);
  detailsOpen      = signal(false);
  liked            = signal(false);

  private observer!: IntersectionObserver;
  private routeSub!: Subscription;

  ngOnInit() {
    this.routeSub = this.route.paramMap.subscribe(params => {
      const id = params.get('id')!;
      this.load(id);
    });
  }

  ngOnDestroy() {
    this.observer?.disconnect();
    this.routeSub?.unsubscribe();
    document.body.style.overflow = '';
  }

  private load(id: string) {
    this.loading.set(true);
    this.error.set(false);
    this.activePhotoIndex.set(0);
    this.lightboxOpen.set(false);
    this.detailsOpen.set(false);
    this.liked.set(this.likedSvc.isLiked(id));
    this.viewedSvc.add(id);

    forkJoin({
      dress:  this.dressService.getById(id),
      atlier: this.atlierService.getInfo()
    }).subscribe({
      next: ({ dress, atlier }) => {
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
    const nowLiked = this.likedSvc.toggle(d.id);
    this.liked.set(nowLiked);
  }

  silhouetteLabel(n: number): string {
    return SILHOUETTE_LABELS[n] ?? 'Classic';
  }

  // ── Gallery ─────────────────────────────────────────────────────
  setPhoto(index: number) { this.activePhotoIndex.set(index); }

  get activePhoto(): DressPhoto | null {
    const d = this.dress();
    if (!d || !d.photos.length) return null;
    return d.photos[this.activePhotoIndex()] ?? null;
  }

  // ── Lightbox ─────────────────────────────────────────────────────
  openLightbox() {
    this.lightboxOpen.set(true);
    document.body.style.overflow = 'hidden';
  }

  openLightboxAt(index: number) {
    this.activePhotoIndex.set(index);
    this.openLightbox();
  }

  closeLightbox() {
    this.lightboxOpen.set(false);
    document.body.style.overflow = '';
  }

  prevPhoto() {
    const d = this.dress();
    if (!d?.photos.length) return;
    const i = this.activePhotoIndex();
    this.activePhotoIndex.set(i > 0 ? i - 1 : d.photos.length - 1);
  }

  nextPhoto() {
    const d = this.dress();
    if (!d?.photos.length) return;
    const i = this.activePhotoIndex();
    this.activePhotoIndex.set(i < d.photos.length - 1 ? i + 1 : 0);
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
