import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserService, User, PaginatedResult } from '../../../../services/user.service';
import { RoleService } from '../../../../services/role.service';
import { ManufacturerService } from '../../../../services/manufacturer.service';
import { ExcelExportService } from '../../../../shared/services/excel-export.service';
import { Role } from '../../../../services/auth.service';
import { Manufacturer } from '../../../../models/manufacturer.model';
import { ListStateService } from '../../../../shared/services/list-state.service';
import { LIST_PAGE_KEYS } from '../../../../shared/constants/list-page-keys';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  styleUrls: ['./user-list.component.css'],
  template: `
    <main class="container">
      <nav aria-label="breadcrumb" id="breadcrumb">
        <ol class="breadcrumb">
          <li class="breadcrumb-item"><a routerLink="/admin">Administration</a></li>
          <li class="breadcrumb-item active">Users</li>
        </ol>
      </nav>

      <div>
        <h3>
          <span>Users</span>
          <span>
            <a routerLink="/admin/users/create" class="fs-3 ms-2" title="Add new user">
              <i class="fa-solid fa-plus"></i>
            </a>
          </span>
        </h3>

        <form>
          <div class="d-flex">
            <input
              type="search"
              class="form-control flex-fill me-2"
              placeholder="Search by ID, username, name, or email"
              [(ngModel)]="searchTerm"
              (input)="onSearchChange($any($event.target).value || '')"
              name="search">
              <div class="btn-group me-2" role="group" aria-label="Filter by account status">
                <button type="button" class="btn status-filter-btn" [class.active]="accountStatusFilter === 'Active'" (click)="onAccountStatusFilterChange('Active')">Active</button>
                <button type="button" class="btn status-filter-btn" [class.active]="accountStatusFilter === 'Locked'" (click)="onAccountStatusFilterChange('Locked')">Locked</button>
                <button type="button" class="btn status-filter-btn" [class.active]="accountStatusFilter === 'Suspended'" (click)="onAccountStatusFilterChange('Suspended')">Suspended</button>
              </div>

            <!-- Advanced Filters Toggle -->
            <a href="#" class="fs-3 me-2" data-bs-toggle="modal" data-bs-target="#user-filter-modal">
              <i class="fa-solid fa-filter"></i>
            </a>

            <div class="dropdown">
              <a href="#" data-bs-toggle="dropdown" aria-expanded="false" class="fs-3">
                <i class="fa-solid fa-bars"></i>
              </a>
              <ul class="dropdown-menu">
                <li>
                  <button type="button" class="dropdown-item" (click)="exportToExcel()" [disabled]="loading">
                    <i class="fa-solid fa-download"></i> Export Results
                  </button>
                </li>
              </ul>
            </div>
          </div>
        </form>

        <!-- Filter Pills -->
        <div class="d-flex flex-wrap gap-2 mt-2 align-items-center">
          <span *ngIf="accountStatusFilter" class="badge text-bg-denim filter-pill clickable" (click)="onAccountStatusFilterChange('')">
            Status: {{accountStatusFilter | titlecase}} ✕
          </span>
          <span *ngIf="searchTerm" class="badge text-bg-denim filter-pill clickable" (click)="onSearchChange('')">
            Search: {{searchTerm}} ✕
          </span>
          <span *ngIf="selectedRoleId != null" class="badge text-bg-denim filter-pill clickable" (click)="removeFilter('role')">
            Role: {{ getRoleName(selectedRoleId) }} ✕
          </span>
          <span *ngIf="selectedManufacturerId != null" class="badge text-bg-denim filter-pill clickable" (click)="removeFilter('manufacturer')">
            Manufacturer: {{ getManufacturerName(selectedManufacturerId) }} ✕
          </span>
          <span *ngIf="accountStatusFilter || searchTerm || selectedRoleId != null || selectedManufacturerId != null"
                class="badge text-bg-denim filter-pill clickable"
                (click)="clearAllFilters()">
            Clear All Filters
          </span>
        </div>


        <div *ngIf="loading" class="text-center my-4">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>

        <div *ngIf="error" class="alert alert-danger mt-3">{{error}}</div>

        <div class="table-responsive mt-3" *ngIf="!loading">
          <table class="table">
            <thead>
              <tr>
                <th style="cursor: pointer;" (click)="onSort('Id')">
                  ID
                  <i class="fa-solid ms-1"
                     [class.fa-sort]="sortBy !== 'Id'"
                     [class.fa-sort-up]="sortBy === 'Id' && sortDirection === 'asc'"
                     [class.fa-sort-down]="sortBy === 'Id' && sortDirection === 'desc'"></i>
                </th>
                <th style="cursor: pointer;" (click)="onSort('UserId')">
                  User ID
                  <i class="fa-solid ms-1"
                     [class.fa-sort]="sortBy !== 'UserId'"
                     [class.fa-sort-up]="sortBy === 'UserId' && sortDirection === 'asc'"
                     [class.fa-sort-down]="sortBy === 'UserId' && sortDirection === 'desc'"></i>
                </th>
                <th style="cursor: pointer;" (click)="onSort('FirstName')">
                  Name
                  <i class="fa-solid ms-1"
                     [class.fa-sort]="sortBy !== 'FirstName'"
                     [class.fa-sort-up]="sortBy === 'FirstName' && sortDirection === 'asc'"
                     [class.fa-sort-down]="sortBy === 'FirstName' && sortDirection === 'desc'"></i>
                </th>
                <th style="cursor: pointer;" (click)="onSort('Company')">
                  Company Name
                  <i class="fa-solid ms-1"
                     [class.fa-sort]="sortBy !== 'Company'"
                     [class.fa-sort-up]="sortBy === 'Company' && sortDirection === 'asc'"
                     [class.fa-sort-down]="sortBy === 'Company' && sortDirection === 'desc'"></i>
                </th>
                <th style="cursor: pointer;" (click)="onSort('Roles')">
                  Roles
                  <i class="fa-solid ms-1"
                     [class.fa-sort]="sortBy !== 'Roles'"
                     [class.fa-sort-up]="sortBy === 'Roles' && sortDirection === 'asc'"
                     [class.fa-sort-down]="sortBy === 'Roles' && sortDirection === 'desc'"></i>
                </th>
                <th style="cursor: pointer;" (click)="onSort('IsActive')">
                  Status
                  <i class="fa-solid ms-1"
                     [class.fa-sort]="sortBy !== 'IsActive'"
                     [class.fa-sort-up]="sortBy === 'IsActive' && sortDirection === 'asc'"
                     [class.fa-sort-down]="sortBy === 'IsActive' && sortDirection === 'desc'"></i>
                </th>
                <th class="text-center">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let u of paginatedItems">
                <td>{{u.id}}</td>
                <td>
                  <span *ngIf="u.userId">{{u.userId}}</span>
                  <span *ngIf="!u.userId" class="badge bg-warning text-dark">Pending Registration</span>
                </td>
                <td>{{u.firstName}} {{u.lastName}}</td>
                <td>{{getCompanyName(u)}}</td>
                <td>{{getRolesNames(u) || '-'}}</td>
                <td>
                  <span class="badge" [class.bg-success]="getAccountStatusName(u) === 'Active'" [class.bg-danger]="getAccountStatusName(u) !== 'Active'">{{getAccountStatusName(u)}}</span>
                </td>
                <td>
                  <div class="d-flex justify-content-center">
                    <a [routerLink]="['/admin/users/view', u.id]" class="me-2 fs-6 icon" title="View User">
                      <i class="fa-solid fa-eye"></i>
                    </a>
                    <a *ngIf="getAccountStatusName(u) === 'Suspended'" href="#" class="me-2 fs-6 icon text-success" (click)="onUnsuspend(u); $event.preventDefault()" title="Unsuspend User">
                      <i class="fa-solid fa-unlock"></i>
                    </a>
                    <a *ngIf="isHeadlessUser(u)" href="#" class="me-2 fs-6 icon text-primary" (click)="onResendInvitation(u); $event.preventDefault()" title="Resend Registration Invitation">
                      <i class="fa-solid fa-envelope"></i>
                    </a>
                    <a href="#" class="fs-6 icon text-danger" (click)="openDeleteModal(u); $event.preventDefault()">
                      <i class="fa-solid fa-trash-can"></i>
                    </a>
                  </div>
                </td>
              </tr>
            </tbody>


          </table>
        </div>

        <div class="d-flex flex-row-reverse mt-3" *ngIf="!loading && paginatedItems.length > 0">
          <ul class="pagination">
            <li class="page-item" [class.disabled]="pagination.currentPage === 1">
              <a href="#" class="page-link" (click)="onPageChange(1); $event.preventDefault()">&lt;&lt;</a>
            </li>
            <li class="page-item" [class.disabled]="pagination.currentPage === 1">
              <a href="#" class="page-link" (click)="onPageChange(pagination.currentPage - 1); $event.preventDefault()">&lt;</a>
            </li>
            <li *ngFor="let page of getPageNumbers()" class="page-item" [class.active]="page === pagination.currentPage" [class.disabled]="page === -1">
              <a href="#" class="page-link" (click)="page !== -1 ? onPageChange(page) : null; $event.preventDefault()">{{page === -1 ? '...' : page}}</a>
            </li>
            <li class="page-item" [class.disabled]="pagination.currentPage === pagination.totalPages">
              <a href="#" class="page-link" (click)="onPageChange(pagination.currentPage + 1); $event.preventDefault()">&gt;</a>
            </li>
            <li class="page-item" [class.disabled]="pagination.currentPage === pagination.totalPages">
              <a href="#" class="page-link" (click)="onPageChange(pagination.totalPages); $event.preventDefault()">&gt;&gt;</a>
            </li>
          </ul>

          <div class="me-5 align-self-center">
            <span>{{pagination.startIndex + 1}} to {{Math.min(pagination.endIndex + 1, pagination.totalItems)}} of {{pagination.totalItems}}</span>
          </div>

          <div class="me-5 align-self-center">
            <label class="form-label m-0" for="pageSize">Rows per page:</label>
            <select class="m-0 p-0 border-top-0 border-start-0 border-end-0 border-bottom-1" [(ngModel)]="pagination.pageSize" [ngModelOptions]="{standalone: true}" (ngModelChange)="onPageSizeChange(+($event))" name="pageSize">
              <option [ngValue]="1">1</option>
              <option [ngValue]="2">2</option>
              <option [ngValue]="5">5</option>
              <option [ngValue]="10">10</option>
              <option [ngValue]="20">20</option>
              <option [ngValue]="50">50</option>
              <option [ngValue]="100">100</option>
            </select>
          </div>
        </div>


        <div *ngIf="!loading && paginatedItems.length === 0" class="text-center my-5">
        <!-- updated below with paginatedItems count -->

          <p class="text-muted">No users found.</p>
          <a routerLink="/admin/users/create" class="btn btn-denim">
            <i class="fa-solid fa-plus me-2"></i>Add First User
          </a>
        </div>
      </div>

      <div *ngIf="showDeleteModal" class="modal fade show d-block" tabindex="-1" role="dialog" aria-modal="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Confirm Deletion</h5>
              <button type="button" class="btn-close" aria-label="Close" (click)="closeDeleteModal()"></button>


            </div>
            <div class="modal-body">
              <p>Are you sure you want to delete this user?</p>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-denim" (click)="closeDeleteModal()">Cancel</button>
              <button type="button" class="btn btn-denim" (click)="confirmDelete()">Delete</button>
            </div>
          </div>
        </div>
      </div>
      <div *ngIf="showDeleteModal" class="modal-backdrop fade show"></div>
    </main>

    <!-- Advanced Filters Modal -->
    <div class="modal fade" id="user-filter-modal" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-lg modal-dialog-scrollable">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Additional Filters</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <div class="row g-3">
              <div class="col-12 col-md-6">
                <label class="form-label">Role</label>
                <select class="form-select" [(ngModel)]="advancedFilterState['roleId']" [ngModelOptions]="{standalone: true}" name="advRole">
                  <option [ngValue]="null">All</option>
                  <option *ngFor="let role of roles" [ngValue]="role.id">{{role.name}}</option>
                </select>
              </div>
              <div class="col-12 col-md-6">
                <label class="form-label">Manufacturer</label>
                <select class="form-select" [(ngModel)]="advancedFilterState['manufacturerId']" [ngModelOptions]="{standalone: true}" name="advManufacturer">
                  <option [ngValue]="null">All</option>
                  <option *ngFor="let mfr of manufacturers" [ngValue]="mfr.id">{{mfr.name}}</option>
                </select>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-outline-secondary" (click)="clearAdvancedFilters()" data-bs-dismiss="modal">Clear</button>
            <button type="button" class="btn btn-denim" (click)="applyAdvancedFilters()" data-bs-dismiss="modal">Apply</button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class UserListComponent implements OnInit, OnDestroy {
  loading = false;
  error = '';

  // filters and sorting
  searchTerm = '';
  accountStatusFilter: '' | 'Active' | 'Locked' | 'Suspended' = '';
  sortBy: string = 'Id';
  sortDirection: 'asc' | 'desc' = 'asc';

  selectedRoleId: number | null = null;
  selectedManufacturerId: number | null = null;

  // Advanced filter state
  advancedFilterState: { [key: string]: any } = {
    roleId: null,
    manufacturerId: null
  };

  // Reference data
  roles: Role[] = [];
  manufacturers: Manufacturer[] = [];

  // pagination
  pagination = {
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0,
    startIndex: 0,
    endIndex: 0
  };

  paginatedItems: User[] = [];
  Math = Math;

  showDeleteModal = false;
  itemToDelete: User | null = null;

  private readonly pageKey = LIST_PAGE_KEYS.USERS;

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private manufacturerService: ManufacturerService,
    private excelExportService: ExcelExportService,
    private listStateService: ListStateService
  ) {}

  ngOnInit(): void {
    this.restoreState();
    this.loadData();
    this.loadRoles();
    this.loadManufacturers();
  }

  ngOnDestroy(): void {
    this.saveCurrentState();
  }

  private saveCurrentState(): void {
    this.listStateService.saveState(this.pageKey, {
      filters: {
        searchTerm: this.searchTerm,
        accountStatusFilter: this.accountStatusFilter,
        selectedRoleId: this.selectedRoleId,
        selectedManufacturerId: this.selectedManufacturerId,
        advancedFilterState: { ...this.advancedFilterState }
      },
      pageIndex: this.pagination.currentPage,
      pageSize: this.pagination.pageSize,
      sortField: this.sortBy,
      sortDirection: this.sortDirection,
      savedAt: Date.now()
    });
  }

  private restoreState(): void {
    const state = this.listStateService.getState(this.pageKey);
    if (!state) return;
    this.pagination.currentPage = state.pageIndex || 1;
    this.pagination.pageSize = state.pageSize || 10;
    this.sortBy = state.sortField || 'Id';
    this.sortDirection = state.sortDirection || 'asc';
    if (state.filters) {
      this.searchTerm = state.filters['searchTerm'] ?? '';
      this.accountStatusFilter = state.filters['accountStatusFilter'] ?? '';
      this.selectedRoleId = state.filters['selectedRoleId'] ?? null;
      this.selectedManufacturerId = state.filters['selectedManufacturerId'] ?? null;
      if (state.filters['advancedFilterState']) {
        this.advancedFilterState = { ...state.filters['advancedFilterState'] };
      }
    }
  }

  loadData(): void {
    this.loading = true;
    this.userService
      .getPaginated(
        this.pagination.currentPage,
        this.pagination.pageSize,
        this.sortBy,
        this.sortDirection,
        this.searchTerm || '',
        this.accountStatusFilter || ''
      )
      .subscribe({
        next: (result: PaginatedResult<User>) => {
          let items = result.items;

          if (this.selectedRoleId != null) {
            items = items.filter(u => this.userHasRole(u, this.selectedRoleId!));
          }
          if (this.selectedManufacturerId != null) {
            items = items.filter(u => this.userHasManufacturer(u, this.selectedManufacturerId!));
          }
          this.paginatedItems = items;
          this.pagination.totalItems = result.totalCount;
          this.pagination.totalPages = result.totalPages;
          this.updatePageWindow();
          this.loading = false;
        },
        error: () => {
          this.error = 'Failed to load users';
          this.loading = false;
        }
      });
  }

  updatePageWindow(): void {
    const start = (this.pagination.currentPage - 1) * this.pagination.pageSize;
    const end = start + this.pagination.pageSize - 1;
    this.pagination.startIndex = start;
    this.pagination.endIndex = Math.min(end, this.pagination.totalItems - 1);
  }

  onSearchChange(value: string): void {
    this.searchTerm = value;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  onAccountStatusFilterChange(value: '' | 'Active' | 'Locked' | 'Suspended'): void {
    this.accountStatusFilter = value;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAllFilters(): void {
    this.searchTerm = '';
    this.accountStatusFilter = '';
    this.selectedRoleId = null;
    this.selectedManufacturerId = null;
    this.advancedFilterState = { roleId: null, manufacturerId: null };
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  onSort(field: string): void {
    if (this.sortBy === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = field;
      this.sortDirection = 'asc';
    }

    if (field === 'Roles') {
      const dir = this.sortDirection === 'asc' ? 1 : -1;
      this.paginatedItems = [...this.paginatedItems].sort((a, b) => {
        const ra = this.getRolesNames(a).toLowerCase();
        const rb = this.getRolesNames(b).toLowerCase();
        return ra < rb ? -1 * dir : ra > rb ? 1 * dir : 0;
      });
      this.saveCurrentState();
    } else {
      this.saveCurrentState();
      this.loadData();
    }
  }

  onPageChange(page: number): void {
    if (page < 1 || page > this.pagination.totalPages) return;
    this.pagination.currentPage = page;
    this.saveCurrentState();
    this.loadData();
  }

  onPageSizeChange(size: number): void {
    this.pagination.pageSize = size;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  getPageNumbers(): number[] {
    const totalPages = this.pagination.totalPages;
    const current = this.pagination.currentPage;
    const pages: number[] = [];

    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      pages.push(1);
      if (current > 4) pages.push(-1);
      const start = Math.max(2, current - 1);
      const end = Math.min(totalPages - 1, current + 1);
      for (let i = start; i <= end; i++) pages.push(i);
      if (current < totalPages - 3) pages.push(-1);
      pages.push(totalPages);
    }

    return pages;
  }

  exportToExcel(): void {
    if (this.loading || !this.paginatedItems.length) return;

    const exportData = this.paginatedItems.map(u => {
      const anyU = u as any;
      const manufacturers = (anyU.manufacturers as { name: string }[] | undefined) || [];
      return {
        'ID': u.id,
        'First Name': u.firstName,
        'Last Name': u.lastName,
        'Email': u.email,
        'Phone': anyU.phoneNumber || '-',
        'Company': anyU.company || '-',
        'Job Title': anyU.jobTitle || '-',
        'Status': this.getAccountStatusName(u),
        'Address': anyU.address || '-',
        'City': anyU.city || '-',
        'State': anyU.state || '-',
        'Postal Code': anyU.postCode || '-',
        'Notes': anyU.notes || '-',
        'Roles': this.getRolesNames(u),
        'Manufacturers': manufacturers.length ? manufacturers.map(m => m.name).join(', ') : '-'
      };
    });

    this.excelExportService.exportToExcel(exportData, 'Users');
  }

  openDeleteModal(item: User): void {
    this.itemToDelete = item;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.itemToDelete = null;
    this.showDeleteModal = false;
  }

  getRolesNames(u: User): string {
    const roles = (u as any).roles as { name: string }[] | undefined;
    return roles && roles.length ? roles.map(r => r.name).join(', ') : '';
  }

  getAccountStatusName(u: User): string {
    const anyU: any = u as any;
    if (anyU.accountStatusName) return anyU.accountStatusName as string;
    if (typeof anyU.accountStatus === 'number') {
      switch (anyU.accountStatus) {
        case 1: return 'Active';
        case 2: return 'Locked';
        case 3: return 'Suspended';
        case 4: return 'Headless';
      }
    }
    return u.isActive ? 'Active' : 'Suspended';
  }

  confirmDelete(): void {
    if (!this.itemToDelete) return;
    this.userService.deleteUser(this.itemToDelete.id).subscribe({
      next: () => {
        this.paginatedItems = this.paginatedItems.filter(i => i.id !== this.itemToDelete!.id);
        this.pagination.totalItems = Math.max(0, this.pagination.totalItems - 1);
        this.updatePageWindow();
        this.closeDeleteModal();
      },
      error: () => {
        this.error = 'Failed to delete user';
        this.closeDeleteModal();
      }
    });
  }

  onUnsuspend(user: User): void {
    if (!confirm(`Are you sure you want to unsuspend user ${user.firstName} ${user.lastName}?`)) {
      return;
    }
    this.userService.unsuspendUser(user.id).subscribe({
      next: () => {
        this.loadData();
      },
      error: () => {
        this.error = 'Failed to unsuspend user';
      }
    });
  }

  loadRoles(): void {
    this.roleService.getAll().subscribe({
      next: (roles) => {
        this.roles = roles || [];
      },
      error: (err) => {
        console.error('Failed to load roles', err);
      }
    });
  }

  loadManufacturers(): void {
    this.manufacturerService.getAllActive().subscribe({
      next: (manufacturers: any) => {
        this.manufacturers = manufacturers || [];
      },
      error: (err: any) => {
        console.error('Failed to load manufacturers', err);
      }
    });
  }

  removeFilter(filterType: string): void {
    if (filterType === 'role') {
      this.selectedRoleId = null;
      this.advancedFilterState['roleId'] = null;
    } else if (filterType === 'manufacturer') {
      this.selectedManufacturerId = null;
      this.advancedFilterState['manufacturerId'] = null;
    }
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  applyAdvancedFilters(): void {
    this.selectedRoleId = this.advancedFilterState['roleId'];
    this.selectedManufacturerId = this.advancedFilterState['manufacturerId'];
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.advancedFilterState = { roleId: null, manufacturerId: null };
    this.selectedRoleId = null;
    this.selectedManufacturerId = null;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  getRoleName(roleId: number): string {
    const role = this.roles.find(r => r.id === roleId);
    return role ? role.name : 'Unknown';
  }

  getManufacturerName(manufacturerId: number): string {
    const manufacturer = this.manufacturers.find(m => m.id === manufacturerId);
    return manufacturer ? manufacturer.name : 'Unknown';
  }

  userHasRole(user: User, roleId: number): boolean {
    const roles = (user as any).roles as { id: number; name: string }[] | undefined;
    return roles ? roles.some(r => r.id === roleId) : false;
  }

  userHasManufacturer(user: User, manufacturerId: number): boolean {
    const manufacturers = (user as any).manufacturers as { id: number; name: string }[] | undefined;
    return manufacturers ? manufacturers.some(m => m.id === manufacturerId) : false;
  }

  getCompanyName(user: User): string {
    return (user as any).company || '-';
  }

  isHeadlessUser(user: User): boolean {
    return (user as any).isHeadless === true || !user.userId;
  }

  onResendInvitation(user: User): void {
    if (!confirm(`Resend registration invitation to ${user.firstName} ${user.lastName}?`)) {
      return;
    }
    this.userService.resendRegistrationInvitation(user.id).subscribe({
      next: () => {
        alert('Registration invitation resent successfully.');
      },
      error: () => {
        this.error = 'Failed to resend registration invitation';
      }
    });
  }
}
