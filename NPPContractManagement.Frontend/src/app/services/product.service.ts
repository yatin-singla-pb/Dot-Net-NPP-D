import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from './api.service';
import {
  Product,
  CreateProductRequest,
  UpdateProductRequest
} from '../models/product.model';

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
export class ProductService {
  private readonly endpoint = 'products';

  constructor(private apiService: ApiService) {}

  getPaginated(
    pageNumber: number,
    pageSize: number,
    sortBy?: string,
    sortDirection: 'asc' | 'desc' = 'asc',
    searchTerm?: string,
    status?: string,
    manufacturerId?: number,
    brand?: string
  ): Observable<PaginatedResult<Product>> {
    const params: any = {
      pageNumber,
      pageSize,
      sortDirection
    };
    if (sortBy) params.sortBy = sortBy;
    if (searchTerm) params.searchTerm = searchTerm;
    if (status) params.status = status;
    if (manufacturerId) params.manufacturerId = manufacturerId;
    if (brand) params.brand = brand;

    return this.apiService.get<any>(this.endpoint, params).pipe(
      map(response => {
        const items = (response.Items || response.items || []).map((dto: any) => this.mapDtoToProduct(dto));
        return {
          items,
          totalCount: response.TotalCount || response.totalCount || items.length,
          pageNumber: response.PageNumber || response.pageNumber || pageNumber,
          pageSize: response.PageSize || response.pageSize || pageSize,
          totalPages: response.TotalPages || response.totalPages || Math.ceil((response.TotalCount || items.length) / (response.PageSize || pageSize))
        } as PaginatedResult<Product>;
      }),
      catchError(this.handleError)
    );
  }

  getById(id: number): Observable<Product> {
    return this.apiService.get<any>(`${this.endpoint}/${id}`).pipe(
      map(dto => this.mapDtoToProduct(dto)),
      catchError(this.handleError)
    );
  }

  create(request: CreateProductRequest): Observable<Product> {
    const dto = this.mapCreateRequestToDto(request);
    return this.apiService.post<any>(this.endpoint, dto).pipe(
      map(res => this.mapDtoToProduct(res)),
      catchError(this.handleError)
    );
  }

  update(id: number, request: UpdateProductRequest): Observable<Product> {
    const dto = this.mapUpdateRequestToDto(request);
    return this.apiService.put<any>(`${this.endpoint}/${id}`, dto).pipe(
      map(res => this.mapDtoToProduct(res)),
      catchError(this.handleError)
    );
  }

  delete(id: number): Observable<any> {
    return this.apiService.delete(`${this.endpoint}/${id}`).pipe(catchError(this.handleError));
  }

  activate(id: number): Observable<any> {
    return this.apiService.patch(`${this.endpoint}/${id}/activate`, {}).pipe(catchError(this.handleError));
  }

  deactivate(id: number): Observable<any> {
    return this.apiService.patch(`${this.endpoint}/${id}/deactivate`, {}).pipe(catchError(this.handleError));
  }

  getByManufacturer(manufacturerId: number): Observable<Product[]> {
    return this.apiService.get<any>(`${this.endpoint}/manufacturer/${manufacturerId}`).pipe(
      map((dtos: any[]) => dtos.map(d => this.mapDtoToProduct(d))),
      catchError(this.handleError)
    );
  }

	  getByManufacturers(manufacturerIds: number[]): Observable<Product[]> {
	    return this.apiService.post<any>(`${this.endpoint}/by-manufacturers`, manufacturerIds || []).pipe(
	      map((dtos: any[]) => (dtos || []).map(d => this.mapDtoToProduct(d))),
	      catchError(this.handleError)
	    );
	  }


  getByCategory(category: string): Observable<Product[]> {
    return this.apiService.get<any>(`${this.endpoint}/category/${encodeURIComponent(category)}`).pipe(
      map((dtos: any[]) => dtos.map(d => this.mapDtoToProduct(d))),
      catchError(this.handleError)
    );
  }

