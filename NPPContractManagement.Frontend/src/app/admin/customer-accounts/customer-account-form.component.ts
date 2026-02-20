import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidatorFn } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { CustomerAccountService } from '../../services/customer-account.service';
import { MemberAccountService } from '../../services/member-account.service';
import { DistributorService } from '../../services/distributor.service';
import { OpCoService } from '../../services/opco.service';
import { CustomerAccount, CreateCustomerAccountRequest, UpdateCustomerAccountRequest, CustomerAccountStatus } from '../../models/customer-account.model';
import { normalizeToNull, isUnassigned } from '../../shared/utils/form-utils';
import { US_STATES } from '../../shared/utils/us-states.constants';

@Component({
  selector: 'app-customer-account-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './customer-account-form.component.html',
  styleUrls: ['./customer-account-form.component.css']
})
export class CustomerAccountFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  isViewMode = false;
  submitting = false;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  customers: any[] = [];
  members: any[] = [];
  distributors: any[] = [];
  opcos: any[] = [];
  usStates = US_STATES;

  account: CustomerAccount | null = null;

  CustomerAccountStatus = CustomerAccountStatus;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private customerAccountService: CustomerAccountService,
    private memberAccountService: MemberAccountService,
    private distributorService: DistributorService,
    private opcoService: OpCoService
  ) {
    this.form = this.createForm();
  }

  ngOnInit(): void {
    this.loadLookups();

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
      this.loadAccount(+id);
    }

    // Update OpCos when distributor changes
    this.form.get('distributorId')?.valueChanges.subscribe((val) => {
      if (val) {
        this.opcoService.getPaginated(1, 1000, undefined, 'Active' as any, val).subscribe({
          next: (resp) => {
            const list = (resp?.items || []).filter(o => o.isActive && (o.status as any) === 'Active');
            this.opcos = list;
            const current = this.form.get('opCoId')?.value;
            const currentId = typeof current === 'object' && current ? current.id : current;
            const exists = list.some(o => (o.id === currentId));
            if (!exists) {
              this.form.patchValue({ opCoId: null }, { emitEvent: false });
            }
          },
          error: () => {
            this.opcos = [];
            this.form.patchValue({ opCoId: null }, { emitEvent: false });
          }
        });
      } else {
        this.opcos = [];
        this.form.patchValue({ opCoId: null }, { emitEvent: false });
      }
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      memberAccountId: [null, [Validators.required]],
      distributorId: [null, [Validators.required]],
      opCoId: [null, []],
      customerName: ['', [Validators.required, Validators.maxLength(200)]],
      customerAccountNumber: ['', [Validators.required, Validators.maxLength(100)]],
      status: ['Active', [Validators.required]],
      address: ['', [Validators.maxLength(500)]],
      city: ['', [Validators.maxLength(100)]],
      state: ['', [Validators.maxLength(100)]],
      zipCode: ['', [Validators.maxLength(20)]],
      country: ['', [Validators.maxLength(100)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      email: ['', [Validators.email, Validators.maxLength(255)]],
      // New fields
      salesRep: ['', [Validators.maxLength(200)]],
      dso: [null, []],
      startDate: [null, []],
      endDate: [null, []],
      tracsAccess: [null, [this.requiredBoolean()]],
      markup: ['', [this.markupValidator()]],
      auditDate: [null, []],
      toEntegra: [false, []],
      dateToEntegra: [null, []],
      combinedUniqueID: ['', [Validators.maxLength(200)]],
      internalNotes: ['', []],
      association: [null, []]
    });
  }

  private loadLookups(): void {
    // Load active Member Accounts
    this.memberAccountService.getPaginated(1, 1000, undefined, 'asc', undefined, 'Active').subscribe({
      next: (res) => (this.members = (res?.items || []).filter(m => m.isActive && (m.status as any) === 'Active')),
      error: () => (this.members = [])
    });

    // Load active Distributors
    this.distributorService.getPaginated(1, 1000, undefined, 'asc', undefined, 'Active').subscribe({
      next: (res) => (this.distributors = (res?.items || []).filter(d => d.isActive && (d.status as any) === 'Active')),
      error: () => (this.distributors = [])
    });

    // Load active OpCos initially; refined by distributor selection later
    this.opcoService.getPaginated(1, 1000, undefined, 'Active' as any).subscribe({
      next: (res) => (this.opcos = (res?.items || []).filter(o => o.isActive && (o.status as any) === 'Active')),
      error: () => (this.opcos = [])
    });
  }

  private toStatusString(status: any): string {
    if (status === null || status === undefined) return 'Active';
    if (typeof status === 'string') return status;
    switch (status) {
      case 1: return 'Active';
      case 2: return 'Inactive';
      case 3: return 'Pending';
      case 4: return 'Suspended';
      case 5: return 'Closed';
      default: return 'Active';
    }
  }

  private loadAccount(id: number): void {
    this.loading = true;
    this.customerAccountService.getById(id).subscribe({
      next: (acc) => {
        this.account = acc;
        this.populateForm(acc);

        // Disable form if in view mode
        if (this.isViewMode) {
          this.form.disable();
        }

        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load customer account.';
        this.loading = false;
      }
    });
  }

  isFieldInvalid(field: string): boolean {
    const c = this.form.get(field);
    return !!(c && c.invalid && (c.dirty || c.touched));
  }

  // Custom validator that treats both true and false as valid booleans, but null/undefined as invalid
  private requiredBoolean(): ValidatorFn {
    return (control: AbstractControl) => {
      const v = control.value;
      return (v === null || v === undefined) ? { required: true } : null;
    };
  }


  private markupValidator(): ValidatorFn {
    return (control: AbstractControl) => {
      const value = control.value;
      if (value === null || value === undefined || value === '') return null; // optional
      return (value === '$' || value === '%') ? null : { invalidMarkup: true };
    };
  }

  onSubmit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(c => c.markAsTouched());
      return;
    }
    this.submitting = true;
    const v = this.form.value;

    if (this.isEditMode && this.account) {
      const req: UpdateCustomerAccountRequest = { ...v, opCoId: normalizeToNull(v.opCoId), isActive: this.account.isActive } as UpdateCustomerAccountRequest;
      this.customerAccountService.update(this.account.id, req).subscribe({
        next: (updatedAccount) => {
          this.submitting = false;
          this.successMessage = 'Customer account updated successfully!';
          this.account = updatedAccount;

          setTimeout(() => {
            this.isViewMode = true;
            this.form.disable();
            this.populateForm(updatedAccount);
            this.router.navigate(['/admin/customer-accounts/view', this.account!.id], { replaceUrl: true });
            this.successMessage = null;
          }, 1000);
        },
        error: () => {
          this.error = 'Failed to update customer account.';
          this.submitting = false;
        }
      });
    } else {
      const req: CreateCustomerAccountRequest = { ...v, opCoId: normalizeToNull(v.opCoId) } as CreateCustomerAccountRequest;
      this.customerAccountService.create(req).subscribe({
        next: () => {
          this.successMessage = 'Customer account created successfully!';
          this.submitting = false;
          setTimeout(() => this.router.navigate(['/admin/customer-accounts']), 800);
        },
        error: () => {
          this.error = 'Failed to create customer account.';
          this.submitting = false;
        }
      });
    }
  }

  toggleEditMode(): void {
    this.isViewMode = false;
    this.form.enable();
    if (this.account) {
      this.router.navigate(['/admin/customer-accounts/edit', this.account.id], { replaceUrl: true });
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.form.disable();
    if (this.account) {
      this.populateForm(this.account);
      this.router.navigate(['/admin/customer-accounts/view', this.account.id], { replaceUrl: true });
    }
  }

  private populateForm(acc: CustomerAccount): void {
    this.form.patchValue({
      memberAccountId: acc.memberAccountId,
      distributorId: acc.distributorId,
      opCoId: normalizeToNull(acc.opCoId),
      customerName: acc.customerName,
      customerAccountNumber: acc.customerAccountNumber,
      status: this.toStatusString(acc.status),
      address: acc.address ?? '',
      city: acc.city ?? '',
      state: acc.state ?? '',
      zipCode: acc.zipCode ?? '',
      country: acc.country ?? '',
      phoneNumber: acc.phoneNumber ?? '',
      email: acc.email ?? '',
      salesRep: acc.salesRep ?? '',
      dso: acc.dso ?? null,
      startDate: acc.startDate ? acc.startDate.substring(0, 10) : null,
      endDate: acc.endDate ? acc.endDate.substring(0, 10) : null,
      tracsAccess: acc.tracsAccess,
      markup: (acc.markup ?? '').toString().slice(0, 1),
      auditDate: acc.auditDate ? acc.auditDate.substring(0, 10) : null,
      toEntegra: acc.toEntegra ?? false,
      dateToEntegra: acc.dateToEntegra ? acc.dateToEntegra.substring(0, 10) : null,
      combinedUniqueID: acc.combinedUniqueID ?? '',
      internalNotes: acc.internalNotes ?? '',
      association: acc.association ?? null
    });
  }

  onCancel(): void {
    if (this.isViewMode) {
      // In view mode, just go back to list
      this.router.navigate(['/admin/customer-accounts']);
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
          this.router.navigate(['/admin/customer-accounts']);
        }
      } else {
        this.router.navigate(['/admin/customer-accounts']);
      }
    }
  }
}

