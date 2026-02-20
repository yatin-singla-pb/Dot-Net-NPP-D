import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from './api.service';
import {
  Manufacturer,
  CreateManufacturerRequest,
  UpdateManufacturerRequest,
  ManufacturerSearchResult,
  ManufacturerStatus
} from '../models/manufacturer.model';

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
export class ManufacturerService {
  private readonly endpoint = 'manufacturers';

  constructor(private apiService: ApiService) {}

  getPaginated(
    pageNumber: number,
    pageSize: number,
    sortBy?: string,
    sortDirection: 'asc' | 'desc' = 'asc',
    searchTerm?: string,
    status?: string,
    state?: string,
    primaryBrokerId?: number
  ): Observable<PaginatedResult<Manufacturer>> {
    const params: any = {
      pageNumber,
      pageSize,
      sortDirection
    };
    if (sortBy) params.sortBy = sortBy;
    if (searchTerm) params.searchTerm = searchTerm;
    if (status) params.status = status;
    if (state) params.state = state;
    if (primaryBrokerId) params.primaryBrokerId = primaryBrokerId;

    return this.apiService.get<any>(this.endpoint, params).pipe(
      map(response => {
        const items = (response.Items || response.items || []).map((dto: any) => this.mapDtoToManufacturer(dto));
        return {
          items,
          totalCount: response.TotalCount || response.totalCount || items.length,
          pageNumber: response.PageNumber || response.pageNumber || pageNumber,
          pageSize: response.PageSize || response.pageSize || pageSize,
          totalPages: response.TotalPages || response.totalPages || Math.ceil((response.TotalCount || items.length) / (response.PageSize || pageSize))
        } as PaginatedResult<Manufacturer>;
      }),
      catchError(this.handleError)
    );
  }

  getManufacturerById(id: number): Observable<Manufacturer> {
    return this.apiService.get<any>(`${this.endpoint}/${id}`).pipe(
      map(dto => this.mapDtoToManufacturer(dto)),
      catchError(this.handleError)
    );
  }

  createManufacturer(manufacturer: CreateManufacturerRequest): Observable<Manufacturer> {
    const payload = this.toCreateDto(manufacturer);
    return this.apiService.post<any>(this.endpoint, payload).pipe(
      map(dto => this.mapDtoToManufacturer(dto)),
      catchError(this.handleError)
    );
  }

  updateManufacturer(id: number, manufacturer: UpdateManufacturerRequest): Observable<Manufacturer> {
    const payload = this.toUpdateDto(manufacturer);
    return this.apiService.put<any>(`${this.endpoint}/${id}`, payload).pipe(
      map(dto => this.mapDtoToManufacturer(dto)),
      catchError(this.handleError)
    );
  }

  deleteManufacturer(id: number): Observable<any> {
    return this.apiService.delete(`${this.endpoint}/${id}`).pipe(catchError(this.handleError));
  }

