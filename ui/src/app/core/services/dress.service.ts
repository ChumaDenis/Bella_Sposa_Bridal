import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { DressListDto, DressDetailDto, DressFilterMeta, PagedResult } from '../models/dress.model';
import { API_BASE } from './api.config';

export interface DressFilterParams {
  page?: number;
  pageSize?: number;
  collectionId?: string;
  silhouette?: number;
  size?: string;
}

@Injectable({ providedIn: 'root' })
export class DressService {
  private http = inject(HttpClient);

  getAll(params: DressFilterParams = {}) {
    let p = new HttpParams()
      .set('page', String(params.page ?? 1))
      .set('pageSize', String(params.pageSize ?? 12));
    if (params.collectionId) p = p.set('collectionId', params.collectionId);
    if (params.silhouette !== undefined && params.silhouette !== null)
      p = p.set('silhouette', String(params.silhouette));
    if (params.size) p = p.set('size', params.size);
    return this.http.get<PagedResult<DressListDto>>(`${API_BASE}/dresses`, { params: p });
  }

  getMeta() {
    return this.http.get<DressFilterMeta>(`${API_BASE}/dresses/meta`);
  }

  getById(id: string) {
    return this.http.get<DressDetailDto>(`${API_BASE}/dresses/${id}`);
  }

  getBySlug(slug: string) {
    return this.http.get<DressDetailDto>(`${API_BASE}/dresses/by/${slug}`);
  }

  getByCollection(collectionId: string, page = 1, pageSize = 12) {
    const p = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));
    return this.http.get<PagedResult<DressListDto>>(
      `${API_BASE}/dresses/collection/${collectionId}`, { params: p }
    );
  }

  getHomepageFeatured() {
    return this.http.get<DressListDto[]>(`${API_BASE}/dresses/homepage`);
  }
}
