import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DistributorProductCodeService } from '../../services/distributor-product-code.service';
import { DistributorService } from '../../services/distributor.service';
import { ProductService } from '../../services/product.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { Distributor } from '../../models/distributor.model';
import { Product } from '../../models/product.model';
import { ListStateService } from '../../shared/services/list-state.service';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-distributor-product-codes-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './distributor-product-codes-list.component.html',
  styleUrls: ['./distributor-product-codes-list.component.css']
})
export class DistributorProductCodesListComponent implements OnInit, OnDestroy {
  items: any[] = [];
  loading = false;
  error: string | null = null;

  // Pagination (align with app-wide pattern)
  pagination = {
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0,
    startIndex: 0,
    endIndex: 0
  };
  sortBy = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // Filters
  searchTerm = '';
  statusFilter: string = '';
  selectedDistributorIds: number[] = [];
  selectedProductIds: number[] = [];

  distributors: Distributor[] = [];
  products: Product[] = [];

  // Advanced Filters modal state
  showAdvancedFilters = false;
  // Dropdown panels inside Advanced Filters modal
  showDistPanel = false;
  showProdPanel = false;
  advDistributorIds: number[] = [];
  advProductIds: number[] = [];
  distributorQuery = '';
  productQuery = '';
  filteredDistributors: Distributor[] = [];
  filteredProducts: Product[] = [];

  // Select-all state for dropdown panels
  distSelectAllChecked = false;
  prodSelectAllChecked = false;

  // Delete modal state
  showDeleteModal = false;
  pendingDeleteId: number | null = null;

  private readonly pageKey = LIST_PAGE_KEYS.DISTRIBUTOR_PRODUCT_CODES;

  constructor(
    private svc: DistributorProductCodeService,
    private distributorService: DistributorService,
    private productService: ProductService,
    private excelExportService: ExcelExportService,
    private listStateService: ListStateService
  ) {}


  ngOnInit(): void {
    this.restoreState();
    this.loadLookups();
    this.loadData();
  }

  ngOnDestroy(): void {
    this.saveCurrentState();
  }

