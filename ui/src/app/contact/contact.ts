import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { AtlierService } from '../core/services/atlier.service';
import { AtlierInfoDto } from '../core/models/atlier.model';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, RouterLink, NavbarComponent, FooterComponent],
  templateUrl: './contact.html',
  styleUrl: './contact.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ContactComponent implements OnInit {
  private atlierService = inject(AtlierService);

  atlier = signal<AtlierInfoDto | null>(null);

  ngOnInit() {
    this.atlierService.getInfo().subscribe(info => this.atlier.set(info));
  }
}
