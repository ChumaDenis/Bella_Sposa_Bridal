import { Component, Input, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DressListDto } from '../../../core/models/dress.model';
import { ViewedDressesService } from '../../../core/services/viewed-dresses.service';

@Component({
  selector: 'app-dress-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dress-card.html',
  styleUrl: './dress-card.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DressCardComponent {
  @Input() dress!: DressListDto;

  private router = inject(Router);
  private viewedDressesService = inject(ViewedDressesService);

  navigate() {
    this.viewedDressesService.add(this.dress.id);
    this.router.navigate(['/catalog', this.dress.id]);
  }
}
