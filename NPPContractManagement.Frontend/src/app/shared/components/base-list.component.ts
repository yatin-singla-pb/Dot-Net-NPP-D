import { Component, OnInit, OnDestroy, Input, inject } from '@angular/core';
import { Subject, takeUntil, debounceTime, distinctUntilChanged, combineLatest } from 'rxjs';
import { PaginationService, PaginationConfig, SortConfig, FilterConfig } from '../services/pagination.service';
import { ExcelExportService } from '../services/excel-export.service';
import { ListStateService, ListState } from '../services/list-state.service';

export interface ListColumn {
  key: string;
  label: string;
  sortable?: boolean;
  type?: 'text' | 'number' | 'date' | 'boolean' | 'status' | 'actions' | 'currency' | 'percentage';
  width?: string;
}

export interface FilterOption {
  key: string;
  label: string;
  type: 'text' | 'select' | 'date' | 'boolean';
  options?: { value: any, label: string }[];
}

@Component({
  template: ''
})
export abstract class BaseListComponent<T> implements OnInit, OnDestroy {
  @Input() entityName: string = '';
  @Input() columns: ListColumn[] = [];
  @Input() filterOptions: FilterOption[] = [];
  // When true, the child component handles paging/filtering server-side and we skip local subscriptions
  @Input() serverDriven: boolean = false;

  // Data properties
  items: T[] = [];
  filteredItems: T[] = [];
  paginatedItems: T[] = [];
  selectedItems: Set<any> = new Set();
  loading = false;
  error: string | null = null;

  // Search and filter properties
  searchTerm = '';
  activeFilters: FilterConfig = {};
  statusFilter = 'active';

  // Pagination properties
  pagination: PaginationConfig = {
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0,
    startIndex: 0,
    endIndex: 0
  };

  sort: SortConfig = {
    field: 'id',
    direction: 'asc'
  };

  // UI state
  showAdvancedFilters = false;
  selectAll = false;

  // Subjects for cleanup
  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  // List state persistence (injected without changing child constructors)
  protected listStateService = inject(ListStateService);

  constructor(
    protected paginationService: PaginationService,
    protected excelExportService: ExcelExportService
  ) {
    // Setup search debouncing
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(term => {
      this.performSearch(term);
    });
  }

  /**
   * Override in child to return a unique page key for state persistence.
   * Return empty string to disable persistence for that page.
   */
  get pageKey(): string {
    return '';
  }

  /**
   * Override in child to return page-specific filter state to save.
   * Base implementation saves common fields.
   */
  protected getFilterState(): { [key: string]: any } {
    return {};
  }

  /**
   * Override in child to restore page-specific filter state.
   * Called before the first loadData().
   */
  protected restoreFilterState(filters: { [key: string]: any }): void {
    // No-op in base; children override
  }

  /** Save the current list state to localStorage */
  protected saveCurrentState(): void {
    if (!this.pageKey) return;
    const state: ListState = {
      filters: this.getFilterState(),
      pageIndex: this.pagination.currentPage,
      pageSize: this.pagination.pageSize,
      sortField: (this as any).sortBy ?? this.sort.field ?? '',
      sortDirection: (this as any).sortDirection ?? this.sort.direction ?? 'asc',
      savedAt: Date.now()
    };
    this.listStateService.saveState(this.pageKey, state);
  }

  /** Restore list state from localStorage. Returns true if state was restored. */
  protected restoreState(): boolean {
    if (!this.pageKey) return false;
    const state = this.listStateService.getState(this.pageKey);
    if (!state) return false;

    // Restore common pagination/sort
    this.pagination.currentPage = state.pageIndex || 1;
    this.pagination.pageSize = state.pageSize || 10;
    if (state.sortField != null) {
      if ('sortBy' in this) {
        (this as any).sortBy = state.sortField;
      } else {
        this.sort.field = state.sortField;
      }
    }
    if (state.sortDirection) {
      if ('sortDirection' in this && this.hasOwnProperty('sortDirection') || (this as any).sortDirection !== undefined) {
        (this as any).sortDirection = state.sortDirection;
      } else {
        this.sort.direction = state.sortDirection;
      }
    }

    // Restore page-specific filters
    if (state.filters) {
      this.restoreFilterState(state.filters);
    }

    return true;
  }

  ngOnInit(): void {
    if (!this.serverDriven) {
      this.initializeSubscriptions();
    }
    this.restoreState();
    this.loadData();
  }

