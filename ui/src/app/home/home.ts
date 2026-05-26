import {
  Component, OnDestroy, OnInit, ElementRef, ViewChild, AfterViewInit, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { DressService } from '../core/services/dress.service';
import { ViewedDressesService } from '../core/services/viewed-dresses.service';
import { DressListDto } from '../core/models/dress.model';
import { AppointmentService } from '../core/services/appointment.service';
import { AppointmentTypeConfigDto } from '../core/models/appointment.model';
import { AtlierService } from '../core/services/atlier.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink, NavbarComponent, FooterComponent],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('heroSection') heroSection!: ElementRef<HTMLElement>;
  @ViewChild('heroVideo')  heroVideo!:  ElementRef<HTMLVideoElement>;
  @ViewChild('aboutVideo') aboutVideo!: ElementRef<HTMLVideoElement>;

  private dressService        = inject(DressService);
  private viewedSvc           = inject(ViewedDressesService);
  private router              = inject(Router);
  private appointmentService  = inject(AppointmentService);
  private atlierService       = inject(AtlierService);

  isMuted      = true;
  isAboutMuted = true;

  private defaultHeroSrc = '/videos/main.mp4';
  heroVideoSrc = signal(this.defaultHeroSrc);

  featuredDresses  = signal<DressListDto[]>([]);
  dressesLoading   = signal(true);
  services         = signal<AppointmentTypeConfigDto[]>([]);

  private observer!:            IntersectionObserver;
  private aboutVideoObserver!:  IntersectionObserver;

  ngOnInit() {
    this.atlierService.getInfo().subscribe({
      next: (info) => {
        const desktop = info.heroVideoDesktop || this.defaultHeroSrc;
        const mobile  = info.heroVideoMobile  || desktop;
        const src = window.innerWidth < 768 ? mobile : desktop;
        this.heroVideoSrc.set(src);
        this.loadHeroVideo(src);
      },
      error: () => {}
    });

    this.appointmentService.getAppointmentTypes().subscribe({
      next: (types) => {
        this.services.set(
          types
            .filter(t => t.isActive)
            .sort((a, b) => a.displayOrder - b.displayOrder)
        );
      },
      error: () => {}
    });

    this.dressService.getHomepageFeatured().subscribe({
      next: (dresses) => {
        if (dresses.length > 0) {
          this.featuredDresses.set(dresses);
          this.dressesLoading.set(false);
          setTimeout(() => this.reObserveReveal(), 80);
        } else {
          this.dressService.getAll({ pageSize: 6 }).subscribe({
            next: (result) => {
              this.featuredDresses.set(result.items);
              this.dressesLoading.set(false);
              setTimeout(() => this.reObserveReveal(), 80);
            },
            error: () => this.dressesLoading.set(false)
          });
        }
      },
      error: () => {
        this.dressService.getAll({ pageSize: 6 }).subscribe({
          next: (result) => {
            this.featuredDresses.set(result.items);
            this.dressesLoading.set(false);
            setTimeout(() => this.reObserveReveal(), 80);
          },
          error: () => this.dressesLoading.set(false)
        });
      }
    });
  }

  ngAfterViewInit() {
    this.initScrollReveal();
    this.initCursor();
    this.startHeroVideo();
    this.initAboutVideo();
  }

  private loadHeroVideo(src: string) {
    const video = this.heroVideo?.nativeElement;
    if (!video) return;
    video.src = src;
    video.muted = true;
    video.load();
    video.play().catch(() => {
      const resume = () => { video.play(); document.removeEventListener('click', resume); };
      document.addEventListener('click', resume, { once: true });
    });
  }

  private startHeroVideo() {
    this.loadHeroVideo(this.heroVideoSrc());
  }

  ngOnDestroy() {
    this.observer?.disconnect();
    this.aboutVideoObserver?.disconnect();
  }

  private initScrollReveal() {
    this.observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('revealed');
          this.observer.unobserve(entry.target);
        }
      });
    }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });

    setTimeout(() => {
      document.querySelectorAll('.reveal').forEach(el => this.observer.observe(el));
    }, 100);
  }

  reObserveReveal() {
    document.querySelectorAll('.reveal:not(.revealed)').forEach(el => this.observer?.observe(el));
  }

  private initCursor() {
    if (window.innerWidth < 768) return;
    const cursor = document.createElement('div');
    cursor.className = 'custom-cursor';
    document.body.appendChild(cursor);

    document.addEventListener('mousemove', (e) => {
      cursor.style.transform = `translate(${e.clientX}px, ${e.clientY}px)`;
    });

    document.addEventListener('mousedown', () => cursor.classList.add('clicking'));
    document.addEventListener('mouseup',   () => cursor.classList.remove('clicking'));

    document.querySelectorAll('a, button, .gallery-img-wrap').forEach(el => {
      el.addEventListener('mouseenter', () => cursor.classList.add('hovering'));
      el.addEventListener('mouseleave', () => cursor.classList.remove('hovering'));
    });
  }

  toggleSound() {
    const video = this.heroVideo?.nativeElement;
    if (!video) return;
    this.isMuted = !this.isMuted;
    video.muted = this.isMuted;
    if (!this.isMuted) video.play();
  }

  toggleAboutSound() {
    const video = this.aboutVideo?.nativeElement;
    if (!video) return;
    this.isAboutMuted = !this.isAboutMuted;
    video.muted = this.isAboutMuted;
  }

  private initAboutVideo() {
    const video = this.aboutVideo?.nativeElement;
    if (!video) return;
    video.muted = true;
    this.aboutVideoObserver = new IntersectionObserver(([entry]) => {
      if (entry.isIntersecting) {
        video.play().catch(() => {});
      } else {
        video.pause();
      }
    }, { threshold: 0.25 });
    this.aboutVideoObserver.observe(video);
  }

  scrollTo(id: string) {
    document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
  }

  goToDress(slug: string) {
    this.router.navigate(['/catalog', slug]);
  }

  goToCatalog() {
    this.router.navigate(['/catalog']);
  }

  goToAppointment() {
    this.router.navigate(['/appointment']);
  }

  readonly marqueeItems = [
    'Bridal Couture', 'Wedding Salon', 'Bella Sposa', 'Bespoke Gowns',
    'Bridal Atelier', 'Wedding Dress', 'Elegance', 'Bridal',
  ];

  readonly currentYear = new Date().getFullYear();

  readonly steps = [
    { num: '01', title: 'Consultation', desc: 'We get to know you — your vision, your style, the feeling you want to carry on your day.' },
    { num: '02', title: 'Fitting',      desc: 'Each detail is tailored to you — until the silhouette feels like it was always yours.' },
    { num: '03', title: 'Your Day',     desc: 'Your gown is ready. You shine — and we take pride in every single stitch.' },
  ];

}
