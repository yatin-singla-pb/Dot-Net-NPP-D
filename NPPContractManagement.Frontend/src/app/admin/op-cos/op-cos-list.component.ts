import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BaseListComponent, ListColumn } from '../../shared/components/base-list.component';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { OpCoService } from '../../services/opco.service';
import { OpCo, OpCoHelper, OpCoStatus } from '../../models/opco.model';
import { Distributor } from '../../models/distributor.model';
import { DistributorService } from '../../services/distributor.service';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-op-cos-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './op-cos-list.component.html',
  styleUrls: ['./op-cos-list.component.css']
})
export class OpCosListComponent extends BaseListComponent<OpCo> implements OnInit {

  override serverDriven: boolean = true;
  override statusFilter: string = '';
  override columns: ListColumn[] = [
    { key: 'id', label: 'ID', sortable: true, type: 'number' },
    { key: 'name', label: 'Name', sortable: true, type: 'text' },
    { key: 'remoteReferenceCode', label: 'Remote Reference Code', sortable: true, type: 'text' },
    { key: 'distributorName', label: 'Distributor', sortable: true, type: 'text' },
    { key: 'state', label: 'State', sortable: true, type: 'text' },
    { key: 'status', label: 'Status', sortable: true, type: 'status' }
  ];

  sortBy: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  distributors: Distributor[] = [];
  advancedFilterState: { [key: string]: any } = { distributorId: '', remoteReferenceCode: '' };
  distributorFilter: string | number | '' = '';
  remoteReferenceCodeFilter: string = '';
  // Expose enum for template
  OpCoStatus = OpCoStatus;

