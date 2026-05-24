import {
  Component, ChangeDetectionStrategy, OnDestroy, AfterViewInit,
  ViewChild, ElementRef, HostListener, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NavbarComponent implements AfterViewInit, OnDestroy {
  @ViewChild('navbar') navbarEl!: ElementRef<HTMLElement>;

  menuOpen = false;
  collectionMenuOpen = false;
  isHomePage = signal(true);

  private megaMenuTimer: ReturnType<typeof setTimeout> | null = null;
  private routerSub: Subscription | null = null;
  private router = inject(Router);

  ngAfterViewInit() {
    this.isHomePage.set(this.router.url === '/');
    this.updateNavbar();

    this.routerSub = this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => {
        this.isHomePage.set(this.router.url === '/');
        this.updateNavbar();
      });
  }

  ngOnDestroy() {
    if (this.megaMenuTimer) clearTimeout(this.megaMenuTimer);
    this.routerSub?.unsubscribe();
  }

  @HostListener('window:scroll')
  onScroll() { this.updateNavbar(); }

  updateNavbar() {
    const nav = this.navbarEl?.nativeElement;
    if (nav) nav.classList.toggle('scrolled', window.scrollY > 80);
  }

  openMegaMenu() {
    if (this.megaMenuTimer) clearTimeout(this.megaMenuTimer);
    this.collectionMenuOpen = true;
  }

  closeMegaMenu() {
    this.megaMenuTimer = setTimeout(() => { this.collectionMenuOpen = false; }, 200);
  }

  toggleMenu() { this.menuOpen = !this.menuOpen; }
  closeMenu()  { this.menuOpen = false; }

  navTo(section: string) {
    this.closeMenu();
    if (this.router.url === '/') {
      document.getElementById(section)?.scrollIntoView({ behavior: 'smooth' });
    } else {
      this.router.navigate(['/']).then(() =>
        setTimeout(() => document.getElementById(section)?.scrollIntoView({ behavior: 'smooth' }), 150)
      );
    }
  }

  goToCatalog() {
    this.closeMegaMenu();
    this.closeMenu();
    this.router.navigate(['/catalog']);
  }

  goToAppointment() {
    this.closeMenu();
    this.router.navigate(['/appointment']);
  }
}
