import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { RegistrationService } from '../../../services/registration.service';

@Component({
  selector: 'app-register-email',
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

          <h4 class="mb-3">Register Your Account</h4>
          <p class="text-muted mb-3">
            Enter the email address associated with your account to receive a verification code.
          </p>

          <div *ngIf="error" class="alert alert-danger mb-3">
            <i class="fa-solid fa-exclamation-triangle me-2"></i>
            {{ error }}
          </div>

          <div *ngIf="success" class="alert alert-success mb-3">
            <i class="fa-solid fa-check-circle me-2"></i>
            {{ success }}
          </div>

          <div class="mb-3 form-floating">
            <input
              type="email"
              class="form-control"
              [class.is-invalid]="form.get('email')?.invalid && form.get('email')?.touched"
              id="email"
              formControlName="email"
              placeholder="Enter your email"
              name="email">
            <label for="email">Email Address</label>
            <div *ngIf="form.get('email')?.invalid && form.get('email')?.touched" class="invalid-feedback">
              Please enter a valid email address.
            </div>
          </div>

          <div class="d-flex justify-content-between">
            <a routerLink="/login" class="btn btn-outline-secondary">Back to Login</a>
            <button
              type="submit"
              class="btn btn-denim"
              [disabled]="loading || form.invalid">
              <span *ngIf="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ loading ? 'Sending...' : 'Send Verification Code' }}
            </button>
          </div>

        </form>
      </div>
    </main>
  `
})
export class RegisterEmailComponent {
  form: FormGroup;
  loading = false;
  error = '';
  success = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private registrationService: RegistrationService
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.error = '';
    this.success = '';

    const email = this.form.value.email!;

    this.registrationService.initiateRegistration(email).subscribe({
      next: () => {
        sessionStorage.setItem('registration_email', email);
        this.success = 'If your email is registered, a verification code has been sent.';
        setTimeout(() => {
          this.router.navigate(['/register/verify']);
        }, 1500);
      },
      error: () => {
        this.loading = false;
        // Always show success message to prevent email enumeration
        this.success = 'If your email is registered, a verification code has been sent.';
        sessionStorage.setItem('registration_email', email);
        setTimeout(() => {
          this.router.navigate(['/register/verify']);
        }, 1500);
      }
    });
  }
}
