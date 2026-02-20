import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { DistributorService } from '../../services/distributor.service';
import { OpCoService } from '../../services/opco.service';
import { Distributor, DistributorStatus, CreateDistributorRequest, UpdateDistributorRequest } from '../../models/distributor.model';
import { OpCo } from '../../models/opco.model';
import { US_STATES } from '../../shared/utils/us-states.constants';

@Component({
  selector: 'app-distributor-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './distributor-form.component.html',
  styleUrls: ['./distributor-form.component.css']
})
export class DistributorFormComponent implements OnInit {
  form: FormGroup;
  distributor: Distributor | null = null;
  isEditMode = false;
  isViewMode = false;  // ADDED for view/edit mode pattern
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;
  nameValidationError: string | null = null;
  usStates = US_STATES;
  opcos: OpCo[] = [];

  DistributorStatus = DistributorStatus;

  constructor(
    private fb: FormBuilder,
    private distributorService: DistributorService,
    private opcoService: OpCoService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.createForm();
  }

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.error = 'You must be logged in to access this page.';
      setTimeout(() => this.router.navigate(['/login']), 1500);
      return;
    }

    // Check if we're in view mode or edit mode
    const url = this.router.url;
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      if (url.includes('/view/')) {
        this.isViewMode = true;
        this.isEditMode = true;
      } else if (url.includes('/edit/')) {
        this.isViewMode = false;
        this.isEditMode = true;
      }
      this.loadDistributor(+id);
    }

    this.form.get('name')?.valueChanges.subscribe(value => {
      if (value && value.length >= 2) {
        this.validateName(value);
      } else {
        this.nameValidationError = null;
      }
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(200),
        Validators.pattern(/^[a-zA-Z0-9\s\-&.,()]+$/)
      ]],
      description: ['', [Validators.maxLength(500)]],
      contactPerson: ['', [Validators.maxLength(200)]],
      email: ['', [Validators.email, Validators.maxLength(255)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      address: ['', [Validators.maxLength(500)]],
      city: ['', [Validators.maxLength(100)]],
      state: ['', [Validators.maxLength(100)]],
      zipCode: ['', [Validators.maxLength(20)]],
      country: ['', [Validators.maxLength(100)]],
      website: ['', [Validators.maxLength(255)]],
      receiveContractProposal: [true],
      status: ['Active', [Validators.required]]
    });
  }

  private loadDistributor(id: number): void {
    this.loading = true;
    this.error = null;

    this.distributorService.getById(id).subscribe({
      next: (d) => {
        this.distributor = d;
        this.populateForm(d);
        this.loadOpCos(d.id);

        // Disable form if in view mode
        if (this.isViewMode) {
          this.form.disable();
        }

        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load distributor. Please try again.';
        this.loading = false;
        console.error('Error loading distributor:', err);
      }
    });
  }

  private populateForm(d: Distributor): void {
    this.form.patchValue({
      name: d.name,
      description: d.description || '',
      contactPerson: d.contactPerson || '',
      email: d.email || '',
      phoneNumber: d.phoneNumber || '',
      address: d.address || '',
      city: d.city || '',
      state: d.state || '',
      zipCode: d.zipCode || '',
      country: d.country || '',
      website: d.website || '',
      receiveContractProposal: d.receiveContractProposal ?? true,
      status: d.status
    });
  }

  private loadOpCos(distributorId: number): void {
    this.opcoService.getByDistributor(distributorId).subscribe({
      next: (items) => { this.opcos = items || []; },
      error: () => { this.opcos = []; }
    });
  }

  private validateName(name: string): void {
    // Placeholder for server-side name validation if available later
    this.nameValidationError = null;
  }

  onSubmit(): void {
    if (this.form.invalid || this.nameValidationError) {
      this.markFormGroupTouched();
      return;
    }

    this.submitting = true;
    this.error = null;
    this.successMessage = null;

    const formValue = this.form.value;

    if (this.isEditMode && this.distributor) {
      this.updateDistributor(formValue);
    } else {
      this.createDistributor(formValue);
    }
  }

  private createDistributor(formValue: any): void {
    const request: CreateDistributorRequest = {
      name: formValue.name.trim(),
      description: formValue.description?.trim() || undefined,
      contactPerson: formValue.contactPerson?.trim() || undefined,
      email: formValue.email?.trim() || undefined,
      phoneNumber: formValue.phoneNumber?.trim() || undefined,
      address: formValue.address?.trim() || undefined,
      city: formValue.city?.trim() || undefined,
      state: formValue.state?.trim() || undefined,
      zipCode: formValue.zipCode?.trim() || undefined,
      country: formValue.country?.trim() || undefined,
      website: formValue.website?.trim() || undefined,
      receiveContractProposal: !!formValue.receiveContractProposal,
      status: formValue.status as DistributorStatus
    };

    this.distributorService.create(request).subscribe({
      next: () => {
        this.successMessage = 'Distributor created successfully!';
        this.submitting = false;
        setTimeout(() => this.router.navigate(['/admin/distributors']), 1000);
      },
      error: (error) => {
        this.handleHttpError(error, 'create');
        this.submitting = false;
      }
    });
  }

  private updateDistributor(formValue: any): void {
    if (!this.distributor) return;

    const request: UpdateDistributorRequest = {
      name: formValue.name.trim(),
      description: formValue.description?.trim(),
      contactPerson: formValue.contactPerson?.trim(),
      email: formValue.email?.trim(),
      phoneNumber: formValue.phoneNumber?.trim(),
      address: formValue.address?.trim(),
      city: formValue.city?.trim(),
      state: formValue.state?.trim(),
      zipCode: formValue.zipCode?.trim(),
      country: formValue.country?.trim(),
      website: formValue.website?.trim(),
      receiveContractProposal: !!formValue.receiveContractProposal,
      status: formValue.status as DistributorStatus,
      isActive: this.distributor.isActive  // Preserve the existing isActive value
    };

    this.distributorService.update(this.distributor.id, request).subscribe({
      next: (updatedDistributor) => {
        this.submitting = false;
        this.successMessage = 'Distributor updated successfully!';
        this.distributor = updatedDistributor;

        setTimeout(() => {
          this.isViewMode = true;
          this.form.disable();
          this.populateForm(updatedDistributor);
          this.router.navigate(['/admin/distributors/view', this.distributor!.id], { replaceUrl: true });
          this.successMessage = null;
        }, 1000);
      },
      error: (error) => {
        this.handleHttpError(error, 'update');
        this.submitting = false;
      }
    });
  }

  toggleEditMode(): void {
    this.isViewMode = false;
    this.form.enable();
    if (this.distributor) {
      this.router.navigate(['/admin/distributors/edit', this.distributor.id], { replaceUrl: true });
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.form.disable();
    if (this.distributor) {
      this.populateForm(this.distributor);
      this.router.navigate(['/admin/distributors/view', this.distributor.id], { replaceUrl: true });
    }
  }

  onCancel(): void {
    if (this.isViewMode) {
      // In view mode, just go back to list
      this.router.navigate(['/admin/distributors']);
    } else if (this.isEditMode) {
      // In edit mode, cancel back to view mode
      if (this.form.dirty) {
        if (confirm('You have unsaved changes. Are you sure you want to cancel?')) {
          this.cancelToViewMode();
        }
      } else {
        this.cancelToViewMode();
      }
    } else {
      // In create mode, go back to list
      if (this.form.dirty) {
        if (confirm('You have unsaved changes. Are you sure you want to leave?')) {
          this.router.navigate(['/admin/distributors']);
        }
      } else {
        this.router.navigate(['/admin/distributors']);
      }
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.form.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  private markFormGroupTouched(): void {
    Object.keys(this.form.controls).forEach(key => {
      const control = this.form.get(key);
      control?.markAsTouched();
    });
  }

  private handleHttpError(error: any, action: 'create' | 'update') {
    if (error.status === 401) {
      this.error = 'Unauthorized. Please log in again.';
      setTimeout(() => this.router.navigate(['/login']), 1500);
    } else if (error.status === 403) {
      this.error = `You do not have permission to ${action} distributors.`;
    } else if (error.status === 400) {
      this.error = error.error?.message || 'Invalid data provided. Please check your input.';
    } else if (error.status === 0) {
      this.error = 'Unable to connect to the server. Please check if the API is running.';
    } else {
      this.error = error.error?.message || error.message || 'An error occurred. Please try again.';
    }
  }
}