  private mapDtoToProduct(dto: any): Product {
    // Map API DTO fields to UI model
    const statusName: string = dto.statusName || dto.StatusName || dto.status || 'Active';
    return {
      id: dto.id ?? dto.Id,
      name: dto.name ?? dto.Name,
      manufacturerProductCode: dto.manufacturerProductCode ?? dto.ManufacturerProductCode ?? '',
      gtin: dto.gtin ?? dto.GTIN,
      upc: dto.upc ?? dto.UPC,
      description: dto.description ?? dto.Description ?? dto.name ?? dto.Name ?? '',
      packSize: dto.packSize ?? dto.PackSize,
      manufacturerId: dto.manufacturerId ?? dto.ManufacturerId,
      manufacturerName: dto.manufacturerName ?? dto.ManufacturerName,
      status: (statusName as any),
      category: dto.category ?? dto.Category,
      subCategory: dto.subCategory ?? dto.SubCategory,
      brand: dto.brand ?? dto.Brand,
      tertiaryCategory: dto.tertiaryCategory ?? dto.TertiaryCategory,
      alwaysList: (dto.alwaysList !== undefined ? dto.alwaysList : dto.AlwaysList) ?? null,
      notes: dto.notes ?? dto.Notes,
      wholesalePrice: dto.wholesalePrice ?? dto.WholesalePrice,
      retailPrice: dto.retailPrice ?? dto.RetailPrice,
      weight: dto.weight ?? dto.Weight,
      dimensions: dto.dimensions ?? dto.Dimensions,
      isActive: dto.isActive ?? dto.IsActive ?? true,
      createdDate: dto.createdDate ?? dto.CreatedDate,
      modifiedDate: dto.modifiedDate ?? dto.ModifiedDate,
      createdBy: dto.createdBy ?? dto.CreatedBy,
      modifiedBy: dto.modifiedBy ?? dto.ModifiedBy
    } as Product;
  }

  private mapCreateRequestToDto(req: CreateProductRequest): any {
    return {
      name: req.name,
      description: req.description,
      manufacturerProductCode: req.manufacturerProductCode,
      gtin: req.gtin,
      upc: req.upc,
      sku: (req as any).sku,
      packSize: req.packSize,
      manufacturerId: req.manufacturerId,
      category: req.category,
      subCategory: req.subCategory,
      brand: (req as any).brand,
      tertiaryCategory: (req as any).tertiaryCategory,
      alwaysList: (req as any).alwaysList,
      notes: (req as any).notes,
      status: req.status ? this.statusToInt(req.status) : 1
    };
  }

  private mapUpdateRequestToDto(req: UpdateProductRequest): any {
    const statusInt = req.status ? this.statusToInt(req.status) : undefined;
    return {
      name: req.name,
      description: req.description,
      manufacturerProductCode: req.manufacturerProductCode,
      gtin: req.gtin,
      upc: req.upc,
      sku: (req as any).sku,
      packSize: req.packSize,
      manufacturerId: req.manufacturerId,
      category: req.category,
      subCategory: req.subCategory,
      brand: (req as any).brand,
      tertiaryCategory: (req as any).tertiaryCategory,
      alwaysList: (req as any).alwaysList !== undefined ? (req as any).alwaysList : null,
      notes: (req as any).notes,
      status: statusInt,
      isActive: statusInt !== undefined ? (statusInt === 1) : undefined
    };
  }

  private statusToInt(status: string): number {
    const s = (status || '').toString().toLowerCase();
    if (s === 'active') return 1;
    if (s === 'inactive') return 2;
    if (s === 'pending') return 3;
    if (s === 'discontinued') return 4;
    return 1;
  }

  private handleError(error: any) {
    console.error('ProductService error:', error);
    return throwError(() => error);
  }

  // Alias methods for compatibility
  getAll(): Observable<Product[]> {
    return this.apiService.get<any>(this.endpoint).pipe(
      map((resp: any) => (resp.items || resp.Items || resp || []).map((dto: any) => this.mapDtoToProduct(dto)))
    );
  }

  // Get all active products (for dropdowns)
  getAllActive(): Observable<Product[]> {
    return this.getPaginated(1, 1000, 'name', 'asc', undefined, 'Active').pipe(
      map(res => res.items)
    );
  }

  // Get distinct brand names for autocomplete
  getBrands(): Observable<string[]> {
    return this.apiService.get<string[]>(`${this.endpoint}/brands`).pipe(
      catchError(this.handleError)
    );
  }
}
