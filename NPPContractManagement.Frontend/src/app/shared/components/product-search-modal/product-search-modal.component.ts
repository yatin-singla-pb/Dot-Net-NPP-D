import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { Product, ProductHelper, ProductStatus } from '../../../models/product.model';
import { Manufacturer } from '../../../models/manufacturer.model';
import { ProductService } from '../../../services/product.service';
import { ManufacturerService } from '../../../services/manufacturer.service';

@Component({
  selector: 'app-product-search-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-search-modal.component.html',
  styleUrls: ['./product-search-modal.component.css']
})
export class ProductSearchModalComponent implements OnInit, OnChanges, OnDestroy {
  @Input() title = 'Search Products';
  @Input() visible = false;
  @Input() manufacturerId?: number;
  @Input() statusFilter?: string;
  @Input() selectionMode: 'single' | 'multi' = 'single';

  @Output() closeModal = new EventEmitter<void>();
  @Output() selectProduct = new EventEmitter<Product>();
  @Output() selectProducts = new EventEmitter<Product[]>();

  // Data
  products: Product[] = [];
  manufacturers: Manufacturer[] = [];
  brands: string[] = [];

  // Filters
  searchTerm = '';
  selectedStatus = '';
  selectedManufacturerId: number | null = null;
  selectedBrand: string | null = null;

  // Sort
  sortBy = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalItems = 0;
  totalPages = 0;

  // UI state
  loading = false;
  error: string | null = null;
  showAdvancedFilters = false;

  // Multi-select
  selectedItems = new Set<number>();
  selectAll = false;

  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();
  private initialized = false;

  constructor(
    private productService: ProductService,
    private manufacturerService: ManufacturerService
  ) {}

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(term => {
      this.searchTerm = term;
      this.currentPage = 1;
      this.loadData();
    });

    this.loadManufacturers();
    this.loadBrands();
    this.initialized = true;

