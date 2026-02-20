import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { OpCo, CreateOpCoRequest, UpdateOpCoRequest } from '../models/opco.model';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class OpCoService {
  private readonly endpoint = 'opcos';

  constructor(
    private http: HttpClient,
    private apiService: ApiService
  ) { }

  private mapDtoToOpCo(dto: any): OpCo {
    // Map status from name or int
    let status: any = 'Active';
    const rawStatusName = dto.StatusName || dto.statusName;
    const rawStatus = dto.Status ?? dto.status;
    if (rawStatusName) {
      status = rawStatusName;
    } else if (rawStatus !== undefined) {
      status = rawStatus === 1 ? 'Active' : rawStatus === 2 ? 'Inactive' : rawStatus === 3 ? 'Pending' : 'Active';
    }

    return {
      id: dto.Id ?? dto.id,
      name: dto.Name ?? dto.name,
      remoteReferenceCode: dto.RemoteReferenceCode ?? dto.remoteReferenceCode,
      distributorId: dto.DistributorId ?? dto.distributorId,
      distributorName: dto.DistributorName ?? dto.distributorName,
      address: dto.Address ?? dto.address,
      city: dto.City ?? dto.city,
      state: dto.State ?? dto.state,
      zipCode: dto.ZipCode ?? dto.zipCode,
      country: dto.Country ?? dto.country,
      phoneNumber: dto.PhoneNumber ?? dto.phoneNumber,
      email: dto.Email ?? dto.email,
      contactPerson: dto.ContactPerson ?? dto.contactPerson,
      status: status,
      isActive: (dto.IsActive ?? dto.isActive) ?? (status === 'Active'),
      createdDate: new Date(dto.CreatedDate ?? dto.createdDate ?? Date.now()),
      modifiedDate: dto.ModifiedDate || dto.modifiedDate ? new Date(dto.ModifiedDate ?? dto.modifiedDate) : undefined,
      createdBy: dto.CreatedBy ?? dto.createdBy,
      modifiedBy: dto.ModifiedBy ?? dto.modifiedBy
    } as OpCo;
  }
  getAll(): Observable<OpCo[]> {
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}`)
      .pipe(
        catchError(this.handleError),
        map(list => (list || []).map(x => this.mapDtoToOpCo(x)))
      );
  }

  /**
   * Get OpCos with pagination and filtering
   */
  getPaginated(
    page: number = 1,
    pageSize: number = 12,
    search?: string,
    status?: string,
    distributorId?: number,
    sortBy?: string,
    sortDirection?: 'asc' | 'desc',
    remoteReferenceCode?: string
  ): Observable<{ items: OpCo[], totalCount: number }> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search) params = params.set('search', search);
    if (status) params = params.set('status', status);
    if (distributorId) params = params.set('distributorId', distributorId.toString());
    if (sortBy) params = params.set('sortBy', sortBy);
    if (sortDirection) params = params.set('sortDirection', sortDirection);
    if (remoteReferenceCode) params = params.set('remoteReferenceCode', remoteReferenceCode);

    return this.http.get<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/paginated`,
      { params }
    ).pipe(
      catchError(this.handleError),
      map(resp => ({
        items: (resp.items || resp.Items || []).map((x: any) => this.mapDtoToOpCo(x)),
        totalCount: resp.totalCount ?? resp.TotalCount ?? 0
      }))
    );
  }

  /**
   * Get OpCo by ID
   */
  getById(id: number): Observable<OpCo> {
    return this.http.get<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`)
      .pipe(catchError(this.handleError), map(dto => this.mapDtoToOpCo(dto)));
  }

  /**
   * Create new OpCo
   */
  create(opco: CreateOpCoRequest): Observable<OpCo> {
    const payload = this.toCreateDto(opco);
    return this.http.post<any>(`${this.apiService.getApiUrl()}/${this.endpoint}`, payload)
      .pipe(catchError(this.handleError), map(dto => this.mapDtoToOpCo(dto)));
  }

  /**
   * Update existing OpCo
   */
  update(id: number, opco: UpdateOpCoRequest): Observable<OpCo> {
    const payload = this.toUpdateDto(opco);
    return this.http.put<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`, payload)
      .pipe(catchError(this.handleError), map(dto => this.mapDtoToOpCo(dto)));
  }

  private toStatusInt(status?: any): number {
    switch (status) {
      case 'Inactive': return 2;
      case 'Pending': return 3;
      case 'Active': return 1;
      default:
        if (typeof status === 'number') return status;
        return 1;
    }
  }

  private emptyToNull<T>(v: any): any {
    return v === '' || v === undefined ? null : v;
  }

  private toCreateDto(req: CreateOpCoRequest): any {
    return {
      name: req.name,
      remoteReferenceCode: this.emptyToNull(req.remoteReferenceCode),
      distributorId: req.distributorId,
      address: this.emptyToNull(req.address),
      city: this.emptyToNull(req.city),
      state: this.emptyToNull(req.state),
      zipCode: this.emptyToNull(req.zipCode),
      country: this.emptyToNull(req.country),
      phoneNumber: this.emptyToNull(req.phoneNumber),
      email: this.emptyToNull(req.email),
      contactPerson: this.emptyToNull(req.contactPerson),
      status: this.toStatusInt(req.status)
    };
  }

  private toUpdateDto(req: UpdateOpCoRequest): any {
    const dto: any = {};
    if (req.name !== undefined) dto.name = req.name;
    if (req.remoteReferenceCode !== undefined) dto.remoteReferenceCode = this.emptyToNull(req.remoteReferenceCode);
    if (req.distributorId !== undefined) dto.distributorId = req.distributorId;
    if (req.address !== undefined) dto.address = this.emptyToNull(req.address);
    if (req.city !== undefined) dto.city = this.emptyToNull(req.city);
    if (req.state !== undefined) dto.state = this.emptyToNull(req.state);
    if (req.zipCode !== undefined) dto.zipCode = this.emptyToNull(req.zipCode);
    if (req.country !== undefined) dto.country = this.emptyToNull(req.country);
    if (req.phoneNumber !== undefined) dto.phoneNumber = this.emptyToNull(req.phoneNumber);
    if (req.email !== undefined) dto.email = this.emptyToNull(req.email);
    if (req.contactPerson !== undefined) dto.contactPerson = this.emptyToNull(req.contactPerson);
    if (req.status !== undefined) dto.status = this.toStatusInt(req.status);
    if (req.isActive !== undefined) dto.isActive = req.isActive;
    return dto;
  }


  /**
   * Delete OpCo
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Get OpCos by distributor
   */
  getByDistributor(distributorId: number): Observable<OpCo[]> {
    return this.http.get<OpCo[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/distributor/${distributorId}`)
      .pipe(catchError(this.handleError));
  }


	  /**
	   * Get OpCos by multiple distributors (deduplicated)
	   */
	  getByDistributorIds(distributorIds: number[]): Observable<OpCo[]> {
	    return this.http.post<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/by-distributors`, distributorIds || [])
	      .pipe(
	        catchError(this.handleError),
	        map(list => (list || []).map(dto => this.mapDtoToOpCo(dto)))
	      );
	  }

  /**
   * Get active OpCos only
   */
  getActive(): Observable<OpCo[]> {
    return this.http.get<OpCo[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/active`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Search OpCos
   */
  search(query: string): Observable<OpCo[]> {
    const params = new HttpParams().set('q', query);
    return this.http.get<OpCo[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/search`, { params })
      .pipe(catchError(this.handleError));
  }

  /**
   * Activate OpCo
   */
  activate(id: number): Observable<OpCo> {
    return this.http.patch<OpCo>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/activate`, {})
      .pipe(catchError(this.handleError));
  }

  /**
   * Deactivate OpCo
   */
  deactivate(id: number): Observable<OpCo> {
    return this.http.patch<OpCo>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/deactivate`, {})
      .pipe(catchError(this.handleError));
  }

  /**
   * Bulk delete OpCos
   */
  bulkDelete(ids: number[]): Observable<void> {
    return this.http.request<void>('delete', `${this.apiService.getApiUrl()}/${this.endpoint}/bulk`, {
      body: { ids }
    }).pipe(catchError(this.handleError));
  }

  /**
   * Export OpCos to Excel
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
    }).pipe(catchError(this.handleError));
  }

  /**
   * Validate remote reference code uniqueness
   */
  validateRemoteReferenceCode(code: string, excludeId?: number): Observable<{ isValid: boolean; message?: string }> {
    let params = new HttpParams().set('code', code);

    if (excludeId) {
      params = params.set('excludeId', excludeId.toString());
    }

    return this.http.get<{ isValid: boolean; message?: string }>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/validate-code`,
      { params }
    ).pipe(catchError(this.handleError));
  }

  /**
   * Handle HTTP errors
   */
  private handleError(error: any): Observable<never> {
    let errorMessage = 'An unknown error occurred';

    if (error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else {
      if (error.status === 401) {
        errorMessage = 'Unauthorized. Please log in again.';
      } else if (error.status === 403) {
        errorMessage = 'Access denied. You do not have permission to perform this action.';
      } else if (error.status === 404) {
        errorMessage = 'OpCo not found.';
      } else if (error.status === 409) {
        errorMessage = 'Remote reference code already exists.';
      } else if (error.status === 422) {
        errorMessage = error.error?.message || 'Validation failed.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      } else if (error.error?.message) {
        errorMessage = error.error.message;
      }
    }

    console.error('OpCo Service Error:', error);
    return throwError(() => new Error(errorMessage));
  }
}
