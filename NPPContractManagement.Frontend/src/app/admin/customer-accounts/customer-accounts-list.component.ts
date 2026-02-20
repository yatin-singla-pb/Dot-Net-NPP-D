import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BaseListComponent, ListColumn, FilterOption } from '../../shared/components/base-list.component';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { CustomerAccountService } from '../../services/customer-account.service';
import { IndustryService } from '../../services/industry.service';
import { DistributorService } from '../../services/distributor.service';
import { OpCoService } from '../../services/opco.service';
import { MemberAccountService } from '../../services/member-account.service';
import { CustomerAccount, CustomerAccountHelper } from '../../models/customer-account.model';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-customer-accounts-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './customer-accounts-list.component.html',
  styleUrls: ['./customer-accounts-list.component.css']
})
export class CustomerAccountsListComponent extends BaseListComponent<CustomerAccount> implements OnInit {

  override statusFilter: string = '';
  override columns: ListColumn[] = [
    { key: 'id', label: 'ID', sortable: true, type: 'number' },
    { key: 'customerAccountNumber', label: 'Account #', sortable: true, type: 'text' },
    { key: 'customerName', label: 'Account Name', sortable: true, type: 'text' },
    { key: 'distributorName', label: 'Distributor', sortable: true, type: 'text' },
    { key: 'opCoName', label: 'Op-Co', sortable: true, type: 'text' },
    { key: 'city', label: 'City', sortable: true, type: 'text' },
    { key: 'startDate', label: 'Start Date', sortable: true, type: 'date' }
  ];

  sortBy: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  selectedStatus: string = 'All';

  // Advanced filter state
  override filterOptions: FilterOption[] = [
    { key: 'memberAccountId', label: 'Member Account', type: 'select' },
    { key: 'distributorId', label: 'Distributor', type: 'select' },
    { key: 'opCoId', label: 'Opco', type: 'select' },
    { key: 'association', label: 'Association', type: 'select' },
    { key: 'dso', label: 'DSO', type: 'text' },
    { key: 'state', label: 'State', type: 'text' }
  ];
  advancedFilterState: { [key: string]: any } = {
    memberAccountId: '',
    distributorId: '',
    opCoId: '',
    association: '',
    dso: '',
    state: ''
  };

  // Timeframe filter state
  timeframeFilter = {
    dateField: '', // 'startDate', 'endDate', 'auditDate', 'dateToEntegra'
    operator: '', // 'beforeOrOn', 'afterOrOn', 'equalTo', 'between'
    date1: '',
    date2: '' // only for 'between' operator
  };

  dateFieldOptions = [
    { value: 'startDate', label: 'Start Date' },
    { value: 'endDate', label: 'End Date' },
    { value: 'auditDate', label: 'Audit Date' }
  ];

  operatorOptions = [
    { value: 'beforeOrOn', label: 'Before or on' },
    { value: 'afterOrOn', label: 'After or on' },
    { value: 'equalTo', label: 'Equal to' },
    { value: 'between', label: 'Between' }
  ];

  industries: any[] = [];
  distributors: any[] = [];
  opcos: any[] = [];
  memberAccounts: any[] = [];
  associationOptions = [
    { value: 1, label: 'CSN' },
    { value: 2, label: 'Combined' },
    { value: 3, label: 'RCDM' },
    { value: 4, label: 'SEMUPC' }
  ];

