import { Component, OnDestroy, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements AfterViewInit, OnDestroy {
  @ViewChild('navbar') navbar!: ElementRef<HTMLElement>;
  @ViewChild('heroSection') heroSection!: ElementRef<HTMLElement>;
  @ViewChild('heroVideo')  heroVideo!:  ElementRef<HTMLVideoElement>;
  @ViewChild('aboutVideo') aboutVideo!: ElementRef<HTMLVideoElement>;

  isMuted      = true;
  isAboutMuted = true;

  private scrollHandler = () => { this.updateNavbar(); };

  private observer!:            IntersectionObserver;
  private aboutVideoObserver!:  IntersectionObserver;
  menuOpen           = false;
  collectionMenuOpen = false;
  private megaMenuTimer: ReturnType<typeof setTimeout> | null = null;

  ngAfterViewInit() {
    window.addEventListener('scroll', this.scrollHandler, { passive: true });
    this.initScrollReveal();
    this.initCursor();
    this.startHeroVideo();
    this.initAboutVideo();
  }

  private startHeroVideo() {
    const video = this.heroVideo?.nativeElement;
    if (!video) return;
    video.muted = true;
    video.play().catch(() => {
      // Autoplay blocked — video will play on first user interaction
      const resume = () => { video.play(); document.removeEventListener('click', resume); };
      document.addEventListener('click', resume, { once: true });
    });
  }

  ngOnDestroy() {
    window.removeEventListener('scroll', this.scrollHandler);
    this.observer?.disconnect();
    this.aboutVideoObserver?.disconnect();
    if (this.megaMenuTimer) clearTimeout(this.megaMenuTimer);
  }

  openMegaMenu()  {
    if (this.megaMenuTimer) clearTimeout(this.megaMenuTimer);
    this.collectionMenuOpen = true;
  }

  closeMegaMenu() {
    this.megaMenuTimer = setTimeout(() => { this.collectionMenuOpen = false; }, 120);
  }

  private updateNavbar() {
    const nav = this.navbar?.nativeElement;
    if (nav) nav.classList.toggle('scrolled', window.scrollY > 80);
  }

  private updateParallax() {
    // No parallax on portrait video — it would push content out of frame
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

  toggleMenu() { this.menuOpen = !this.menuOpen; }
  closeMenu()  { this.menuOpen = false; }

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
    this.closeMenu();
    document.getElementById(id)?.scrollIntoView({ behavior: 'smooth' });
  }

  readonly dresses = [
    { src: '/images/dress-1.jpg', alt: 'Ball gown, off-shoulder' },
    { src: '/images/dress-2.jpg', alt: 'A-line with statement bow' },
    { src: '/images/dress-3.jpg', alt: 'Mermaid with overskirt' },
    { src: '/images/dress-4.jpg', alt: 'Mermaid, back silhouette' },
    { src: '/images/dress-5.jpg', alt: 'Ball gown with cascading ruffles' },
    { src: '/images/dress-6.jpg', alt: 'Minimalist satin column' },
  ];

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

  readonly services = [
    {
      num: '01',
      title: 'Initial Appointment',
      desc: 'Your first visit to our boutique. We get to know you, your vision, and your wedding story — then guide you through a curated selection of gowns.',
      detail: 'Up to 90 minutes · Complimentary',
    },
    {
      num: '02',
      title: 'Second Appointment',
      desc: 'Return to refine your favourites. We narrow the styles, work on silhouette, and move closer to the dress that is truly yours.',
      detail: 'Up to 60 minutes · By Invitation',
    },
    {
      num: '03',
      title: 'VIP Appointment',
      desc: 'An exclusive private experience — the boutique is reserved entirely for you. Champagne, personal styling, and unhurried time to find your perfect gown.',
      detail: 'Private Boutique · Champagne Included',
    },
  ];
}
