import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AtlierInfoDto } from '../models/atlier.model';
import { API_BASE } from './api.config';

@Injectable({ providedIn: 'root' })
export class AtlierService {
  private http = inject(HttpClient);

  getInfo() {
    return this.http.get<AtlierInfoDto>(`${API_BASE}/atlier`);
  }

  upsert(dto: AtlierInfoDto) {
    return this.http.put<AtlierInfoDto>(`${API_BASE}/atlier`, dto);
  }
}
