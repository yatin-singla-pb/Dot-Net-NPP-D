import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppConfigService } from '../config/app.config.service';

export interface ProposalProduct {
  productId: number;
  priceTypeId?: number | null;
  quantity?: number | null;
  metaJson?: string | null;
  productProposalStatusId?: number | null;
  // Pricing fields
  uom?: string | null; // Cases | Pounds
  billbacksAllowed?: boolean;
  allowance?: number | null;
  commercialDelPrice?: number | null;
  commercialFobPrice?: number | null;
  commodityDelPrice?: number | null;
  commodityFobPrice?: number | null;
  // Additional fields
  pua?: number | null;
  ffsPrice?: number | null;
  noiPrice?: boolean | null;
  ptv?: number | null;
  internalNotes?: string | null;
  manufacturerNotes?: string | null;
}

export interface Proposal {
  id: number;
  title: string;
  proposalTypeId: number;
  proposalStatusId: number;
  proposalStatusName?: string | null;
  manufacturerId?: number | null;
  startDate?: string | null;
  endDate?: string | null;
  dueDate?: string | null;
  internalNotes?: string | null;
  rejectReason?: string | null;
  isActive: boolean;
  createdDate?: Date;
  modifiedDate?: Date;
  createdBy?: string | null;
  modifiedBy?: string | null;
  products: ProposalProduct[];
  distributorIds: number[];
  industryIds: number[];
  opcoIds: number[];
}

export interface ProposalCreateDto {
  title: string;
  proposalTypeId: number;
  proposalStatusId: number;
  manufacturerId?: number | null;
  startDate?: string | null;
  endDate?: string | null;
  dueDate?: string | null;
  internalNotes?: string | null;
  products: ProposalProductCreateDto[];
  distributorIds: number[];
  industryIds: number[];
  opcoIds: number[];
  amendedContractId?: number | null;
}

export interface ProposalProductCreateDto {
  productId: number;
  priceTypeId?: number | null;
  quantity?: number | null;
  productProposalStatusId?: number | null;
  amendmentActionId?: number | null;
  // Pricing and packing fields used in UI/deduplication
  proposedPrice?: number | null;
  packingList?: string | null;
  metaJson?: string | null;
  // Pricing fields aligned with backend DTO
  uom?: string | null;
  billbacksAllowed?: boolean;
  allowance?: number | null;
  commercialDelPrice?: number | null;
  commercialFobPrice?: number | null;
  commodityDelPrice?: number | null;
  commodityFobPrice?: number | null;
  // Additional fields
  pua?: number | null;
  ffsPrice?: number | null;
  noiPrice?: boolean | null;
  ptv?: number | null;
  internalNotes?: string | null;
  manufacturerNotes?: string | null;
}

export interface ProposalType {
  id: number;
  name: string;
  isActive: boolean;
}

export interface ProposalStatus {
  id: number;
  name: string;
  isActive: boolean;
}

export interface PriceType {
  id: number;
  name: string;
  isActive: boolean;
}

export interface ProductProposalStatus {
  id: number;
  name: string;
  isActive: boolean;
}

export interface ProposalStatusHistory {
  changedDate: string;
  changedBy?: string | null;
  fromStatus?: string | null;
  toStatus?: string | null;
  comment?: string | null;
}


export interface ProposalProductEditHistory {
  id: number;
  productId: number;
  productName?: string | null;
  changeType: string;
  previousJson?: string | null;
  currentJson?: string | null;
  changedDate: string;
  changedBy?: string | null;
}

export interface OpCoConflict {
  opCoId: number;
  opCoName: string;
}

export interface ProductConflict {
  productId: number;
  productName: string;
  manufacturerProductCode?: string | null;
  conflictingContractId: number;
  conflictingContractName: string;
  conflictingContractVersionNumber: number;
  conflictingContractForeignId?: string | null;
  conflictingManufacturerName?: string | null;
  overlappingOpCos: OpCoConflict[];
  isNationwideConflict: boolean;
  proposalStartDate: string;
  proposalEndDate: string;
  contractStartDate: string;
  contractEndDate: string;
  overlapStartDate: string;
  overlapEndDate: string;
}

export interface ProposalConflictResult {
  proposalId: number;
  hasConflicts: boolean;
  totalConflictCount: number;
  conflicts: ProductConflict[];
}


