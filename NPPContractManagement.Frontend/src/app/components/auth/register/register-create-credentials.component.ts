import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { RegistrationService } from '../../../services/registration.service';
import { Subject, debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';

@Component({
  selector: 'app-register-create-credentials',
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
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="col-5">

          <h4 class="mb-3">Create Your Credentials</h4>
          <p class="text-muted mb-3">
            Welcome, <strong>{{ firstName }} {{ lastName }}</strong>! Choose a User ID and password for your account.
          </p>

          <div *ngIf="error" class="alert alert-danger mb-3">
            <i class="fa-solid fa-exclamation-triangle me-2"></i>
            {{ error }}
          </div>

          <!-- User ID -->
          <div class="mb-3 form-floating">
            <input
              type="text"
              class="form-control"
              [class.is-invalid]="form.get('userId')?.invalid && form.get('userId')?.touched"
              [class.is-valid]="form.get('userId')?.valid && form.get('userId')?.touched && userIdAvailable"
              id="userId"
              formControlName="userId"
              placeholder="Choose a User ID"
              name="userId">
            <label for="userId">User ID</label>
            <div *ngIf="form.get('userId')?.invalid && form.get('userId')?.touched" class="invalid-feedback">
              <span *ngIf="form.get('userId')?.hasError('required')">User ID is required.</span>
              <span *ngIf="form.get('userId')?.hasError('minlength')">User ID must be at least 3 characters.</span>
            </div>
            <div *ngIf="checkingUserId" class="form-text">
              <span class="spinner-border spinner-border-sm me-1"></span> Checking availability...
            </div>
            <div *ngIf="!checkingUserId && userIdAvailable === true && form.get('userId')?.valid" class="valid-feedback d-block">
              User ID is available.
            </div>
            <div *ngIf="!checkingUserId && userIdAvailable === false" class="invalid-feedback d-block">
              {{ userIdMessage || 'This User ID is already taken.' }}
            </div>
          </div>

          <!-- Password -->
          <div class="mb-3 form-floating">
            <input
              type="password"
              class="form-control"
              [class.is-invalid]="form.get('password')?.invalid && form.get('password')?.touched"
              id="password"
              formControlName="password"
              placeholder="Password"
              name="password">
            <label for="password">Password</label>
            <div *ngIf="form.get('password')?.invalid && form.get('password')?.touched" class="invalid-feedback">
              <span *ngIf="form.get('password')?.hasError('required')">Password is required.</span>
              <span *ngIf="form.get('password')?.hasError('minlength')">Password must be at least 8 characters.</span>
            </div>
            <!-- Password strength meter -->
            <div *ngIf="form.get('password')?.value" class="mt-1">
              <div class="progress" style="height: 4px;">
                <div class="progress-bar"
                  [class.bg-danger]="passwordStrength <= 1"
                  [class.bg-warning]="passwordStrength === 2"
                  [class.bg-info]="passwordStrength === 3"
                  [class.bg-success]="passwordStrength >= 4"
                  [style.width.%]="passwordStrength * 25">
                </div>
              </div>
              <small class="text-muted">
                {{ passwordStrengthLabel }}
              </small>
            </div>
          </div>

          <!-- Confirm Password -->
          <div class="mb-3 form-floating">
            <input
              type="password"
              class="form-control"
              [class.is-invalid]="form.get('confirmPassword')?.touched && form.hasError('passwordMismatch')"
              id="confirmPassword"
              formControlName="confirmPassword"
              placeholder="Confirm Password"
              name="confirmPassword">
            <label for="confirmPassword">Confirm Password</label>
            <div *ngIf="form.get('confirmPassword')?.touched && form.hasError('passwordMismatch')" class="invalid-feedback">
              Passwords do not match.
            </div>
          </div>

          <div class="d-flex justify-content-between">
            <a routerLink="/register" class="btn btn-outline-secondary">Cancel</a>
            <button
              type="submit"
              class="btn btn-denim"
              [disabled]="loading || form.invalid || !userIdAvailable">
              <span *ngIf="loading" class="spinner-border spinner-border-sm me-2"></span>
              {{ loading ? 'Creating...' : 'Complete Registration' }}
            </button>
          </div>

        </form>
      </div>
    </main>
  `
})
export class RegisterCreateCredentialsComponent implements OnInit {
  form: FormGroup;
  loading = false;
  error = '';
  firstName = '';
  lastName = '';
  registrationToken = '';

  checkingUserId = false;
  userIdAvailable: boolean | null = null;
  userIdMessage = '';

  private userIdCheck$ = new Subject<string>();

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private registrationService: RegistrationService
  ) {
    this.form = this.fb.group({
      userId: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    this.registrationToken = sessionStorage.getItem('registration_token') || '';
    this.firstName = sessionStorage.getItem('registration_firstName') || '';
    this.lastName = sessionStorage.getItem('registration_lastName') || '';

    if (!this.registrationToken) {
      this.router.navigate(['/register']);
      return;
    }

    // Real-time userId availability check
    this.userIdCheck$.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(userId => {
        if (!userId || userId.length < 3) {
          this.userIdAvailable = null;
          this.checkingUserId = false;
          return of(null);
        }
        this.checkingUserId = true;
        return this.registrationService.checkUserId(userId);
      })
    ).subscribe({
      next: (result) => {
        this.checkingUserId = false;
        if (result) {
          this.userIdAvailable = result.isAvailable;
          this.userIdMessage = result.message || '';
        }
      },
      error: () => {
        this.checkingUserId = false;
        this.userIdAvailable = null;
      }
    });

    // Subscribe to userId value changes
    this.form.get('userId')?.valueChanges.subscribe(value => {
      this.userIdAvailable = null;
      this.userIdCheck$.next(value || '');
    });
  }

  get passwordStrength(): number {
    const pwd = this.form.get('password')?.value || '';
    let score = 0;
    if (pwd.length >= 8) score++;
    if (/[a-z]/.test(pwd) && /[A-Z]/.test(pwd)) score++;
    if (/\d/.test(pwd)) score++;
    if (/[^a-zA-Z0-9]/.test(pwd)) score++;
    return score;
  }

  get passwordStrengthLabel(): string {
    switch (this.passwordStrength) {
      case 0: return 'Too short';
      case 1: return 'Weak';
      case 2: return 'Fair';
      case 3: return 'Good';
      case 4: return 'Strong';
      default: return '';
    }
  }

  passwordMatchValidator(control: AbstractControl) {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    if (password && confirmPassword && password !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.form.invalid || !this.userIdAvailable) return;

    this.loading = true;
    this.error = '';

    this.registrationService.completeRegistration(
      this.registrationToken,
      this.form.value.userId!,
      this.form.value.password!
    ).subscribe({
      next: () => {
        // Clear session data
        sessionStorage.removeItem('registration_email');
        sessionStorage.removeItem('registration_token');
        sessionStorage.removeItem('registration_firstName');
        sessionStorage.removeItem('registration_lastName');
        this.router.navigate(['/register/success']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message || 'Registration failed. Please try again.';
      }
    });
  }
}
