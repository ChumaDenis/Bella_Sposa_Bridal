import {
  Component, ChangeDetectionStrategy, OnInit, inject,
  signal, computed, AfterViewInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { DressCardComponent } from './components/dress-card/dress-card';
import { DressService } from '../core/services/dress.service';
import { CollectionService } from '../core/services/collection.service';
import { DressListDto } from '../core/models/dress.model';
import { CollectionDto } from '../core/models/collection.model';

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CommonModule, NavbarComponent, FooterComponent, DressCardComponent],
  templateUrl: './catalog.html',
  styleUrl: './catalog.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CatalogComponent implements OnInit, AfterViewInit {
  private dressService = inject(DressService);
  private collectionService = inject(CollectionService);

  dresses = signal<DressListDto[]>([]);
  collections = signal<CollectionDto[]>([]);
  activeCollection = signal<string | null>(null);
  activeSilhouette = signal<string | null>(null);
  loading = signal(true);

  private observer!: IntersectionObserver;

  filteredDresses = computed(() => {
    let result = this.dresses();
    const col = this.activeCollection();
    const sil = this.activeSilhouette();
    if (col) {
      result = result.filter(d => d.collectionNames.some(
        cn => this.collections().find(c => c.id === col)?.name === cn
      ));
    }
    if (sil) {
      result = result.filter(d => d.silhouette === sil);
    }
    return result;
  });

  silhouettes = computed(() => {
    const all = this.dresses().map(d => d.silhouette).filter(Boolean);
    return [...new Set(all)];
  });

  ngOnInit() {
    forkJoin({
      dresses: this.dressService.getAll(),
      collections: this.collectionService.getAll()
    }).subscribe({
      next: ({ dresses, collections }) => {
        this.dresses.set(dresses);
        this.collections.set(collections);
        this.loading.set(false);
        setTimeout(() => this.initReveal(), 100);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  ngAfterViewInit() {
    // Reveal for static elements already in DOM
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

  selectCollection(id: string | null) {
    this.activeCollection.set(id);
    setTimeout(() => this.initReveal(), 50);
  }

  selectSilhouette(name: string | null) {
    this.activeSilhouette.set(name);
    setTimeout(() => this.initReveal(), 50);
  }
}
