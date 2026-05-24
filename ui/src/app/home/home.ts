import { Component, OnDestroy, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';

@Component({
  selector: 'app-home',
  imports: [CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements AfterViewInit, OnDestroy {
  @ViewChild('heroSection') heroSection!: ElementRef<HTMLElement>;
  @ViewChild('heroVideo')  heroVideo!:  ElementRef<HTMLVideoElement>;
  @ViewChild('aboutVideo') aboutVideo!: ElementRef<HTMLVideoElement>;

  isMuted      = true;
  isAboutMuted = true;

  private observer!:            IntersectionObserver;
  private aboutVideoObserver!:  IntersectionObserver;

  ngAfterViewInit() {
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
      const resume = () => { video.play(); document.removeEventListener('click', resume); };
      document.addEventListener('click', resume, { once: true });
    });
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
