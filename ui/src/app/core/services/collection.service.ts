import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CollectionDto } from '../models/collection.model';
import { API_BASE } from './api.config';

@Injectable({ providedIn: 'root' })
export class CollectionService {
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<CollectionDto[]>(`${API_BASE}/collections`);
  }

  getById(id: string) {
    return this.http.get<CollectionDto>(`${API_BASE}/collections/${id}`);
  }
}
