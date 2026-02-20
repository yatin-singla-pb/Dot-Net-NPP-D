import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { UserService, CreateUserRequest, UpdateUserRequest, User } from '../../../../services/user.service';
import { RoleService, Role } from '../../../../services/role.service';
import { ManufacturerService } from '../../../../services/manufacturer.service';
import { IndustryService } from '../../../../services/industry.service';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  template: `
    <main class="container">
      <nav aria-label="breadcrumb" id="breadcrumb">
        <ol class="breadcrumb">
          <li class="breadcrumb-item"><a routerLink="/admin">Administration</a></li>
          <li class="breadcrumb-item"><a routerLink="/admin/users">Users</a></li>
          <li class="breadcrumb-item active">{{isViewMode ? 'View' : (isEdit ? 'Edit User' : 'Create User')}}</li>
        </ol>
      </nav>

      <h3>{{isViewMode ? 'View' : (isEdit ? 'Edit User' : 'Create User')}}</h3>

      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="mt-3">
        <div class="row g-3">

          <!-- Id (Read-only, shown only in edit/view mode) -->
          <div class="col-12" *ngIf="isEdit || isViewMode">
            <label class="form-label">Id</label>
            <input
              type="text"
              class="form-control"
              [value]="id"
              readonly
              disabled>
          </div>

          <!-- User ID hidden in create mode (headless accounts) -->
          <div class="col-md-3" *ngIf="isEdit || isViewMode">
            <label class="form-label">User ID</label>
            <input class="form-control" formControlName="userId" placeholder="User ID" [readonly]="true">
            <small *ngIf="!form.get('userId')?.value" class="text-warning">Pending Registration</small>
          </div>
          <div class="col-md-3">
            <label class="form-label">First Name <span class="text-danger">*</span></label>
            <input class="form-control" formControlName="firstName" placeholder="First Name">
          </div>
          <div class="col-md-3">
            <label class="form-label">Last Name <span class="text-danger">*</span></label>
            <input class="form-control" formControlName="lastName" placeholder="Last Name">
          </div>
          <div class="col-md-3">
            <label class="form-label">Email <span class="text-danger">*</span></label>
            <input class="form-control" formControlName="email" type="email" placeholder="Email">
          </div>

          <div class="col-md-3">
            <label class="form-label">Phone</label>
            <input class="form-control" formControlName="phoneNumber" placeholder="Phone">
          </div>
          <div class="col-md-3">
            <label class="form-label">Company <span class="text-danger">*</span></label>
            <input class="form-control" formControlName="company" placeholder="Company">
          </div>
          <div class="col-md-3">
            <label class="form-label">Job Title <span class="text-danger">*</span></label>
            <input class="form-control" formControlName="jobTitle" placeholder="Job Title">
          </div>
          <div class="col-md-3">
            <label class="form-label">Status <span class="text-danger">*</span></label>
            <select class="form-select" formControlName="status">
              <option [ngValue]="1">Active</option>
              <option [ngValue]="2">Locked</option>
              <option [ngValue]="3">Suspended</option>
              <option [ngValue]="4">Headless</option>
            </select>
          </div>

          <div class="col-md-6">
            <label class="form-label">Address</label>
            <input class="form-control" formControlName="address" placeholder="Address">
          </div>
          <div class="col-md-2">
            <label class="form-label">City</label>
            <input class="form-control" formControlName="city" placeholder="City">
          </div>
          <div class="col-md-2">
            <label class="form-label">State</label>
            <input class="form-control" formControlName="state" placeholder="State">
          </div>
          <div class="col-md-2">
            <label class="form-label">Postal Code</label>
            <input class="form-control" formControlName="postCode" placeholder="Postal Code">
          </div>

          <div class="col-md-12">
            <label class="form-label">Notes</label>
            <textarea class="form-control" rows="3" formControlName="notes" placeholder="Internal notes..."></textarea>
          </div>

          <div class="col-md-4">
            <label class="form-label">Industry</label>
            <select class="form-select" formControlName="industryId">
              <option [ngValue]="null">Select industry</option>
              <option *ngFor="let ind of industries" [ngValue]="ind.id">{{ind.name}}</option>
            </select>
          </div>

          <div class="col-md-4">
            <label class="form-label">Group Email</label>
            <select class="form-select" formControlName="groupEmail">
              <option [ngValue]="false">N</option>
              <option [ngValue]="true">Y</option>
            </select>
          </div>

          <div class="col-md-4">
            <label class="form-label">Failed Auth Attempts</label>
            <input type="number" class="form-control" formControlName="failedAuthAttempts" min="0" step="1">
          </div>

          <div class="col-md-6">
            <label class="form-label">Roles <span class="text-danger">*</span></label>
            <select class="form-select" formControlName="roleIds" multiple>
              <option *ngFor="let r of roles" [ngValue]="r.id">{{r.name}}</option>
            </select>
          </div>

          <div class="col-md-6" *ngIf="isManufacturerRoleSelected()">
            <label class="form-label">Manufacturers</label>
            <div class="form-control p-2" (click)="toggleManufacturersPanel()" tabindex="0">
              <span class="badge text-bg-denim rounded-pill me-1 mb-1" *ngFor="let m of selectedManufacturers">
                {{m.name}}
                <button type="button" class="btn-close btn-close-white btn-sm ms-1" aria-label="Remove" (click)="removeManufacturer(m.id); $event.stopPropagation()"></button>
              </span>
              <span class="text-muted" *ngIf="!selectedManufacturers.length">Select manufacturers...</span>
            </div>
            <div class="position-relative">
              <div class="dropdown-menu d-block w-100 p-2" *ngIf="showManufacturersPanel">
                <input class="form-control mb-2" [value]="manufacturerFilter" (input)="applyManufacturerFilter($any($event.target).value)" placeholder="Search manufacturers...">
                <div style="max-height:240px; overflow:auto;">
                  <div class="form-check" *ngFor="let m of filteredManufacturers">
                    <input class="form-check-input" type="checkbox" [checked]="isSelected(m.id)" (change)="toggleManufacturer(m)">
                    <label class="form-check-label">{{m.name}}</label>
                  </div>
                </div>
                <div class="mt-2 text-end">
                  <button type="button" class="btn btn-sm btn-secondary me-2" (click)="closeManufacturersPanel()">Close</button>
                  <button type="button" class="btn btn-sm btn-denim" (click)="applySelected()">Apply</button>
                </div>
              </div>
            </div>
            <div *ngIf="form.get('manufacturerIds')?.invalid && form.get('manufacturerIds')?.touched" class="text-danger small">Manufacturers are required for users with Manufacturer role.</div>
          </div>



        </div>

        <div *ngIf="error" class="alert alert-danger mt-3">
          <i class="fa-solid fa-exclamation-triangle me-2"></i>{{ error }}
        </div>

        <!-- Form Actions -->
        <div class="mt-4 d-flex justify-content-end">
          <!-- View Mode Actions -->
          <div *ngIf="isViewMode" class="d-flex">
            <button type="button" class="btn btn-denim me-2" (click)="onCancel()">
              Back to List
            </button>
            <button type="button" class="btn btn-denim" (click)="toggleEditMode()">
              <i class="fa-solid fa-pen me-2"></i>Edit
            </button>
          </div>

          <!-- Edit/Create Mode Actions -->
          <div *ngIf="!isViewMode" class="d-flex">
            <button type="button" class="btn btn-denim me-2" (click)="onCancel()">
              Cancel
            </button>
            <button class="btn btn-denim" type="submit" [disabled]="form.invalid || saving">
              {{ saving ? 'Saving...' : (isEdit ? 'Update User' : 'Create User') }}
            </button>
          </div>
        </div>
      </form>
    </main>
  `
})
export class UserFormComponent implements OnInit {
  isEdit = false;
  isViewMode = false;
  saving = false;
  error: string | null = null;
  id: number | null = null;
  roles: Role[] = [];
  industries: any[] = [];
  selectedManufacturers: { id: number; name: string }[] = [];
  allManufacturers: { id: number; name: string }[] = [];
  filteredManufacturers: { id: number; name: string }[] = [];
  loadingManufacturers = false;
  manufacturerFilter = '';
  showManufacturersPanel = false;



  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,

