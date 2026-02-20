import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { BaseListComponent, ListColumn } from '../../shared/components/base-list.component';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { ContractService } from '../../services/contract.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { AuthService } from '../../services/auth.service';
import { IndustryService } from '../../services/industry.service';
import { Contract, ContractHelper } from '../../models/contract.model';
import { BulkRenewalDialogComponent } from '../../components/bulk-renewal-dialog/bulk-renewal-dialog.component';
import { BulkRenewalResponse, BulkRenewalRequest } from '../../models/bulk-renewal.model';
import { BulkRenewalService } from '../../services/bulk-renewal.service';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-contracts-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, MatDialogModule],
  templateUrl: './contracts-list.component.html',
  styleUrls: ['./contracts-list.component.css']

})
export class ContractsListComponent extends BaseListComponent<Contract> implements OnInit {

  override columns: ListColumn[] = [
    { key: 'id', label: 'ID', sortable: true, type: 'number' },
    { key: 'name', label: 'Name', sortable: true, type: 'text' },
    { key: 'manufacturer', label: 'Manufacturer', sortable: false, type: 'text' },
    { key: 'opCos', label: 'OpCos', sortable: false, type: 'text' },
    { key: 'proposal', label: 'Proposal', sortable: false, type: 'text' },

    { key: 'foreignContractId', label: 'Foreign ID', sortable: true, type: 'text' },
    { key: 'dateRange', label: 'Contract Period', sortable: true, type: 'text' },
    { key: 'expiry', label: 'Expiry Status', sortable: true, type: 'text' }
  ];

  // Sorting state
  sortBy: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // Delete modal state
  showDeleteModal = false;
  itemToDelete: Contract | null = null;

  // Bulk renewal modal
  showBulkRenewalModal = false;
  bulkRenewalCount = 0;

