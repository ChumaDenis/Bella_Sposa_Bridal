import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ViewedDressesService {
  private readonly KEY = 'bs_viewed_dresses';
  private readonly MAX = 5;

  add(dressId: string): void {
    const ids = this.getIds().filter(id => id !== dressId);
    localStorage.setItem(this.KEY, JSON.stringify([dressId, ...ids].slice(0, this.MAX)));
  }

  getIds(): string[] {
    try { return JSON.parse(localStorage.getItem(this.KEY) ?? '[]'); }
    catch { return []; }
  }
}
