import { Injectable } from '@angular/core';

export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUser: User = {
    id: 1,
    email: 'habib.benradhouene@gmail.com',
    firstName: 'Habib',
    lastName: 'Ben Radhouene',
    phone: '55123456'
  };

  isAuthenticated(): boolean {
    return true;
  }

  getUser(): User {
    return this.currentUser;
  }

  getEmail(): string {
    return this.currentUser.email;
  }

  getPhone(): string {
    return this.currentUser.phone;
  }

  getFullName(): string {
    return `${this.currentUser.firstName} ${this.currentUser.lastName}`;
  }
}