  ngOnDestroy(): void {
    this.saveCurrentState();
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeSubscriptions(): void {
    // Subscribe to pagination changes
    combineLatest([
      this.paginationService.pagination$,
      this.paginationService.sort$,
      this.paginationService.filter$
    ]).pipe(
      takeUntil(this.destroy$)
    ).subscribe(([pagination, sort, filters]) => {
      this.pagination = pagination;
      this.sort = sort;
      this.activeFilters = filters;
      this.applyFiltersAndPagination();
    });
  }

  // Abstract methods to be implemented by child components
  abstract loadData(): void;
  abstract deleteItem(id: any): void;
  abstract getItemId(item: T): any;

  // Search functionality
  onSearchChange(term: string): void {
    this.searchTerm = term;
    this.searchSubject.next(term);
  }

  private performSearch(term: string): void {
    this.paginationService.setFilter('search', term);
  }

  // Filter functionality
  onStatusFilterChange(status: string): void {
    this.statusFilter = status;
    this.paginationService.setFilter('status', status);
  }

  onAdvancedFilterChange(key: string, value: any): void {
    this.paginationService.setFilter(key, value);
  }

  clearAllFilters(): void {
    this.searchTerm = '';
    this.statusFilter = 'active';
    this.paginationService.clearFilters();
    if (this.pageKey) {
      this.listStateService.clearState(this.pageKey);
    }
  }

  toggleAdvancedFilters(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  // Sorting functionality
  onSort(field: string): void {
    this.paginationService.updateSort(field);
  }

  getSortIcon(field: string): string {
    if (this.sort.field !== field) return 'fa-sort';
    return this.sort.direction === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
  }

  // Pagination functionality
  onPageChange(page: number): void {
    this.paginationService.goToPage(page);
  }

  onPageSizeChange(size: number): void {
    this.paginationService.changePageSize(size);
  }

  getPageNumbers(): number[] {
    return this.paginationService.getPageNumbers();
  }

  // Selection functionality
  onSelectAll(): void {
    this.selectAll = !this.selectAll;
    if (this.selectAll) {
      this.paginatedItems.forEach(item => {
        this.selectedItems.add(this.getItemId(item));
      });
    } else {
      this.selectedItems.clear();
    }
  }

  onSelectItem(item: T): void {
    const id = this.getItemId(item);
    if (this.selectedItems.has(id)) {
      this.selectedItems.delete(id);
    } else {
      this.selectedItems.add(id);
    }
    this.updateSelectAllState();
  }

  isSelected(item: T): boolean {
    return this.selectedItems.has(this.getItemId(item));
  }

  protected updateSelectAllState(): void {
    const visibleIds = this.paginatedItems.map(item => this.getItemId(item));
    this.selectAll = visibleIds.length > 0 && visibleIds.every(id => this.selectedItems.has(id));
  }

  // Bulk operations
  deleteSelected(): void {
    if (this.selectedItems.size === 0) return;

    if (confirm(`Are you sure you want to delete ${this.selectedItems.size} selected items?`)) {
      this.selectedItems.forEach(id => this.deleteItem(id));
      this.selectedItems.clear();
      this.selectAll = false;
    }
  }

  // Export functionality
  exportToExcel(): void {
    const dataToExport = this.excelExportService.prepareDataForExport(
      this.filteredItems,
      ['id', 'createdDate', 'modifiedDate'] // Exclude technical fields
    );

    const formattedData = this.excelExportService.formatHeaders(dataToExport);

    this.excelExportService.exportToExcel(
      formattedData,
      `${this.entityName}_Export`,
      this.entityName
    );
  }

  exportSelectedToExcel(): void {
    if (this.selectedItems.size === 0) {
      alert('Please select items to export');
      return;
    }

    const selectedData = this.items.filter(item =>
      this.selectedItems.has(this.getItemId(item))
    );

    const dataToExport = this.excelExportService.prepareDataForExport(
      selectedData,
      ['id', 'createdDate', 'modifiedDate']
    );

    const formattedData = this.excelExportService.formatHeaders(dataToExport);

    this.excelExportService.exportToExcel(
      formattedData,
      `${this.entityName}_Selected_Export`,
      this.entityName
    );
  }

  // Data processing
  protected applyFiltersAndPagination(): void {
    // Skip local processing if server-driven; child component handles it
    if (this.serverDriven) {
      return;
    }

    // Apply filters
    this.filteredItems = this.applyFilters(this.items);

    // Update total items (guarded inside PaginationService to prevent loops)
    this.paginationService.setTotalItems(this.filteredItems.length);

    // Apply sorting
    this.filteredItems = this.applySorting(this.filteredItems);

    // Apply pagination
    const { startIndex, pageSize } = this.pagination;
    this.paginatedItems = this.filteredItems.slice(startIndex, startIndex + pageSize);

    // Update selection state
    this.updateSelectAllState();
  }

  private applyFilters(items: T[]): T[] {
    let filtered = [...items];

    // Apply search filter
    if (this.activeFilters['search']) {
      const searchTerm = this.activeFilters['search'].toLowerCase();
      filtered = filtered.filter(item =>
        this.matchesSearchTerm(item, searchTerm)
      );
    }

    // Apply status filter
    if (this.activeFilters['status']) {
      filtered = filtered.filter(item =>
        this.matchesStatus(item, this.activeFilters['status'])
      );
    }

    // Apply other filters
    Object.keys(this.activeFilters).forEach(key => {
      if (key !== 'search' && key !== 'status') {
        const value = this.activeFilters[key];
        if (value !== null && value !== undefined && value !== '') {
          filtered = filtered.filter(item =>
            this.matchesFilter(item, key, value)
          );
        }
      }
    });

    return filtered;
  }

  private applySorting(items: T[]): T[] {
    if (!this.sort.field) return items;

    return [...items].sort((a, b) => {
      const aValue = this.getNestedProperty(a, this.sort.field);
      const bValue = this.getNestedProperty(b, this.sort.field);

      let comparison = 0;
      if (aValue < bValue) comparison = -1;
      if (aValue > bValue) comparison = 1;

      return this.sort.direction === 'desc' ? -comparison : comparison;
    });
  }

  // Helper methods to be overridden by child components if needed
  protected matchesSearchTerm(item: T, searchTerm: string): boolean {
    return JSON.stringify(item).toLowerCase().includes(searchTerm);
  }

  protected matchesStatus(item: T, status: string): boolean {
    const itemStatus = (item as any).status || (item as any).isActive;
    if (status === 'active') {
      return itemStatus === 'Active' || itemStatus === true;
    }
    return itemStatus !== 'Active' && itemStatus !== true;
  }

  protected matchesFilter(item: T, key: string, value: any): boolean {
    const itemValue = this.getNestedProperty(item, key);
    return itemValue === value;
  }

  private getNestedProperty(obj: any, path: string): any {
    return path.split('.').reduce((current, prop) => current?.[prop], obj);
  }
}