@Injectable({ providedIn: 'root' })
export class ProposalService {
  private readonly base: string;

  constructor(private http: HttpClient, private api: AppConfigService) {
    this.base = `${this.api.apiUrl}/v1/proposals`;
  }

  /**
   * Get proposals by status name
   */
  getByStatus(
    statusName: string,
    limit: number = 5
  ): Observable<any[]> {
    // Get paginated results and filter by status name
    return this.getPaginated(1, limit).pipe(
      map((response: any) => {
        const proposals = response?.data || [];
        // Filter by status name if provided
        if (statusName) {
          return proposals.filter((p: any) =>
            p.proposalStatusName?.toLowerCase() === statusName.toLowerCase()
          );
        }
        return proposals;
      })
    );
  }

  /**
   * Get proposals by status name for a specific manufacturer
   */
  getByStatusAndManufacturer(
    statusName: string,
    manufacturerId: number,
    limit: number = 5,
    daysBack?: number
  ): Observable<Proposal[]> {
    let opts: any = {
      manufacturerId: manufacturerId
    };

    // If daysBack is specified, filter by created date
    if (daysBack) {
      const fromDate = new Date();
      fromDate.setDate(fromDate.getDate() - daysBack);
      opts.createdDateFrom = fromDate.toISOString().split('T')[0];
    }

    // Get paginated results and filter by status name
    return this.getPaginated(1, limit, undefined, opts).pipe(
      map((response: any) => {
        const proposals = response?.data || [];
        // Filter by status name if provided
        if (statusName) {
          return proposals.filter((p: Proposal) =>
            p.proposalStatusName?.toLowerCase() === statusName.toLowerCase()
          );
        }
        return proposals;
      })
    );
  }

  getPaginated(
    page: number,
    pageSize: number,
    search?: string,
    opts?: {
      proposalStatusId?: number;
      proposalTypeId?: number;
      manufacturerId?: number;
      startDateFrom?: string;
      startDateTo?: string;
      endDateFrom?: string;
      endDateTo?: string;
      createdDateFrom?: string;
      createdDateTo?: string;
      idFrom?: number;
      idTo?: number;
      sortBy?: string;
      sortDirection?: 'asc' | 'desc';
    }
  ): Observable<any> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    if (opts) {
      if (opts.proposalStatusId != null) params = params.set('proposalStatusId', String(opts.proposalStatusId));
      if (opts.proposalTypeId != null) params = params.set('proposalTypeId', String(opts.proposalTypeId));
      if (opts.manufacturerId != null) params = params.set('manufacturerId', String(opts.manufacturerId));
      if (opts.startDateFrom) params = params.set('startDateFrom', opts.startDateFrom);
      if (opts.startDateTo) params = params.set('startDateTo', opts.startDateTo);
      if (opts.endDateFrom) params = params.set('endDateFrom', opts.endDateFrom);
      if (opts.endDateTo) params = params.set('endDateTo', opts.endDateTo);
      if (opts.createdDateFrom) params = params.set('createdDateFrom', opts.createdDateFrom);
      if (opts.createdDateTo) params = params.set('createdDateTo', opts.createdDateTo);
      if (opts.idFrom != null) params = params.set('idFrom', String(opts.idFrom));
      if (opts.idTo != null) params = params.set('idTo', String(opts.idTo));
      if (opts.sortBy) params = params.set('sortBy', opts.sortBy);
      if (opts.sortDirection) params = params.set('sortDirection', opts.sortDirection);
    }

