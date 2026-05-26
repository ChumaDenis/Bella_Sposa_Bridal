import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { API_BASE } from './api.config';

const TOKEN_KEY = 'bella_admin_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);

  login(username: string, password: string) {
    return this.http
      .post<{ token: string }>(`${API_BASE}/auth/login`, { username, password })
      .pipe(tap(res => localStorage.setItem(TOKEN_KEY, res.token)));
  }

  logout() {
    localStorage.removeItem(TOKEN_KEY);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem(TOKEN_KEY);
  }
}
