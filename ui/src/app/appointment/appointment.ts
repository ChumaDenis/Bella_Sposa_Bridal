import {
  Component, ChangeDetectionStrategy, ChangeDetectorRef, OnInit, inject, signal
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
import { LikedDressesService } from '../core/services/liked-dresses.service';
import { DressListDto } from '../core/models/dress.model';
import { AtlierInfoDto } from '../core/models/atlier.model';
import { AppointmentTypeConfigDto } from '../core/models/appointment.model';

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
  private dressService      = inject(DressService);
  private atlierService     = inject(AtlierService);
  private viewedSvc         = inject(ViewedDressesService);
  private likedSvc          = inject(LikedDressesService);
  private cdr               = inject(ChangeDetectorRef);

  loading           = signal(false);
  success           = signal(false);
  error             = signal<string | null>(null);
  shownDresses      = signal<DressListDto[]>([]);
  usingLiked        = signal(false);
  atlier            = signal<AtlierInfoDto | null>(null);
  bookedSlots       = signal<string[]>([]);
  slotsLoading      = signal(false);
  appointmentTypes  = signal<AppointmentTypeConfigDto[]>([]);

  readonly morningSlots   = ['10:00', '12:00'];
  readonly afternoonSlots = ['14:00', '16:00'];
  readonly eveningSlots   = ['18:00'];

  private localDateStr(d: Date): string {
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
  }
  get minDate(): string { return this.localDateStr(new Date()); }
  get maxDate(): string { const d = new Date(); d.setMonth(d.getMonth() + 1); return this.localDateStr(d); }

  get selectedTime(): string { return this.form.get('appointmentTime')?.value ?? ''; }

  get selectedTypeInfo(): AppointmentTypeConfigDto | null {
    const id = this.form.get('type')?.value;
    if (id == null) return null;
    return this.appointmentTypes().find(t => t.id === id) ?? null;
  }

  selectTime(time: string) {
    if (this.isBooked(time)) return;
    this.form.patchValue({ appointmentTime: time });
  }

  isBooked(time: string): boolean {
    return this.bookedSlots().includes(time);
  }

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    phone: ['', Validators.required],
    email: [''],
    appointmentDate: ['', Validators.required],
    appointmentTime: ['', Validators.required],
    type: [null as number | null, Validators.required],
    notes: ['']
  });

  ngOnInit() {
    this.appointmentService.getAppointmentTypes().subscribe({
      next: (types) => {
        const active = types
          .filter(t => t.isActive)
          .sort((a, b) => a.displayOrder - b.displayOrder);
        this.appointmentTypes.set(active);
        if (active.length > 0) {
          this.form.patchValue({ type: active[0].id });
        }
        this.cdr.markForCheck();
      },
      error: () => {}
    });

    this.atlierService.getInfo().subscribe({
      next: (atlier) => this.atlier.set(atlier),
      error: () => {}
    });

    const likedIds  = this.likedSvc.getIds();
    const viewedIds = this.viewedSvc.getIds();
    const ids = likedIds.length > 0 ? likedIds : viewedIds;
    this.usingLiked.set(likedIds.length > 0);

    if (ids.length > 0) {
      this.dressService.getAll({ pageSize: 300 }).subscribe({
        next: (result) => {
          const matched = ids
            .map(id => result.items.find(d => d.id === id))
            .filter((d): d is DressListDto => !!d);
          this.shownDresses.set(matched);
        },
        error: () => {}
      });
    }

    this.form.get('appointmentDate')!.valueChanges.subscribe(date => {
      if (date) {
        this.loadBookedSlots(date);
      } else {
        this.bookedSlots.set([]);
      }
    });
  }

  private loadBookedSlots(date: string) {
    this.slotsLoading.set(true);
    this.appointmentService.getBookedSlots(date).subscribe({
      next: slots => {
        this.bookedSlots.set(slots);
        this.slotsLoading.set(false);
        // Clear selected time if it just became booked
        if (this.selectedTime && slots.includes(this.selectedTime)) {
          this.form.patchValue({ appointmentTime: '' });
        }
      },
      error: () => this.slotsLoading.set(false)
    });
  }

  submitCallback() {
    // Clear date/time touched so their validation errors disappear
    this.form.get('appointmentDate')!.markAsUntouched();
    this.form.get('appointmentTime')!.markAsUntouched();

    const v = this.form.value;
    if (!v.firstName || !v.lastName || !v.phone) {
      this.form.get('firstName')!.markAsTouched();
      this.form.get('lastName')!.markAsTouched();
      this.form.get('phone')!.markAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const likedIds = this.likedSvc.getIds();
    const viewedDressIds = likedIds.length > 0 ? likedIds : this.viewedSvc.getIds();
    const callbackNote = v.notes?.trim()
      ? `${v.notes.trim()}\n\n[Client requested a callback to arrange a convenient time]`
      : '[Client requested a callback to arrange a convenient time]';

    // Use far-future placeholder so slot-conflict check never triggers
    const appointmentDateTime = '2099-01-01T00:00:00Z';

    this.appointmentService.create({
      firstName: v.firstName as string,
      lastName: v.lastName as string,
      phone: v.phone as string,
      email: v.email || null,
      appointmentDateTime,
      type: v.type as number,
      notes: callbackNote,
      viewedDressIds
    }).subscribe({
      next: () => { this.loading.set(false); this.success.set(true); },
      error: () => {
        this.loading.set(false);
        this.error.set('Something went wrong. Please try again or contact us directly.');
      }
    });
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.value;
    const dateStr = value.appointmentDate as string;
    const timeStr = value.appointmentTime as string;
    // Send as UTC directly so backend stores exact slot label time
    const appointmentDateTime = `${dateStr}T${timeStr}:00Z`;

    this.loading.set(true);
    this.error.set(null);

    const likedIds = this.likedSvc.getIds();
    const viewedDressIds = likedIds.length > 0 ? likedIds : this.viewedSvc.getIds();

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
      error: (err) => {
        this.loading.set(false);
        if (err.status === 409) {
          this.error.set('This time slot has just been taken. Please select a different time.');
          this.form.patchValue({ appointmentTime: '' });
          const date = this.form.value.appointmentDate as string;
          if (date) this.loadBookedSlots(date);
        } else {
          this.error.set('Something went wrong. Please try again or contact us directly.');
        }
      }
    });
  }
}