    private roleService: RoleService,
    private manufacturerService: ManufacturerService,
    private industryService: IndustryService
  ) {}

  ngOnInit(): void {
    // Build form
    this.form = this.fb.group({
      userId: [''],

      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
      phoneNumber: [''],
      company: ['', [Validators.required]],
      jobTitle: ['', [Validators.required]],
      address: [''],
      city: [''],
      state: [''],
      postCode: [''],
      notes: [''],
      industryId: [null],
      groupEmail: [false],
      status: [1, [Validators.required]],
      failedAuthAttempts: [0],
      roleIds: [<number[]>[], [Validators.required]],
      manufacturerIds: [<number[]>[]],
      isActive: [true]
    });

	    // Keep isActive in sync with Status: Active(1) -> true; Locked/Suspended -> false
	    this.form.get('status')!.valueChanges.subscribe(() => this.syncIsActiveWithStatus());
	    // Initialize the derived isActive once at startup
	    this.syncIsActiveWithStatus();


    // Toggle required validator on manufacturerIds based on role selection
    this.form.get('roleIds')!.valueChanges.subscribe(() => this.updateManufacturerValidators());
    this.updateManufacturerValidators();

    this.roleService.getAllRoles().subscribe(roles => { this.roles = roles; this.updateManufacturerValidators(); });
    this.industryService.getAll().subscribe(industries => { this.industries = industries; });
    // Keep chips in sync when selection changes
    this.form.get('manufacturerIds')!.valueChanges.subscribe((ids: number[]) => {
      this.syncSelectedFromIds(ids || []);
    });

    // Check if we're in view mode or edit mode
    const url = this.router.url;
    const idParam = this.route.snapshot.paramMap.get('id');

    this.isEdit = !!idParam;
    if (this.isEdit) {
      if (url.includes('/view/')) {
        this.isViewMode = true;
      } else if (url.includes('/edit/')) {
        this.isViewMode = false;
      }

      this.id = Number(idParam);
      this.form.get('userId')?.disable();
      this.userService.getUserById(this.id).subscribe(user => {
        this.form.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email,
          phoneNumber: user.phoneNumber,
          company: user.company,
          jobTitle: user.jobTitle ?? user.title,
          address: user.address,
          city: user.city,
          state: user.state,




          postCode: user.postCode,
          notes: user.notes,
          industryId: (user as any).industryId ?? null,
          groupEmail: (user as any).groupEmail ?? false,
          status: this.mapStatusToNumber((user as any).accountStatus ?? user.status),
          failedAuthAttempts: user.failedAuthAttempts ?? 0,
          isActive: user.isActive,
        });

        // Ensure isActive reflects status after loading
        this.syncIsActiveWithStatus();

        // Roles mapping (ids)
        const roleIds = (user.roles || []).map(r => r.id);
        this.form.get('roleIds')?.setValue(roleIds);
        // Load existing manufacturers
        this.loadUserManufacturers(this.id!);

        // Disable form if in view mode
        if (this.isViewMode) {
          this.form.disable();
        }
      });

    }
  }


  private updateManufacturerValidators(): void {
    const ctrl = this.form.get('manufacturerIds')!;
    if (this.isManufacturerRoleSelected()) {
      ctrl.addValidators([Validators.required]);


    } else {
      ctrl.clearValidators();
      this.selectedManufacturers = [];
      ctrl.setValue([]);
    }

    // Load all manufacturers for dropdown (both add and edit)
    this.loadingManufacturers = true;
    this.manufacturerService.getAll().subscribe({
      next: ms => {
        this.allManufacturers = (ms || []).map(m => ({ id: m.id, name: m.name }));
        this.filteredManufacturers = [...this.allManufacturers];
        this.loadingManufacturers = false;
        const currentIds: number[] = this.form.get('manufacturerIds')!.value || [];
        this.syncSelectedFromIds(currentIds);
      },
      error: _ => { this.loadingManufacturers = false; }
    });

    ctrl.updateValueAndValidity({ emitEvent: false });
  }

  isManufacturerRoleSelected(): boolean {
    const selected: number[] = this.form.get('roleIds')!.value || [];
    const mRole = this.roles.find(r => (r.name || '').toLowerCase() === 'manufacturer');
    return !!mRole && selected.includes(mRole.id);
  }

  private loadUserManufacturers(userId: number): void {
    this.userService.getUserManufacturers(userId).subscribe(list => {
      this.selectedManufacturers = list;
      this.form.get('manufacturerIds')!.setValue(list.map(x => x.id));
    });
  }

  onManufacturerPanelOpened(opened: any): void {
    if (opened) {
      this.manufacturerFilter = '';
      this.filteredManufacturers = [...this.allManufacturers];
    }
  }

  applyManufacturerFilter(term: string): void {
    this.manufacturerFilter = term || '';
    const t = this.manufacturerFilter.toLowerCase();
    this.filteredManufacturers = (this.allManufacturers || []).filter(m => (m.name || '').toLowerCase().includes(t));
  }

  onManufacturerSelectionChange(): void {
    const ids: number[] = this.form.get('manufacturerIds')!.value || [];
    this.syncSelectedFromIds(ids);
  }

  private syncSelectedFromIds(ids: number[]): void {
    const set = new Set(ids || []);
    this.selectedManufacturers = (this.allManufacturers || []).filter(m => set.has(m.id));
  }

  removeManufacturer(id: number): void {
    this.selectedManufacturers = this.selectedManufacturers.filter(x => x.id !== id);
    this.form.get('manufacturerIds')!.setValue(this.selectedManufacturers.map(x => x.id));
  }

  toggleManufacturersPanel(): void {
    if (!this.isManufacturerRoleSelected()) return;
    this.showManufacturersPanel = !this.showManufacturersPanel;
    if (this.showManufacturersPanel) {
      this.applyManufacturerFilter('');
    }
  }

  closeManufacturersPanel(): void {
    this.showManufacturersPanel = false;
  }

  applySelected(): void {
    this.closeManufacturersPanel();
  }

  isSelected(id: number): boolean {
    const ids: number[] = this.form.get('manufacturerIds')!.value || [];
    return ids.includes(id);
  }

  toggleManufacturer(m: { id: number; name: string }): void {
    const ctrl = this.form.get('manufacturerIds')!;
    const ids: number[] = [...(ctrl.value || [])];
    const idx = ids.indexOf(m.id);
    if (idx >= 0) {
      ids.splice(idx, 1);
    } else {
      ids.push(m.id);
    }
    ctrl.setValue(ids);
    this.syncSelectedFromIds(ids);
    ctrl.markAsTouched();
  }


  private syncIsActiveWithStatus(): void {
    const s = this.form.get('status')!.value as number;
    this.form.get('isActive')!.setValue(s === 1, { emitEvent: false });
  }


  onSubmit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    this.error = null;

    if (this.isEdit && this.id) {
      const payload: UpdateUserRequest = {
        firstName: this.form.value.firstName!,
        lastName: this.form.value.lastName!,
        email: this.form.value.email!,
        phoneNumber: this.form.value.phoneNumber || undefined,
        company: this.form.value.company || undefined,
        jobTitle: this.form.value.jobTitle || undefined,
        address: this.form.value.address || undefined,
        city: this.form.value.city || undefined,
        state: this.form.value.state || undefined,
        postCode: this.form.value.postCode || undefined,
        notes: this.form.value.notes || undefined,
        industryId: this.form.value.industryId ?? undefined,
        groupEmail: this.form.value.groupEmail ?? false,
        status: this.form.value.status!,
        accountStatus: this.form.value.status!,
        failedAuthAttempts: this.form.value.failedAuthAttempts || 0,
        isActive: this.form.value.isActive!,
        roleIds: this.form.value.roleIds as number[],
        manufacturerIds: this.isManufacturerRoleSelected() ? ((this.form.get('manufacturerIds')!.value as number[]) || []) : []
      };
      this.userService.updateUser(this.id, payload).subscribe({
        next: () => {
          this.saving = false;
          // Return to view mode after successful update
          this.isViewMode = true;
          this.form.disable();
          this.router.navigate(['/admin/users/view', this.id], { replaceUrl: true });
        },
        error: (e) => { this.error = e?.error?.message || e?.message || 'Failed to update user'; this.saving = false; }
      });
    } else {
      const payload: CreateUserRequest = {
        userId: '',  // Empty string -> backend creates headless account
        firstName: this.form.value.firstName!,
        lastName: this.form.value.lastName!,
        email: this.form.value.email!,
        phoneNumber: this.form.value.phoneNumber || undefined,
        company: this.form.value.company || undefined,
        jobTitle: this.form.value.jobTitle || undefined,
        address: this.form.value.address || undefined,
        city: this.form.value.city || undefined,
        state: this.form.value.state || undefined,
        postCode: this.form.value.postCode || undefined,
        notes: this.form.value.notes || undefined,
        industryId: this.form.value.industryId ?? undefined,
        groupEmail: this.form.value.groupEmail ?? false,
        status: this.form.value.status!,
        accountStatus: this.form.value.status!,
        failedAuthAttempts: this.form.value.failedAuthAttempts || 0,
        roleIds: this.form.value.roleIds as number[],
        manufacturerIds: this.isManufacturerRoleSelected() ? ((this.form.get('manufacturerIds')!.value as number[]) || []) : []
      };
      this.userService.createUser(payload).subscribe({
        next: (_created: any) => this.router.navigate(['/admin/users']),
        error: (e) => { this.error = e?.error?.message || e?.message || 'Failed to create user'; this.saving = false; }
      });
    }
  }

  toggleEditMode(): void {
    this.isViewMode = false;
    this.form.enable();
    // Re-disable userId field (it's always disabled in edit mode)
    this.form.get('userId')?.disable();
    if (this.id) {
      this.router.navigate(['/admin/users/edit', this.id], { replaceUrl: true });
    }
  }

  onCancel(): void {
    if (this.isViewMode) {
      // In view mode, just go back to list
      this.router.navigate(['/admin/users']);
    } else if (this.isEdit) {
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
          this.router.navigate(['/admin/users']);
        }
      } else {
        this.router.navigate(['/admin/users']);
      }
    }
  }

  private cancelToViewMode(): void {
    this.isViewMode = true;
    this.form.disable();
    if (this.id) {
      // Reload user data
      this.userService.getUserById(this.id).subscribe(user => {
        this.form.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email,
          phoneNumber: user.phoneNumber,
          company: user.company,
          jobTitle: user.jobTitle ?? user.title,
          address: user.address,
          city: user.city,
          state: user.state,
          postCode: user.postCode,
          notes: user.notes,
          industryId: (user as any).industryId ?? null,
          groupEmail: (user as any).groupEmail ?? false,
          status: this.mapStatusToNumber((user as any).accountStatus ?? user.status),
          failedAuthAttempts: user.failedAuthAttempts ?? 0,
          isActive: user.isActive,
        });
        this.syncIsActiveWithStatus();
        const roleIds = (user.roles || []).map(r => r.id);
        this.form.get('roleIds')?.setValue(roleIds);
      });
      this.router.navigate(['/admin/users/view', this.id], { replaceUrl: true });
    }
  }

  private mapStatusToNumber(val: any): number {
    if (typeof val === 'number') {
      return val || 1;
    }
    if (typeof val === 'string') {
      const s = val.trim().toLowerCase();
      if (s === 'active') return 1;
      if (s === 'locked') return 2;
      if (s === 'suspended') return 3;
      if (s === 'inactive') return 3; // fallback mapping if API uses 'Inactive'
    }
    return 1;
  }
}

