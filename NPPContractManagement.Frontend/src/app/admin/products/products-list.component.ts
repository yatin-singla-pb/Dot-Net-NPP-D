import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BaseListComponent, ListColumn } from '../../shared/components/base-list.component';
import { PaginationService } from '../../shared/services/pagination.service';
import { ExcelExportService } from '../../shared/services/excel-export.service';
import { ProductService } from '../../services/product.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { Product, ProductHelper } from '../../models/product.model';
import { Manufacturer } from '../../models/manufacturer.model';
import { LIST_PAGE_KEYS } from '../../shared/constants/list-page-keys';

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.css']
})
export class ProductsListComponent extends BaseListComponent<Product> implements OnInit {
  override statusFilter: string = '';
  sortBy: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  manufacturers: Manufacturer[] = [];
  selectedManufacturerId: number | null = null;
  brands: string[] = [];
  selectedBrand: string | null = null;
  brandFilter: string = '';
  filteredBrands: string[] = [];

  override columns: ListColumn[] = [
    { key: 'id', label: 'ID', sortable: true, type: 'number' },
    { key: 'name', label: 'Name', sortable: true, type: 'text' },
    { key: 'manufacturerProductCode', label: 'Product Code', sortable: true, type: 'text' },
    { key: 'brand', label: 'Brand', sortable: true, type: 'text' },
    { key: 'manufacturerName', label: 'Manufacturer', sortable: true, type: 'text' },
    { key: 'packSize', label: 'Pack Size', sortable: false, type: 'text' },
    { key: 'status', label: 'Status', sortable: true, type: 'status' }
  ];

  // Delete modal state
  showDeleteModal = false;
  itemToDelete: Product | null = null;

  constructor(
    protected override paginationService: PaginationService,
    protected override excelExportService: ExcelExportService,
    private productService: ProductService,
    private manufacturerService: ManufacturerService
  ) {
    super(paginationService, excelExportService);
    this.entityName = 'Products';
  }

  override get pageKey(): string {
    return LIST_PAGE_KEYS.PRODUCTS;
  }

  protected override getFilterState(): { [key: string]: any } {
    return {
      searchTerm: this.searchTerm,
      statusFilter: this.statusFilter,
      selectedManufacturerId: this.selectedManufacturerId,
      selectedBrand: this.selectedBrand
    };
  }

  protected override restoreFilterState(filters: { [key: string]: any }): void {
    this.searchTerm = filters['searchTerm'] ?? '';
    this.statusFilter = filters['statusFilter'] ?? '';
    this.selectedManufacturerId = filters['selectedManufacturerId'] ?? null;
    this.selectedBrand = filters['selectedBrand'] ?? null;
  }

  override ngOnInit(): void {
    super.ngOnInit();
    this.loadManufacturers();
    this.loadBrands();
  }

  private loadManufacturers(): void {
    this.manufacturerService.getAllActive().subscribe({
      next: (items) => { this.manufacturers = items || []; },
      error: () => {}
    });
  }

  private loadBrands(): void {
    this.productService.getBrands().subscribe({
      next: (items) => {
        this.brands = items || [];
        this.filteredBrands = [...this.brands];
      },
      error: () => {}
    });
  }

