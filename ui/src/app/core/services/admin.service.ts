import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from './api.config';
import { DressListDto, DressDetailDto, NavDressItem } from '../models/dress.model';
import { CollectionDto, CollectionNameDto } from '../models/collection.model';
import {
  AppointmentDto, AppointmentFileDto, AppointmentTypeConfigDto,
  TimeSlotDto, DayScheduleDto
} from '../models/appointment.model';

export interface AddPhotoPayload {
  url: string;
  altText?: string;
  type: number;
  order: number;
}

export interface AddVideoPayload {
  url: string;
  thumbnailUrl?: string | null;
  type: number;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);

  // ─── Dresses ────────────────────────────────────────────────────
  getAllDresses(includeDeleted = false) {
    return this.http.get<DressListDto[]>(`${API_BASE}/dresses/admin?includeDeleted=${includeDeleted}`);
  }

  getDress(id: string) {
    return this.http.get<DressDetailDto>(`${API_BASE}/dresses/${id}`);
  }

  createDress(dto: unknown) {
    return this.http.post<DressDetailDto>(`${API_BASE}/dresses`, dto);
  }

  updateDress(id: string, dto: unknown) {
    return this.http.put<DressDetailDto>(`${API_BASE}/dresses/${id}`, dto);
  }

  deleteDress(id: string) {
    return this.http.delete(`${API_BASE}/dresses/${id}`);
  }

  restoreDress(id: string) {
    return this.http.patch(`${API_BASE}/dresses/${id}/restore`, null);
  }

  toggleActive(dressId: string, isActive: boolean) {
    return this.http.patch(`${API_BASE}/dresses/${dressId}/active`, isActive, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  setHomepageFeatured(dressId: string, isFeatured: boolean, order: number) {
    return this.http.patch(`${API_BASE}/dresses/${dressId}/homepage-featured`, { isFeatured, order });
  }

  // ─── Photos ─────────────────────────────────────────────────────
  uploadPhoto(file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<{ url: string }>(`${API_BASE}/upload`, form);
  }

  addPhoto(dressId: string, payload: AddPhotoPayload) {
    return this.http.post(`${API_BASE}/dresses/${dressId}/photos`, payload);
  }

  deletePhoto(dressId: string, photoId: string) {
    return this.http.delete(`${API_BASE}/dresses/${dressId}/photos/${photoId}`);
  }

  reorderPhotos(dressId: string, orderedIds: string[]) {
    return this.http.patch(`${API_BASE}/dresses/${dressId}/photos/reorder`, orderedIds);
  }

  // ─── Videos ─────────────────────────────────────────────────────
  uploadVideo(file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<{ url: string }>(`${API_BASE}/upload/video`, form);
  }

  addVideo(dressId: string, payload: AddVideoPayload) {
    return this.http.post(`${API_BASE}/dresses/${dressId}/videos`, payload);
  }

  deleteVideo(dressId: string, videoId: string) {
    return this.http.delete(`${API_BASE}/dresses/${dressId}/videos/${videoId}`);
  }

  // ─── Collections ─────────────────────────────────────────────────
  getAllCollections(includeDeleted = false) {
    return this.http.get<CollectionDto[]>(`${API_BASE}/collections/admin?includeDeleted=${includeDeleted}`);
  }

  getCollectionNames() {
    return this.http.get<CollectionNameDto[]>(`${API_BASE}/collections/names`);
  }

  createCollection(dto: unknown) {
    return this.http.post<CollectionDto>(`${API_BASE}/collections`, dto);
  }

  updateCollection(id: string, dto: unknown) {
    return this.http.put<CollectionDto>(`${API_BASE}/collections/${id}`, dto);
  }

  deleteCollection(id: string) {
    return this.http.delete(`${API_BASE}/collections/${id}`);
  }

  restoreCollection(id: string) {
    return this.http.patch(`${API_BASE}/collections/${id}/restore`, null);
  }

  toggleCollectionActive(id: string, isActive: boolean) {
    return this.http.patch(`${API_BASE}/collections/${id}/active`, isActive, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  setCollectionFeatured(id: string, isFeatured: boolean, order: number) {
    return this.http.patch(`${API_BASE}/collections/${id}/featured`, { isFeatured, order });
  }

  getNavDresses(collectionId: string) {
    return this.http.get<NavDressItem[]>(`${API_BASE}/collections/${collectionId}/nav-dresses`);
  }

  setNavOrder(collectionId: string, dressId: string, order: number | null) {
    return this.http.patch(`${API_BASE}/collections/${collectionId}/dresses/${dressId}/nav-order`, order, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  // ─── Appointments ────────────────────────────────────────────────
  getAllAppointments() {
    return this.http.get<AppointmentDto[]>(`${API_BASE}/appointments`);
  }

  createAppointment(dto: {
    firstName: string; lastName: string; phone: string; email: string | null;
    appointmentDateTime: string; type: number; notes: string | null; viewedDressIds: string[];
  }) {
    return this.http.post<AppointmentDto>(`${API_BASE}/appointments`, dto);
  }

  getAppointment(id: string) {
    return this.http.get<AppointmentDto>(`${API_BASE}/appointments/${id}`);
  }

  updateAppointmentStatus(id: string, status: number) {
    return this.http.patch(`${API_BASE}/appointments/${id}/status`, { status });
  }

  rescheduleAppointment(id: string, appointmentDateTime: string, force = false) {
    return this.http.patch(`${API_BASE}/appointments/${id}/reschedule`, { appointmentDateTime, force });
  }

  deleteAppointment(id: string) {
    return this.http.delete(`${API_BASE}/appointments/${id}`);
  }

  updateAdminNotes(id: string, adminNotes: string | null) {
    return this.http.patch(`${API_BASE}/appointments/${id}/admin-notes`, { adminNotes });
  }

  uploadAppointmentFile(id: string, file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<AppointmentFileDto>(`${API_BASE}/appointments/${id}/files`, form);
  }

  deleteAppointmentFile(appointmentId: string, fileId: string) {
    return this.http.delete(`${API_BASE}/appointments/${appointmentId}/files/${fileId}`);
  }

  // ─── Appointment Types ───────────────────────────────────────────
  getAppointmentTypes() {
    return this.http.get<AppointmentTypeConfigDto[]>(`${API_BASE}/appointment-types`);
  }

  createAppointmentType(dto: { name: string; price: number | null; description: string | null; mainDescription: string | null; detail: string | null; displayOrder: number; isActive: boolean }) {
    return this.http.post<AppointmentTypeConfigDto>(`${API_BASE}/appointment-types`, dto);
  }

  updateAppointmentType(id: number, dto: { name: string; price: number | null; description: string | null; mainDescription: string | null; detail: string | null; displayOrder: number; isActive: boolean }) {
    return this.http.put<void>(`${API_BASE}/appointment-types/${id}`, dto);
  }

  deleteAppointmentType(id: number) {
    return this.http.delete(`${API_BASE}/appointment-types/${id}`);
  }

  // ─── Schedule ────────────────────────────────────────────────────
  getTimeSlots() {
    return this.http.get<TimeSlotDto[]>(`${API_BASE}/schedule/time-slots`);
  }

  replaceTimeSlots(slots: string[]) {
    return this.http.put(`${API_BASE}/schedule/time-slots`, { slots });
  }

  getDaySchedule(date: string) {
    return this.http.get<DayScheduleDto>(`${API_BASE}/schedule/day/${date}`);
  }

  setDaySchedule(date: string, isClosed: boolean, customSlots: string[] | null) {
    return this.http.put(`${API_BASE}/schedule/day/${date}`, { isClosed, customSlots });
  }

  deleteDaySchedule(date: string) {
    return this.http.delete(`${API_BASE}/schedule/day/${date}`);
  }
}
