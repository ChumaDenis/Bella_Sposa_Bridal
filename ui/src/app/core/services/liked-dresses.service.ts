import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LikedDressesService {
  private readonly KEY = 'bs_liked_dresses';

  toggle(id: string): boolean {
    const ids = this.getIds();
    const idx = ids.indexOf(id);
    if (idx > -1) {
      ids.splice(idx, 1);
    } else {
      ids.unshift(id);
    }
    localStorage.setItem(this.KEY, JSON.stringify(ids));
    return idx === -1; // true = now liked
  }

  isLiked(id: string): boolean {
    return this.getIds().includes(id);
  }

  getIds(): string[] {
    try { return JSON.parse(localStorage.getItem(this.KEY) ?? '[]'); }
    catch { return []; }
  }

  hasAny(): boolean {
    return this.getIds().length > 0;
  }
}
