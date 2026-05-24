import {
  Component, ChangeDetectionStrategy, OnInit, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from '../shared/navbar/navbar';
import { FooterComponent } from '../shared/footer/footer';
import { AppointmentService } from '../core/services/appointment.service';
import { DressService } from '../core/services/dress.service';
import { AtlierService } from '../core/services/atlier.service';
import { ViewedDressesService } from '../core/services/viewed-dresses.service';
import { DressListDto } from '../core/models/dress.model';
import { AtlierInfoDto } from '../core/models/atlier.model';

@Component({
  selector: 'app-appointment',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, NavbarComponent, FooterComponent],
  templateUrl: './appointment.html',
  styleUrl: './appointment.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppointmentComponent implements OnInit {
  private fb = inject(FormBuilder);
  private appointmentService = inject(AppointmentService);
  private dressService = inject(DressService);
  private atlierService = inject(AtlierService);
  private viewedDressesService = inject(ViewedDressesService);

  loading = signal(false);
  success = signal(false);
  error = signal<string | null>(null);
  viewedDresses = signal<DressListDto[]>([]);
  atlier = signal<AtlierInfoDto | null>(null);

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    phone: ['', Validators.required],
    email: [''],
    appointmentDate: ['', Validators.required],
    appointmentTime: ['', Validators.required],
    type: [0 as number, Validators.required],
    notes: ['']
  });

  ngOnInit() {
    // Load atlier info
    this.atlierService.getInfo().subscribe({
      next: (atlier) => this.atlier.set(atlier),
      error: () => {}
    });

    // Load viewed dresses
    const ids = this.viewedDressesService.getIds();
    if (ids.length > 0) {
      this.dressService.getAll().subscribe({
        next: (dresses) => {
          const viewed = dresses.filter(d => ids.includes(d.id));
          // Preserve order of viewed
          const ordered = ids.map(id => viewed.find(d => d.id === id)).filter((d): d is DressListDto => !!d);
          this.viewedDresses.set(ordered);
        },
        error: () => {}
      });
    }
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.value;
    const dateStr = value.appointmentDate as string;
    const timeStr = value.appointmentTime as string;
    const appointmentDateTime = new Date(`${dateStr}T${timeStr}`).toISOString();

    this.loading.set(true);
    this.error.set(null);

    const viewedDressIds = this.viewedDressesService.getIds();

    this.appointmentService.create({
      firstName: value.firstName as string,
      lastName: value.lastName as string,
      phone: value.phone as string,
      email: value.email || null,
      appointmentDateTime,
      type: value.type as number,
      notes: value.notes || null,
      viewedDressIds
    }).subscribe({
      next: () => {
        this.loading.set(false);
        this.success.set(true);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Something went wrong. Please try again or contact us directly.');
      }
    });
  }
}
