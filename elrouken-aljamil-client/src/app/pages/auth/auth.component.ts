import { Component } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule, RouterLink],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {
  step: 'welcome' | 'login' | 'account-type' | 'verify-email' | 'password' | 'phone' | 'verify-phone' = 'welcome';
  private apiUrl = 'https://localhost:7283/api/auth';

  email = '';
  maskedEmail = '';
  emailCode = ['', '', '', '', ''];
  password = '';
  showPassword = false;
  accountType = 'personal';
  phone = '';
  phoneCode = ['', '', '', '', '', ''];
  showPhoneModal = false;
  error = '';
  loading = false;

  // Password rules
  get hasMinLength() { return this.password.length >= 8; }
  get hasMaxLength() { return this.password.length <= 50; }
  get hasDigit() { return /\d/.test(this.password); }
  get hasLower() { return /[a-z]/.test(this.password); }
  get hasUpper() { return /[A-Z]/.test(this.password); }
  get hasSpecial() { return /[^a-zA-Z0-9]/.test(this.password); }
  get passwordValid() { return this.hasMinLength && this.hasMaxLength && this.hasDigit && this.hasLower && this.hasUpper && this.hasSpecial; }

  constructor(private http: HttpClient, private router: Router) {}

  // Step: Welcome
  goToLogin() { this.step = 'login'; }
  goToRegister() { this.step = 'login'; }
  quitToHome() { this.router.navigate(['/']); }

  // Step: Login - check email
  submitEmail() {
    this.error = '';
    if (!this.email || !this.email.includes('@')) {
      this.error = 'Veuillez saisir un email valide.';
      return;
    }
    this.loading = true;
    this.http.post<any>(`${this.apiUrl}/check-email`, { email: this.email }).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.exists) {
          // User exists -> login flow (simplified: go to password directly)
          this.step = 'password';
        } else {
          // New user -> registration flow
          this.step = 'account-type';
        }
      },
      error: () => { this.loading = false; this.error = 'Erreur serveur.'; }
    });
  }

  // Step: Account type -> send email code
  selectAccountType(type: string) {
    this.accountType = type;
    this.loading = true;
    this.http.post<any>(`${this.apiUrl}/send-code`, { email: this.email }).subscribe({
      next: (res) => {
        this.loading = false;
        this.maskedEmail = res.maskedEmail;
        this.step = 'verify-email';
      },
      error: () => { this.loading = false; this.error = 'Erreur envoi code.'; }
    });
  }

  // Step: Verify email code
  onEmailCodeInput(index: number, event: any) {
    const val = event.target.value;
    if (val && index < 4) {
      const next = event.target.nextElementSibling;
      if (next) next.focus();
    }
    // Auto-submit when all filled
    if (this.emailCode.every(c => c)) {
      this.verifyEmailCode();
    }
  }

  verifyEmailCode() {
    const code = this.emailCode.join('');
    this.loading = true;
    this.error = '';
    this.http.post<any>(`${this.apiUrl}/verify-code`, { email: this.email, code }).subscribe({
      next: () => { this.loading = false; this.step = 'password'; },
      error: () => { this.loading = false; this.error = 'Code invalide ou expiré.'; }
    });
  }

  resendEmailCode() {
    this.http.post<any>(`${this.apiUrl}/send-code`, { email: this.email }).subscribe();
  }

  // Step: Set password
  submitPassword() {
    if (!this.passwordValid) return;
    this.loading = true;
    this.error = '';
    this.http.post<any>(`${this.apiUrl}/set-password`, { email: this.email, password: this.password, accountType: this.accountType }).subscribe({
      next: () => { this.loading = false; this.step = 'phone'; },
      error: () => { this.loading = false; this.error = 'Erreur serveur.'; }
    });
  }

  // Step: Phone
  submitPhone() {
    if (!this.phone || this.phone.length < 8) {
      this.error = 'Numéro de téléphone invalide.';
      return;
    }
    this.loading = true;
    this.error = '';
    this.http.post<any>(`${this.apiUrl}/send-sms-code`, { email: this.email, phone: '+216' + this.phone }).subscribe({
      next: () => { this.loading = false; this.step = 'verify-phone'; },
      error: () => { this.loading = false; this.error = 'Erreur envoi SMS.'; }
    });
  }

  // Step: Verify phone
  onPhoneCodeInput(index: number, event: any) {
    const val = event.target.value;
    if (val && index < 5) {
      const next = event.target.nextElementSibling;
      if (next) next.focus();
    }
    if (this.phoneCode.every(c => c)) {
      this.verifyPhoneCode();
    }
  }

  verifyPhoneCode() {
    const code = this.phoneCode.join('');
    this.loading = true;
    this.error = '';
    this.http.post<any>(`${this.apiUrl}/verify-phone`, { email: this.email, code }).subscribe({
      next: (res) => {
        this.loading = false;
        // Store user in localStorage (mock session)
        localStorage.setItem('user', JSON.stringify(res));
        this.router.navigate(['/deposer']);
      },
      error: () => { this.loading = false; this.error = 'Code invalide ou expiré.'; }
    });
  }

  resendSmsCode() {
    this.http.post<any>(`${this.apiUrl}/send-sms-code`, { email: this.email, phone: '+216' + this.phone }).subscribe();
  }

  // Login existing user
  loginWithPassword() {
    if (!this.password) { this.error = 'Mot de passe requis.'; return; }
    this.loading = true;
    this.error = '';
    this.http.post<any>(`${this.apiUrl}/login`, { email: this.email, password: this.password }).subscribe({
      next: (res) => {
        this.loading = false;
        localStorage.setItem('user', JSON.stringify(res));
        this.router.navigate(['/deposer']);
      },
      error: () => { this.loading = false; this.error = 'Email ou mot de passe incorrect.'; }
    });
  }

  goBack() {
    if (this.step === 'login') this.step = 'welcome';
    else if (this.step === 'account-type') this.step = 'login';
    else if (this.step === 'verify-email') this.step = 'account-type';
    else if (this.step === 'password') this.step = 'verify-email';
    else if (this.step === 'phone') this.step = 'password';
    else if (this.step === 'verify-phone') this.step = 'phone';
  }
}
