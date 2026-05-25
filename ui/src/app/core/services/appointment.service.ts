import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateAppointmentDto, AppointmentDto } from '../models/appointment.model';
import { API_BASE } from './api.config';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private http = inject(HttpClient);

  create(dto: CreateAppointmentDto) {
    return this.http.post<AppointmentDto>(`${API_BASE}/appointments`, dto);
  }

  getBookedSlots(date: string) {
    return this.http.get<string[]>(`${API_BASE}/appointments/booked-slots?date=${date}`);
  }
}
