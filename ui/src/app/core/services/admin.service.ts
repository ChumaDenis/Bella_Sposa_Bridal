import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from './api.config';
import { DressListDto, DressDetailDto, DressPhoto } from '../models/dress.model';

export interface AddPhotoPayload {
  url: string;
  altText?: string;
  type: number;
  order: number;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);

  getAllDresses() {
    return this.http.get<DressListDto[]>(`${API_BASE}/dresses/admin`);
  }

  getDress(id: string) {
    return this.http.get<DressDetailDto>(`${API_BASE}/dresses/${id}`);
  }

  uploadPhoto(file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<{ url: string }>(`${API_BASE}/upload`, form);
  }

  addPhoto(dressId: string, payload: AddPhotoPayload) {
    return this.http.post<DressPhoto>(`${API_BASE}/dresses/${dressId}/photos`, payload);
  }

  deletePhoto(dressId: string, photoId: string) {
    return this.http.delete(`${API_BASE}/dresses/${dressId}/photos/${photoId}`);
  }

  toggleActive(dressId: string, isActive: boolean) {
    return this.http.patch(`${API_BASE}/dresses/${dressId}/active`, isActive, {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
