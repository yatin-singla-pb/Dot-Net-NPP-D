import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BaseListComponent, ListColumn } from '../../shared/components/base-list.component';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { ProposalService, Proposal } from '../../services/proposal.service';
import { AuthService } from '../../services/auth.service';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-proposals-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './proposals-list.component.html',
  styleUrls: ['./proposals-list.component.css']
})
export class ProposalsListComponent extends BaseListComponent<Proposal> implements OnInit {
  override serverDriven: boolean = true;
  override columns: ListColumn[] = [
    { key: 'id', label: 'ID', sortable: true, type: 'number' },
    { key: 'title', label: 'Title', sortable: true, type: 'text' },
    { key: 'proposalTypeId', label: 'Type', sortable: true, type: 'text' },
    { key: 'proposalStatusId', label: 'Status', sortable: true, type: 'text' }
  ];

  // Sorting properties
  sortBy: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // Lookup data
  proposalTypes: any[] = [];
  proposalStatuses: any[] = [];
  private readonly statusNameMap: Record<number, string> = {
    1: 'Requested',
    2: 'Pending',
    3: 'Saved',
    4: 'Submitted',
    5: 'Completed'
  };

  // Manufacturers lookup
  manufacturers: Array<{ id: number; name: string }> = [];

  // Expose Math for template
  Math = Math;

  // Filters
  selectedStatusId: number | null = null;
  advancedFilterState: { [key: string]: any } = {};
  selectedTypeId: number | null = null;
  selectedManufacturerId: number | null = null;
  // Date and range filters
  startDateFrom?: string;
  startDateTo?: string;
  endDateFrom?: string;
  endDateTo?: string;
  createdDateFrom?: string;
  createdDateTo?: string;
  idFrom?: number;
  idTo?: number;

