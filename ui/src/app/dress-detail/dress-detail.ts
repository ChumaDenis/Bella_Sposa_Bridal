import {
  Component, ChangeDetectionStrategy, OnInit, inject,
  signal, AfterViewInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { DressCardComponent } from '../catalog/components/dress-card/dress-card';
import { DressService } from '../core/services/dress.service';
import { AtlierService } from '../core/services/atlier.service';
import { ViewedDressesService } from '../core/services/viewed-dresses.service';
import { DressDetailDto, DressPhoto } from '../core/models/dress.model';
import { AtlierInfoDto } from '../core/models/atlier.model';

@Component({
  selector: 'app-dress-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, FooterComponent, DressCardComponent],
  templateUrl: './dress-detail.html',
  styleUrl: './dress-detail.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DressDetailComponent implements OnInit, AfterViewInit {
  private route = inject(ActivatedRoute);
  private dressService = inject(DressService);
  private atlierService = inject(AtlierService);
  private viewedDressesService = inject(ViewedDressesService);
  private router = inject(Router);

  dress = signal<DressDetailDto | null>(null);
  atlier = signal<AtlierInfoDto | null>(null);
  loading = signal(true);
  selectedPhoto = signal<DressPhoto | null>(null);
  activePhotoIndex = signal(0);

  private observer!: IntersectionObserver;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.viewedDressesService.add(id);

    forkJoin({
      dress: this.dressService.getById(id),
      atlier: this.atlierService.getInfo()
    }).subscribe({
      next: ({ dress, atlier }) => {
        this.dress.set(dress);
        this.atlier.set(atlier);
        this.loading.set(false);
        setTimeout(() => this.initReveal(), 100);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  ngAfterViewInit() {
    setTimeout(() => this.initReveal(), 200);
  }

  initReveal() {
    if (this.observer) this.observer.disconnect();
    this.observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('visible');
          this.observer.unobserve(entry.target);
        }
      });
    }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });
    document.querySelectorAll('.reveal').forEach(el => this.observer.observe(el));
  }

  openLightbox(photo: DressPhoto) {
    this.selectedPhoto.set(photo);
    document.body.style.overflow = 'hidden';
  }

  closeLightbox() {
    this.selectedPhoto.set(null);
    document.body.style.overflow = '';
  }

  prevPhoto() {
    const d = this.dress();
    if (!d || !d.photos.length) return;
    const idx = this.activePhotoIndex();
    const newIdx = idx > 0 ? idx - 1 : d.photos.length - 1;
    this.activePhotoIndex.set(newIdx);
    this.selectedPhoto.set(d.photos[newIdx]);
  }

  nextPhoto() {
    const d = this.dress();
    if (!d || !d.photos.length) return;
    const idx = this.activePhotoIndex();
    const newIdx = idx < d.photos.length - 1 ? idx + 1 : 0;
    this.activePhotoIndex.set(newIdx);
    this.selectedPhoto.set(d.photos[newIdx]);
  }

  bookAppointment() {
    this.router.navigate(['/appointment']);
  }
}
