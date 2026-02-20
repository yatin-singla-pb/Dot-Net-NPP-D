import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BaseListComponent, ListColumn, FilterOption } from '../../shared/components/base-list.component';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { IndustryService } from '../../services/industry.service';
import { Industry, IndustryStatus } from '../../models/industry.model';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-industries-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './industries-list.component.html',
  styleUrls: ['./industries-list.component.css']
})
export class IndustriesListComponent extends BaseListComponent<Industry> implements OnInit {

  override columns: ListColumn[] = [
    { key: 'id', label: 'ID', sortable: true, type: 'number' },
    { key: 'name', label: 'Industry Name', sortable: true, type: 'text' },
    { key: 'status', label: 'Status', sortable: true, type: 'status' }
  ];

  // Add properties for search and filtering
  override searchTerm = '';
  override statusFilter = '';
  sortBy = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // Make IndustryStatus available in template
  IndustryStatus = IndustryStatus;

  // Properties for template
  override filteredItems: Industry[] = [];
  override paginatedItems: Industry[] = [];
  override selectAll = false;
  override selectedItems = new Set<number>();
  override pagination = {
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0,
    startIndex: 0,
    endIndex: 0
  };

  // Delete modal state
  showDeleteModal = false;
  itemToDelete: Industry | null = null;

  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private industryService: IndustryService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Industries';
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.INDUSTRIES;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.statusFilter = filters['statusFilter'] ?? '';
  }

  override ngOnInit(): void {
    super.ngOnInit();
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    // Use the proper paginated API call with all filters and sorting
    this.industryService.getPaginated(
      this.pagination.currentPage,
      this.pagination.pageSize,
      this.sortBy || undefined,
      this.sortDirection,
      this.searchTerm || undefined,
      this.statusFilter || undefined
    ).subscribe({
      next: (response) => {
        this.items = response.items;
        this.filteredItems = [...response.items];
        this.paginatedItems = [...response.items]; // ensure table binds to current page
        this.selectAll = false;
        this.selectedItems.clear();
        this.pagination.totalItems = response.totalCount;
        this.pagination.totalPages = Math.ceil(response.totalCount / this.pagination.pageSize);
        this.updatePaginationIndices();
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load industries. Please try again.';
        console.error('Error loading industries:', error);
        this.loading = false;
      }
    });
  }

  deleteItem(id: number): void {
    this.industryService.delete(id).subscribe({
      next: () => {
        this.loadData(); // Reload data after deletion
      },
      error: (error) => {
        this.error = 'Failed to delete industry. Please try again.';
        console.error('Error deleting industry:', error);
      }
    });
  }

  getItemId(item: Industry): number {
    return item.id;
  }

  openDeleteModal(item: Industry): void {
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

  // Search functionality
  override onSearchChange(value: string): void {
    this.searchTerm = value;
    this.pagination.currentPage = 1; // Reset to first page
    this.saveCurrentState();
    this.loadData(); // Reload data with new search term
  }

  // Status filter functionality
  override onStatusFilterChange(status: string): void {
    const next = this.statusFilter === status ? '' : status;
    this.statusFilter = next;
    this.pagination.currentPage = 1; // Reset to first page
    this.saveCurrentState();
    this.loadData(); // Reload data with new filter
  }

  // Clear all filters
  override clearAllFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.sortBy = '';
    this.sortDirection = 'asc';
    this.pagination.currentPage = 1; // Reset to first page
    this.listStateService.clearState(this.pageKey);
    this.loadData(); // Reload data without filters
  }

  // Sorting functionality
  override onSort(column: string): void {
    if (this.sortBy === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortDirection = 'asc';
    }
    this.saveCurrentState();
    this.loadData(); // Reload data with new sorting
  }

  // Apply filters, search, and pagination
  override applyFiltersAndPagination(): void {
    let filteredItems = [...this.items];

    // Apply search filter
    if (this.searchTerm) {
      const searchLower = this.searchTerm.toLowerCase();
      filteredItems = filteredItems.filter(item =>
        item.name.toLowerCase().includes(searchLower) ||
        item.id.toString().includes(searchLower)
      );
    }

    // Apply status filter
    if (this.statusFilter) {
      filteredItems = filteredItems.filter(item =>
        item.status.toLowerCase() === this.statusFilter.toLowerCase()
      );
    }

    // Apply sorting
    if (this.sortBy) {
      filteredItems.sort((a, b) => {
        let aValue: any = a[this.sortBy as keyof Industry];
        let bValue: any = b[this.sortBy as keyof Industry];

        // Handle different data types
        if (typeof aValue === 'string') {
          aValue = aValue.toLowerCase();
          bValue = bValue.toLowerCase();
        }

        if (aValue < bValue) {
          return this.sortDirection === 'asc' ? -1 : 1;
        }
        if (aValue > bValue) {
          return this.sortDirection === 'asc' ? 1 : -1;
        }
        return 0;
      });
    }

    // Update filtered items and pagination
    this.filteredItems = filteredItems;
    this.updatePagination();
  }

  // Update pagination based on filtered items
  private updatePagination(): void {
    const totalItems = this.filteredItems.length;
    const totalPages = Math.ceil(totalItems / this.pagination.pageSize);

    this.pagination = {
      ...this.pagination,
      totalItems,
      totalPages,
      currentPage: Math.min(this.pagination.currentPage, totalPages || 1)
    };

    const startIndex = (this.pagination.currentPage - 1) * this.pagination.pageSize;
    const endIndex = startIndex + this.pagination.pageSize;

    this.paginatedItems = this.filteredItems.slice(startIndex, endIndex);

    this.pagination.startIndex = startIndex;
    this.pagination.endIndex = Math.min(endIndex - 1, totalItems - 1);
  }

  // Selection methods
  override onSelectAll(): void {
    this.selectAll = !this.selectAll;
    if (this.selectAll) {
      this.paginatedItems.forEach(item => this.selectedItems.add(item.id));
    } else {
      this.selectedItems.clear();
    }
  }

  override onSelectItem(item: Industry): void {
    if (this.selectedItems.has(item.id)) {
      this.selectedItems.delete(item.id);
    } else {
      this.selectedItems.add(item.id);
    }
    this.selectAll = this.selectedItems.size === this.paginatedItems.length;
  }

  override isSelected(item: Industry): boolean {
    return this.selectedItems.has(item.id);
  }

  // Pagination methods
  override onPageChange(page: number): void {
    this.pagination.currentPage = page;
    this.saveCurrentState();
    this.loadData(); // Reload data with new page
  }

  override onPageSizeChange(pageSize: number): void {
    this.pagination.pageSize = pageSize;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData(); // Reload data with new page size
  }

  // Helper method to update pagination indices
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
        pages.push(-1); // ellipsis
        pages.push(totalPages);
      } else if (currentPage >= totalPages - 3) {
        pages.push(1);
        pages.push(-1); // ellipsis
        for (let i = totalPages - 4; i <= totalPages; i++) {
          pages.push(i);
        }
      } else {
        pages.push(1);
        pages.push(-1); // ellipsis
        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
          pages.push(i);
        }
        pages.push(-1); // ellipsis
        pages.push(totalPages);
      }
    }

    return pages;
  }

  // Delete selected items
  override deleteSelected(): void {
    if (this.selectedItems.size === 0) return;

    const selectedIds = Array.from(this.selectedItems);
    if (confirm(`Are you sure you want to delete ${selectedIds.length} selected industries?`)) {
      // For now, delete one by one (could be optimized with bulk delete API)
      selectedIds.forEach(id => {
        this.industryService.delete(id).subscribe({
          next: () => {
            this.selectedItems.delete(id);
            if (this.selectedItems.size === 0) {
              this.loadData(); // Reload data when all deletions are complete
            }
          },
          error: (error) => {
            console.error('Error deleting industry:', error);
          }
        });
      });
    }
  }

  // Export methods
  override exportToExcel(): void {
    if (this.loading || !this.paginatedItems.length) return;

    const exportData = this.paginatedItems.map(industry => ({
      'ID': industry.id,
      'Industry Name': industry.name,
      'Description': industry.description || '-',
      'Status': industry.status,
      'Is Active': industry.isActive ? 'Yes' : 'No',
      'Created Date': industry.createdDate ? new Date(industry.createdDate).toLocaleDateString() : '-',
      'Modified Date': industry.modifiedDate ? new Date(industry.modifiedDate).toLocaleDateString() : '-',
      'Created By': industry.createdBy || '-',
      'Modified By': industry.modifiedBy || '-'
    }));

    this.excelExportService.exportToExcel(exportData, 'Industries');
  }

  // Expose Math for template
  Math = Math;
  Object = Object;
}
