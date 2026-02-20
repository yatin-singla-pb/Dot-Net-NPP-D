import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface PaginationConfig {
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  startIndex: number;
  endIndex: number;
}

export interface SortConfig {
  field: string;
  direction: 'asc' | 'desc';
}

export interface FilterConfig {
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class PaginationService {
  private paginationSubject = new BehaviorSubject<PaginationConfig>({
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0,
    startIndex: 0,
    endIndex: 0
  });

  private sortSubject = new BehaviorSubject<SortConfig>({
    field: 'id',
    direction: 'asc'
  });

  private filterSubject = new BehaviorSubject<FilterConfig>({});

  pagination$ = this.paginationSubject.asObservable();
  sort$ = this.sortSubject.asObservable();
  filter$ = this.filterSubject.asObservable();

  constructor() { }

  /**
   * Update pagination configuration
   */
  updatePagination(config: Partial<PaginationConfig>): void {
    const current = this.paginationSubject.value;
    const updated = { ...current, ...config };

    // Recalculate derived values
    updated.totalPages = Math.ceil((updated.totalItems || 0) / (updated.pageSize || 1));
    updated.startIndex = (updated.currentPage - 1) * updated.pageSize;
    updated.endIndex = Math.min(updated.startIndex + updated.pageSize - 1, Math.max(updated.totalItems - 1, 0));

    // Guard: do not emit if nothing actually changed to prevent feedback loops
    const changed = (
      current.currentPage !== updated.currentPage ||
      current.pageSize !== updated.pageSize ||
      current.totalItems !== updated.totalItems ||
      current.totalPages !== updated.totalPages ||
      current.startIndex !== updated.startIndex ||
      current.endIndex !== updated.endIndex
    );
    if (!changed) {
      return;
    }

    this.paginationSubject.next(updated);
  }

  /**
   * Set total items and recalculate pagination
   */
  setTotalItems(totalItems: number): void {
    this.updatePagination({ totalItems });
  }

  /**
   * Change page size
   */
  changePageSize(pageSize: number): void {
    this.updatePagination({ pageSize, currentPage: 1 });
  }

  /**
   * Go to specific page
   */
  goToPage(page: number): void {
    const current = this.paginationSubject.value;
    if (page >= 1 && page <= current.totalPages) {
      this.updatePagination({ currentPage: page });
    }
  }

  /**
   * Go to next page
   */
  nextPage(): void {
    const current = this.paginationSubject.value;
    if (current.currentPage < current.totalPages) {
      this.goToPage(current.currentPage + 1);
    }
  }

  /**
   * Go to previous page
   */
  previousPage(): void {
    const current = this.paginationSubject.value;
    if (current.currentPage > 1) {
      this.goToPage(current.currentPage - 1);
    }
  }

  /**
   * Go to first page
   */
  firstPage(): void {
    this.goToPage(1);
  }

  /**
   * Go to last page
   */
  lastPage(): void {
    const current = this.paginationSubject.value;
    this.goToPage(current.totalPages);
  }

  /**
   * Update sort configuration
   */
  updateSort(field: string, direction?: 'asc' | 'desc'): void {
    const current = this.sortSubject.value;
    
    // If same field, toggle direction
    if (current.field === field && !direction) {
      direction = current.direction === 'asc' ? 'desc' : 'asc';
    } else if (!direction) {
      direction = 'asc';
    }

    this.sortSubject.next({ field, direction });
    
    // Reset to first page when sorting changes
    this.goToPage(1);
  }

  /**
   * Update filter configuration
   */
  updateFilter(filters: FilterConfig): void {
    this.filterSubject.next(filters);
    
    // Reset to first page when filters change
    this.goToPage(1);
  }

  /**
   * Add or update a single filter
   */
  setFilter(key: string, value: any): void {
    const current = this.filterSubject.value;
    const updated = { ...current, [key]: value };
    
    // Remove filter if value is null, undefined, or empty string
    if (value === null || value === undefined || value === '') {
      delete updated[key];
    }
    
    this.updateFilter(updated);
  }

  /**
   * Remove a specific filter
   */
  removeFilter(key: string): void {
    const current = this.filterSubject.value;
    const updated = { ...current };
    delete updated[key];
    this.updateFilter(updated);
  }

  /**
   * Clear all filters
   */
  clearFilters(): void {
    this.updateFilter({});
  }

  /**
   * Get current pagination state
   */
  getCurrentPagination(): PaginationConfig {
    return this.paginationSubject.value;
  }

  /**
   * Get current sort state
   */
  getCurrentSort(): SortConfig {
    return this.sortSubject.value;
  }

  /**
   * Get current filter state
   */
  getCurrentFilters(): FilterConfig {
    return this.filterSubject.value;
  }

  /**
   * Reset all states to default
   */
  reset(): void {
    this.paginationSubject.next({
      currentPage: 1,
      pageSize: 10,
      totalItems: 0,
      totalPages: 0,
      startIndex: 0,
      endIndex: 0
    });
    
    this.sortSubject.next({
      field: 'id',
      direction: 'asc'
    });
    
    this.filterSubject.next({});
  }

  /**
   * Generate page numbers for pagination display
   */
  getPageNumbers(): number[] {
    const current = this.paginationSubject.value;
    const { currentPage, totalPages } = current;
    const pages: number[] = [];
    
    if (totalPages <= 7) {
      // Show all pages if 7 or fewer
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Show first page
      pages.push(1);
      
      if (currentPage > 4) {
        pages.push(-1); // Ellipsis
      }
      
      // Show pages around current page
      const start = Math.max(2, currentPage - 1);
      const end = Math.min(totalPages - 1, currentPage + 1);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
      
      if (currentPage < totalPages - 3) {
        pages.push(-1); // Ellipsis
      }
      
      // Show last page
      if (totalPages > 1) {
        pages.push(totalPages);
      }
    }
    
    return pages;
  }
}