    return this.http.get<any>(`${this.base}`, { params }).pipe(
      map(resp => {
        const data = Array.isArray(resp?.data) ? resp.data : [];
        return {
          ...resp,
          data: data.map((dto: any) => this.mapDtoToProposal(dto))
        };
      })
    );
  }

  getById(id: number): Observable<Proposal> {
    return this.http.get<any>(`${this.base}/${id}`).pipe(
      map(dto => this.mapDtoToProposal(dto))
    );
  }

  create(payload: ProposalCreateDto): Observable<Proposal> {
    return this.http.post<any>(`${this.base}`, payload).pipe(
      map(dto => this.mapDtoToProposal(dto))
    );
  }

  update(id: number, payload: ProposalCreateDto): Observable<Proposal> {
    return this.http.put<any>(`${this.base}/${id}`, payload).pipe(
      map(dto => this.mapDtoToProposal(dto))
    );
  }

  getProductEditHistory(id: number): Observable<ProposalProductEditHistory[]> {
    return this.http.get<ProposalProductEditHistory[]>(`${this.base}/${id}/product-history`);
  }


  clone(id: number): Observable<Proposal> {
    return this.http.post<any>(`${this.base}/${id}/clone`, {}).pipe(
      map(dto => this.mapDtoToProposal(dto))
    );
  }

  submit(id: number): Observable<boolean> {
    return this.http.post<boolean>(`${this.base}/${id}/submit`, {});
  }

  getStatusHistory(id: number): Observable<ProposalStatusHistory[]> {
    return this.http.get<ProposalStatusHistory[]>(`${this.base}/${id}/history`);
  }

  accept(id: number): Observable<boolean> {
    return this.http.post<boolean>(`${this.base}/${id}/accept`, {});
  }

  reject(id: number, payload: { reason?: string | null }): Observable<boolean> {
    return this.http.post<boolean>(`${this.base}/${id}/reject`, payload || {});
  }

  getConflicts(id: number): Observable<ProposalConflictResult> {
    return this.http.get<ProposalConflictResult>(`${this.base}/${id}/conflicts`);
  }

  exportProducts(proposalId: number): Observable<Blob> {
    return this.http.get(`${this.base}/${proposalId}/products/export`, { responseType: 'blob' });
  }

  downloadTemplate(manufacturerId: number): Observable<Blob> {
    return this.http.get(`${this.base}/products/excel-template/${manufacturerId}`, { responseType: 'blob' });
  }

  importExcel(manufacturerId: number, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${this.base}/products/excel-import/${manufacturerId}`, formData);
  }

  batch(proposals: ProposalCreateDto[]): Observable<number> {
    return this.http.post<number>(`${this.base}/batch`, proposals);
  }

  private mapDtoToProposal(dto: any): Proposal {
    const productsRaw = dto.Products ?? dto.products ?? [];

    const products: ProposalProduct[] = Array.isArray(productsRaw)
      ? productsRaw.map((p: any) => ({
          productId: p.ProductId ?? p.productId ?? p.Id ?? p.id,
          priceTypeId: p.PriceTypeId ?? p.priceTypeId ?? null,
          quantity: p.Quantity ?? p.quantity ?? null,
          metaJson: p.MetaJson ?? p.metaJson ?? null,
          productProposalStatusId: p.ProductProposalStatusId ?? p.productProposalStatusId ?? null,
          uom: p.Uom ?? p.uom ?? null,
          billbacksAllowed: (p.BillbacksAllowed ?? p.billbacksAllowed) === true,
          allowance: p.Allowance ?? p.allowance ?? null,
          commercialDelPrice: p.CommercialDelPrice ?? p.commercialDelPrice ?? null,
          commercialFobPrice: p.CommercialFobPrice ?? p.commercialFobPrice ?? null,
          commodityDelPrice: p.CommodityDelPrice ?? p.commodityDelPrice ?? null,
          commodityFobPrice: p.CommodityFobPrice ?? p.commodityFobPrice ?? null,
          pua: p.Pua ?? p.pua ?? null,
          ffsPrice: p.FfsPrice ?? p.ffsPrice ?? null,
          noiPrice: p.NoiPrice ?? p.noiPrice ?? null,
          ptv: p.Ptv ?? p.ptv ?? null,
          internalNotes: p.InternalNotes ?? p.internalNotes ?? null,
          manufacturerNotes: p.ManufacturerNotes ?? p.manufacturerNotes ?? null
        }))
      : [];

    const distributorIds = (dto.DistributorIds ?? dto.distributorIds ?? [])
      .filter((x: any) => x !== null && x !== undefined)
      .map((x: any) => Number(x));

    const industryIds = (dto.IndustryIds ?? dto.industryIds ?? [])
      .filter((x: any) => x !== null && x !== undefined)
      .map((x: any) => Number(x));

    const opcoIds = (dto.OpCoIds ?? dto.OpcoIds ?? dto.opCoIds ?? dto.opcoIds ?? [])
      .filter((x: any) => x !== null && x !== undefined)
      .map((x: any) => Number(x));

    const proposal: Proposal = {
      id: dto.Id ?? dto.id,
      title: dto.Title ?? dto.title ?? '',
      proposalTypeId: dto.ProposalTypeId ?? dto.proposalTypeId ?? 0,
      proposalStatusId: dto.ProposalStatusId ?? dto.proposalStatusId ?? 0,
      proposalStatusName: dto.ProposalStatusName ?? dto.proposalStatusName ?? null,
      manufacturerId: dto.ManufacturerId ?? dto.manufacturerId ?? null,
      startDate: dto.StartDate ?? dto.startDate ?? null,
      endDate: dto.EndDate ?? dto.endDate ?? null,
      internalNotes: dto.InternalNotes ?? dto.internalNotes ?? null,
      rejectReason: dto.RejectReason ?? dto.rejectReason ?? null,
      isActive: (dto.IsActive ?? dto.isActive) !== false,
      createdDate: dto.CreatedDate || dto.createdDate ? new Date(dto.CreatedDate ?? dto.createdDate) : undefined,
      modifiedDate: dto.ModifiedDate || dto.modifiedDate ? new Date(dto.ModifiedDate ?? dto.modifiedDate) : undefined,
      createdBy: dto.CreatedBy ?? dto.createdBy ?? null,
      modifiedBy: dto.ModifiedBy ?? dto.modifiedBy ?? null,
      products,
      distributorIds,
      industryIds,
      opcoIds
    };

    return proposal;
  }


  // Lookup data methods
  getProposalTypes(): Observable<ProposalType[]> {
    return this.http.get<ProposalType[]>(`${this.api.apiUrl}/v1/lookup/proposal-types`);
  }

  getProposalStatuses(): Observable<ProposalStatus[]> {
    return this.http.get<ProposalStatus[]>(`${this.api.apiUrl}/v1/lookup/proposal-statuses`);
  }

  getPriceTypes(): Observable<PriceType[]> {
    return this.http.get<PriceType[]>(`${this.api.apiUrl}/v1/lookup/price-types`);
  }

  getProductProposalStatuses(): Observable<ProductProposalStatus[]> {
    return this.http.get<ProductProposalStatus[]>(`${this.api.apiUrl}/v1/lookup/product-proposal-statuses`);
  }

  // Anonymous lookups for dropdowns
  getManufacturers(): Observable<Array<{ id: number; name: string }>> {
    return this.http.get<Array<{ id: number; name: string }>>(`${this.api.apiUrl}/v1/lookup/manufacturers`);
  }

  getDistributors(): Observable<Array<{ id: number; name: string }>> {
    return this.http.get<Array<{ id: number; name: string }>>(`${this.api.apiUrl}/v1/lookup/distributors`);
  }

  getIndustries(): Observable<Array<{ id: number; name: string }>> {
    return this.http.get<Array<{ id: number; name: string }>>(`${this.api.apiUrl}/v1/lookup/industries`);
  }

  getOpCos(): Observable<Array<{ id: number; name: string; distributorId: number }>> {
    return this.http.get<Array<{ id: number; name: string; distributorId: number }>>(`${this.api.apiUrl}/v1/lookup/opcos`);
  }

  getProductsByManufacturers(manufacturerIds: number[]): Observable<Array<{ id: number; name: string; productName?: string; description?: string; manufacturerId: number; manufacturerProductCode: string; packSize: string; brand?: string; gtin?: string; notes?: string; status?: string; isActive: boolean }>> {
    return this.http.post<Array<{ id: number; name: string; productName?: string; description?: string; manufacturerId: number; manufacturerProductCode: string; packSize: string; brand?: string; gtin?: string; notes?: string; status?: string; isActive: boolean }>>(
      `${this.api.apiUrl}/v1/lookup/products/by-manufacturers`,
      manufacturerIds || []
    );
  }

  getOpCosByDistributors(distributorIds: number[]): Observable<Array<{ id: number; name: string; distributorId: number }>> {
    return this.http.post<Array<{ id: number; name: string; distributorId: number }>>(
      `${this.api.apiUrl}/v1/lookup/opcos/by-distributors`,
      distributorIds || []
    );
  }
}
