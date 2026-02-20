import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MemberAccountService } from '../../services/member-account.service';
import { Industry } from '../../models/industry.model';
import { IndustryService } from '../../services/industry.service';
import { CustomerAccountService } from '../../services/customer-account.service';
import { CustomerAccount } from '../../models/customer-account.model';
import { CreateMemberAccountRequest, MemberAccount, PayType, UpdateMemberAccountRequest } from '../../models/member-account.model';
import { US_STATES } from '../../shared/utils/us-states.constants';

@Component({
  selector: 'app-member-account-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './member-account-form.component.html',
  styleUrls: ['./member-account-form.component.css']
})
export class MemberAccountFormComponent implements OnInit {
  form: FormGroup;
  isEditMode = false;
  isViewMode = false;
  submitting = false;
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  account: MemberAccount | null = null;
  industries: Industry[] = [];
  customerAccounts: CustomerAccount[] = [];
  usStates = US_STATES;

  PayType = PayType;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private memberAccountService: MemberAccountService,
    private industryService: IndustryService,
    private customerAccountService: CustomerAccountService
  ) {
    this.form = this.createForm();
  }

  // Track whether the user has manually edited the Salesforce Account Name
  private salesforceSyncEnabled = true;

  ngOnInit(): void {
    this.loadIndustries();

    // Auto-populate Salesforce Account Name from Member Name while sync is enabled
    this.form.get('facilityName')?.valueChanges.subscribe((val: string) => {
      if (this.salesforceSyncEnabled) {
        this.form.get('salesforceAccountName')?.setValue(val || '', { emitEvent: false });
      }
    });

    // Check if we're in view mode or edit mode
    const url = this.router.url;
    const idParam = this.route.snapshot.paramMap.get('id');

    if (idParam) {
      if (url.includes('/view/')) {
        this.isViewMode = true;
        this.isEditMode = true;
      } else if (url.includes('/edit/')) {
        this.isViewMode = false;
        this.isEditMode = true;
      }
      this.loadAccount(+idParam);
    }
  }

  private createForm(): FormGroup {
    const today = new Date().toISOString().substring(0, 10);
    return this.fb.group({
      memberNumber: ['', [Validators.required, Validators.maxLength(100)]],
      facilityName: ['', [Validators.required, Validators.maxLength(200)]],
      address: ['', [Validators.maxLength(500)]],
      city: ['', [Validators.maxLength(100)]],
      state: ['', [Validators.maxLength(100)]],
      zipCode: ['', [Validators.maxLength(20)]],
      country: ['', [Validators.maxLength(100)]],
      phoneNumber: ['', [Validators.maxLength(20)]],

      industryId: [null, [Validators.required]],
      w9: ['', []],
      taxId: ['', [Validators.maxLength(500)]],
      businessType: ['', [Validators.maxLength(200)]],

      // Extended fields
      lopDate: [today, []],
      internalNotes: ['', []],
      clientGroupEnrollment: [null, [Validators.min(0)]],
      salesforceAccountName: ['', [Validators.required, Validators.maxLength(200)]],
      vmapNumber: ['', [Validators.maxLength(100)]],
      vmSupplierName: ['', [Validators.maxLength(200)]],
      vmSupplierSite: ['', [Validators.maxLength(200)]],
      payType: [''],
      parentMemberAccountNumber: ['', [Validators.maxLength(100)]],
      entegraGPONumber: ['', [Validators.maxLength(100)]],
      clientGroupNumber: ['', [Validators.maxLength(100)]],
      entegraIdNumber: ['', [Validators.maxLength(100)]],

      status: ['Active', [Validators.required]],
      isActive: [true, []]
    });
  }

  isFieldInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    const raw = this.form.value;

    const payload: any = {
      memberNumber: raw.memberNumber,
      facilityName: raw.facilityName,
      address: raw.address || null,
      city: raw.city || null,
      state: raw.state || null,
      zipCode: raw.zipCode || null,
      country: raw.country || null,
      phoneNumber: raw.phoneNumber || null,

      industryId: raw.industryId ?? null,
      w9: raw.w9 || '',
      taxId: raw.taxId || null,
      businessType: raw.businessType || null,
      // extended
      lopDate: raw.lopDate || new Date().toISOString().substring(0, 10),
      internalNotes: raw.internalNotes || null,
      clientGroupEnrollment: (raw.clientGroupEnrollment === null || raw.clientGroupEnrollment === "" || isNaN(+raw.clientGroupEnrollment)) ? undefined : Math.max(0, Math.floor(+raw.clientGroupEnrollment)),
      salesforceAccountName: raw.salesforceAccountName,
      vmapNumber: raw.vmapNumber || null,
      vmSupplierName: raw.vmSupplierName || null,
      vmSupplierSite: raw.vmSupplierSite || null,
      payType: raw.payType || null,
      parentMemberAccountNumber: raw.parentMemberAccountNumber || null,
      entegraGPONumber: raw.entegraGPONumber || null,
      clientGroupNumber: raw.clientGroupNumber || null,
      entegraIdNumber: raw.entegraIdNumber || null,
      status: raw.status === 'Active' ? 1 : (raw.status === 'Inactive' ? 2 : 1),
      isActive: !!raw.isActive
    } as CreateMemberAccountRequest | UpdateMemberAccountRequest;

    if (this.isEditMode && this.account) {
      this.memberAccountService.update(this.account.id, payload as UpdateMemberAccountRequest).subscribe({
        next: (updatedAccount) => {
          this.submitting = false;
          this.successMessage = 'Member account updated successfully!';
          this.account = updatedAccount;

          setTimeout(() => {
            this.isViewMode = true;
            this.form.disable();
            this.populateForm(updatedAccount);
            this.router.navigate(['/admin/member-accounts/view', this.account!.id], { replaceUrl: true });
            this.successMessage = null;
          }, 1000);
        },
        error: (err) => {
          this.error = 'Failed to update member account';
          this.submitting = false;
          console.error(err);
        }
      });
    } else {
      this.memberAccountService.create(payload as CreateMemberAccountRequest).subscribe({
        next: () => {
          this.successMessage = 'Member account created successfully';
          this.router.navigate(['/admin/member-accounts']);
        },
        error: (err) => {
          this.error = 'Failed to create member account';
          this.submitting = false;
          console.error(err);
        }
      });
    }
  }

  private loadIndustries(): void {
    this.industryService.getActive().subscribe({
      next: (list) => {
        this.industries = list || [];
      },
      error: (err) => {
        console.error('Failed to load industries', err);
      }
    });
  }

  private loadAccount(id: number): void {
    this.loading = true;
    this.memberAccountService.getById(id).subscribe({
      next: (acc) => {
        this.account = acc;
        this.populateForm(acc);
        this.loadCustomerAccounts(acc.id);

        // Disable form if in view mode
        if (this.isViewMode) {
          this.form.disable();
        }

        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load member account';
        this.loading = false;
        console.error(err);
      }
    });
  }

  private mapPayTypeToInt(val: string): number | null {
    const s = (val ?? '').toString().trim().toLowerCase();
    switch (s) {
      case 'ach': return 1;
      case 'check': return 2;
      case 'wire': return 3;
      default: return null;
    }
  }

  private mapPayTypeToString(val: any): string {
    if (val === null || val === undefined) return 'ACH';
    if (typeof val === 'string') {
      const s = val.trim();
      if (['ACH','Check','Wire'].some(o => o.toLowerCase() === s.toLowerCase())) {
        return ['ACH','Check','Wire'].find(o => o.toLowerCase() === s.toLowerCase())!;
      }
      return 'ACH';
    }
    const n = Number(val);
    switch (n) {
      case 1: return 'ACH';
      case 2: return 'Check';
      case 3: return 'Wire';
      default: return 'ACH';
    }
  }

  private mapStatusToString(val: any): string {
    // Handles both numeric and string/enum inputs from API
    if (typeof val === 'number') {
      switch (val) {
        case 1: return 'Active';
        case 2: return 'Inactive';
        case 3: return 'Pending';
        case 4: return 'Suspended';
        default: return 'Active';
      }
    }
    if (typeof val === 'string') {
      const normalized = val.trim();
      if (['Active', 'Inactive', 'Pending', 'Suspended'].includes(normalized)) {
        return normalized as any;
      }
    }
    return 'Active';
  }

  onSalesforceManualEdit(): void {
    this.salesforceSyncEnabled = false;
  }

  toggleEditMode(): void {
    this.isViewMode = false;
    this.form.enable();
    if (this.account) {
      this.router.navigate(['/admin/member-accounts/edit', this.account.id], { replaceUrl: true });
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.form.disable();
    if (this.account) {
      this.populateForm(this.account);
      this.router.navigate(['/admin/member-accounts/view', this.account.id], { replaceUrl: true });
    }
  }

  private populateForm(acc: MemberAccount): void {
    // Disable sync when loading existing data so Member Name changes don't overwrite saved Salesforce name
    this.salesforceSyncEnabled = false;
    this.form.patchValue({
      memberNumber: acc.memberNumber,
      facilityName: acc.facilityName,
      address: acc.address || '',
      city: acc.city || '',
      state: acc.state || '',
      zipCode: acc.zipCode || '',
      country: acc.country || '',
      phoneNumber: acc.phoneNumber || '',

      industryId: acc.industryId ?? null,
      w9: acc.w9 || '',
      taxId: acc.taxId || '',
      businessType: acc.businessType || '',

      lopDate: acc.lopDate ? (new Date(acc.lopDate).toISOString().substring(0,10)) : new Date().toISOString().substring(0,10),
      internalNotes: acc.internalNotes || '',
      clientGroupEnrollment: (acc.clientGroupEnrollment ?? null),
      salesforceAccountName: acc.salesforceAccountName || '',
      vmapNumber: acc.vmapNumber || '',
      vmSupplierName: acc.vmSupplierName || '',
      vmSupplierSite: acc.vmSupplierSite || '',
      payType: (acc as any).payTypeName ?? (acc as any).payType ?? '',
      parentMemberAccountNumber: acc.parentMemberAccountNumber || '',
      entegraGPONumber: acc.entegraGPONumber || '',
      clientGroupNumber: acc.clientGroupNumber || '',
      entegraIdNumber: acc.entegraIdNumber || '',

      status: this.mapStatusToString(acc.status),
      isActive: acc.isActive
    });
  }

  private loadCustomerAccounts(memberAccountId: number): void {
    this.customerAccountService.getCustomerAccountsByMemberAccount(memberAccountId).subscribe({
      next: (items) => { this.customerAccounts = items || []; },
      error: () => { this.customerAccounts = []; }
    });
  }

  onCancel(): void {
    if (this.isViewMode) {
      // In view mode, just go back to list
      this.router.navigate(['/admin/member-accounts']);
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
          this.router.navigate(['/admin/member-accounts']);
        }
      } else {
        this.router.navigate(['/admin/member-accounts']);
      }
    }
  }

}