  applyBrandFilter(term: string): void {
    this.brandFilter = term || '';
    const t = this.brandFilter.toLowerCase();
    this.filteredBrands = this.brands.filter(b => b.toLowerCase().includes(t));
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    const statusParam = this.statusFilter || undefined;

    this.productService.getPaginated(
      this.pagination.currentPage,
      this.pagination.pageSize,
      this.sortBy || undefined,
      this.sortDirection,
      this.searchTerm || undefined,
      statusParam,
      this.selectedManufacturerId || undefined,
      this.selectedBrand || undefined
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
        this.error = 'Failed to load products. Please try again.';
        this.loading = false;
        console.error('Error loading products:', error);
      }
    });
  }

  deleteItem(id: number): void {
    this.productService.delete(id).subscribe({
      next: () => {
        this.loadData();
      },
      error: (error) => {
        this.error = 'Failed to delete product. Please try again.';
        console.error('Error deleting product:', error);
      }
    });
  }

  getItemId(item: Product): number { return item.id; }

  openDeleteModal(item: Product): void { this.itemToDelete = item; this.showDeleteModal = true; }
  closeDeleteModal(): void { this.showDeleteModal = false; this.itemToDelete = null; }
  confirmDelete(): void { if (!this.itemToDelete) return; const id = this.itemToDelete.id; this.deleteItem(id); this.closeDeleteModal(); }

  override onSearchChange(term: string): void { this.searchTerm = term; this.pagination.currentPage = 1; this.saveCurrentState(); this.loadData(); }
  override onStatusFilterChange(status: string): void {
    const next = this.statusFilter === status ? '' : status;
    this.statusFilter = next;
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }
  override onSort(column: string): void {
    if (this.sortBy === column) this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    else { this.sortBy = column; this.sortDirection = 'asc'; }
    this.saveCurrentState();
    this.loadData();
  }
  override onPageChange(page: number): void { this.pagination.currentPage = page; this.saveCurrentState(); this.loadData(); }
  override onPageSizeChange(pageSize: number): void { this.pagination.pageSize = pageSize; this.pagination.currentPage = 1; this.saveCurrentState(); this.loadData(); }

  override clearAllFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.selectedManufacturerId = null;
    this.selectedBrand = null;
    this.sortBy = '';
    this.sortDirection = 'asc';
    this.pagination.currentPage = 1;
    this.listStateService.clearState(this.pageKey);
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.selectedManufacturerId = null;
    this.selectedBrand = null;
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
    if (key === 'id') return (a?.[key] ?? 0) - (b?.[key] ?? 0);
    return av.localeCompare(bv);
  }

  protected override matchesStatus(item: Product, status: string): boolean {
    switch (status) {
      case 'Active': return ProductHelper.isActive(item);
      case 'Inactive': return !ProductHelper.isActive(item) && !ProductHelper.isDiscontinued(item);
      case 'Discontinued': return ProductHelper.isDiscontinued(item);
      default: return (item.status as any) === status;
    }
  }

  // Helper methods for template
  getStatusColor(status: string): string { return ProductHelper.getStatusColor(status as any); }
  getShortDescription(product: Product): string { return ProductHelper.getShortDescription(product, 40); }
  formatCurrency(amount?: number): string { return ProductHelper.formatCurrency(amount); }
  canEdit(product: Product): boolean { return ProductHelper.canEdit(product); }
  canDelete(product: Product): boolean { return ProductHelper.canDelete(product); }

  getActiveFilterPills(): { key: string, label: string }[] {
    const pills: { key: string, label: string }[] = [];
    if (this.statusFilter) pills.push({ key: 'status', label: `Status: ${this.statusFilter}` });
    if (this.searchTerm) pills.push({ key: 'search', label: `Search: ${this.searchTerm}` });
    if (this.selectedManufacturerId) pills.push({ key: 'manufacturerId', label: `Manufacturer: ${this.getManufacturerNameById(this.selectedManufacturerId)}` });
    if (this.selectedBrand) pills.push({ key: 'brand', label: `Brand: ${this.selectedBrand}` });
    return pills;
  }

  removeFilter(key: string): void {
    if (key === 'search') this.searchTerm = '';
    else if (key === 'status') this.statusFilter = '';
    else if (key === 'manufacturerId') this.selectedManufacturerId = null;
    else if (key === 'brand') this.selectedBrand = null;
    this.paginationService.removeFilter(key);
    this.pagination.currentPage = 1;
    this.saveCurrentState();
    this.loadData();
  }

  getManufacturerNameById(id: number | null): string {
    if (!id) return '';
    return this.manufacturers.find(m => m.id === id)?.name || `${id}`;
  }

  private updatePaginationIndices(): void {
    this.pagination.startIndex = (this.pagination.currentPage - 1) * this.pagination.pageSize;
    this.pagination.endIndex = Math.min(
      this.pagination.startIndex + this.pagination.pageSize - 1,
      this.pagination.totalItems - 1
    );
  }

  override exportToExcel(): void {
    const dataToExport = this.excelExportService.prepareDataForExport(
      this.filteredItems,
      ['id', 'createdDate', 'modifiedDate']
    );
    const formattedData = this.excelExportService.formatHeaders(dataToExport, {
      manufacturerProductCode: 'MFR Product Code'
    });
    this.excelExportService.exportToExcel(formattedData, `${this.entityName}_Export`, this.entityName);
  }

  Math = Math;
  Object = Object;
}
