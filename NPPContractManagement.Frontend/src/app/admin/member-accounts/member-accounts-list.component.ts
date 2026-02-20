import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BaseListComponent, ListColumn } from '../../shared/components/base-list.component';
import { Industry } from '../../models/industry.model';
import { IndustryService } from '../../services/industry.service';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { MemberAccountService } from '../../services/member-account.service';
import { MemberAccount, MemberAccountHelper } from '../../models/member-account.model';
import { US_STATES } from '../../shared/utils/us-states.constants';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-member-accounts-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './member-accounts-list.component.html',
  styleUrls: ['./member-accounts-list.component.css']
})
export class MemberAccountsListComponent extends BaseListComponent<MemberAccount> implements OnInit {
  sortBy: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // Advanced filter state
  override showAdvancedFilters = false;
  advancedFilterState: { [key: string]: any } = { industryId: '', state: '' };
  industries: Industry[] = [];
  industryFilter: number | '' = '';
  stateFilter: string = '';
  usStates = US_STATES;

  override columns: ListColumn[] = [
	  { key: 'id', label: 'ID', sortable: true, type: 'number' },
	  { key: 'memberNumber', label: 'Member Number', sortable: true, type: 'text' },
	  { key: 'parentMemberAccountNumber', label: 'Parent Number', sortable: true, type: 'text' },
	  { key: 'facilityName', label: 'Member Name', sortable: true, type: 'text' },
	  { key: 'city', label: 'City', sortable: true, type: 'text' },
	  { key: 'state', label: 'State', sortable: true, type: 'text' },
	  { key: 'w9', label: 'W9 Status', sortable: true, type: 'boolean' },
	  { key: 'status', label: 'Status', sortable: true, type: 'status' }
  ];

