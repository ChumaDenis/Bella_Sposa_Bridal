import {
  Component, ChangeDetectionStrategy, OnDestroy, AfterViewInit,
  ViewChild, ElementRef, HostListener, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { forkJoin, Subscription } from 'rxjs';
import { CollectionService } from '../../core/services/collection.service';
import { DressService } from '../../core/services/dress.service';
import { CollectionDto } from '../../core/models/collection.model';
import { DressListDto } from '../../core/models/dress.model';

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

  private router           = inject(Router);
  private collectionService = inject(CollectionService);
  private dressService     = inject(DressService);

  menuOpen             = false;
  collectionMenuOpen   = false;
  isHomePage           = signal(true);

  collections          = signal<CollectionDto[]>([]);
  collectionDresses    = signal<Record<string, DressListDto[]>>({});
  expandedCollectionId = signal<string | null>(null);

  private megaMenuTimer: ReturnType<typeof setTimeout> | null = null;
  private routerSub: Subscription | null = null;

  ngAfterViewInit() {
    this.isHomePage.set(this.router.url === '/');
    this.updateNavbar();

    this.routerSub = this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => {
        this.isHomePage.set(this.router.url === '/');
        this.updateNavbar();
      });

    this.collectionService.getAll().subscribe(cols => {
      this.collections.set(cols);
      if (cols.length > 0) this.expandedCollectionId.set(cols[0].id);

      if (cols.length > 0) {
        const ids = cols.map(c => c.id);
        forkJoin(ids.map(id => this.dressService.getByCollection(id))).subscribe(arrays => {
          const map: Record<string, DressListDto[]> = {};
          ids.forEach((id, i) => { map[id] = arrays[i]; });
          this.collectionDresses.set(map);
        });
      }
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

  expandCollection(id: string) {
    this.expandedCollectionId.set(id);
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

  goToCollection(id: string) {
    this.closeMegaMenu();
    this.closeMenu();
    this.router.navigate(['/catalog'], { queryParams: { collection: id } });
  }

  goToDress(id: string) {
    this.closeMegaMenu();
    this.router.navigate(['/catalog', id]);
  }

  goToAppointment() {
    this.closeMenu();
    this.router.navigate(['/appointment']);
  }
}
