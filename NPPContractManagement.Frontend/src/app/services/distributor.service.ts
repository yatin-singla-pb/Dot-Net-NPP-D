import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from './api.service';
import {
  Distributor,
  CreateDistributorRequest,
  UpdateDistributorRequest,
  DistributorSearchResult,
  DistributorStatus
} from '../models/distributor.model';

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class DistributorService {
  private readonly endpoint = 'distributors';

  constructor(private apiService: ApiService) {}

  getAllDistributors(): Observable<Distributor[]> {
    return this.apiService.get<Distributor[]>(this.endpoint);
  }

  getPaginated(
    pageNumber: number,
    pageSize: number,
    sortBy?: string,
    sortDirection: 'asc' | 'desc' = 'asc',
    searchTerm?: string,
    status?: string,
    receiveContractProposal?: boolean | string,
    state?: string
  ): Observable<PaginatedResult<Distributor>> {
    const params: any = {
      pageNumber,
      pageSize,
      sortDirection
    };
    if (sortBy) params.sortBy = sortBy;
    if (searchTerm) params.searchTerm = searchTerm;
    if (status) params.status = status;
    if (receiveContractProposal !== undefined && receiveContractProposal !== null && receiveContractProposal !== '') {
      params.receiveContractProposal = receiveContractProposal;
    }
    if (state) params.state = state;

    return this.apiService.get<any>(this.endpoint, params).pipe(
      map(response => {
        const items = (response.Items || response.items || []).map((dto: any) => this.mapDtoToDistributor(dto));
        return {
          items,
          totalCount: response.TotalCount || response.totalCount || items.length,
          pageNumber: response.PageNumber || response.pageNumber || pageNumber,
          pageSize: response.PageSize || response.pageSize || pageSize,
          totalPages: response.TotalPages || response.totalPages || Math.ceil((response.TotalCount || items.length) / (response.PageSize || pageSize))
        } as PaginatedResult<Distributor>;
      }),
      catchError(this.handleError)
    );
  }

  getDistributorById(id: number): Observable<Distributor> {
    return this.apiService.get<any>(`${this.endpoint}/${id}`).pipe(
      map(dto => this.mapDtoToDistributor(dto)),
      catchError(this.handleError)
    );
  }

  createDistributor(distributor: CreateDistributorRequest): Observable<Distributor> {
    const payload = this.toCreateDto(distributor);
    return this.apiService.post<Distributor>(this.endpoint, payload);
  }

  updateDistributor(id: number, distributor: UpdateDistributorRequest): Observable<Distributor> {
    const payload = this.toUpdateDto(distributor);
    return this.apiService.put<Distributor>(`${this.endpoint}/${id}`, payload);
  }

  deleteDistributor(id: number): Observable<any> {
    return this.apiService.delete(`${this.endpoint}/${id}`);
  }

  // Map API DTO to Angular model
  private mapDtoToDistributor(dto: any): Distributor {
    // Resolve status from string or int
    let status: DistributorStatus;
    if (dto.StatusName || dto.statusName) {
      const statusName = dto.StatusName || dto.statusName;
      switch (statusName) {
        case 'Active': status = DistributorStatus.Active; break;
        case 'Inactive': status = DistributorStatus.Inactive; break;
        case 'Pending': status = DistributorStatus.Pending; break;
        default: status = DistributorStatus.Active;
      }
    } else {
      const statusInt = dto.Status || dto.status || 1;
      switch (statusInt) {
        case 1: status = DistributorStatus.Active; break;
        case 2: status = DistributorStatus.Inactive; break;
        case 3: status = DistributorStatus.Pending; break;
        default: status = DistributorStatus.Active;
      }
    }

    const rcpRaw = (dto as any).ReceiveContractProposal ?? (dto as any).receiveContractProposal;
    const receiveContractProposal = typeof rcpRaw === 'string'
      ? rcpRaw.toLowerCase() === 'true'
      : !!rcpRaw;


    return {
      id: dto.Id || dto.id,
      name: dto.Name || dto.name,
      status: status,
      description: dto.Description || dto.description,
      address: dto.Address || dto.address,
      city: dto.City || dto.city,
      state: dto.State || dto.state,
      zipCode: dto.ZipCode || dto.zipCode,
      country: dto.Country || dto.country,
      phoneNumber: dto.PhoneNumber || dto.phoneNumber,
      email: dto.Email || dto.email,
      website: dto.Website || dto.website,
      contactPerson: dto.ContactPerson || dto.contactPerson,
      receiveContractProposal: receiveContractProposal,
      isActive: dto.IsActive !== undefined ? dto.IsActive : (dto.isActive ?? true),
      createdDate: new Date(dto.CreatedDate || dto.createdDate || Date.now()),
      modifiedDate: dto.ModifiedDate || dto.modifiedDate ? new Date(dto.ModifiedDate || dto.modifiedDate) : undefined,
      createdBy: dto.CreatedBy || dto.createdBy,
      modifiedBy: dto.ModifiedBy || dto.modifiedBy,
      opCosCount: dto.OpCosCount || dto.opCosCount
    } as Distributor;
  }

  private handleError(error: any) {
    return throwError(() => error);
  }

  private toStatusInt(status?: string | DistributorStatus): number {
    switch (status) {
      case 'Inactive':
      case DistributorStatus.Inactive:
        return 2;
      case 'Pending':
      case DistributorStatus.Pending:
        return 3;
      case 'Active':
      case DistributorStatus.Active:
      default:
        return 1;
    }
  }

  private emptyToNull<T>(v: any): any {
    return v === '' || v === undefined ? null : v;
  }

  private toCreateDto(req: CreateDistributorRequest): any {
    return {
      name: req.name,
      description: this.emptyToNull(req.description),
      contactPerson: this.emptyToNull(req.contactPerson),
      email: this.emptyToNull(req.email),
      phoneNumber: this.emptyToNull(req.phoneNumber),
      address: this.emptyToNull(req.address),
      city: this.emptyToNull(req.city),
      state: this.emptyToNull(req.state),
      zipCode: this.emptyToNull(req.zipCode),
      country: this.emptyToNull(req.country),
      receiveContractProposal: req.receiveContractProposal ?? true,
      status: this.toStatusInt(req.status)
    };
  }

  private toUpdateDto(req: UpdateDistributorRequest): any {
    const dto: any = {};
    if (req.name !== undefined) dto.name = req.name;
    if (req.description !== undefined) dto.description = this.emptyToNull(req.description);
    if (req.contactPerson !== undefined) dto.contactPerson = this.emptyToNull(req.contactPerson);
    if (req.email !== undefined) dto.email = this.emptyToNull(req.email);
    if (req.phoneNumber !== undefined) dto.phoneNumber = this.emptyToNull(req.phoneNumber);
    if (req.address !== undefined) dto.address = this.emptyToNull(req.address);
    if (req.city !== undefined) dto.city = this.emptyToNull(req.city);
    if (req.state !== undefined) dto.state = this.emptyToNull(req.state);
    if (req.zipCode !== undefined) dto.zipCode = this.emptyToNull(req.zipCode);
    if (req.country !== undefined) dto.country = this.emptyToNull(req.country);
    if (req.receiveContractProposal !== undefined) dto.receiveContractProposal = req.receiveContractProposal;
    if (req.status !== undefined) dto.status = this.toStatusInt(req.status);
    if (req.isActive !== undefined) dto.isActive = req.isActive;
    return dto;
  }

  activateDistributor(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/activate`, {});
  }

  deactivateDistributor(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/deactivate`, {});
  }

  searchDistributors(params: any): Observable<DistributorSearchResult> {
    return this.apiService.get<DistributorSearchResult>(`${this.endpoint}/search`, params);
  }

  exportToExcel(params?: any): Observable<Blob> {
    return this.apiService.getBlob(`${this.endpoint}/export`, params);
  }

  bulkDelete(ids: number[]): Observable<any> {
    return this.apiService.post(`${this.endpoint}/bulk-delete`, { ids });
  }

  // Alias methods for compatibility
  getAll(): Observable<Distributor[]> {
    return this.getAllDistributors();
  }

  getById(id: number): Observable<Distributor> {
    return this.getDistributorById(id);
  }

  create(distributor: CreateDistributorRequest): Observable<Distributor> {
    return this.createDistributor(distributor);
  }

  update(id: number, distributor: UpdateDistributorRequest): Observable<Distributor> {
    return this.updateDistributor(id, distributor);
  }

  delete(id: number): Observable<any> {
    return this.deleteDistributor(id);
  }
}
