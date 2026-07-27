import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

export interface CheckEmailResponse { exists: boolean; }
export interface SendCodeResponse { maskedEmail: string; }
export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  phone: string;
  firstName: string;
  lastName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  checkEmail(email: string): Observable<CheckEmailResponse> {
    return this.http.post<CheckEmailResponse>(`${this.apiUrl}/check-email`, { email });
  }

  sendCode(email: string): Observable<SendCodeResponse> {
    return this.http.post<SendCodeResponse>(`${this.apiUrl}/send-code`, { email });
  }

  verifyCode(email: string, code: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/verify-code`, { email, code });
  }

  register(email: string, password: string, accountType: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/register`, { email, password, accountType });
  }

  sendSmsCode(email: string, phone: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/send-sms-code`, { email, phone });
  }

  verifyPhone(email: string, code: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/verify-phone`, { email, code }).pipe(
      tap(res => this.authService.saveSession(res))
    );
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap(res => this.authService.saveSession(res))
    );
  }
}
