import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { OpCoService } from '../../services/opco.service';
import { DistributorService } from '../../services/distributor.service';
import { CustomerAccountService } from '../../services/customer-account.service';
import { CustomerAccount } from '../../models/customer-account.model';
import { OpCo, OpCoStatus, CreateOpCoRequest, UpdateOpCoRequest } from '../../models/opco.model';
import { Distributor } from '../../models/distributor.model';
import { US_STATES } from '../../shared/utils/us-states.constants';

@Component({
  selector: 'app-op-co-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './op-co-form.component.html',
  styleUrls: ['./op-co-form.component.css']
})
export class OpCoFormComponent implements OnInit {
  form: FormGroup;
  opco: OpCo | null = null;
  customerAccounts: CustomerAccount[] = [];
  isEditMode = false;
  isViewMode = false;  // ADDED for view/edit mode pattern
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;
  remoteCodeValidationError: string | null = null;
  usStates = US_STATES;

  distributors: Distributor[] = [];
  OpCoStatus = OpCoStatus;

  constructor(
    private fb: FormBuilder,
    private opCoService: OpCoService,
    private distributorService: DistributorService,
    private customerAccountService: CustomerAccountService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.createForm();
  }

  ngOnInit(): void {
    // Load distributors for dropdown
    this.distributorService.getPaginated(1, 1000, 'name', 'asc').subscribe({
      next: (resp: any) => this.distributors = resp?.items ?? [],
      error: () => this.distributors = []
    });

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
      this.loadOpCo(+id);
    }

