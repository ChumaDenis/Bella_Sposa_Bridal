import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { shareReplay } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { SilhouetteTypeDto } from '../models/silhouette.model';
import { API_BASE } from './api.config';

@Injectable({ providedIn: 'root' })
export class SilhouetteService {
  private http = inject(HttpClient);

  private cache$: Observable<SilhouetteTypeDto[]> | null = null;

  getAll(): Observable<SilhouetteTypeDto[]> {
    if (!this.cache$) {
      this.cache$ = this.http.get<SilhouetteTypeDto[]>(`${API_BASE}/silhouettes`).pipe(shareReplay(1));
    }
    return this.cache$;
  }

  invalidate() { this.cache$ = null; }

  create(name: string) {
    this.invalidate();
    return this.http.post<SilhouetteTypeDto>(`${API_BASE}/silhouettes`, { name });
  }

  delete(id: number) {
    this.invalidate();
    return this.http.delete(`${API_BASE}/silhouettes/${id}`, { observe: 'response' });
  }

  rename(id: number, name: string) {
    this.invalidate();
    return this.http.put<SilhouetteTypeDto>(`${API_BASE}/silhouettes/${id}`, { name });
  }

  reorder(ids: number[]) {
    this.invalidate();
    return this.http.put(`${API_BASE}/silhouettes/reorder`, { ids });
  }
}