  // Advanced Filters
  manufacturers: { id: number; name: string }[] = [];
  industries: { id: number; name: string }[] = [];
  advancedFilterState: { [key: string]: any } = {
    manufacturerId: '',
    industryId: '',
    startDate: '',
    endDate: ''
  };
  manufacturerFilter: string = '';
  industryFilter: string = '';
  startDateFilter: string = '';
  endDateFilter: string = '';

  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private contractService: ContractService,
    private manufacturerService: ManufacturerService,
    private industryService: IndustryService,
    private authService: AuthService,
    private dialog: MatDialog,
    private bulkRenewalService: BulkRenewalService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Contracts';
    this.statusFilter = '';
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.CONTRACTS;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter,
      manufacturerFilter: this.manufacturerFilter,
      industryFilter: this.industryFilter,
      startDateFilter: this.startDateFilter,
      endDateFilter: this.endDateFilter,
      advancedFilterState: { ...this.advancedFilterState }
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.statusFilter = filters['statusFilter'] ?? '';
    this.manufacturerFilter = filters['manufacturerFilter'] ?? '';
    this.industryFilter = filters['industryFilter'] ?? '';
    this.startDateFilter = filters['startDateFilter'] ?? '';
    this.endDateFilter = filters['endDateFilter'] ?? '';
    if (filters['advancedFilterState']) {
      this.advancedFilterState = { ...filters['advancedFilterState'] };
    }
  }

  override ngOnInit(): void {
    super.ngOnInit();
    if (this.isManufacturerUser()) {
      // Ensure manufacturer filter is cleared/hidden for Manufacturer users
      this.manufacturerFilter = '';
      this.advancedFilterState['manufacturerId'] = '';
    }
    this.loadManufacturers();
    this.loadIndustries();
  }

  private loadManufacturers(): void {
    this.manufacturerService.getAllActive().subscribe({
      next: (list) => {
        this.manufacturers = (list || []).map(m => ({ id: m.id, name: m.name }));
      },
      error: () => {}
    });
  }

  private loadIndustries(): void {
    this.industryService.getActive().subscribe({
      next: (list) => {
        this.industries = (list || []).map(i => ({ id: (i as any).id, name: (i as any).name }));
      },
      error: () => {}
    });
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    const statusParam = this.statusFilter || undefined;
    const manufacturerIdParam = this.manufacturerFilter ? Number(this.manufacturerFilter) : undefined;
    const industryIdParam = this.industryFilter ? Number(this.industryFilter) : undefined;
    const startDateParam = this.startDateFilter || undefined;
    const endDateParam = this.endDateFilter || undefined;

    this.contractService.getPaginated(
      this.pagination.currentPage,
      this.pagination.pageSize,
      this.searchTerm || undefined,
      statusParam,
      manufacturerIdParam,
      industryIdParam,
      startDateParam,
      endDateParam,
      this.sortBy || undefined,
      this.sortDirection,
      false // show only non-suspended (active) contracts by default
    ).subscribe({
      next: (response) => {
        // Start with server results
        let list = response.items ?? [];
        // Manufacturer role: restrict to contracts for assigned manufacturer(s)
        if (this.isManufacturerUser()) {
          const allowed = new Set((this.authService.manufacturerIds || []).map((x: any) => Number(x)));
          list = list.filter((c: any) => c && c.manufacturerId != null && allowed.has(Number(c.manufacturerId)));
        }
        this.items = list;
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
        this.error = 'Failed to load contracts. Please try again.';
        this.loading = false;
        console.error('Error loading contracts:', error);
      }
    });
  }

  applyAdvancedFilters(): void {
    this.manufacturerFilter = this.advancedFilterState['manufacturerId'] || '';
    this.industryFilter = this.advancedFilterState['industryId'] || '';
    this.startDateFilter = this.advancedFilterState['startDate'] || '';
    this.endDateFilter = this.advancedFilterState['endDate'] || '';
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.manufacturerFilter = '';
    this.industryFilter = '';
    this.startDateFilter = '';
    this.endDateFilter = '';
    this.advancedFilterState['manufacturerId'] = '';
    this.advancedFilterState['industryId'] = '';
    this.advancedFilterState['startDate'] = '';
    this.advancedFilterState['endDate'] = '';
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  deleteItem(id: number): void {
    // Soft-delete by suspending the contract so it no longer appears in non-suspended listings
    this.contractService.suspend(id, 'Deleted from listing').subscribe({
      next: () => {
        this.loadData();
      },
      error: (error) => {
        this.error = 'Failed to delete (suspend) contract. Please try again.';
        console.error('Error suspending contract:', error);
      }
    });
  }

  getItemId(item: Contract): number {
    return item.id;
  }

  openDeleteModal(item: Contract): void {
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

  suspendContract(contract: Contract): void {
    const reason = prompt('Please enter a reason for suspending this contract:');
    if (reason !== null) {
      this.contractService.suspend(contract.id, reason).subscribe({
        next: () => {
          this.loadData();
        },
        error: (error) => {
          this.error = 'Failed to suspend contract. Please try again.';
          console.error('Error suspending contract:', error);
        }
      });
    }
  }

  activateContract(contract: Contract): void {
    const displayName = contract.name || `#${contract.id}`;
    if (confirm(`Are you sure you want to activate contract "${displayName}"?`)) {
      this.contractService.activate(contract.id).subscribe({
        next: () => {
          this.loadData();
        },
        error: (error) => {
          this.error = 'Failed to activate contract. Please try again.';
          console.error('Error activating contract:', error);
        }
      });
    }
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

  // Helper methods for template
  getStatusColor(status: string): string {
    return ContractHelper.getStatusColor(status as any);
  }

  formatDateRange(startDate: Date, endDate: Date): string {
    return ContractHelper.formatDateRange(startDate, endDate);
  }

  formatCurrency(amount?: number): string {
    return ContractHelper.formatCurrency(amount);
  }

  isExpired(contract: Contract): boolean {
    return ContractHelper.isExpired(contract);
  }

  isExpiringSoon(contract: Contract): boolean {
    return ContractHelper.isExpiringSoon(contract);
  }

  getDaysUntilExpiry(contract: Contract): number {
    return ContractHelper.getDaysUntilExpiry(contract);
  }

  canSuspend(contract: Contract): boolean {
    return ContractHelper.canSuspend(contract);
  }

  canActivate(contract: Contract): boolean {
    return ContractHelper.canActivate(contract);
  }

  protected override matchesStatus(item: Contract, status: string): boolean {
    if (!status) return true;
    const s = (status || '').toLowerCase();
    const itemStatus = (item.status || '').toString().toLowerCase();
    return itemStatus === s;
  }

  private compareValues(a: any, b: any, key: string): number {
    switch (key) {
      case 'id':
        return (a?.id ?? 0) - (b?.id ?? 0);
      case 'dateRange': {
        const as = new Date(a?.startDate ?? 0).getTime();
        const bs = new Date(b?.startDate ?? 0).getTime();
        return as - bs;
      }
      case 'expiry': {
        const ad = this.getDaysUntilExpiry(a);
        const bd = this.getDaysUntilExpiry(b);
        return ad - bd;
      }
      default: {
        const av = (a?.[key] ?? '').toString().toLowerCase();
        const bv = (b?.[key] ?? '').toString().toLowerCase();
        return av.localeCompare(bv);
      }
    }
  }

  // Override sort icon to use local sort state
  override getSortIcon(field: string): string {
    if (this.sortBy !== field) return 'fa-sort';
    return this.sortDirection === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }

  override clearAllFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.sortBy = '';
    this.sortDirection = 'asc';
    this.manufacturerFilter = '';
    this.industryFilter = '';
    this.startDateFilter = '';
    this.endDateFilter = '';
    this.advancedFilterState = { manufacturerId: '', industryId: '', startDate: '', endDate: '' };
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  private updatePaginationIndices(): void {
    this.pagination.startIndex = (this.pagination.currentPage - 1) * this.pagination.pageSize;
    this.pagination.endIndex = Math.min(
      this.pagination.startIndex + this.pagination.pageSize - 1,
      this.pagination.totalItems - 1
    );
  }

  override applyFiltersAndPagination(): void {
    this.filteredItems = [...this.items];
    this.paginatedItems = [...this.items];
    this.updatePaginationIndices();
    this.updateSelectAllState();
  }

  override onSelectAll(): void {
    this.selectAll = !this.selectAll;
    if (this.selectAll) {
      // Select all visible items
      this.paginatedItems.forEach(item => {
        this.selectedItems.add(this.getItemId(item));
      });
    } else {
      // Deselect all
      this.selectedItems.clear();
    }
  }

  toggleSelection(item: Contract): void {
    const id = this.getItemId(item);
    if (this.selectedItems.has(id)) {
      this.selectedItems.delete(id);
    } else {
      this.selectedItems.add(id);
    }
    this.updateSelectAllState();
  }

  // Expose utilities for template
  Math = Math;
  Object = Object;

  getManufacturerNameById(idStr: string): string {
    const id = Number(idStr);
    const m = this.manufacturers.find(x => x.id === id);
    return m?.name || idStr || '';
  }

  getManufacturerName(contract: Contract): string {
    // First try to use the manufacturerName if it's already populated
    if (contract.manufacturerName) {
      return contract.manufacturerName;
    }
    // Otherwise, look it up from the manufacturers array
    if (contract.manufacturerId != null) {
      const m = this.manufacturers.find(x => x.id === contract.manufacturerId);
      return m?.name || '';
    }
    return '';
  }

  getOpCoNames(contract: Contract): string {
    // Return comma-separated list of OpCo names
    if (contract.opCos && contract.opCos.length > 0) {
      return contract.opCos.map(oc => oc.opCoName || `OpCo ${oc.opCoId}`).join(', ');
    }
    return '-';
  }

  isManufacturerUser(): boolean {
    return this.authService.hasRole('Manufacturer');
  }

  isContractViewerUser(): boolean {
    return this.authService.isContractViewer();
  }

  canModify(): boolean {
    return this.authService.isAdminOrManager();
  }

  getIndustryNameById(idStr: string): string {
    const id = Number(idStr);
    const i = this.industries.find(x => x.id === id);
    return i?.name || idStr || '';
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
    }
    this.paginationService.removeFilter(key);
  }


  // Derive proposal id for older contracts that encoded it in manufacturerReferenceNumber like 'PROPOSAL-123'
  deriveProposalId(item: Contract): number | null {
    if (!item) return null;
    if (item.proposalId != null && !isNaN(Number(item.proposalId))) {
      return Number(item.proposalId);
    }
    const ref = (item.manufacturerReferenceNumber ?? '').toString();
    const m = ref.match(/proposal[-\s#:]*(\d+)/i);
    return (m && !isNaN(Number(m[1]))) ? Number(m[1]) : null;
  }

  // Bulk Renewal Methods

  /**
   * Check if user can use bulk renewal feature
   */
  canUseBulkRenewal(): boolean {
    return this.authService.hasRole('System Administrator') ||
           this.authService.hasRole('Contract Manager');
  }

  /**
   * Open bulk renewal confirmation modal for selected contracts
   */
  openBulkRenewalDialog(): void {
    if (this.selectedItems.size === 0) {
      alert('Please select at least one contract to create renewal proposals');
      return;
    }

    this.bulkRenewalCount = this.selectedItems.size;
    this.showBulkRenewalModal = true;
  }

  /**
   * Close bulk renewal modal
   */
  closeBulkRenewalModal(): void {
    this.showBulkRenewalModal = false;
    this.bulkRenewalCount = 0;
  }

  /**
   * Confirm bulk renewal
   */
  confirmBulkRenewal(): void {
    const selectedIds = Array.from(this.selectedItems);
    this.createBulkRenewalProposals(selectedIds);
    this.closeBulkRenewalModal();
  }

  /**
   * Create bulk renewal proposals without pricing adjustments
   */
  private createBulkRenewalProposals(contractIds: number[]): void {
    this.loading = true;
    this.error = null;

    const request: BulkRenewalRequest = {
      contractIds: contractIds,
      proposalDueDate: undefined, // No due date
      additionalProductIds: []
      // No pricing adjustment
    };

    this.bulkRenewalService.createRenewalProposals(request).subscribe({
      next: (result: BulkRenewalResponse) => {
        this.handleBulkRenewalResult(result);
        this.loading = false;
      },
      error: (error: any) => {
        this.error = error?.error?.message || 'Failed to create renewal proposals';
        this.loading = false;
        alert(this.error);
      }
    });
  }

  /**
   * Handle bulk renewal result
   */
  handleBulkRenewalResult(result: BulkRenewalResponse): void {
    // Clear selections
    this.selectedItems.clear();
    this.selectAll = false;

    // Show result message
    if (result.success) {
      alert(`✅ Successfully created ${result.successfulProposals} renewal proposal(s)!\n\nYou can view them in the Proposals section.`);
    } else {
      const message = `Created ${result.successfulProposals} of ${result.totalContracts} renewal proposals.\n\n` +
                     `${result.failedProposals} failed. Details:\n\n` +
                     result.results
                       .filter(r => !r.success)
                       .map(r => `• Contract ${r.contractNumber}: ${r.errorMessage}`)
                       .join('\n');
      alert(message);
    }

    // Optionally reload the contract list
    // this.loadData();
  }

}
