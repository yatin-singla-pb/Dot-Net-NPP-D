import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { RegistrationService } from '../../../services/registration.service';

@Component({
  selector: 'app-register-verify-code',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <header class="container fixed-top">
      <div class="d-flex">
        <img src="assets/images/npp_logo_denim.jpg" alt="NPP Logo" />
      </div>
    </header>

    <main class="container">
      <div class="row justify-content-center">
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="col-4">

          <h4 class="mb-3">Enter Verification Code</h4>
          <p class="text-muted mb-3">
            A 6-digit verification code has been sent to <strong>{{ email }}</strong>.
            Enter it below to continue.
          </p>

          <div *ngIf="error" class="alert alert-danger mb-3">
            <i class="fa-solid fa-exclamation-triangle me-2"></i>
            {{ error }}
          </div>

          <div *ngIf="resendSuccess" class="alert alert-success mb-3">
            <i class="fa-solid fa-check-circle me-2"></i>
            A new verification code has been sent to your email.
          </div>

          <div class="mb-3 form-floating">
            <input
              type="text"
              class="form-control text-center"
              [class.is-invalid]="form.get('code')?.invalid && form.get('code')?.touched"
              id="code"
              formControlName="code"
              placeholder="000000"
              maxlength="6"
              style="font-size: 24px; letter-spacing: 8px; font-family: monospace;"
              name="code">
            <label for="code">Verification Code</label>
            <div *ngIf="form.get('code')?.invalid && form.get('code')?.touched" class="invalid-feedback">
              Please enter a 6-digit code.
            </div>
          </div>

          <div class="d-flex justify-content-between">
            <button
              type="button"
              class="btn btn-outline-secondary"
              (click)="resendCode()"
              [disabled]="resendLoading">
              <span *ngIf="resendLoading" class="spinner-border spinner-border-sm me-2"></span>
              Resend Code
            </button>
            <button
              type="submit"
              class="btn btn-denim"
              [disabled]="loading || form.invalid">
              <span *ngIf="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ loading ? 'Verifying...' : 'Verify Code' }}
            </button>
          </div>

          <div class="text-center mt-3">
            <a routerLink="/register" class="text-muted">Start over</a>
          </div>

        </form>
      </div>
    </main>
  `
})
export class RegisterVerifyCodeComponent implements OnInit {
  form: FormGroup;
  loading = false;
  resendLoading = false;
  error = '';
  resendSuccess = false;
  email = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private registrationService: RegistrationService
  ) {
    this.form = this.fb.group({
      code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6), Validators.pattern(/^\d{6}$/)]]
    });
  }

  ngOnInit(): void {
    this.email = sessionStorage.getItem('registration_email') || '';
    if (!this.email) {
      this.router.navigate(['/register']);
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.error = '';
    this.resendSuccess = false;

    this.registrationService.verifyCode(this.email, this.form.value.code!).subscribe({
      next: (response) => {
        sessionStorage.setItem('registration_token', response.registrationToken);
        sessionStorage.setItem('registration_firstName', response.firstName);
        sessionStorage.setItem('registration_lastName', response.lastName);
        this.router.navigate(['/register/credentials']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message || 'Invalid or expired verification code. Please try again.';
      }
    });
  }

  resendCode(): void {
    this.resendLoading = true;
    this.error = '';
    this.resendSuccess = false;

    this.registrationService.initiateRegistration(this.email).subscribe({
      next: () => {
        this.resendLoading = false;
        this.resendSuccess = true;
      },
      error: () => {
        this.resendLoading = false;
        this.resendSuccess = true; // Always show success to prevent email enumeration
      }
    });
  }
}
