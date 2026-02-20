import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { UserService, UserOption } from '../../services/user.service';
import { Manufacturer, ManufacturerStatus, CreateManufacturerRequest, UpdateManufacturerRequest } from '../../models/manufacturer.model';
import { US_STATES } from '../../shared/utils/us-states.constants';

@Component({
  selector: 'app-manufacturer-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './manufacturer-form.component.html',
  styleUrls: ['./manufacturer-form.component.css']
})
export class ManufacturerFormComponent implements OnInit {
  form: FormGroup;
  manufacturer: Manufacturer | null = null;
  isEditMode = false;
  isViewMode = false;  // ADDED for view/edit mode pattern
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;
  nameValidationError: string | null = null;

  // Broker dropdown state
  showBrokerPanel = false;
  brokerFilter = '';
  eligibleBrokers: UserOption[] = [];
  filteredBrokers: UserOption[] = [];
  selectedBrokerName: string | null = null;

  // Contact Person dropdown state
  showContactPanel = false;
  contactFilter = '';
  associatedUsers: UserOption[] = [];
  filteredContacts: UserOption[] = [];
  selectedContactName: string | null = null;

  usStates = US_STATES;

  ManufacturerStatus = ManufacturerStatus;

  constructor(
    private fb: FormBuilder,
    private manufacturerService: ManufacturerService,
    private userService: UserService,
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
      this.loadManufacturer(+id);
    } else {
      // On create, load all manufacturer-role users
      this.loadEligibleBrokers();
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
      aka: ['', [Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(500)]],
      contactPerson: ['', [Validators.maxLength(200)]],
      contactPersonId: [null],
      phoneNumber: ['', [Validators.maxLength(20)]],
      website: ['', [Validators.maxLength(200)]],
      address: ['', [Validators.maxLength(500)]],
      city: ['', [Validators.maxLength(100)]],
      state: ['', [Validators.maxLength(100)]],
      zipCode: ['', [Validators.maxLength(20)]],
      country: ['', [Validators.maxLength(100)]],
      primaryBrokerId: [null],
      status: ['Active', [Validators.required]]
    });
  }

  private loadManufacturer(id: number): void {
    this.loading = true;
    this.error = null;

    this.manufacturerService.getManufacturerById(id).subscribe({
      next: (m) => {
        this.manufacturer = m;
        this.populateForm(m);

        // Disable form if in view mode
        if (this.isViewMode) {
          this.form.disable();
        }

        this.loading = false;
        this.loadEligibleBrokers(m.id);
      },
      error: (err) => {
        this.error = 'Failed to load manufacturer. Please try again.';
        this.loading = false;
        console.error('Error loading manufacturer:', err);
      }
    });
  }

  private populateForm(m: Manufacturer): void {
    this.form.patchValue({
      name: m.name,
      aka: m.aka || '',
      description: m.description || '',
      contactPerson: m.contactPerson || '',
      contactPersonId: m.contactPersonId ?? null,
      phoneNumber: m.phoneNumber || '',
      website: m.website || '',
      address: m.address || '',
      city: m.city || '',
      state: m.state || '',
      zipCode: m.zipCode || '',
      country: m.country || '',
      primaryBrokerId: m.primaryBrokerId ?? null,
      status: m.status

    });
    this.selectedBrokerName = m.primaryBrokerName ?? null;
    this.selectedContactName = m.contactPersonName ?? null;
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

    if (this.isEditMode && this.manufacturer) {
      this.updateManufacturer(formValue);
    } else {
      this.createManufacturer(formValue);
    }
  }

  private createManufacturer(formValue: any): void {
    const request: CreateManufacturerRequest = {
      name: formValue.name.trim(),
      aka: formValue.aka?.trim() || undefined,
      description: formValue.description?.trim() || undefined,
      contactPerson: formValue.contactPerson?.trim() || undefined,
      contactPersonId: formValue.contactPersonId ?? undefined,
      phoneNumber: formValue.phoneNumber?.trim() || undefined,
      website: formValue.website?.trim() || undefined,
      address: formValue.address?.trim() || undefined,
      city: formValue.city?.trim() || undefined,
      state: formValue.state?.trim() || undefined,
      zipCode: formValue.zipCode?.trim() || undefined,
      country: formValue.country?.trim() || undefined,
      primaryBrokerId: formValue.primaryBrokerId ?? undefined,
      status: formValue.status as ManufacturerStatus
    };

    this.manufacturerService.createManufacturer(request).subscribe({
      next: () => {
        this.successMessage = 'Manufacturer created successfully!';
        this.submitting = false;
        setTimeout(() => this.router.navigate(['/admin/manufacturers']), 1000);
      },
      error: (error) => {
        this.handleHttpError(error, 'create');
        this.submitting = false;
      }
    });
  }

  private updateManufacturer(formValue: any): void {
    if (!this.manufacturer) return;

    const request: UpdateManufacturerRequest = {
      name: formValue.name.trim(),
      aka: formValue.aka?.trim(),
      description: formValue.description?.trim(),
      contactPerson: formValue.contactPerson?.trim(),
      contactPersonId: formValue.contactPersonId ?? null,
      phoneNumber: formValue.phoneNumber?.trim(),
      website: formValue.website?.trim(),
      address: formValue.address?.trim(),
      city: formValue.city?.trim(),
      state: formValue.state?.trim(),
      zipCode: formValue.zipCode?.trim(),
      country: formValue.country?.trim(),
      primaryBrokerId: formValue.primaryBrokerId ?? null,
      status: formValue.status as ManufacturerStatus,
      isActive: this.manufacturer.isActive  // Preserve the existing isActive value
    };

    this.manufacturerService.updateManufacturer(this.manufacturer.id, request).subscribe({
      next: (updatedManufacturer) => {
        this.submitting = false;
        this.successMessage = 'Manufacturer updated successfully!';
        this.manufacturer = updatedManufacturer;

        setTimeout(() => {
          this.isViewMode = true;
          this.form.disable();
          this.populateForm(updatedManufacturer);
          this.router.navigate(['/admin/manufacturers/view', this.manufacturer!.id], { replaceUrl: true });
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
    if (this.manufacturer) {
      this.router.navigate(['/admin/manufacturers/edit', this.manufacturer.id], { replaceUrl: true });
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.form.disable();
    if (this.manufacturer) {
      this.populateForm(this.manufacturer);
      this.router.navigate(['/admin/manufacturers/view', this.manufacturer.id], { replaceUrl: true });
    }
  }

  onCancel(): void {
    if (this.isViewMode) {
      // In view mode, just go back to list
      this.router.navigate(['/admin/manufacturers']);
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
          this.router.navigate(['/admin/manufacturers']);
        }
      } else {
        this.router.navigate(['/admin/manufacturers']);
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
      this.error = `You do not have permission to ${action} manufacturers.`;
    } else if (error.status === 400) {
      this.error = error.error?.message || 'Invalid data provided. Please check your input.';
    } else if (error.status === 0) {
      this.error = 'Unable to connect to the server. Please check if the API is running.';
    } else {
      this.error = error.error?.message || error.message || 'An error occurred. Please try again.';
    }
  }

  // --- Primary Broker dropdown helpers ---
  private loadEligibleBrokers(manufacturerId?: number): void {
    this.userService.getEligibleBrokers(manufacturerId).subscribe({
      next: (users) => {
        this.eligibleBrokers = users || [];
        this.applyBrokerFilter(this.brokerFilter);
      },
      error: (err) => {
        console.error('Error loading eligible brokers', err);
      }
    });
  }

  toggleBrokerPanel(): void {
    this.showBrokerPanel = !this.showBrokerPanel;
  }

  closeBrokerPanel(): void {
    this.showBrokerPanel = false;
  }

  applyBrokerFilter(value: string): void {
    this.brokerFilter = value || '';
    const term = this.brokerFilter.trim().toLowerCase();
    if (!term) {
      this.filteredBrokers = [...this.eligibleBrokers];
      return;
    }
    this.filteredBrokers = this.eligibleBrokers.filter(u =>
      u.name.toLowerCase().includes(term) || (u.email && u.email.toLowerCase().includes(term))
    );
  }

  selectBroker(opt: UserOption): void {
    this.form.get('primaryBrokerId')?.setValue(opt?.id ?? null);
    this.selectedBrokerName = opt?.name || null;
    this.closeBrokerPanel();
  }

  // Contact Person methods
  private loadAssociatedUsers(manufacturerId?: number): void {
    if (!manufacturerId) {
      this.associatedUsers = [];
      this.filteredContacts = [];
      return;
    }

    this.userService.getUsersByManufacturer(manufacturerId).subscribe({
      next: (users) => {
        this.associatedUsers = users || [];
        this.filteredContacts = [...this.associatedUsers];
        this.applyContactFilter(this.contactFilter);
      },
      error: (err) => {
        console.error('Error loading associated users', err);
      }
    });
  }

  toggleContactPanel(): void {
    this.showContactPanel = !this.showContactPanel;
    // Load associated users when opening the panel in edit mode
    if (this.showContactPanel && this.manufacturer?.id) {
      this.loadAssociatedUsers(this.manufacturer.id);
    }
  }

  closeContactPanel(): void {
    this.showContactPanel = false;
  }

  applyContactFilter(value: string): void {
    this.contactFilter = value || '';
    const term = this.contactFilter.trim().toLowerCase();
    if (!term) {
      this.filteredContacts = [...this.associatedUsers];
      return;
    }
    this.filteredContacts = this.associatedUsers.filter(u =>
      u.name.toLowerCase().includes(term) || (u.email && u.email.toLowerCase().includes(term))
    );
  }

  selectContact(opt: UserOption): void {
    this.form.get('contactPersonId')?.setValue(opt?.id ?? null);
    this.selectedContactName = opt?.name || null;
    this.closeContactPanel();
  }

}