  // Delete modal state
  showDeleteModal = false;
  itemToDelete: OpCo | null = null;


  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private opCoService: OpCoService,
    private distributorService: DistributorService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Op-Cos';
  }

  override ngOnInit(): void {
    super.ngOnInit();
    // Load distributors for filter dropdown using paginated API
    this.distributorService.getPaginated(1, 1000, 'name', 'asc')
      .subscribe({
        next: (resp: any) => this.distributors = resp?.items ?? [],
        error: () => this.distributors = []
      });
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.OP_COS;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter,
      distributorFilter: this.distributorFilter,
      remoteReferenceCodeFilter: this.remoteReferenceCodeFilter,
      advancedFilterState: { ...this.advancedFilterState }
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.statusFilter = filters['statusFilter'] ?? '';
    this.distributorFilter = filters['distributorFilter'] ?? '';
    this.remoteReferenceCodeFilter = filters['remoteReferenceCodeFilter'] ?? '';
    if (filters['advancedFilterState']) {
      this.advancedFilterState = { ...filters['advancedFilterState'] };
    }
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    const statusParam = this.statusFilter || undefined;

    this.opCoService.getPaginated(
      this.pagination.currentPage,
      this.pagination.pageSize,
      this.searchTerm || undefined,
      statusParam,
      this.advancedFilterState['distributorId'] ? Number(this.advancedFilterState['distributorId']) : undefined,
      this.sortBy || undefined,
      this.sortDirection,
      this.advancedFilterState['remoteReferenceCode'] || undefined
    ).subscribe({
      next: (response: any) => {
        // Filter to only show active records
        this.items = (response.items ?? []).filter((item: any) => item.isActive);
        this.filteredItems = [...this.items];
        this.paginatedItems = [...this.items];
        if (this.sortBy) {
          this.paginatedItems.sort((a: any, b: any) => this.compareValues(a, b, this.sortBy));
          if (this.sortDirection === 'desc') this.paginatedItems.reverse();
        }
        this.selectAll = false;
        this.selectedItems.clear();
        this.pagination.totalItems = this.items.length;
        this.pagination.totalPages = Math.ceil(this.pagination.totalItems / this.pagination.pageSize);
        this.updatePaginationIndices();
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load Op-Cos. Please try again.';
        this.loading = false;
        console.error('Error loading Op-Cos:', error);
      }
    });
  }

  deleteItem(id: number): void {
    this.opCoService.delete(id).subscribe({
      next: () => {
        this.loadData();
      },
      error: (error) => {
        this.error = 'Failed to delete Op-Co. Please try again.';
        console.error('Error deleting Op-Co:', error);
      }
    });
  }

  getItemId(item: OpCo): number {
    return item.id;
  }

  openDeleteModal(item: OpCo): void {
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

  protected override matchesStatus(item: OpCo, status: string): boolean {
    switch (status) {
      case 'active':
        return OpCoHelper.isActive(item);
      case 'inactive':
        return !OpCoHelper.isActive(item) && item.status !== 'Pending';
      case 'pending':
        return item.status === 'Pending';
      default:
        return item.status === status;
    }
  }

  // Helper methods for template
  getStatusColor(status: string): string {
    return OpCoHelper.getStatusColor(status as any);
  }

  formatAddress(opCo: OpCo): string {
    return OpCoHelper.formatAddress(opCo);
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
          case 'distributorId':
            const d = this.distributors.find(x => x.id === Number(value));
            label = `Distributor: ${d?.name ?? value}`;
            break;
          case 'remoteReferenceCode':
            label = `Remote Ref: ${value}`;
            break;
          default:
            label = `${key}: ${value}`;
        }
        pills.push({ key, label });
      }
    });

    return pills;
  }

  // Server-driven interactions
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
  override clearAllFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.sortBy = '';
    this.sortDirection = 'asc';
    this.advancedFilterState['distributorId'] = '';
    this.advancedFilterState['remoteReferenceCode'] = '';
    this.distributorFilter = '';
    this.remoteReferenceCodeFilter = '';
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
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
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else if (currentPage <= 4) {
      for (let i = 1; i <= 5; i++) pages.push(i);
      pages.push(-1);
      pages.push(totalPages);
    } else if (currentPage >= totalPages - 3) {
      pages.push(1);
      pages.push(-1);
      for (let i = totalPages - 4; i <= totalPages; i++) pages.push(i);
    } else {
      pages.push(1);
      pages.push(-1);
      for (let i = currentPage - 1; i <= currentPage + 1; i++) pages.push(i);
      pages.push(-1);
      pages.push(totalPages);
    }
    return pages;
  }

  removeFilter(key: string): void {
    if (key === 'search') {
      this.searchTerm = '';
    } else if (key === 'status') {
      this.statusFilter = '';
    } else if (key === 'distributorId') {
      this.advancedFilterState['distributorId'] = '';
      this.distributorFilter = '';
    } else if (key === 'remoteReferenceCode') {
      this.advancedFilterState['remoteReferenceCode'] = '';
      this.remoteReferenceCodeFilter = '';
    }
    this.paginationService.removeFilter(key);
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  applyAdvancedFilters(): void {
    const dist = this.advancedFilterState['distributorId'];
    if (dist) {
      this.paginationService.setFilter('distributorId', dist);
    } else {
      this.paginationService.removeFilter('distributorId');
    }

    const rrc = this.advancedFilterState['remoteReferenceCode'];
    if (rrc) {
      this.paginationService.setFilter('remoteReferenceCode', rrc);
    } else {
      this.paginationService.removeFilter('remoteReferenceCode');
    }

    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.advancedFilterState['distributorId'] = '';
    this.advancedFilterState['remoteReferenceCode'] = '';
    this.distributorFilter = '';
    this.remoteReferenceCodeFilter = '';
    this.paginationService.removeFilter('distributorId');
    this.paginationService.removeFilter('remoteReferenceCode');
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  getDistributorNameById(id: any): string {
    const d = this.distributors.find(x => x.id === Number(id));
    return d?.name ?? String(id ?? '');
  }

  // Expose utilities for template
  Math = Math;
  Object = Object;
}