  // Accordion state + cached histories
  expandedId: number | null = null;
  editHistoryCache: Record<number, Array<{ id: number; productId: number; productName?: string | null; changeType: string; previousJson?: string | null; currentJson?: string | null; changedDate: string; changedBy?: string | null }>> = {};

  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private service: ProposalService,
    private authService: AuthService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Proposals';
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.PROPOSALS;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      selectedStatusId: this.selectedStatusId,
      selectedTypeId: this.selectedTypeId,
      selectedManufacturerId: this.selectedManufacturerId,
      startDateFrom: this.startDateFrom,
      startDateTo: this.startDateTo,
      endDateFrom: this.endDateFrom,
      endDateTo: this.endDateTo,
      createdDateFrom: this.createdDateFrom,
      createdDateTo: this.createdDateTo,
      idFrom: this.idFrom,
      idTo: this.idTo,
      advancedFilterState: { ...this.advancedFilterState }
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.selectedStatusId = filters['selectedStatusId'] ?? null;
    this.selectedTypeId = filters['selectedTypeId'] ?? null;
    this.selectedManufacturerId = filters['selectedManufacturerId'] ?? null;
    this.startDateFrom = filters['startDateFrom'] ?? undefined;
    this.startDateTo = filters['startDateTo'] ?? undefined;
    this.endDateFrom = filters['endDateFrom'] ?? undefined;
    this.endDateTo = filters['endDateTo'] ?? undefined;
    this.createdDateFrom = filters['createdDateFrom'] ?? undefined;
    this.createdDateTo = filters['createdDateTo'] ?? undefined;
    this.idFrom = filters['idFrom'] ?? undefined;
    this.idTo = filters['idTo'] ?? undefined;
    if (filters['advancedFilterState']) {
      this.advancedFilterState = { ...filters['advancedFilterState'] };
    }
  }

  override ngOnInit(): void {
    super.ngOnInit();
    this.loadLookupData();
  }

  loadData(): void {
    this.loading = true;

    const isManufacturer = this.authService.hasRole('Manufacturer');
    const requestedPage = this.pagination.currentPage;
    const requestedPageSize = this.pagination.pageSize;

    // For Manufacturer users, fetch a larger slice once and paginate on the client
    const page = isManufacturer ? 1 : this.pagination.currentPage;
    const pageSize = isManufacturer ? 1000 : this.pagination.pageSize;

    this.service.getPaginated(
      page,
      pageSize,
      this.searchTerm || undefined,
      {
        proposalStatusId: this.selectedStatusId ?? undefined,
        proposalTypeId: this.selectedTypeId ?? undefined,
        manufacturerId: this.selectedManufacturerId ?? undefined,
        startDateFrom: this.startDateFrom || undefined,
        startDateTo: this.startDateTo || undefined,
        endDateFrom: this.endDateFrom || undefined,
        endDateTo: this.endDateTo || undefined,
        createdDateFrom: this.createdDateFrom || undefined,
        createdDateTo: this.createdDateTo || undefined,
        idFrom: this.idFrom ?? undefined,
        idTo: this.idTo ?? undefined,
        sortBy: this.sortBy || undefined,
        sortDirection: this.sortDirection || undefined,
      }
    ).subscribe({
      next: (resp) => {
        let list = resp.data || [];

        if (isManufacturer) {
          // Restrict strictly to proposals for assigned manufacturer(s); include all statuses
          const allowed = new Set((this.authService.manufacturerIds || []).map((x: any) => Number(x)));

          list = list.filter((p: any) => {
            const mId = p?.manufacturerId;
            return mId != null && allowed.has(Number(mId));
          });

          // Apply client-side pagination for manufacturer users
          this.items = list;
          this.filteredItems = [...this.items];
          const start = (requestedPage - 1) * requestedPageSize;
          const end = start + requestedPageSize;
          this.paginatedItems = this.items.slice(start, end);
          this.pagination.totalItems = this.items.length;
          this.pagination.totalPages = Math.ceil(this.pagination.totalItems / requestedPageSize);
          this.pagination.startIndex = start;
          this.pagination.endIndex = Math.min(end - 1, this.pagination.totalItems - 1);
        } else {
          // Admin/NPP: hide proposals still in Saved state (not visible until submitted)
          // Only hide when no explicit status filter is applied
          const saved = this.proposalStatuses.find(s => s.name === 'Saved');
          const savedId = saved?.id != null ? Number(saved.id) : 3; // fallback to 3 per seed
          if (this.selectedStatusId == null) {
            list = list.filter((p: any) => Number(p.proposalStatusId) !== savedId);
          }

          this.items = list;
          this.filteredItems = [...this.items];
          this.paginatedItems = [...this.items]; // server paginates for Admin/NPP
          this.pagination.totalItems = resp?.totalCount ?? this.items.length;
          this.pagination.totalPages = Math.ceil(this.pagination.totalItems / this.pagination.pageSize);
          this.pagination.startIndex = (this.pagination.currentPage - 1) * this.pagination.pageSize;
          this.pagination.endIndex = Math.min(this.pagination.startIndex + this.paginatedItems.length - 1, this.pagination.totalItems - 1);
        }

        this.loading = false;
      },
      error: (err) => {
        this.error = err?.message || 'Failed to load proposals';
        this.loading = false;
      }
    });
  }

  loadLookupData(): void {
    // Load proposal types
    this.service.getProposalTypes().subscribe({
      next: (types) => this.proposalTypes = types,
      error: (err) => console.error('Failed to load proposal types:', err)
    });

    // Load proposal statuses
    this.service.getProposalStatuses().subscribe({
      next: (statuses) => this.proposalStatuses = statuses,
      error: (err) => console.error('Failed to load proposal statuses:', err)
    });

    // Load manufacturers for advanced filters
    this.service.getManufacturers().subscribe({
      next: (mans) => this.manufacturers = mans || [],
      error: (err) => console.error('Failed to load manufacturers:', err)
    });
  }

  toggleExpand(item: Proposal): void {
    if (this.expandedId === item.id) {
      this.expandedId = null;
      return;
    }
    this.expandedId = item.id;
    if (!this.editHistoryCache[item.id]) {
      this.service.getProductEditHistory(item.id).subscribe({
        next: (hist) => this.editHistoryCache[item.id] = hist || [],
        error: () => this.editHistoryCache[item.id] = []
      });
    }
  }

  formatEditSummary(h: { previousJson?: string | null; currentJson?: string | null; changeType: string }): string {
    try {
      const prev = h.previousJson ? JSON.parse(h.previousJson) : {};
      const curr = h.currentJson ? JSON.parse(h.currentJson) : {};
      const parts: string[] = [];
      const add = (label: string, a: any, b: any) => { if (a !== b) parts.push(`${label}: ${a ?? '—'} → ${b ?? '—'}`); };
      add('UOM', prev.uom, curr.uom);
      add('FOB', prev.commercialFobPrice, curr.commercialFobPrice);
      add('DEL', prev.commercialDelPrice, curr.commercialDelPrice);
      add('Allowance', prev.allowance, curr.allowance);
      return parts.length ? parts.join('; ') : h.changeType;
    } catch { return h.changeType; }
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
    this.selectedStatusId = null;
    this.selectedTypeId = null;
    this.selectedManufacturerId = null;
    this.startDateFrom = undefined;
    this.startDateTo = undefined;
    this.endDateFrom = undefined;
    this.endDateTo = undefined;
    this.createdDateFrom = undefined;
    this.createdDateTo = undefined;
    this.idFrom = undefined;
    this.idTo = undefined;
    this.advancedFilterState = {};
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  // Satisfy BaseList API (not used by Proposals UI buttons)
  override onStatusFilterChange(status: string): void {
    this.statusFilter = status || '';
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  onStatusButtonClick(statusId: number | null): void {
    const next = this.selectedStatusId === statusId ? null : statusId;
    this.selectedStatusId = next;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  onStatusSelectChange(value: number | null): void {
    this.selectedStatusId = value;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }


  applyAdvancedFilters(): void {
    this.selectedTypeId = this.advancedFilterState['proposalTypeId'] ?? null;
    this.selectedManufacturerId = this.advancedFilterState['manufacturerId'] ?? null;
    this.startDateFrom = this.advancedFilterState['startDateFrom'] || undefined;
    this.startDateTo = this.advancedFilterState['startDateTo'] || undefined;
    this.endDateFrom = this.advancedFilterState['endDateFrom'] || undefined;
    this.endDateTo = this.advancedFilterState['endDateTo'] || undefined;
    this.createdDateFrom = this.advancedFilterState['createdDateFrom'] || undefined;
    this.createdDateTo = this.advancedFilterState['createdDateTo'] || undefined;
    this.idFrom = this.advancedFilterState['idFrom'] ? Number(this.advancedFilterState['idFrom']) : undefined;
    this.idTo = this.advancedFilterState['idTo'] ? Number(this.advancedFilterState['idTo']) : undefined;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.advancedFilterState = {};
    this.selectedTypeId = null;
    this.selectedManufacturerId = null;
    this.startDateFrom = undefined;
    this.startDateTo = undefined;
    this.endDateFrom = undefined;
    this.endDateTo = undefined;
    this.createdDateFrom = undefined;
    this.createdDateTo = undefined;
    this.idFrom = undefined;
    this.idTo = undefined;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  removeFilter(key: string): void {
    switch (key) {
      case 'status':
        this.selectedStatusId = null;
        this.pagination.currentPage = 1;
        this.saveCurrentState();
        this.loadData();
        return;
      case 'search':
        this.onSearchChange('');
        return;
      case 'proposalTypeId':
        this.selectedTypeId = null;
        this.advancedFilterState['proposalTypeId'] = null;
        break;
      case 'manufacturerId':
        this.selectedManufacturerId = null;
        this.advancedFilterState['manufacturerId'] = null;
        break;
      case 'startDateFrom':
        this.startDateFrom = undefined;
        this.advancedFilterState['startDateFrom'] = '';
        break;
      case 'startDateTo':
        this.startDateTo = undefined;
        this.advancedFilterState['startDateTo'] = '';
        break;
      case 'endDateFrom':
        this.endDateFrom = undefined;
        this.advancedFilterState['endDateFrom'] = '';
        break;
      case 'endDateTo':
        this.endDateTo = undefined;
        this.advancedFilterState['endDateTo'] = '';
        break;
      case 'createdDateFrom':
        this.createdDateFrom = undefined;
        this.advancedFilterState['createdDateFrom'] = '';
        break;
      case 'createdDateTo':
        this.createdDateTo = undefined;
        this.advancedFilterState['createdDateTo'] = '';
        break;
      case 'idFrom':
        this.idFrom = undefined;
        this.advancedFilterState['idFrom'] = '';
        break;
      case 'idTo':
        this.idTo = undefined;
        this.advancedFilterState['idTo'] = '';
        break;
    }
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  getManufacturerName(id: number | null | undefined): string {
    if (id == null) return '';
    const m = this.manufacturers.find(x => x.id === id);
    return m ? m.name : `Manufacturer ${id}`;
  }

  exportCsv(): void {
    this.excelExportService.exportToExcel(this.filteredItems, this.entityName);
  }

  getProposalTypeName(typeId: number): string {
    const type = this.proposalTypes.find(t => t.id === typeId);
    return type ? type.name : `Type ${typeId}`;
  }

  getProposalStatusName(statusId: number | null | undefined): string {
    if (statusId == null) {
      return '';
    }
    const numericId = Number(statusId);
    const status = this.proposalStatuses.find(s => Number(s.id) === numericId);
    if (status && status.name) {
      return status.name;
    }
    const seeded = this.statusNameMap[numericId];
    if (seeded) {
      return seeded;
    }
    return `Status ${numericId}`;
  }

  isCompleted(item: Proposal): boolean {
    // Check by status ID (5 = Completed) or by name
    if (item.proposalStatusId === 5) {
      return true;
    }
    const name = (item.proposalStatusName || this.getProposalStatusName(item.proposalStatusId) || '').toLowerCase();
    return name === 'completed';
  }

  canEdit(item: Proposal): boolean {
    // Contract Viewer cannot edit
    if (this.authService.isContractViewer()) return false;
    // Cannot edit completed proposals
    return !this.isCompleted(item);
  }

  canModify(): boolean {
    return this.authService.isAdminOrManager() || this.authService.isManufacturer();
  }

  deleteItem(id: any): void {
    // Proposals don't support delete operation - this is a no-op
    console.warn('Delete operation not supported for proposals');
  }

  getItemId(item: Proposal): any {
    return item.id;
  }
  getStatusButtonClasses(status: { id: number; name?: string }): { [klass: string]: boolean } {
    const active = this.selectedStatusId === status.id;
    return {
      'btn': true,
      'btn-sm': true,
      'status-filter-btn': true,
      // Saved / Submitted / Under Review / Rejected / Approved mapping by id
      'btn-primary': status.id === 1 && active,
      'btn-outline-primary': status.id === 1 && !active,
      'btn-warning': status.id === 2 && active,
      'btn-outline-warning': status.id === 2 && !active,
      'text-dark': (status.id === 2 || status.id === 3) && active,
      'btn-info': status.id === 3 && active,
      'btn-outline-info': status.id === 3 && !active,
      'btn-secondary': status.id === 4 && active,
      'btn-outline-secondary': status.id === 4 && !active,
      'btn-success': status.id === 5 && active,
      'btn-outline-success': status.id === 5 && !active,
    };
  }

}

