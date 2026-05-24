import {
  Component, ChangeDetectionStrategy, OnInit, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { CollectionService } from '../core/services/collection.service';
import { CollectionDto } from '../core/models/collection.model';

@Component({
  selector: 'app-collections',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, FooterComponent],
  templateUrl: './collections.html',
  styleUrl: './collections.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CollectionsComponent implements OnInit {
  private collectionService = inject(CollectionService);
  private router = inject(Router);

  collections = signal<CollectionDto[]>([]);
  loading = signal(true);

  ngOnInit() {
    this.collectionService.getAll().subscribe({
      next: (cols) => {
        this.collections.set(cols);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  goToCollection(id: string) {
    this.router.navigate(['/collections', id]);
  }
}
