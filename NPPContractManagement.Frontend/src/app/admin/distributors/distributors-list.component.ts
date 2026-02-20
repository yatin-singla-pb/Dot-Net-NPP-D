import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BaseListComponent, ListColumn, FilterOption } from '../../shared/components/base-list.component';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { DistributorService } from '../../services/distributor.service';
import { Distributor, DistributorHelper, DistributorStatus } from '../../models/distributor.model';
import { US_STATES } from '../../shared/utils/us-states.constants';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-distributors-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './distributors-list.component.html',
  styleUrls: ['./distributors-list.component.css']
})
export class DistributorsListComponent extends BaseListComponent<Distributor> implements OnInit {

  override statusFilter: string = '';
  override columns: ListColumn[] = [
    { key: 'id', label: 'ID', sortable: true, type: 'number' },
    { key: 'name', label: 'Distributor Name', sortable: true, type: 'text' },
    { key: 'contactPerson', label: 'Primary Contact', sortable: true, type: 'text' },
    { key: 'city', label: 'City', sortable: true, type: 'text' },
    { key: 'state', label: 'State', sortable: true, type: 'text' },
    { key: 'status', label: 'Status', sortable: true, type: 'status' }
  ];

  sortBy: string = '';
  // Advanced filter state
  override filterOptions: FilterOption[] = [
    { key: 'state', label: 'State', type: 'select' },
    {
      key: 'receiveContractProposal',
      label: 'Receive Contract Proposal',
      type: 'select',
      options: [
        { value: 'true', label: 'Yes' },
        { value: 'false', label: 'No' }
      ]
    }
  ];
  advancedFilterState: { [key: string]: any } = { state: '', receiveContractProposal: '' };
  stateFilter: string = '';
  receiveContractProposalFilter: '' | 'true' | 'false' = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  usStates = US_STATES;

  // Expose enum for template comparisons
  DistributorStatus = DistributorStatus;

  // Delete modal state
  showDeleteModal = false;
  itemToDelete: Distributor | null = null;

  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private distributorService: DistributorService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Distributors';
  }

  override ngOnInit(): void {
    super.ngOnInit();
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.DISTRIBUTORS;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter,
      stateFilter: this.stateFilter,
      receiveContractProposalFilter: this.receiveContractProposalFilter,
      advancedFilterState: { ...this.advancedFilterState }
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.statusFilter = filters['statusFilter'] ?? '';
    this.stateFilter = filters['stateFilter'] ?? '';
    this.receiveContractProposalFilter = filters['receiveContractProposalFilter'] ?? '';
    if (filters['advancedFilterState']) {
      this.advancedFilterState = { ...filters['advancedFilterState'] };
    }
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    const statusParam = this.statusFilter || undefined;
    const stateParam = this.stateFilter || undefined;

    this.distributorService.getPaginated(
      this.pagination.currentPage,
      this.pagination.pageSize,
      this.sortBy || undefined,
      this.sortDirection,
      this.searchTerm || undefined,
      statusParam,
      this.receiveContractProposalFilter || undefined,
      stateParam
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
        this.pagination.totalPages = Math.ceil(this.pagination.totalItems / this.pagination.pageSize);
        this.updatePaginationIndices();
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load distributors. Please try again.';
        this.loading = false;
        console.error('Error loading distributors:', error);
      }
    });
  }

  deleteItem(id: number): void {
    this.distributorService.delete(id).subscribe({
      next: () => {
        this.loadData();
      },
      error: (error) => {
        this.error = 'Failed to delete distributor. Please try again.';
        console.error('Error deleting distributor:', error);
      }
    });
  }

  getItemId(item: Distributor): number {
    return item.id;
  }

  openDeleteModal(item: Distributor): void {
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
    const next = this.statusFilter === status ? '' : status;
    this.statusFilter = next;
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
    this.sortBy = '';
    this.sortDirection = 'asc';
    this.stateFilter = '';
    this.receiveContractProposalFilter = '';
    this.advancedFilterState['state'] = '';
    this.advancedFilterState['receiveContractProposal'] = '';
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  override getSortIcon(field: string): string {
    if (this.sortBy !== field) return 'fa-sort';
    return this.sortDirection === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }

  private compareValues(a: any, b: any, key: string): number {
    const av = (a?.[key] ?? '').toString().toLowerCase();
    const bv = (b?.[key] ?? '').toString().toLowerCase();
    if (key === 'id' || key === 'opCosCount') {
      return (a?.[key] ?? 0) - (b?.[key] ?? 0);
    }
    return av.localeCompare(bv);
  }

  // Ensure base reactive pipeline does not interfere with server-side paging
  override applyFiltersAndPagination(): void {
    this.filteredItems = [...this.items];
    this.paginatedItems = [...this.items];
    this.updatePaginationIndices();
    // Update selectAll state based on currently visible items
    const visibleIds = this.paginatedItems.map(i => this.getItemId(i));
    this.selectAll = visibleIds.length > 0 && visibleIds.every(id => this.selectedItems.has(id));
  }

  protected override matchesStatus(item: Distributor, status: string): boolean {
    switch (status) {
      case 'active':
        return DistributorHelper.isActive(item);
      case 'inactive':
        return !DistributorHelper.isActive(item);
      case 'proposals':
        return item.receiveContractProposal;
      default:
        return (item.status as any) === status;
    }
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

  // Helper methods for template
  getStatusColor(status: string): string {
    return DistributorHelper.getStatusColor(status as any);
  }

  formatAddress(distributor: Distributor): string {
    return DistributorHelper.formatAddress(distributor);
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
    } else if (key === 'state') {
      this.stateFilter = '';
      this.advancedFilterState['state'] = '';
    } else if (key === 'receiveContractProposal') {
      this.receiveContractProposalFilter = '';
      this.advancedFilterState['receiveContractProposal'] = '';
    }
    this.paginationService.removeFilter(key);
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  applyAdvancedFilters(): void {
    this.stateFilter = this.advancedFilterState['state'] || '';
    this.receiveContractProposalFilter = this.advancedFilterState['receiveContractProposal'] || '';
    if (this.stateFilter) {
      this.paginationService.setFilter('state', this.stateFilter);
    } else {
      this.paginationService.removeFilter('state');
    }
    if (this.receiveContractProposalFilter) {
      this.paginationService.setFilter('receiveContractProposal', this.receiveContractProposalFilter);
    } else {
      this.paginationService.removeFilter('receiveContractProposal');
    }
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.stateFilter = '';
    this.receiveContractProposalFilter = '';
    this.advancedFilterState['state'] = '';
    this.advancedFilterState['receiveContractProposal'] = '';
    this.paginationService.removeFilter('state');
    this.paginationService.removeFilter('receiveContractProposal');
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  // Expose utilities for template
  Math = Math;
  Object = Object;
}
