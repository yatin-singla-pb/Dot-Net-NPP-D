import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from './api.service';
import {
  DistributorProductCode,
  CreateDistributorProductCodeRequest,
  UpdateDistributorProductCodeRequest,
  DistributorProductCodeSearchResult
} from '../models/distributor-product-code.model';

@Injectable({ providedIn: 'root' })
export class DistributorProductCodeService {
  private readonly endpoint = 'distributor-product-codes';

  constructor(private api: ApiService) {}

  getPaginated(
    pageNumber = 1,
    pageSize = 10,
    sortBy?: string,
    sortDirection: 'asc' | 'desc' = 'asc',
    searchTerm?: string,
    distributorIds?: number[],
    productIds?: number[],
    productStatus?: number
  ): Observable<DistributorProductCodeSearchResult> {
    const params: any = { page: pageNumber, pageSize, sortDirection };
    if (sortBy) params.sortBy = sortBy;
    if (searchTerm) params.searchTerm = searchTerm;
    if (Array.isArray(distributorIds) && distributorIds.length) params.distributorIds = distributorIds.join(',');
    if (Array.isArray(productIds) && productIds.length) params.productIds = productIds.join(',');
    if (typeof productStatus === 'number') params.productStatus = productStatus;

    return this.api.get<any>(this.endpoint, params).pipe(
      map(resp => ({
        items: resp.items ?? [],
        totalCount: resp.totalCount ?? 0,
        pageNumber: resp.pageNumber ?? pageNumber,
        pageSize: resp.pageSize ?? pageSize,
        totalPages: resp.totalPages ?? 0
      })),
      catchError(err => throwError(() => err))
    );
  }

  getById(id: number): Observable<DistributorProductCode> {
    return this.api.get<DistributorProductCode>(`${this.endpoint}/${id}`);
  }

  create(req: CreateDistributorProductCodeRequest): Observable<DistributorProductCode> {
    const payload = { ...req, distributorCode: (req.distributorCode || '').trim() };
    return this.api.post<DistributorProductCode>(this.endpoint, payload);
  }

  update(id: number, req: UpdateDistributorProductCodeRequest): Observable<DistributorProductCode> {
    const payload: any = { ...req };
    if (payload.distributorCode !== undefined && payload.distributorCode !== null) {
      payload.distributorCode = String(payload.distributorCode).trim();
    }
    return this.api.put<DistributorProductCode>(`${this.endpoint}/${id}`, payload);
  }

  delete(id: number): Observable<any> {
    return this.api.delete(`${this.endpoint}/${id}`);
  }
}