  // Delete modal state
  showDeleteModal = false;
  itemToDelete: MemberAccount | null = null;

  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private memberAccountService: MemberAccountService,
    private industryService: IndustryService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Member Accounts';
    this.serverDriven = true;
  }

  override ngOnInit(): void {
    // Initial load: no filters applied (status, search, advanced)
    this.statusFilter = '';
    this.searchTerm = '';
    this.pagination.currentPage = 1;
    this.loadIndustries();
    this.loadData();
  }

  private loadIndustries(): void {
    this.industryService.getActive().subscribe({
      next: (list) => this.industries = list || [],
      error: (err) => console.error('Failed to load industries', err)
    });
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.MEMBER_ACCOUNTS;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter,
      industryFilter: this.industryFilter,
      stateFilter: this.stateFilter,
      advancedFilterState: { ...this.advancedFilterState }
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.statusFilter = filters['statusFilter'] ?? '';
    this.industryFilter = filters['industryFilter'] ?? '';
    this.stateFilter = filters['stateFilter'] ?? '';
    if (filters['advancedFilterState']) {
      this.advancedFilterState = { ...filters['advancedFilterState'] };
    }
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    const statusParam = this.statusFilter || undefined;

    this.memberAccountService.getPaginated(
      this.pagination.currentPage,
      this.pagination.pageSize,
      this.sortBy || undefined,
      this.sortDirection,
      this.searchTerm || undefined,
      statusParam,
      this.industryFilter || undefined,
      this.stateFilter || undefined
    ).subscribe({
      next: (response) => {
        this.items = response.items ?? [];
        this.filteredItems = [...this.items];
        this.paginatedItems = [...this.items];

        // Client-side fallback sort to reflect header click immediately
        if (this.sortBy) {
          this.paginatedItems.sort((a: any, b: any) => this.compareValues(a, b, this.sortBy));
          if (this.sortDirection === 'desc') this.paginatedItems.reverse();
        }

        this.selectAll = false;
        this.selectedItems.clear();
        this.pagination.totalItems = response.totalCount ?? this.items.length;
        this.pagination.totalPages = Math.ceil((this.pagination.totalItems || 0) / (this.pagination.pageSize || 1));
        this.updatePaginationIndices();
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load member accounts. Please try again.';
        this.loading = false;
        console.error('Error loading member accounts:', error);
      }
    });
  }

  // Export: include ID for Member Accounts exports (BaseListComponent excludes id by default)
  override exportToExcel(): void {
    const dataToExport = this.excelExportService.prepareDataForExport(
      this.filteredItems,
      ['createdDate', 'modifiedDate']
    );

    const formattedData = this.excelExportService.formatHeaders(dataToExport);

    this.excelExportService.exportToExcel(
      formattedData,
      `${this.entityName}_Export`,
      this.entityName
    );
  }

  override exportSelectedToExcel(): void {
    if (this.selectedItems.size === 0) {
      alert('Please select items to export');
      return;
    }

    const selectedData = this.items.filter(item =>
      this.selectedItems.has(this.getItemId(item))
    );

    const dataToExport = this.excelExportService.prepareDataForExport(
      selectedData,
      ['createdDate', 'modifiedDate']
    );

    const formattedData = this.excelExportService.formatHeaders(dataToExport);

    this.excelExportService.exportToExcel(
      formattedData,
      `${this.entityName}_Selected_Export`,
      this.entityName
    );
  }
  // Server-driven overrides
  override onSearchChange(term: string): void {
    this.searchTerm = term;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  override onStatusFilterChange(status: string): void {
    const next = this.statusFilter === status ? '' : status;
    this.statusFilter = next;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  override onSort(field: string): void {
    if (this.sortBy === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = field;
      this.sortDirection = 'asc';
    }
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  override getSortIcon(field: string): string {
    if (this.sortBy !== field) return 'fa-sort';
    return this.sortDirection === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }

  override onPageChange(page: number): void {
    this.pagination.currentPage = Math.max(1, Math.min(page, this.pagination.totalPages || 1));
    this.saveCurrentState();
    this.loadData();
  }

  override onPageSizeChange(size: number): void {
    this.pagination.pageSize = size;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  override clearAllFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.sortBy = '';
    this.sortDirection = 'asc';
    // Clear advanced filters
    this.industryFilter = '';
    this.stateFilter = '';
    this.advancedFilterState['industryId'] = '';
    this.advancedFilterState['state'] = '';
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }


  deleteItem(id: number): void {
    this.memberAccountService.delete(id).subscribe({
      next: () => {
        this.loadData();
      },
      error: (error) => {
        this.error = 'Failed to delete member account. Please try again.';
        console.error('Error deleting member account:', error);
      }
    });
  }

  getItemId(item: MemberAccount): number {
    return item.id;
  }

  openDeleteModal(item: MemberAccount): void {
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

  protected override matchesStatus(item: MemberAccount, status: string): boolean {
    switch (status) {
      case 'active':
        return MemberAccountHelper.isActive(item);
      case 'inactive':
        return !MemberAccountHelper.isActive(item) && !MemberAccountHelper.isSuspended(item);
      case 'w9-missing':
        return !item.w9;
      default:
        return item.status === status;
    }
  }

  // Helper methods for template
  getStatusColor(status: string): string {
    return MemberAccountHelper.getStatusColor(status as any);
  }

  getDisplayName(memberAccount: MemberAccount): string {
    return MemberAccountHelper.getDisplayName(memberAccount);
  }

  formatAddress(memberAccount: MemberAccount): string {
    return MemberAccountHelper.formatAddress(memberAccount);
  }

  getW9Status(memberAccount: MemberAccount): string {
    return MemberAccountHelper.getW9Status(memberAccount);
  }

  getW9StatusColor(memberAccount: MemberAccount): string {
    return MemberAccountHelper.getW9StatusColor(memberAccount);
  }

  canEdit(memberAccount: MemberAccount): boolean {
    return MemberAccountHelper.canEdit(memberAccount);
  }

  canDelete(memberAccount: MemberAccount): boolean {
    return MemberAccountHelper.canDelete(memberAccount);
  }

  // Advanced Filters control
  override toggleAdvancedFilters(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  override onAdvancedFilterChange(key: string, value: any): void {
    this.advancedFilterState[key] = value;
  }

  applyAdvancedFilters(): void {
    const val = this.advancedFilterState['industryId'];
    this.industryFilter = val !== '' && val !== null && val !== undefined ? Number(val) : '';
    const stateVal = this.advancedFilterState['state'];
    this.stateFilter = stateVal !== '' && stateVal !== null && stateVal !== undefined ? stateVal : '';
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.industryFilter = '';
    this.stateFilter = '';
    this.advancedFilterState['industryId'] = '';
    this.advancedFilterState['state'] = '';
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  removeFilter(key: string): void {
    if (key === 'search') {
      this.onSearchChange('');
      return;
    }
    if (key === 'status') {
      this.onStatusFilterChange('');
      return;
    }
    if (key === 'industryId') {
      this.industryFilter = '';
      this.advancedFilterState['industryId'] = '';
    }
    if (key === 'state') {
      this.stateFilter = '';
      this.advancedFilterState['state'] = '';
    }
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  getIndustryNameById(id: number | ''): string {
    if (id === '' || id === null || id === undefined) return '';
    const found = this.industries.find(i => i.id === Number(id));
    return found?.name || '';
  }

  private compareValues(a: any, b: any, key: string): number {
    const av = (a?.[key] ?? '').toString().toLowerCase();
    const bv = (b?.[key] ?? '').toString().toLowerCase();
    if (key === 'id') {
      return (a?.[key] ?? 0) - (b?.[key] ?? 0);
    }
    return av.localeCompare(bv);
  }

  private updatePaginationIndices(): void {
    this.pagination.startIndex = (this.pagination.currentPage - 1) * this.pagination.pageSize;
    this.pagination.endIndex = Math.min(
      this.pagination.startIndex + this.pagination.pageSize - 1,
      this.pagination.totalItems - 1
    );
  }

  override getPageNumbers(): number[] {
    const pages: number[] = [];
    const totalPages = this.pagination.totalPages;
    const currentPage = this.pagination.currentPage;

    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      if (currentPage <= 4) {
        for (let i = 1; i <= 5; i++) {
          pages.push(i);
        }
        pages.push(-1);
        pages.push(totalPages);
      } else if (currentPage >= totalPages - 3) {
        pages.push(1);
        pages.push(-1);
        for (let i = totalPages - 4; i <= totalPages; i++) {
          pages.push(i);
        }
      } else {
        pages.push(1);
        pages.push(-1);
        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
          pages.push(i);
        }
        pages.push(-1);
        pages.push(totalPages);
      }
    }

    return pages;
  }

  // Expose utilities for template
  Math = Math;
  Object = Object;
}
