import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DressListDto, DressDetailDto } from '../models/dress.model';
import { API_BASE } from './api.config';

@Injectable({ providedIn: 'root' })
export class DressService {
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<DressListDto[]>(`${API_BASE}/dresses`);
  }

  getById(id: string) {
    return this.http.get<DressDetailDto>(`${API_BASE}/dresses/${id}`);
  }

  getByCollection(collectionId: string) {
    return this.http.get<DressListDto[]>(`${API_BASE}/dresses/collection/${collectionId}`);
  }
}
