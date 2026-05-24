import {
  Component, ChangeDetectionStrategy, OnInit, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { DressCardComponent } from '../catalog/components/dress-card/dress-card';
import { CollectionService } from '../core/services/collection.service';
import { DressService } from '../core/services/dress.service';
import { CollectionDto } from '../core/models/collection.model';
import { DressListDto } from '../core/models/dress.model';

@Component({
  selector: 'app-collection-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, FooterComponent, DressCardComponent],
  templateUrl: './collection-detail.html',
  styleUrl: './collection-detail.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CollectionDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private collectionService = inject(CollectionService);
  private dressService = inject(DressService);
  private router = inject(Router);

  collection = signal<CollectionDto | null>(null);
  dresses = signal<DressListDto[]>([]);
  loading = signal(true);
  notFound = signal(false);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.router.navigate(['/collections']); return; }

    forkJoin({
      collection: this.collectionService.getById(id),
      dresses: this.dressService.getByCollection(id)
    }).subscribe({
      next: ({ collection, dresses }) => {
        this.collection.set(collection);
        this.dresses.set(dresses);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.notFound.set(true);
      }
    });
  }

  goBack() {
    this.router.navigate(['/collections']);
  }
}
