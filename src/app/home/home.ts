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

  private scrollHandler = () => {
    this.updateNavbar();
    this.updateParallax();
  };

  private observer!: IntersectionObserver;
  menuOpen = false;

  ngAfterViewInit() {
    window.addEventListener('scroll', this.scrollHandler, { passive: true });
    this.initScrollReveal();
    this.initCursor();
  }

  ngOnDestroy() {
    window.removeEventListener('scroll', this.scrollHandler);
    this.observer?.disconnect();
  }

  private updateNavbar() {
    const nav = this.navbar?.nativeElement;
    if (nav) nav.classList.toggle('scrolled', window.scrollY > 80);
  }

  private updateParallax() {
    const hero = this.heroSection?.nativeElement;
    if (!hero) return;
    const video = hero.querySelector('.hero-video') as HTMLElement;
    if (video) video.style.transform = `translateY(${window.scrollY * 0.35}px)`;
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
}