  activateManufacturer(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/activate`, {});
  }

  deactivateManufacturer(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/deactivate`, {});
  }

  // Mapping helpers
  private mapDtoToManufacturer(dto: any): Manufacturer {
    let status: ManufacturerStatus;
    if (dto.StatusName || dto.statusName) {
      const statusName = (dto.StatusName || dto.statusName) as string;
      switch (statusName) {
        case 'Active': status = ManufacturerStatus.Active; break;
        case 'Inactive': status = ManufacturerStatus.Inactive; break;
        case 'Pending': status = ManufacturerStatus.Pending; break;
        default: status = ManufacturerStatus.Active;
      }
    } else {
      const statusInt = dto.Status || dto.status || 1;
      switch (statusInt) {
        case 1: status = ManufacturerStatus.Active; break;
        case 2: status = ManufacturerStatus.Inactive; break;
        case 3: status = ManufacturerStatus.Pending; break;
        default: status = ManufacturerStatus.Active;
      }
    }

    return {
      id: dto.Id || dto.id,
      name: dto.Name || dto.name,
      aka: dto.AKA || dto.aka,
      description: dto.Description || dto.description,
      status: status,
      address: dto.Address || dto.address,
      city: dto.City || dto.city,
      state: dto.State || dto.state,
      zipCode: dto.ZipCode || dto.zipCode,
      country: dto.Country || dto.country,
      phoneNumber: dto.PhoneNumber || dto.phoneNumber,
      email: dto.Email || dto.email,
      website: dto.Website || dto.website,
      primaryBrokerId: (dto.PrimaryBrokerId ?? dto.primaryBrokerId) as number | undefined,
      primaryBrokerName: dto.PrimaryBrokerName ?? dto.primaryBrokerName,
      contactPerson: dto.ContactPerson || dto.contactPerson,
      contactPersonId: (dto.ContactPersonId ?? dto.contactPersonId) as number | undefined,
      contactPersonName: dto.ContactPersonName ?? dto.contactPersonName,
      isActive: dto.IsActive !== undefined ? dto.IsActive : (dto.isActive ?? true),
      createdDate: new Date(dto.CreatedDate || dto.createdDate || Date.now()),
      modifiedDate: dto.ModifiedDate || dto.modifiedDate ? new Date(dto.ModifiedDate || dto.modifiedDate) : undefined,
      createdBy: dto.CreatedBy || dto.createdBy,
      modifiedBy: dto.ModifiedBy || dto.modifiedBy
    } as Manufacturer;
  }

  private toStatusInt(status?: string | ManufacturerStatus): number {
    switch (status) {
      case 'Inactive':
      case ManufacturerStatus.Inactive:
        return 2;
      case 'Pending':
      case ManufacturerStatus.Pending:
        return 3;
      case 'Active':
      case ManufacturerStatus.Active:
      default:
        return 1;
    }
  }

  private emptyToNull<T>(v: any): any {
    return v === '' || v === undefined ? null : v;
    }

  private toCreateDto(req: CreateManufacturerRequest): any {
    return {
      name: req.name,
      aka: this.emptyToNull(req.aka),
      description: this.emptyToNull(req.description),
      contactPerson: this.emptyToNull(req.contactPerson),
      contactPersonId: req.contactPersonId ?? null,
      email: this.emptyToNull(req.email),
      phoneNumber: this.emptyToNull(req.phoneNumber),
      address: this.emptyToNull(req.address),
      city: this.emptyToNull(req.city),
      state: this.emptyToNull(req.state),
      zipCode: this.emptyToNull(req.zipCode),
      country: this.emptyToNull(req.country),
      primaryBrokerId: req.primaryBrokerId ?? null,
      status: this.toStatusInt(req.status)
    };
  }

  private toUpdateDto(req: UpdateManufacturerRequest): any {
    const dto: any = {};
    if (req.name !== undefined) dto.name = req.name;
    if (req.aka !== undefined) dto.aka = this.emptyToNull(req.aka);
    if (req.description !== undefined) dto.description = this.emptyToNull(req.description);
    if (req.contactPerson !== undefined) dto.contactPerson = this.emptyToNull(req.contactPerson);
    if (req.contactPersonId !== undefined) dto.contactPersonId = req.contactPersonId ?? null;
    if (req.email !== undefined) dto.email = this.emptyToNull(req.email);
    if (req.phoneNumber !== undefined) dto.phoneNumber = this.emptyToNull(req.phoneNumber);
    if (req.address !== undefined) dto.address = this.emptyToNull(req.address);
    if (req.city !== undefined) dto.city = this.emptyToNull(req.city);
    if (req.state !== undefined) dto.state = this.emptyToNull(req.state);
    if (req.zipCode !== undefined) dto.zipCode = this.emptyToNull(req.zipCode);
    if (req.country !== undefined) dto.country = this.emptyToNull(req.country);
    if (req.primaryBrokerId !== undefined) dto.primaryBrokerId = req.primaryBrokerId ?? null;
    if (req.status !== undefined) dto.status = this.toStatusInt(req.status);
    if (req.isActive !== undefined) dto.isActive = req.isActive;
    return dto;
  }

  private handleError(error: any) {
    return throwError(() => error);
  }

  // Alias methods for compatibility
  getAll(): Observable<Manufacturer[]> {
    return this.apiService.get<any>(this.endpoint).pipe(
      map((resp: any) => (resp.items || resp.Items || resp || []).map((dto: any) => this.mapDtoToManufacturer(dto)))
    );
  }

  // Get all active manufacturers (for dropdowns)
  getAllActive(): Observable<Manufacturer[]> {
    return this.getPaginated(1, 1000, 'name', 'asc', undefined, 'Active').pipe(
      map(res => res.items)
    );
  }

  getById(id: number): Observable<Manufacturer> {
    return this.getManufacturerById(id);
  }

  create(manufacturer: CreateManufacturerRequest): Observable<Manufacturer> {
    return this.createManufacturer(manufacturer);
  }

  update(id: number, manufacturer: UpdateManufacturerRequest): Observable<Manufacturer> {
    return this.updateManufacturer(id, manufacturer);
  }

  delete(id: number): Observable<any> {
    return this.deleteManufacturer(id);
  }
}