  // Delete modal state
  showDeleteModal = false;
  itemToDelete: CustomerAccount | null = null;

  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private customerAccountService: CustomerAccountService,
    private industryService: IndustryService,
    private distributorService: DistributorService,
    private opCoService: OpCoService,
    private memberAccountService: MemberAccountService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Customer Accounts';
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.CUSTOMER_ACCOUNTS;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter,
      selectedStatus: this.selectedStatus,
      advancedFilterState: { ...this.advancedFilterState },
      timeframeFilter: { ...this.timeframeFilter }
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.statusFilter = filters['statusFilter'] ?? '';
    this.selectedStatus = filters['selectedStatus'] ?? 'All';
    if (filters['advancedFilterState']) {
      this.advancedFilterState = { ...filters['advancedFilterState'] };
    }
    if (filters['timeframeFilter']) {
      this.timeframeFilter = { ...filters['timeframeFilter'] };
    }
  }

  override ngOnInit(): void {
    super.ngOnInit();
    this.selectedStatus = this.statusFilter || 'All';
    this.industryService.getActive().subscribe({ next: (inds) => (this.industries = inds || []) });
    this.distributorService.getPaginated(1, 1000, undefined, 'asc', undefined, 'Active').subscribe({ next: (resp) => (this.distributors = resp.items || []) });
    this.opCoService.getPaginated(1, 1000, undefined, 'Active').subscribe({ next: (resp) => (this.opcos = resp.items || []) });
    this.memberAccountService.getPaginated(1, 1000, undefined, 'asc', undefined, 'Active').subscribe({ next: (resp) => (this.memberAccounts = resp.items || []) });
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    const params: any = {
      pageNumber: this.pagination.currentPage,
      pageSize: this.pagination.pageSize,
      sortBy: this.sortBy || undefined,
      sortDirection: this.sortDirection,
      searchTerm: this.searchTerm || undefined,
      status: this.statusFilter || undefined,
      isActive: true,
      memberAccountId: this.advancedFilterState['memberAccountId'] || undefined,
      distributorId: this.advancedFilterState['distributorId'] || undefined,
      opCoId: this.advancedFilterState['opCoId'] || undefined,
      association: this.advancedFilterState['association'] || undefined,
      dso: this.advancedFilterState['dso'] || undefined,
      state: this.advancedFilterState['state'] || undefined,
      // Timeframe filter
      timeframeDateField: this.timeframeFilter.dateField || undefined,
      timeframeOperator: this.timeframeFilter.operator || undefined,
      timeframeDate1: this.timeframeFilter.date1 || undefined,
      timeframeDate2: this.timeframeFilter.date2 || undefined
    };

    this.customerAccountService.searchCustomerAccounts(params).subscribe({
      next: (resp) => {
        this.items = resp.items || [];
        // Apply client-side status filtering for non Active/Inactive values
        let displayed = [...this.items];
        const sel = (this.selectedStatus || '').toLowerCase();
        if (sel && sel !== 'all') {
          if (sel === 'active' || sel === 'inactive') {
            // server already filtered; keep displayed as-is
          } else {
            displayed = displayed.filter(i => (i.status || '').toString().toLowerCase() === sel);
          }
        }
        this.filteredItems = [...displayed];
        this.paginatedItems = [...displayed];
        if (this.sortBy) {
          this.paginatedItems.sort((a: any, b: any) => this.compareValues(a, b, this.sortBy));
          if (this.sortDirection === 'desc') this.paginatedItems.reverse();
        }
        this.selectAll = false;
        this.selectedItems.clear();
        this.pagination.totalItems = (sel && sel !== 'all' && sel !== 'active' && sel !== 'inactive')
          ? displayed.length
          : (resp.totalCount || this.items.length);
        this.pagination.totalPages = Math.ceil(this.pagination.totalItems / this.pagination.pageSize);
        this.updatePaginationIndices();
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load customer accounts. Please try again.';
        this.loading = false;
        console.error('Error loading customer accounts:', error);
      }
    });
  }

  deleteItem(id: number): void {
    this.customerAccountService.delete(id).subscribe({
      next: () => {
        this.loadData();
      },
      error: (error) => {
        this.error = 'Failed to delete customer account. Please try again.';
        console.error('Error deleting customer account:', error);
      }
    });
  }

  getItemId(item: CustomerAccount): number {
    return item.id;
  }

  openDeleteModal(item: CustomerAccount): void {
    this.itemToDelete = item;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.itemToDelete = null;
  }

  confirmDelete(): void {
    if (!this.itemToDelete) return;
    const id = this.itemToDelete.id;
    this.deleteItem(id);
    this.closeDeleteModal();
  }

  // Override BaseListComponent to trigger server-side reloads
  override onSearchChange(term: string): void {
    this.searchTerm = term;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  override onStatusFilterChange(status: string): void {
    this.statusFilter = status || '';
    this.selectedStatus = this.statusFilter || 'All';
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  onStatusSelectChange(value: string): void {
    this.selectedStatus = value;
    if (value === 'All') {
      this.statusFilter = '';
    } else {
      this.statusFilter = value;
    }
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  override onSort(column: string): void {
    if (this.sortBy === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortDirection = 'asc';
    }
    this.saveCurrentState();
    this.loadData();
  }

  override onPageChange(page: number): void {
    this.pagination.currentPage = page;
    this.saveCurrentState();
    this.loadData();
  }

  override onPageSizeChange(pageSize: number): void {
    this.pagination.pageSize = pageSize;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  override clearAllFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.selectedStatus = 'All';
    this.sortBy = '';
    this.sortDirection = 'asc';
    this.advancedFilterState = {
      memberAccountId: '',
      distributorId: '',
      opCoId: '',
      association: '',
      dso: '',
      state: ''
    };
    this.timeframeFilter = {
      dateField: '',
      operator: '',
      date1: '',
      date2: ''
    };
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  override getSortIcon(field: string): string {
    if (this.sortBy !== field) return 'fa-sort';
    return this.sortDirection === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }

  applyAdvancedFilters(): void {
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.advancedFilterState = {
      memberAccountId: '',
      distributorId: '',
      opCoId: '',
      association: '',
      dso: '',
      state: ''
    };
    this.timeframeFilter = {
      dateField: '',
      operator: '',
      date1: '',
      date2: ''
    };
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  private compareValues(a: any, b: any, key: string): number {
    // Numeric comparisons
    if (key === 'id') {
      const an = Number(a?.[key] ?? 0);
      const bn = Number(b?.[key] ?? 0);
      return an - bn;
    }
    // Boolean comparisons (false < true)
    if (key === 'tracsAccess' || key === 'toEntegra') {
      const ab = Boolean(a?.[key]);
      const bb = Boolean(b?.[key]);
      return (ab === bb) ? 0 : (ab ? 1 : -1);
    }
    // Fallback string compare
    const av = (a?.[key] ?? '').toString().toLowerCase();
    const bv = (b?.[key] ?? '').toString().toLowerCase();
    return av.localeCompare(bv);
  }

  private updatePaginationIndices(): void {
    this.pagination.startIndex = (this.pagination.currentPage - 1) * this.pagination.pageSize;
    this.pagination.endIndex = Math.min(
      this.pagination.startIndex + this.pagination.pageSize - 1,
      this.pagination.totalItems - 1
    );
  }

  protected override matchesStatus(item: CustomerAccount, status: string): boolean {
    switch (status) {
      case 'active':
        return CustomerAccountHelper.isActive(item);
      case 'inactive':
        return !CustomerAccountHelper.isActive(item);
      default:
        return (item.status as any) === status;
    }
  }

  // Helper methods for template
  getStatusColor(status: string): string {
    return CustomerAccountHelper.getStatusColor(status as any);
  }

  getDisplayName(customerAccount: CustomerAccount): string {
    return CustomerAccountHelper.getDisplayName(customerAccount);
  }

  formatCurrency(amount?: number): string {
    return CustomerAccountHelper.formatCurrency(amount);
  }


  canEdit(customerAccount: CustomerAccount): boolean {
    return CustomerAccountHelper.canEdit(customerAccount);
  }

  canDelete(customerAccount: CustomerAccount): boolean {
    return CustomerAccountHelper.canDelete(customerAccount);
  }

  canSuspend(customerAccount: CustomerAccount): boolean {
    return CustomerAccountHelper.canSuspend(customerAccount);
  }

  getActiveFilterPills(): { key: string, label: string }[] {
    const pills: { key: string, label: string }[] = [];

    Object.keys(this.activeFilters).forEach(key => {
      const value = this.activeFilters[key];
      if (value !== null && value !== undefined && value !== '') {
        let label = '';
        switch (key) {
          case 'search':
            label = `Search: ${value}`;
            break;
          case 'status':
            label = `Status: ${value}`;
            break;
          default:
            label = `${key}: ${value}`;
        }
        pills.push({ key, label });
      }
    });

    return pills;
  }

  removeFilter(key: string): void {
    if (key === 'search') {
      this.searchTerm = '';
    } else if (key === 'status') {
      this.statusFilter = '';
    } else if (key === 'timeframe') {
      this.timeframeFilter = { dateField: '', operator: '', date1: '', date2: '' };
    } else if (['memberAccountId','distributorId','opCoId','association','dso','state'].includes(key)) {
      this.advancedFilterState[key] = '';
    }
    this.paginationService.removeFilter(key);
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  hasTimeframeFilter(): boolean {
    return !!(this.timeframeFilter.dateField && this.timeframeFilter.operator && this.timeframeFilter.date1);
  }

  getTimeframeFilterLabel(): string {
    if (!this.hasTimeframeFilter()) return '';
    const field = this.dateFieldOptions.find(f => f.value === this.timeframeFilter.dateField)?.label || '';
    const op = this.operatorOptions.find(o => o.value === this.timeframeFilter.operator)?.label || '';
    const date1 = this.timeframeFilter.date1;
    const date2 = this.timeframeFilter.date2;
    if (this.timeframeFilter.operator === 'between') {
      return `${field} ${op} ${date1} and ${date2}`;
    }
    return `${field} ${op} ${date1}`;
  }

  // Expose utilities for template
  Math = Math;
  Object = Object;

  getIndustryNameById(id: number | string): string {
    const nid = typeof id === 'string' ? parseInt(id, 10) : id;
    const ind = this.industries.find(i => i.id === nid);
    return ind ? ind.name : (id?.toString() || '');
  }

  getDistributorNameById(id: number | string): string {
    const nid = typeof id === 'string' ? parseInt(id, 10) : id;
    const d = this.distributors.find(i => i.id === nid);
    return d ? (d.name || d.distributorName || d.Name) : (id?.toString() || '');
  }

  getOpCoNameById(id: number | string): string {
    const nid = typeof id === 'string' ? parseInt(id, 10) : id;
    const o = this.opcos.find(i => i.id === nid);
    return o ? (o.name || o.opCoName || o.Name) : (id?.toString() || '');
  }

  getMemberAccountNameById(id: number | string): string {
    const nid = typeof id === 'string' ? parseInt(id, 10) : id;
    const m = this.memberAccounts.find(i => i.id === nid);
    return m ? (m.facilityName || m.memberAccountName || m.FacilityName) : (id?.toString() || '');
  }

  getAssociationName(val: number | string): string {
    const n = typeof val === 'string' ? parseInt(val, 10) : val;
    const found = this.associationOptions.find(a => a.value === n);
    return found ? found.label : (val?.toString() || '');
  }
}