  private saveCurrentState(): void {
    this.listStateService.saveState(this.pageKey, {
      filters: {
        searchTerm: this.searchTerm,
        statusFilter: this.statusFilter,
        selectedDistributorIds: [...this.selectedDistributorIds],
        selectedProductIds: [...this.selectedProductIds]
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
    this.sortBy = state.sortField || '';
    this.sortDirection = state.sortDirection || 'asc';
    if (state.filters) {
      this.searchTerm = state.filters['searchTerm'] ?? '';
      this.statusFilter = state.filters['statusFilter'] ?? '';
      this.selectedDistributorIds = state.filters['selectedDistributorIds'] ?? [];
      this.selectedProductIds = state.filters['selectedProductIds'] ?? [];
    }
  }

  loadLookups(): void {
    this.distributorService.getPaginated(1, 1000, 'name', 'asc').subscribe({
      next: (resp: any) => { this.distributors = resp.items ?? []; this.filteredDistributors = this.distributors; },
      error: () => { this.distributors = []; this.filteredDistributors = []; }
    });
    this.productService.getPaginated(1, 1000, 'description', 'asc').subscribe({
      next: (resp: any) => { this.products = resp.items ?? []; this.filteredProducts = this.products; },
      error: () => { this.products = []; this.filteredProducts = []; }
    });
  }

  loadData(): void {
    this.loading = true;
    this.error = null;
    const statusNum = this.statusFilter
      ? (this.statusFilter === 'Active' ? 1 : this.statusFilter === 'Inactive' ? 2 : undefined)
      : undefined;
    this.svc.getPaginated(
      this.pagination.currentPage,
      this.pagination.pageSize,
      this.sortBy || undefined,
      this.sortDirection,
      this.searchTerm || undefined,
      this.selectedDistributorIds,
      this.selectedProductIds,
      statusNum
    ).subscribe({
      next: (resp) => {
        this.items = resp.items ?? [];
        this.pagination.totalItems = resp.totalCount ?? 0;
        this.pagination.currentPage = resp.pageNumber ?? this.pagination.currentPage;
        this.pagination.pageSize = resp.pageSize ?? this.pagination.pageSize;
        this.pagination.totalPages = resp.totalPages ?? Math.ceil((this.pagination.totalItems || 0) / this.pagination.pageSize);
        this.pagination.startIndex = this.pagination.totalItems > 0 ? (this.pagination.currentPage - 1) * this.pagination.pageSize : 0;
        this.pagination.endIndex = Math.min(this.pagination.currentPage * this.pagination.pageSize - 1, Math.max(this.pagination.totalItems - 1, 0));
      },
      error: (err) => {
        this.error = err?.error?.message || 'Failed to load data';
      }
    }).add(() => this.loading = false);
  }

  onSearchChange(v: string): void { this.searchTerm = (v || '').trim(); this.pagination.currentPage = 1; this.saveCurrentState(); this.loadData(); }
  onPageChange(p: number): void { if (p < 1 || p > this.pagination.totalPages) return; this.pagination.currentPage = p; this.saveCurrentState(); this.loadData(); }
  onPageSizeChange(size: number): void { if (size < 1) return; this.pagination.pageSize = size; this.pagination.currentPage = 1; this.saveCurrentState(); this.loadData(); }
  getPageNumbers(): number[] {
    const total = this.pagination.totalPages;
    const current = this.pagination.currentPage;
    const window = 2;
    const pages: number[] = [];
    for (let p = 1; p <= total; p++) {
      if (p === 1 || p === total || (p >= current - window && p <= current + window)) {
        pages.push(p);
      } else if (pages[pages.length - 1] !== -1) {
        pages.push(-1);
      }
    }
    return pages;
  }
  onSort(field: string): void {
    if (this.sortBy === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = field;
      this.sortDirection = 'asc';
    }
    this.saveCurrentState();
    this.loadData();
  }
  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.selectedDistributorIds = [];
    this.selectedProductIds = [];
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  openDeleteModal(item: any): void { this.pendingDeleteId = item?.id ?? null; this.showDeleteModal = true; }
  closeDeleteModal(): void { this.showDeleteModal = false; this.pendingDeleteId = null; }
  confirmDelete(): void {
    if (!this.pendingDeleteId) return;
    this.svc.delete(this.pendingDeleteId).subscribe({
      next: () => { this.closeDeleteModal(); this.loadData(); },
      error: (err) => { this.error = err?.error?.message || 'Failed to delete'; this.closeDeleteModal(); }
    });
  }

  // Filter helpers
  clearAllFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.selectedDistributorIds = [];
    this.selectedProductIds = [];
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  openAdvancedFilters(): void {
    this.advDistributorIds = [...this.selectedDistributorIds];
    this.advProductIds = [...this.selectedProductIds];
    this.distributorQuery = '';
    this.productQuery = '';
    this.filteredDistributors = this.distributors;
    this.filteredProducts = this.products;
    this.updateDistSelectAllState();
    this.updateProdSelectAllState();
    this.showDistPanel = false;
    this.showProdPanel = false;
    this.showAdvancedFilters = true;
  }
  closeAdvancedFilters(): void {
    this.showAdvancedFilters = false;
    this.showDistPanel = false;
    this.showProdPanel = false;
  }

  onAdvancedToggle(type: 'distributor' | 'product', id: number, checked: boolean): void {
    const arr = type === 'distributor' ? this.advDistributorIds : this.advProductIds;
    if (checked) {
      if (!arr.includes(id)) arr.push(id);
    } else {
      const idx = arr.indexOf(id);
      if (idx >= 0) arr.splice(idx, 1);
    }
    if (type === 'distributor') this.updateDistSelectAllState(); else this.updateProdSelectAllState();
  }

  applyAdvancedFilters(): void {
    this.selectedDistributorIds = [...this.advDistributorIds];
    this.selectedProductIds = [...this.advProductIds];
    this.pagination.currentPage = 1;
    this.closeAdvancedFilters();
    this.saveCurrentState();
    this.loadData();
  }

  clearAdvancedOnly(): void {
    this.advDistributorIds = [];
    this.advProductIds = [];
    this.distSelectAllChecked = false;
    this.prodSelectAllChecked = false;
    this.showDistPanel = false;
    this.showProdPanel = false;
  }

  toggleDistPanel(): void {
    this.showDistPanel = !this.showDistPanel;
    if (this.showDistPanel) this.showProdPanel = false;
  }
  toggleProdPanel(): void {
    this.showProdPanel = !this.showProdPanel;
    if (this.showProdPanel) this.showDistPanel = false;
  }

  removeSelectedDistributor(id: number): void {
    const idx = this.selectedDistributorIds.indexOf(id);
    if (idx >= 0) { this.selectedDistributorIds.splice(idx, 1); this.pagination.currentPage = 1; this.saveCurrentState(); this.loadData(); }
  }
  removeSelectedProduct(id: number): void {
    const idx = this.selectedProductIds.indexOf(id);
    if (idx >= 0) { this.selectedProductIds.splice(idx, 1); this.pagination.currentPage = 1; this.saveCurrentState(); this.loadData(); }
  }

  applyDistributorFilter(term: string): void {
    this.distributorQuery = term;
    const t = (term || '').toLowerCase();
    this.filteredDistributors = !t ? this.distributors : this.distributors.filter(d => (d.name || '').toLowerCase().includes(t));
    this.updateDistSelectAllState();
  }
  applyProductFilter(term: string): void {
    this.productQuery = term;
    const t = (term || '').toLowerCase();
    this.filteredProducts = !t ? this.products : this.products.filter(p => (p.description || '').toLowerCase().includes(t));
    this.updateProdSelectAllState();
  }

  // Display helpers for collapsed selectors
  getAdvDistributorDisplay(max: number = 2): string {
    if (!this.advDistributorIds?.length) return 'Select distributors...';
    const names = this.advDistributorIds
      .map(id => this.distributors.find(d => d.id === id)?.name)
      .filter((n): n is string => !!n);
    if (names.length <= max) return names.join(', ');
    return `${names.slice(0, max).join(', ')} +${names.length - max} more`;
  }
  getAdvProductDisplay(max: number = 2): string {
    if (!this.advProductIds?.length) return 'Select products...';
    const names = this.advProductIds
      .map(id => this.products.find(p => p.id === id)?.description)
      .filter((n): n is string => !!n);
    if (names.length <= max) return names.join(', ');
    return `${names.slice(0, max).join(', ')} +${names.length - max} more`;
  }
  // Select All handlers for panels
  toggleAllDistributors(checked: boolean): void {
    const ids = (this.filteredDistributors || []).map(d => d.id);
    if (checked) {
      ids.forEach(id => { if (!this.advDistributorIds.includes(id)) this.advDistributorIds.push(id); });
    } else {
      this.advDistributorIds = this.advDistributorIds.filter(id => !ids.includes(id));
    }
    this.updateDistSelectAllState();
  }
  toggleAllProducts(checked: boolean): void {
    const ids = (this.filteredProducts || []).map(p => p.id);
    if (checked) {
      ids.forEach(id => { if (!this.advProductIds.includes(id)) this.advProductIds.push(id); });
    } else {
      this.advProductIds = this.advProductIds.filter(id => !ids.includes(id));
    }
    this.updateProdSelectAllState();
  }

  private updateDistSelectAllState(): void {
    const filtered = this.filteredDistributors || [];
    this.distSelectAllChecked = filtered.length > 0 && filtered.every(d => this.advDistributorIds.includes(d.id));
  }
  private updateProdSelectAllState(): void {
    const filtered = this.filteredProducts || [];
    this.prodSelectAllChecked = filtered.length > 0 && filtered.every(p => this.advProductIds.includes(p.id));
  }


  onStatusFilterChange(status: string): void {
    const next = this.statusFilter === status ? '' : status;
    this.statusFilter = next;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  toggleAdvancedFilters(): void {
    if (!this.showAdvancedFilters) this.openAdvancedFilters();
    else this.closeAdvancedFilters();
  }

  clearAdvancedFilters(): void {
    this.selectedDistributorIds = [];
    this.selectedProductIds = [];
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  getDistributorName(id: any): string {
    const num = Number(id);
    const found = this.distributors.find(d => d.id === num);
    return found?.name ?? `${id}`;
  }

  getProductName(id: any): string {
    const num = Number(id);
    const found = this.products.find(p => p.id === num);
    return found?.description ?? `${id}`;
  }

  exportToExcel(): void {
    if (this.loading || !this.items.length) return;

    const exportData = this.items.map(item => ({
      'ID': item.id,
      'Distributor': item.distributorName || item.distributorId || '-',
      'Dist. Code': item.distributorCode || '-',
      'Mfr Code': item.manufacturerProductCode || '-',
      'Description': item.productDescription || item.productName || '-',
      'Brand': item.brand || '-',
      'Manufacturer': item.manufacturerName || '-',
      'Catch Weight': item.catchWeight ? 'Yes' : 'No',
      'E-Brand': item.eBrand ? 'Yes' : 'No'
    }));

    this.excelExportService.exportToExcel(exportData, 'Distributor_Product_Codes');
  }

  // Expose for template
  Math = Math;
}
