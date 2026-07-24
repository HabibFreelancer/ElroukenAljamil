import { Component } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../services/auth-api.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule, RouterLink],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {
  step: 'welcome' | 'login' | 'login-password' | 'account-type' | 'verify-email' | 'password' | 'phone' | 'verify-phone' = 'welcome';

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

  get hasMinLength() { return this.password.length >= 8; }
  get hasMaxLength() { return this.password.length <= 50; }
  get hasDigit() { return /\d/.test(this.password); }
  get hasLower() { return /[a-z]/.test(this.password); }
  get hasUpper() { return /[A-Z]/.test(this.password); }
  get hasSpecial() { return /[^a-zA-Z0-9]/.test(this.password); }
  get passwordValid() { return this.hasMinLength && this.hasMaxLength && this.hasDigit && this.hasLower && this.hasUpper && this.hasSpecial; }

  constructor(private authApi: AuthApiService, private router: Router) {}

  goToLogin() { this.step = 'login'; }
  goToRegister() { this.step = 'login'; }
  quitToHome() { this.router.navigate(['/']); }

  submitEmail() {
    this.error = '';
    if (!this.email || !this.email.includes('@')) { this.error = 'Veuillez saisir un email valide.'; return; }
    this.loading = true;
    this.authApi.checkEmail(this.email).subscribe({
      next: (res) => {
        this.loading = false;
        this.step = res.exists ? 'login-password' : 'account-type';
      },
      error: () => { this.loading = false; this.error = 'Erreur serveur.'; }
    });
  }

  selectAccountType(type: string) {
    this.accountType = type;
    this.loading = true;
    this.authApi.sendCode(this.email).subscribe({
      next: (res) => { this.loading = false; this.maskedEmail = res.maskedEmail; this.step = 'verify-email'; },
      error: () => { this.loading = false; this.error = 'Erreur envoi code.'; }
    });
  }

  onEmailCodeInput(index: number, event: any) {
    const val = event.target.value;
    if (val && index < 4) { const next = event.target.nextElementSibling; if (next) next.focus(); }
    if (this.emailCode.every(c => c)) this.verifyEmailCode();
  }

  verifyEmailCode() {
    this.loading = true;
    this.error = '';
    this.authApi.verifyCode(this.email, this.emailCode.join('')).subscribe({
      next: () => { this.loading = false; this.step = 'password'; },
      error: () => { this.loading = false; this.error = 'Code invalide ou expiré.'; }
    });
  }

  resendEmailCode() {
    this.authApi.sendCode(this.email).subscribe();
  }

  submitPassword() {
    if (!this.passwordValid) return;
    this.loading = true;
    this.error = '';
    this.authApi.register(this.email, this.password, this.accountType).subscribe({
      next: () => { this.loading = false; this.step = 'phone'; },
      error: (err) => { this.loading = false; this.error = err.error?.message || 'Erreur serveur.'; }
    });
  }

  submitPhone() {
    if (!this.phone || this.phone.length < 8) { this.error = 'Numéro de téléphone invalide.'; return; }
    this.loading = true;
    this.error = '';
    this.authApi.sendSmsCode(this.email, '+216' + this.phone).subscribe({
      next: () => { this.loading = false; this.step = 'verify-phone'; },
      error: () => { this.loading = false; this.error = 'Erreur envoi SMS.'; }
    });
  }

  onPhoneCodeInput(index: number, event: any) {
    const val = event.target.value;
    if (val && index < 5) { const next = event.target.nextElementSibling; if (next) next.focus(); }
    if (this.phoneCode.every(c => c)) this.verifyPhoneCode();
  }

  verifyPhoneCode() {
    this.loading = true;
    this.error = '';
    this.authApi.verifyPhone(this.email, this.phoneCode.join('')).subscribe({
      next: () => { this.loading = false; this.router.navigate(['/deposer']); },
      error: () => { this.loading = false; this.error = 'Code invalide ou expiré.'; }
    });
  }

  resendSmsCode() {
    this.authApi.sendSmsCode(this.email, '+216' + this.phone).subscribe();
  }

  loginWithPassword() {
    if (!this.password) { this.error = 'Mot de passe requis.'; return; }
    this.loading = true;
    this.error = '';
    this.authApi.login(this.email, this.password).subscribe({
      next: () => { this.loading = false; this.router.navigate(['/deposer']); },
      error: () => { this.loading = false; this.error = 'Email ou mot de passe incorrect.'; }
    });
  }

  goBack() {
    this.error = '';
    if (this.step === 'login') this.step = 'welcome';
    else if (this.step === 'login-password') this.step = 'login';
    else if (this.step === 'account-type') this.step = 'login';
    else if (this.step === 'verify-email') this.step = 'account-type';
    else if (this.step === 'password') this.step = 'verify-email';
    else if (this.step === 'phone') this.step = 'password';
    else if (this.step === 'verify-phone') this.step = 'phone';
  }
}
