import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

export interface User {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private router: Router) {}

  isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    if (!token) return false;
    // Check if token is expired
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 > Date.now();
    } catch { return false; }
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUser(): User | null {
    const data = localStorage.getItem('user');
    if (data) return JSON.parse(data);
    return null;
  }

  getEmail(): string {
    return this.getUser()?.email || '';
  }

  getPhone(): string {
    return this.getUser()?.phone || '';
  }

  getFullName(): string {
    const u = this.getUser();
    if (u) return `${u.firstName || ''} ${u.lastName || ''}`.trim();
    return '';
  }

  saveSession(res: { token: string; userId: string; email: string; phone: string; firstName: string; lastName: string }) {
    localStorage.setItem('token', res.token);
    localStorage.setItem('user', JSON.stringify({
      userId: res.userId, email: res.email, phone: res.phone,
      firstName: res.firstName, lastName: res.lastName
    }));
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/']);
  }

  requireAuth(): boolean {
    if (!this.isAuthenticated()) {
      this.router.navigate(['/auth']);
      return false;
    }
    return true;
  }
}