    // Load data on first open
    if (this.visible) {
      this.resetAndLoad();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible && this.initialized) {
      this.resetAndLoad();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private resetAndLoad(): void {
    this.searchTerm = '';
    this.selectedStatus = this.statusFilter || '';
    this.selectedManufacturerId = this.manufacturerId || null;
    this.selectedBrand = null;
    this.sortBy = '';
    this.sortDirection = 'asc';
    this.currentPage = 1;
    this.selectedItems.clear();
    this.selectAll = false;
    this.error = null;
    this.loadData();
  }

  private loadManufacturers(): void {
    this.manufacturerService.getAllActive().subscribe({
      next: items => this.manufacturers = items || [],
      error: () => this.manufacturers = []
    });
  }

  private loadBrands(): void {
    this.productService.getBrands().subscribe({
      next: items => this.brands = items || [],
      error: () => this.brands = []
    });
  }

  loadData(): void {
    this.loading = true;
    this.error = null;

    this.productService.getPaginated(
      this.currentPage,
      this.pageSize,
      this.sortBy || undefined,
      this.sortDirection,
      this.searchTerm || undefined,
      this.selectedStatus || undefined,
      this.selectedManufacturerId || undefined,
      this.selectedBrand || undefined
    ).subscribe({
      next: response => {
        this.products = response.items ?? [];
        this.totalItems = response.totalCount ?? 0;
        this.totalPages = response.totalPages ?? Math.ceil(this.totalItems / this.pageSize);
        this.loading = false;
        // Update selectAll state
        this.selectAll = this.products.length > 0 && this.products.every(p => this.selectedItems.has(p.id));
      },
      error: () => {
        this.error = 'Failed to load products.';
        this.loading = false;
      }
    });
  }

  // Search
  onSearchInput(term: string): void {
    this.searchSubject.next(term);
  }

  // Status filter
  onStatusFilter(status: string): void {
    this.selectedStatus = this.selectedStatus === status ? '' : status;
    this.currentPage = 1;
    this.loadData();
  }

  // Advanced filters
  toggleAdvancedFilters(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  applyAdvancedFilters(): void {
    this.currentPage = 1;
    this.showAdvancedFilters = false;
    this.loadData();
  }

  clearAdvancedFilters(): void {
    this.selectedManufacturerId = null;
    this.selectedBrand = null;
    this.currentPage = 1;
    this.loadData();
  }

  clearAllFilters(): void {
    this.searchTerm = '';
    this.selectedStatus = '';
    this.selectedManufacturerId = null;
    this.selectedBrand = null;
    this.sortBy = '';
    this.sortDirection = 'asc';
    this.currentPage = 1;
    this.loadData();
  }

  removeStatusFilter(): void {
    this.selectedStatus = '';
    this.currentPage = 1;
    this.loadData();
  }

  removeSearchFilter(): void {
    this.searchTerm = '';
    this.currentPage = 1;
    this.loadData();
  }

  removeManufacturerFilter(): void {
    this.selectedManufacturerId = null;
    this.currentPage = 1;
    this.loadData();
  }

  removeBrandFilter(): void {
    this.selectedBrand = null;
    this.currentPage = 1;
    this.loadData();
  }

  // Sort
  onSort(column: string): void {
    if (this.sortBy === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortDirection = 'asc';
    }
    this.currentPage = 1;
    this.loadData();
  }

  getSortIcon(field: string): string {
    if (this.sortBy !== field) return 'fa-sort';
    return this.sortDirection === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }

  // Pagination
  onPageChange(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.loadData();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
    this.loadData();
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    if (this.totalPages <= 7) {
      for (let i = 1; i <= this.totalPages; i++) pages.push(i);
    } else {
      pages.push(1);
      if (this.currentPage > 4) pages.push(-1);
      const start = Math.max(2, this.currentPage - 1);
      const end = Math.min(this.totalPages - 1, this.currentPage + 1);
      for (let i = start; i <= end; i++) pages.push(i);
      if (this.currentPage < this.totalPages - 3) pages.push(-1);
      if (this.totalPages > 1) pages.push(this.totalPages);
    }
    return pages;
  }

  get startIndex(): number {
    return (this.currentPage - 1) * this.pageSize;
  }

  get endIndex(): number {
    return Math.min(this.startIndex + this.pageSize, this.totalItems);
  }

  // Selection — single mode
  onRowClick(product: Product): void {
    if (this.selectionMode === 'single') {
      this.selectProduct.emit(product);
      this.closeModal.emit();
    }
  }

  // Selection — multi mode
  toggleItemSelection(product: Product): void {
    if (this.selectedItems.has(product.id)) {
      this.selectedItems.delete(product.id);
    } else {
      this.selectedItems.add(product.id);
    }
    this.selectAll = this.products.length > 0 && this.products.every(p => this.selectedItems.has(p.id));
  }

  toggleSelectAll(): void {
    if (this.selectAll) {
      this.products.forEach(p => this.selectedItems.delete(p.id));
      this.selectAll = false;
    } else {
      this.products.forEach(p => this.selectedItems.add(p.id));
      this.selectAll = true;
    }
  }

  isSelected(product: Product): boolean {
    return this.selectedItems.has(product.id);
  }

  confirmMultiSelect(): void {
    const selected = this.products.filter(p => this.selectedItems.has(p.id));
    this.selectProducts.emit(selected);
    this.closeModal.emit();
  }

  // Helpers
  getStatusColor(status: string): string {
    return ProductHelper.getStatusColor(status as ProductStatus);
  }

  getManufacturerName(id: number | null): string {
    if (!id) return '';
    return this.manufacturers.find(m => m.id === id)?.name || `${id}`;
  }

  get hasActiveFilters(): boolean {
    return !!(this.selectedStatus || this.searchTerm || this.selectedManufacturerId || this.selectedBrand);
  }

  onClose(): void {
    this.closeModal.emit();
  }

  Math = Math;
}
