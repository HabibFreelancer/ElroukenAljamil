import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private router: Router) {}

  isAuthenticated(): boolean {
    return !!localStorage.getItem('user');
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
    if (u) return `${u.firstName} ${u.lastName}`.trim();
    return '';
  }

  logout() {
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
