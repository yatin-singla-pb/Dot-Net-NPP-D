import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { UserService } from '../../../services/user.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <main class="container">
      <h3>My Account</h3>

      <div *ngIf="loading" class="text-center py-5">
        <div class="spinner-border" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>

      <div *ngIf="!loading && error && !profile" class="alert alert-danger mt-3">
        <i class="fa-solid fa-exclamation-triangle me-2"></i>{{ error }}
      </div>

      <form *ngIf="!loading && profile" [formGroup]="form" (ngSubmit)="onSave()" class="mt-3">
        <!-- Read-only info -->
        <div class="row g-3 mb-3">
          <div class="col-md-3">
            <label class="form-label">User ID</label>
            <input type="text" class="form-control" [value]="profile.userId || 'Pending Registration'" readonly disabled>
          </div>
          <div class="col-md-3">
            <label class="form-label">Role</label>
            <input type="text" class="form-control" [value]="roleNames" readonly disabled>
          </div>
          <div class="col-md-3">
            <label class="form-label">Account Status</label>
            <input type="text" class="form-control" [value]="accountStatusLabel" readonly disabled>
          </div>
          <div class="col-md-3">
            <label class="form-label">Last Login</label>
            <input type="text" class="form-control" [value]="profile.lastLoginDate ? (profile.lastLoginDate | date:'short') : '—'" readonly disabled>
          </div>
        </div>

        <hr>

        <!-- Editable fields -->
        <div class="row g-3">
          <div class="col-md-3">
            <label class="form-label">First Name <span class="text-danger">*</span></label>
            <input class="form-control" formControlName="firstName" [class.is-invalid]="isInvalid('firstName')">
            <div class="invalid-feedback" *ngIf="isInvalid('firstName')">First name is required.</div>
          </div>
          <div class="col-md-3">
            <label class="form-label">Last Name <span class="text-danger">*</span></label>
            <input class="form-control" formControlName="lastName" [class.is-invalid]="isInvalid('lastName')">
            <div class="invalid-feedback" *ngIf="isInvalid('lastName')">Last name is required.</div>
          </div>
          <div class="col-md-3">
            <label class="form-label">Email <span class="text-danger">*</span></label>
            <input class="form-control" formControlName="email" type="email" [class.is-invalid]="isInvalid('email')">
            <div class="invalid-feedback" *ngIf="isInvalid('email')">Valid email is required.</div>
          </div>
          <div class="col-md-3">
            <label class="form-label">Phone</label>
            <input class="form-control" formControlName="phoneNumber">
          </div>

          <div class="col-md-3">
            <label class="form-label">Company</label>
            <input class="form-control" formControlName="company">
          </div>
          <div class="col-md-3">
            <label class="form-label">Job Title</label>
            <input class="form-control" formControlName="jobTitle">
          </div>

          <div class="col-md-6">
            <label class="form-label">Address</label>
            <input class="form-control" formControlName="address">
          </div>
          <div class="col-md-2">
            <label class="form-label">City</label>
            <input class="form-control" formControlName="city">
          </div>
          <div class="col-md-2">
            <label class="form-label">State</label>
            <input class="form-control" formControlName="state">
          </div>
          <div class="col-md-2">
            <label class="form-label">Postal Code</label>
            <input class="form-control" formControlName="postCode">
          </div>
        </div>

        <div *ngIf="error" class="alert alert-danger mt-3">
          <i class="fa-solid fa-exclamation-triangle me-2"></i>{{ error }}
        </div>

        <div *ngIf="successMessage" class="alert alert-success mt-3">
          <i class="fa-solid fa-check-circle me-2"></i>{{ successMessage }}
        </div>

        <div class="mt-4 d-flex justify-content-end">
          <button class="btn btn-denim" type="submit" [disabled]="form.invalid || saving || form.pristine">
            {{ saving ? 'Saving...' : 'Save Changes' }}
          </button>
        </div>
      </form>
    </main>
  `
})
export class ProfileComponent implements OnInit {
  form!: FormGroup;
  loading = true;
  saving = false;
  error: string | null = null;
  successMessage: string | null = null;
  profile: any = null;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
      phoneNumber: [''],
      company: [''],
      jobTitle: [''],
      address: [''],
      city: [''],
      state: [''],
      postCode: ['']
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  get roleNames(): string {
    if (!this.profile?.roles?.length) return '—';
    return this.profile.roles.map((r: any) => r.name).join(', ');
  }

  get accountStatusLabel(): string {
    const status = this.profile?.accountStatus;
    switch (status) {
      case 0: return 'Active';
      case 1: return 'Active';
      case 2: return 'Locked';
      case 3: return 'Suspended';
      case 4: return 'Headless';
      default: return this.profile?.accountStatusName || 'Active';
    }
  }

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  private loadProfile(): void {
    this.loading = true;
    this.error = null;

    this.userService.getProfile().subscribe({
      next: (user) => {
        this.profile = user;
        this.form.patchValue({
          firstName: user.firstName || '',
          lastName: user.lastName || '',
          email: user.email || '',
          phoneNumber: (user as any).phoneNumber || (user as any).phone || '',
          company: (user as any).company || '',
          jobTitle: (user as any).jobTitle || (user as any).title || '',
          address: (user as any).address || '',
          city: (user as any).city || '',
          state: (user as any).state || '',
          postCode: (user as any).postCode || ''
        });
        this.form.markAsPristine();
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.message || 'Failed to load profile';
        this.loading = false;
      }
    });
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = null;
    this.successMessage = null;

    const payload = this.form.value;

    this.userService.updateProfile(payload).subscribe({
      next: (updatedUser) => {
        this.profile = updatedUser;
        this.form.markAsPristine();
        this.saving = false;
        this.successMessage = 'Profile updated successfully.';

        // Update stored user data so the header reflects changes
        const storedUser = this.authService.currentUser;
        if (storedUser) {
          storedUser.firstName = updatedUser.firstName;
          storedUser.lastName = updatedUser.lastName;
          storedUser.email = updatedUser.email;
          localStorage.setItem('npp_user', JSON.stringify(storedUser));
        }
      },
      error: (err) => {
        this.error = err?.error?.message || 'Failed to update profile';
        this.saving = false;
      }
    });
  }
}