    this.form.get('remoteReferenceCode')?.valueChanges.subscribe(value => {
      this.remoteCodeValidationError = null;
      const code = (value || '').toString().trim();
      if (code.length >= 2) {
        this.validateRemoteReferenceCode(code);
      }
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
      remoteReferenceCode: ['', [Validators.required, Validators.maxLength(100)]],
      distributorId: [null, [Validators.required]],
      contactPerson: ['', [Validators.maxLength(200)]],
      email: ['', [Validators.email, Validators.maxLength(255)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      internalNotes: ['', [Validators.maxLength(1000)]],
      address: ['', [Validators.maxLength(500)]],
      city: ['', [Validators.maxLength(100)]],
      state: ['', [Validators.maxLength(100)]],
      zipCode: ['', [Validators.maxLength(20)]],
      country: ['', [Validators.maxLength(100)]],
      status: ['Active', [Validators.required]]
    });
  }

  private loadOpCo(id: number): void {
    this.loading = true;
    this.error = null;
    this.opCoService.getById(id).subscribe({
      next: (o) => {
        this.opco = o;
        this.loadCustomerAccounts(o.id);
        this.populateForm(o);

        // Disable form if in view mode
        if (this.isViewMode) {
          this.form.disable();
        }

        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load Op-Co. Please try again.';
        this.loading = false;
        console.error('Error loading Op-Co:', err);
      }
    });
  }

  private populateForm(o: OpCo): void {
    this.form.patchValue({
      name: o.name,
      remoteReferenceCode: o.remoteReferenceCode || '',
      distributorId: o.distributorId || null,
      contactPerson: o.contactPerson || '',
      email: o.email || '',
      phoneNumber: o.phoneNumber || '',
      internalNotes: (o as any).internalNotes || '',
      address: o.address || '',
      city: o.city || '',
      state: o.state || '',
      zipCode: o.zipCode || '',
      country: o.country || '',
      status: o.status || 'Active'
    });
  }

  private validateRemoteReferenceCode(code: string): void {
    const excludeId = this.isEditMode && this.opco ? this.opco.id : undefined;
    this.opCoService.validateRemoteReferenceCode(code, excludeId).subscribe({
      next: (resp) => {
        this.remoteCodeValidationError = resp.isValid ? null : (resp.message || 'Code already exists');
      },
      error: () => {
        // Do not block on validation error; allow submission
        this.remoteCodeValidationError = null;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid || this.remoteCodeValidationError) {
      this.markFormGroupTouched();
      return;
    }

    this.submitting = true;
    this.error = null;
    this.successMessage = null;

    const v = this.form.value;

    if (this.isEditMode && this.opco) {
      const request: UpdateOpCoRequest = {
        name: v.name?.trim(),
        remoteReferenceCode: v.remoteReferenceCode?.trim(),
        distributorId: Number(v.distributorId),
        contactPerson: v.contactPerson?.trim(),
        email: v.email?.trim(),
        phoneNumber: v.phoneNumber?.trim(),
        internalNotes: v.internalNotes?.trim(),
        address: v.address?.trim(),
        city: v.city?.trim(),
        state: v.state?.trim(),
        zipCode: v.zipCode?.trim(),
        country: v.country?.trim(),
        status: v.status as OpCoStatus,
        isActive: this.opco.isActive  // Preserve the existing isActive value
      };

      this.opCoService.update(this.opco.id, request).subscribe({
        next: (updatedOpCo) => {
          this.submitting = false;
          this.successMessage = 'Op-Co updated successfully!';
          this.opco = updatedOpCo;

          setTimeout(() => {
            this.isViewMode = true;
            this.form.disable();
            this.populateForm(updatedOpCo);
            this.router.navigate(['/admin/op-cos/view', this.opco!.id], { replaceUrl: true });
            this.successMessage = null;
          }, 1000);
        },
        error: (error) => {
          this.handleHttpError(error, 'update');
          this.submitting = false;
        }
      });
    } else {
      const request: CreateOpCoRequest = {
        name: v.name?.trim(),
        remoteReferenceCode: v.remoteReferenceCode?.trim(),
        distributorId: Number(v.distributorId),
        contactPerson: v.contactPerson?.trim() || undefined,
        email: v.email?.trim() || undefined,
        phoneNumber: v.phoneNumber?.trim() || undefined,
        internalNotes: v.internalNotes?.trim() || undefined,
        address: v.address?.trim() || undefined,
        city: v.city?.trim() || undefined,
        state: v.state?.trim() || undefined,
        zipCode: v.zipCode?.trim() || undefined,
        country: v.country?.trim() || undefined,
        status: v.status as OpCoStatus
      };

      this.opCoService.create(request).subscribe({
        next: () => {
          this.successMessage = 'Op-Co created successfully!';
          this.submitting = false;
          setTimeout(() => this.router.navigate(['/admin/op-cos']), 800);
        },
        error: (error) => {
          this.handleHttpError(error, 'create');
          this.submitting = false;
        }
      });
    }
  }

  toggleEditMode(): void {
    this.isViewMode = false;
    this.form.enable();
    if (this.opco) {
      this.router.navigate(['/admin/op-cos/edit', this.opco.id], { replaceUrl: true });
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.form.disable();
    if (this.opco) {
      this.populateForm(this.opco);
      this.router.navigate(['/admin/op-cos/view', this.opco.id], { replaceUrl: true });
    }
  }

  private loadCustomerAccounts(opCoId: number): void {
    this.customerAccountService.getCustomerAccountsByOpCo(opCoId).subscribe({
      next: (items) => { this.customerAccounts = items || []; },
      error: () => { this.customerAccounts = []; }
    });
  }

  onCancel(): void {
    if (this.isViewMode) {
      // In view mode, just go back to list
      this.router.navigate(['/admin/op-cos']);
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
          this.router.navigate(['/admin/op-cos']);
        }
      } else {
        this.router.navigate(['/admin/op-cos']);
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
      setTimeout(() => this.router.navigate(['/login']), 1200);
    } else if (error.status === 403) {
      this.error = `You do not have permission to ${action} Op-Cos.`;
    } else if (error.status === 400) {
      this.error = error.error?.message || 'Invalid data provided. Please check your input.';
    } else if (error.status === 0) {
      this.error = 'Unable to connect to the server. Please check if the API is running.';
    } else {
      this.error = error.error?.message || error.message || 'An error occurred. Please try again.';
    }
  }
}

