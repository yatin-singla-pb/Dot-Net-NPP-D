import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Industry, IndustryStatus, CreateIndustryRequest, UpdateIndustryRequest } from '../models/industry.model';
import { ApiService } from './api.service';

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class IndustryService {
  private readonly endpoint = 'industries';

  constructor(
    private http: HttpClient,
    private apiService: ApiService
  ) { }

  /**
   * Get paginated industries
   */
  getPaginated(
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy?: string,
    sortDirection: string = 'asc',
    searchTerm?: string,
    status?: string
  ): Observable<PaginatedResult<Industry>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('sortDirection', sortDirection);

    if (sortBy) {
      params = params.set('sortBy', sortBy);
    }

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    if (status) {
      params = params.set('status', status);
    }

    const url = `${this.apiService.getApiUrl()}/${this.endpoint}`;

    return this.http.get<any>(url, { params })
      .pipe(
        map(response => {
          const items = (response.Items || response.items || []);
          const mappedItems = items.map((dto: any) => this.mapDtoToIndustry(dto));

          return {
            items: mappedItems,
            totalCount: response.TotalCount || response.totalCount || 0,
            pageNumber: response.PageNumber || response.pageNumber || 1,
            pageSize: response.PageSize || response.pageSize || 10,
            totalPages: response.TotalPages || response.totalPages || 1,
            hasPreviousPage: (response.PageNumber || response.pageNumber || 1) > 1,
            hasNextPage: (response.PageNumber || response.pageNumber || 1) < (response.TotalPages || response.totalPages || 1)
          };
        }),
        catchError(error => {
          console.error('🚨 API call failed:', error);
          return this.handleError(error);
        })
      );
  }

  /**
   * Get all industries (legacy method)
   */
  getAll(): Observable<Industry[]> {
    return this.getPaginated(1, 1000).pipe(
      map(result => result.items)
    );
  }

  /**
   * Get industries - simple method for component
   */
  getIndustries(): Observable<Industry[]> {
    const url = `${this.apiService.getApiUrl()}/${this.endpoint}`;

    return this.http.get<any>(url).pipe(
      map(response => {
        // If response has Items property, return just the items
        if (response.Items || response.items) {
          return (response.Items || response.items).map((dto: any) => this.mapDtoToIndustry(dto));
        }
        // If response is direct array, map it
        if (Array.isArray(response)) {
          return response.map((dto: any) => this.mapDtoToIndustry(dto));
        }
        // Fallback
        return [];
      }),
      catchError(this.handleError)
    );
  }



  /**
   * Get industry by ID
   */
  getById(id: number): Observable<Industry> {
    return this.http.get<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`)
      .pipe(
        map(dto => this.mapDtoToIndustry(dto)),
        catchError(this.handleError)
      );
  }

  /**
   * Create new industry
   */
  create(industry: CreateIndustryRequest): Observable<Industry> {
    const payload = this.toCreateDto(industry);
    return this.http.post<Industry>(`${this.apiService.getApiUrl()}/${this.endpoint}`, payload)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Update existing industry
   */
  update(id: number, industry: UpdateIndustryRequest): Observable<Industry> {
    const payload = this.toUpdateDto(industry);
    return this.http.put<Industry>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`, payload)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Delete industry
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Activate industry
   */
  activate(id: number): Observable<Industry> {
    return this.http.patch<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/activate`, {})
      .pipe(
        map(dto => this.mapDtoToIndustry(dto)),
        catchError(this.handleError)
      );
  }

  /**
   * Deactivate industry
   */
  deactivate(id: number): Observable<Industry> {
    return this.http.patch<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/deactivate`, {})
      .pipe(
        map(dto => this.mapDtoToIndustry(dto)),
        catchError(this.handleError)
      );
  }

  /**
   * Search industries by name
   */
  search(query: string): Observable<Industry[]> {
    const params = new HttpParams().set('q', query);
    
    return this.http.get<Industry[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/search`, { params })
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Get active industries only
   */
  getActive(): Observable<Industry[]> {
    return this.http.get<Industry[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/active`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Bulk delete industries
   */
  bulkDelete(ids: number[]): Observable<void> {
    return this.http.request<void>('delete', `${this.apiService.getApiUrl()}/${this.endpoint}/bulk`, {
      body: { ids }
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Bulk update status
   */
  bulkUpdateStatus(ids: number[], status: string): Observable<void> {
    return this.http.patch<void>(`${this.apiService.getApiUrl()}/${this.endpoint}/bulk/status`, {
      ids,
      status
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Export industries to Excel
   */
  exportToExcel(filters?: any): Observable<Blob> {
    let params = new HttpParams();
    
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== null && filters[key] !== undefined && filters[key] !== '') {
          params = params.set(key, filters[key]);
        }
      });
    }

    return this.http.get(`${this.apiService.getApiUrl()}/${this.endpoint}/export`, {
      params,
      responseType: 'blob'
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Get industry statistics
   */
  getStatistics(): Observable<{
    total: number;
    active: number;
    inactive: number;
    recentlyCreated: number;
  }> {
    return this.http.get<{
      total: number;
      active: number;
      inactive: number;
      recentlyCreated: number;
    }>(`${this.apiService.getApiUrl()}/${this.endpoint}/statistics`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Validate industry name uniqueness
   */
  validateName(name: string, excludeId?: number): Observable<{ isValid: boolean; message?: string }> {
    let params = new HttpParams().set('name', name);
    
    if (excludeId) {
      params = params.set('excludeId', excludeId.toString());
    }

    return this.http.get<{ isValid: boolean; message?: string }>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/validate-name`,
      { params }
    ).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Map API DTO to Angular model
   */
  private mapDtoToIndustry(dto: any): Industry {
    // Map status - try both StatusName (string) and Status (integer)
    let status: IndustryStatus;
    if (dto.StatusName || dto.statusName) {
      // Use string status name if available
      const statusName = dto.StatusName || dto.statusName;
      switch (statusName) {
        case 'Active':
          status = IndustryStatus.Active;
          break;
        case 'Inactive':
          status = IndustryStatus.Inactive;
          break;
        case 'Pending':
          status = IndustryStatus.Pending;
          break;
        default:
          status = IndustryStatus.Active;
      }
    } else {
      // Fallback to integer status
      const statusInt = dto.Status || dto.status || 1;
      switch (statusInt) {
        case 1:
          status = IndustryStatus.Active;
          break;
        case 2:
          status = IndustryStatus.Inactive;
          break;
        case 3:
          status = IndustryStatus.Pending;
          break;
        default:
          status = IndustryStatus.Active;
      }
    }

    const mapped = {
      id: dto.Id || dto.id,
      name: dto.Name || dto.name,
      description: dto.Description || dto.description,
      status: status,
      isActive: dto.IsActive !== undefined ? dto.IsActive : dto.isActive,
      createdDate: new Date(dto.CreatedDate || dto.createdDate),
      modifiedDate: (dto.ModifiedDate || dto.modifiedDate) ? new Date(dto.ModifiedDate || dto.modifiedDate) : undefined,
      createdBy: dto.CreatedBy || dto.createdBy,
      modifiedBy: dto.ModifiedBy || dto.modifiedBy
    };

    return mapped;
  }

  private toStatusInt(status?: string | IndustryStatus): number {
    switch (status) {
      case 'Inactive':
      case IndustryStatus.Inactive:
        return 2;
      case 'Pending':
      case IndustryStatus.Pending:
        return 3;
      case 'Active':
      case IndustryStatus.Active:
      default:
        return 1;
    }
  }

  private toCreateDto(req: CreateIndustryRequest): any {
    return {
      name: req.name,
      description: req.description ?? null,
      status: this.toStatusInt(req.status)
    };
  }

  private toUpdateDto(req: UpdateIndustryRequest): any {
    const dto: any = {};
    if (req.name !== undefined) dto.name = req.name;
    if (req.description !== undefined) dto.description = req.description ?? null;
    if (req.status !== undefined) dto.status = this.toStatusInt(req.status);
    if (req.isActive !== undefined) dto.isActive = req.isActive;
    return dto;
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: any): Observable<never> {
    let errorMessage = 'An unknown error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      if (error.status === 401) {
        errorMessage = 'Unauthorized. Please log in again.';
      } else if (error.status === 403) {
        errorMessage = 'Access denied. You do not have permission to perform this action.';
      } else if (error.status === 404) {
        errorMessage = 'Industry not found.';
      } else if (error.status === 409) {
        errorMessage = 'Industry name already exists.';
      } else if (error.status === 422) {
        errorMessage = error.error?.message || 'Validation failed.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      } else if (error.error?.message) {
        errorMessage = error.error.message;
      }
    }

    console.error('Industry Service Error:', error);
    return throwError(() => new Error(errorMessage));
  }
}
