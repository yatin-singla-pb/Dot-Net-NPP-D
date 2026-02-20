import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BaseListComponent, ListColumn, FilterOption } from '../../shared/components/base-list.component';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { UserService } from '../../services/user.service';
import { Manufacturer, ManufacturerHelper, ManufacturerStatus } from '../../models/manufacturer.model';
import { US_STATES } from '../../shared/utils/us-states.constants';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-manufacturers-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './manufacturers-list.component.html',
  styleUrls: ['./manufacturers-list.component.css']
})
export class ManufacturersListComponent extends BaseListComponent<Manufacturer> implements OnInit {

  override statusFilter: string = '';
  override columns: ListColumn[] = [
    { key: 'id', label: 'ID', sortable: true, type: 'number' },
    { key: 'name', label: 'Manufacturer Name', sortable: true, type: 'text' },
    { key: 'address', label: 'Address', sortable: true, type: 'text' },
    { key: 'phoneNumber', label: 'Phone', sortable: true, type: 'text' },
    { key: 'status', label: 'Status', sortable: true, type: 'status' }
  ];

  sortBy: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // Advanced filter state
  override filterOptions: FilterOption[] = [
    { key: 'state', label: 'State', type: 'select' },
    { key: 'primaryBrokerId', label: 'Primary Broker', type: 'select' }
  ];
  advancedFilterState: { [key: string]: any } = { state: '', primaryBrokerId: '' };
  usStates = US_STATES;
  eligibleBrokers: any[] = [];

  // Expose enum for template comparisons
  ManufacturerStatus = ManufacturerStatus;

  // Delete modal state
  showDeleteModal = false;
  itemToDelete: Manufacturer | null = null;

  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private manufacturerService: ManufacturerService,
    private userService: UserService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Manufacturers';
  }

  override ngOnInit(): void {
    super.ngOnInit();
    this.loadEligibleBrokers();
  }

  private loadEligibleBrokers(): void {
    this.userService.getEligibleBrokers().subscribe({
      next: (users) => {
        this.eligibleBrokers = users || [];
      },
      error: (err) => {
        console.error('Error loading eligible brokers', err);
      }
    });
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.MANUFACTURERS;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter,
      advancedFilterState: { ...this.advancedFilterState }
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.statusFilter = filters['statusFilter'] ?? '';
    if (filters['advancedFilterState']) {
      this.advancedFilterState = { ...filters['advancedFilterState'] };
    }
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    const statusParam = this.statusFilter || undefined;
    const stateParam = this.advancedFilterState['state'] || undefined;
    const primaryBrokerIdParam = this.advancedFilterState['primaryBrokerId'] || undefined;

    this.manufacturerService.getPaginated(
      this.pagination.currentPage,
      this.pagination.pageSize,
      this.sortBy || undefined,
      this.sortDirection,
      this.searchTerm || undefined,
      statusParam,
      stateParam,
      primaryBrokerIdParam
    ).subscribe({
      next: (response) => {
        this.items = response.items ?? [];
        this.filteredItems = [...this.items];
        this.paginatedItems = [...this.items];
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
        this.error = 'Failed to load manufacturers. Please try again.';
        this.loading = false;
        console.error('Error loading manufacturers:', error);
      }
    });
  }

  deleteItem(id: number): void {
    this.manufacturerService.delete(id).subscribe({
      next: () => {
        this.loadData();
      },
      error: (error) => {
        this.error = 'Failed to delete manufacturer. Please try again.';
        console.error('Error deleting manufacturer:', error);
      }
    });
  }

  getItemId(item: Manufacturer): number {
    return item.id;
  }

  openDeleteModal(item: Manufacturer): void {
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
    this.advancedFilterState = { state: '', primaryBrokerId: '' };
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  applyAdvancedFilters(): void {
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.advancedFilterState = { state: '', primaryBrokerId: '' };
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  override getSortIcon(field: string): string {
    if (this.sortBy !== field) return 'fa-sort';
    return this.sortDirection === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }

  private compareValues(a: any, b: any, key: string): number {
    const av = (a?.[key] ?? '').toString().toLowerCase();
    const bv = (b?.[key] ?? '').toString().toLowerCase();
    if (key === 'id') {
      return (a?.[key] ?? 0) - (b?.[key] ?? 0);
    }
    return av.localeCompare(bv);
  }

  // Ensure base reactive pipeline does not interfere with server-side paging
  override applyFiltersAndPagination(): void {
    this.filteredItems = [...this.items];
    this.paginatedItems = [...this.items];
    this.updatePaginationIndices();
    const visibleIds = this.paginatedItems.map(i => this.getItemId(i));
    this.selectAll = visibleIds.length > 0 && visibleIds.every(id => this.selectedItems.has(id));
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

  private updatePaginationIndices(): void {
    this.pagination.startIndex = (this.pagination.currentPage - 1) * this.pagination.pageSize;
    this.pagination.endIndex = Math.min(
      this.pagination.startIndex + this.pagination.pageSize - 1,
      this.pagination.totalItems - 1
    );
  }

  // Helper methods for template
  getStatusColor(status: string): string {
    return ManufacturerHelper.getStatusColor(status as any);
  }

  formatAddress(manufacturer: Manufacturer): string {
    return ManufacturerHelper.formatAddress(manufacturer);
  }

  Math = Math;
  Object = Object;

  getBrokerNameById(id: number | string): string {
    const nid = typeof id === 'string' ? parseInt(id, 10) : id;
    const broker = this.eligibleBrokers.find(b => b.id === nid);
    return broker ? broker.name : (id?.toString() || '');
  }

  getPrimaryBrokerName(manufacturer: Manufacturer): string {
    if (!manufacturer.primaryBrokerId) {
      return '';
    }
    return this.getBrokerNameById(manufacturer.primaryBrokerId);
  }
}
