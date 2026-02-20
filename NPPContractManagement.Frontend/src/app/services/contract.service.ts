import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError, of, forkJoin } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Contract, CreateContractRequest, UpdateContractRequest, ContractStatus } from '../models/contract.model';
import { ContractVersion, CreateContractVersionRequest, ContractVersionPrice } from '../models/contract-version.model';
import { ApiService } from './api.service';

import { ContractDistributorAssignment, CreateContractDistributorAssignmentRequest, ContractManufacturerAssignment, CreateContractManufacturerAssignmentRequest, ContractOpCoAssignment, CreateContractOpCoAssignmentRequest, ContractIndustryAssignment, CreateContractIndustryAssignmentRequest } from '../models/contract-assignments.model';

@Injectable({
  providedIn: 'root'
})
export class ContractService {
  private readonly endpoint = 'contracts';

  // Versions API
  getVersions(contractId: number): Observable<ContractVersion[]> {
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/versions`)
      .pipe(map(list => (list || []).map(v => this.mapVersionDto(v))), catchError(this.handleError));
  }

  getVersion(contractId: number, versionId: number): Observable<ContractVersion> {
    return this.http.get<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/versions/${versionId}`)
      .pipe(map(v => this.mapVersionDto(v)), catchError(this.handleError));
  }

  createVersion(contractId: number, req: CreateContractVersionRequest): Observable<ContractVersion> {
    const payload = this.toCreateVersionDto(req);
    return this.http.post<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/versions`, payload)
      .pipe(map(v => this.mapVersionDto(v)), catchError(this.handleError));
  }

  constructor(
    private http: HttpClient,
    private apiService: ApiService
  ) { }

  // Convenience helpers for edit screen
  getLatestVersion(contractId: number): Observable<ContractVersion | null> {
    return this.getVersions(contractId).pipe(
      map(list => {
        const versions = list || [];
        if (!versions.length) return null;
        return versions.reduce((max, v) => v.versionNumber > (max?.versionNumber ?? 0) ? v : max, versions[0]);
      })
    );
  }

  getContractPricing(contractId: number): Observable<ContractVersionPrice[]> {
    return this.getLatestVersion(contractId).pipe(map(v => (v?.prices || [])));
  }

  getContractProducts(contractId: number): Observable<{ id: number; name: string }[]> {
    return this.getLatestVersion(contractId).pipe(
      map(v => {
        const prices = v?.prices || [];
        const mapPN = new Map<number, string>();
        prices.forEach(p => {
          if (p.productId) mapPN.set(p.productId, p.productName || String(p.productId));
        });
        return Array.from(mapPN.entries()).map(([id, name]) => ({ id, name }));
      })
    );
  }


  // Version utilities
  cloneVersion(contractId: number, versionNo: number): Observable<ContractVersion> {
    return this.http.post<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/cloneVersion/${versionNo}`, {})
      .pipe(map(v => this.mapVersionDto(v)), catchError(this.handleError));
  }

  compareVersions(contractId: number, versionA: number, versionB: number): Observable<any> {
    const params = new HttpParams().set('contractId', contractId).set('versionA', versionA).set('versionB', versionB);
    return this.http.get<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/compare`, { params })
      .pipe(catchError(this.handleError));
  }

  /**
   * Get all contracts
   */
  // Historical prices by product (for review/awards UI)
  getPricesByProduct(productId: number): Observable<any[]> {
    const params = new HttpParams().set('productId', String(productId));
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/contract-prices`, { params })
      .pipe(catchError(this.handleError));
  }


  getAll(): Observable<Contract[]> {
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}`)
      .pipe(
        map(list => (list || []).map(dto => this.mapDtoToContract(dto))),
        catchError(this.handleError)
      );
  }

  /**
   * Get contracts with pagination and filtering
   */
  getPaginated(
    page: number = 1,
    pageSize: number = 12,
    search?: string,
    status?: string,
    manufacturerId?: number,
    industryId?: number,
    startDate?: string | Date,
    endDate?: string | Date,
    sortBy?: string,
    sortDirection?: 'asc' | 'desc',
    isSuspended?: boolean
  ): Observable<{ items: Contract[], totalCount: number }> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (search) params = params.set('searchTerm', search);
    if (status) {
      const statusInt = this.toStatusInt(status);
      if (statusInt !== null) params = params.set('status', statusInt.toString());
    }
    if (isSuspended !== undefined) params = params.set('isSuspended', String(isSuspended));
    if (manufacturerId) params = params.set('manufacturerId', manufacturerId.toString());
    if (industryId) params = params.set('industryId', industryId.toString());
    if (startDate) {
      const sd = typeof startDate === 'string' ? startDate : startDate.toISOString().slice(0,10);
      params = params.set('startDate', sd);
    }
    if (endDate) {
      const ed = typeof endDate === 'string' ? endDate : endDate.toISOString().slice(0,10);
      params = params.set('endDate', ed);
    }
    // Backend currently sorts by CreatedDate desc; sort parameters are ignored for now

    return this.http.get<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/search`,
      { params }
    ).pipe(
      map(resp => {
        const items = (resp?.data || []).map((dto: any) => this.mapDtoToContract(dto));
        return {
          items,
          totalCount: resp?.totalCount ?? items.length
        };
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Get expiring contracts without proposals
   */
  getExpiringWithoutProposals(daysThreshold: number = 90): Observable<Contract[]> {
    const params = new HttpParams().set('daysThreshold', daysThreshold.toString());
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/expiring-without-proposals`, { params })
      .pipe(
        map(list => (list || []).map(dto => this.mapDtoToContract(dto))),
        catchError(this.handleError)
      );
  }

  /**
   * Get contract by ID
   */
  getById(id: number): Observable<Contract> {
    return this.http.get<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`)
      .pipe(
        map(dto => this.mapDtoToContract(dto)),
        catchError(this.handleError)
      );
  }

  /**
   * Create new contract
   */
  create(contract: CreateContractRequest): Observable<Contract> {
    const payload = this.toCreateDto(contract);
    return this.http.post<Contract>(`${this.apiService.getApiUrl()}/${this.endpoint}`, payload)
      .pipe(catchError(this.handleError));
  }

  /**
   * Update existing contract
   */
  update(id: number, contract: UpdateContractRequest): Observable<Contract> {
    const payload = this.toUpdateDto(contract);
    return this.http.put<Contract>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`, payload)
      .pipe(catchError(this.handleError));
  }

  /**
   * Delete contract
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Get contracts by manufacturer
   */
  getByManufacturer(manufacturerId: number): Observable<Contract[]> {
    return this.http.get<Contract[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/manufacturer/${manufacturerId}`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Get active contracts only
   */
  getActive(): Observable<Contract[]> {
    return this.http.get<Contract[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/active`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Get expiring contracts
   */
  getExpiring(daysThreshold: number = 30): Observable<Contract[]> {
    const params = new HttpParams().set('daysThreshold', daysThreshold.toString());
    return this.http.get<Contract[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/expiring`, { params })
      .pipe(catchError(this.handleError));
  }

  /**
   * Search contracts
   */
  search(query: string): Observable<Contract[]> {
    const params = new HttpParams().set('q', query);
    return this.http.get<Contract[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/search`, { params })
      .pipe(catchError(this.handleError));
  }

  /**
   * Activate contract
   */
  activate(id: number): Observable<Contract> {
    return this.http.patch<Contract>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/activate`, {})
      .pipe(catchError(this.handleError));
  }

  /**
   * Suspend contract
   */
  suspend(id: number, reason?: string): Observable<Contract> {
    const body = reason ? { reason } : {};
    return this.http.patch<Contract>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/suspend`, body)
      .pipe(catchError(this.handleError));
  }
  /**
   * Unsuspend contract
   */
  unsuspend(id: number, reason?: string): Observable<Contract> {
    const body = reason ? { reason } : {};
    return this.http.patch<Contract>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/unsuspend`, body)
      .pipe(catchError(this.handleError));
  }


  /**
   * Terminate contract
   */
  terminate(id: number, reason?: string): Observable<Contract> {
    const body = reason ? { reason } : {};
    return this.http.patch<Contract>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/terminate`, body)
      .pipe(catchError(this.handleError));
  }

  /**
   * Renew contract
   */
  renew(id: number, newEndDate: Date): Observable<Contract> {
    return this.http.patch<Contract>(`${this.apiService.getApiUrl()}/${this.endpoint}/${id}/renew`, {
      newEndDate: newEndDate.toISOString()
    }).pipe(catchError(this.handleError));
  }

  /**
   * Bulk delete contracts
   */
  bulkDelete(ids: number[]): Observable<void> {
    return this.http.request<void>('delete', `${this.apiService.getApiUrl()}/${this.endpoint}/bulk`, {
      body: { ids }
    }).pipe(catchError(this.handleError));
  }

  /**
   * Export contracts to Excel
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


  private mapDtoToContract(dto: any): Contract {
    const contract: Contract = {
      id: dto.Id ?? dto.id,
      // New schema
      name: dto.Name ?? dto.name ?? null,
      // Manufacturer (legacy fields for backward compatibility)
      manufacturerId: (dto.ManufacturerId ?? dto.manufacturerId) !== undefined ? Number(dto.ManufacturerId ?? dto.manufacturerId) : undefined,
      manufacturerName: dto.ManufacturerName ?? dto.manufacturerName ?? null,
      // External refs
      foreignContractId: dto.ForeignContractId ?? dto.foreignContractId ?? null,
      suspendedDate: dto.SuspendedDate ? new Date(dto.SuspendedDate) : (dto.suspendedDate ? new Date(dto.suspendedDate) : undefined),
      startDate: new Date(dto.StartDate ?? dto.startDate),
      endDate: new Date(dto.EndDate ?? dto.endDate),
      // Notes
      internalNotes: dto.InternalNotes ?? dto.internalNotes ?? null,
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: dto.ManufacturerReferenceNumber ?? dto.manufacturerReferenceNumber ?? null,
      manufacturerBillbackName: dto.ManufacturerBillbackName ?? dto.manufacturerBillbackName ?? null,
      manufacturerTermsAndConditions: dto.ManufacturerTermsAndConditions ?? dto.manufacturerTermsAndConditions ?? null,
      manufacturerNotes: dto.ManufacturerNotes ?? dto.manufacturerNotes ?? null,
      contactPerson: dto.ContactPerson ?? dto.contactPerson ?? null,
      entegraContractType: dto.EntegraContractType ?? dto.entegraContractType ?? null,
      entegraVdaProgram: dto.EntegraVdaProgram ?? dto.entegraVdaProgram ?? null,
      isSuspended: (dto.IsSuspended ?? dto.isSuspended) === true,
      sendToPerformance: (dto.SendToPerformance ?? dto.sendToPerformance) === true,
      currentVersionNumber: dto.CurrentVersionNumber ?? dto.currentVersionNumber ?? 1,
      isActive: true,
      createdDate: new Date(dto.CreatedDate ?? dto.createdDate ?? Date.now()),
      modifiedDate: dto.ModifiedDate || dto.modifiedDate ? new Date(dto.ModifiedDate ?? dto.modifiedDate) : undefined,
      createdBy: dto.CreatedBy ?? dto.createdBy,
      modifiedBy: dto.ModifiedBy ?? dto.modifiedBy,
      proposalId: (dto.ProposalId ?? dto.proposalId) !== undefined ? Number(dto.ProposalId ?? dto.proposalId) : undefined,
      proposalTitle: dto.ProposalTitle ?? dto.proposalTitle ?? undefined,

    } as Contract;

    const distList = dto.Distributors ?? dto.distributors ?? [];
    if (Array.isArray(distList) && distList.length) {
      (contract as any).distributors = distList.map((d: any) => ({
        contractId: contract.id,
        distributorId: d.Id ?? d.id ?? d.DistributorId ?? d.distributorId,
        distributorName: d.Name ?? d.name ?? d.DistributorName ?? d.distributorName,
        createdDate: new Date(dto.CreatedDate ?? dto.createdDate ?? Date.now()),
        createdBy: dto.CreatedBy ?? dto.createdBy
      }));
    }

    const opcoList = dto.OpCos ?? dto.opCos ?? dto.opcos ?? [];
    if (Array.isArray(opcoList) && opcoList.length) {
      (contract as any).opCos = opcoList.map((o: any) => ({
        contractId: contract.id,
        opCoId: o.Id ?? o.id ?? o.OpCoId ?? o.opCoId,
        opCoName: o.Name ?? o.name ?? o.OpCoName ?? o.opCoName,
        createdDate: new Date(dto.CreatedDate ?? dto.createdDate ?? Date.now()),
        createdBy: dto.CreatedBy ?? dto.createdBy
      }));
    }

    const indList = dto.Industries ?? dto.industries ?? [];
    if (Array.isArray(indList) && indList.length) {
      (contract as any).industries = indList.map((i: any) => ({
        contractId: contract.id,
        industryId: i.Id ?? i.id ?? i.IndustryId ?? i.industryId,
        industryName: i.Name ?? i.name ?? i.IndustryName ?? i.industryName,
        createdDate: new Date(dto.CreatedDate ?? dto.createdDate ?? Date.now()),
        createdBy: dto.CreatedBy ?? dto.createdBy
      }));
    }

    const cpList = dto.ContractProducts ?? dto.contractProducts ?? dto.Products ?? dto.products ?? [];
    if (Array.isArray(cpList) && cpList.length) {
      (contract as any).contractProducts = cpList.map((p: any) => ({
        productId: p.ProductId ?? p.productId ?? p.Id ?? p.id,
        productName: p.ProductName ?? p.productName ?? p.Name ?? p.name
      }));
    }

    return contract;
  }

  private emptyToNull<T>(v: any): any {
    return v === '' || v === undefined ? null : v;
  }

  private toCreateDto(req: CreateContractRequest): any {
    return {
      name: this.emptyToNull(req.name),
      startDate: req.startDate ? new Date(req.startDate).toISOString() : null,
      endDate: req.endDate ? new Date(req.endDate).toISOString() : null,
      internalNotes: this.emptyToNull(req.internalNotes),
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: this.emptyToNull(req.manufacturerReferenceNumber),
      manufacturerBillbackName: this.emptyToNull(req.manufacturerBillbackName),
      manufacturerTermsAndConditions: this.emptyToNull(req.manufacturerTermsAndConditions),
      manufacturerNotes: this.emptyToNull(req.manufacturerNotes),
      contactPerson: this.emptyToNull(req.contactPerson),
      entegraContractType: this.emptyToNull(req.entegraContractType),
      entegraVdaProgram: this.emptyToNull(req.entegraVdaProgram),
      proposalId: (req.proposalId !== undefined ? Number(req.proposalId) : null),

      foreignContractId: this.emptyToNull(req.foreignContractId),
      sendToPerformance: !!req.sendToPerformance,
      distributorIds: Array.from(new Set((req.distributorIds ?? []).filter((x: any) => x !== null && x !== undefined))).map(Number),
      opCoIds: Array.from(new Set((req.opCoIds ?? []).filter((x: any) => x !== null && x !== undefined))).map(Number),
      industryIds: Array.from(new Set((req.industryIds ?? []).filter((id: any) => id !== null && id !== undefined))).map(Number),
      productIds: Array.from(new Set((req.productIds ?? []).filter((x: any) => x !== null && x !== undefined))).map(Number),
      prices: (req.prices ?? []).map(p => ({
        versionNumber: p.versionNumber ?? undefined,
        productId: Number(p.productId),
        priceType: p.priceType,
        allowance: p.allowance ?? null,
        commercialDelPrice: p.commercialDelPrice ?? null,
        commercialFobPrice: p.commercialFobPrice ?? null,
        commodityDelPrice: p.commodityDelPrice ?? null,
        commodityFobPrice: p.commodityFobPrice ?? null,
        uom: p.uom,
        estimatedQty: p.estimatedQty ?? null,
        billbacksAllowed: !!p.billbacksAllowed,
        pua: p.pua ?? null,
        ffsPrice: p.ffsPrice ?? null,
        noiPrice: p.noiPrice ?? null,
        ptv: p.ptv ?? null,
        internalNotes: p.internalNotes ?? null
      }))
    };
  }

  private toUpdateDto(req: UpdateContractRequest): any {
    const dto: any = {};
    // New schema
    if (req.name !== undefined) dto.name = this.emptyToNull(req.name);
    // Legacy
    if (req.title !== undefined) dto.title = this.emptyToNull(req.title ?? req.name);
    if (req.manufacturerId !== undefined) dto.manufacturerId = req.manufacturerId;
    if (req.startDate !== undefined) dto.startDate = req.startDate ? new Date(req.startDate).toISOString() : null;
    if (req.endDate !== undefined) dto.endDate = req.endDate ? new Date(req.endDate).toISOString() : null;
    // Notes
    if (req.internalNotes !== undefined) dto.internalNotes = this.emptyToNull(req.internalNotes);
    if (req.notes !== undefined) dto.notes = this.emptyToNull(req.notes ?? req.internalNotes);
    // Manufacturer/Entegra metadata
    if (req.manufacturerReferenceNumber !== undefined) dto.manufacturerReferenceNumber = this.emptyToNull(req.manufacturerReferenceNumber);
    if (req.manufacturerBillbackName !== undefined) dto.manufacturerBillbackName = this.emptyToNull(req.manufacturerBillbackName);
    if (req.manufacturerTermsAndConditions !== undefined) dto.manufacturerTermsAndConditions = this.emptyToNull(req.manufacturerTermsAndConditions);
    if (req.manufacturerNotes !== undefined) dto.manufacturerNotes = this.emptyToNull(req.manufacturerNotes);
    if (req.contactPerson !== undefined) dto.contactPerson = this.emptyToNull(req.contactPerson);
    if (req.entegraContractType !== undefined) dto.entegraContractType = this.emptyToNull(req.entegraContractType);
    if (req.entegraVdaProgram !== undefined) dto.entegraVdaProgram = this.emptyToNull(req.entegraVdaProgram);
    // External refs
    if (req.foreignContractId !== undefined) dto.foreignContractId = this.emptyToNull(req.foreignContractId);
    if (req.foreignContractID !== undefined) dto.foreignContractID = this.emptyToNull(req.foreignContractID ?? req.foreignContractId);

    if (req.sendToPerformance !== undefined) dto.sendToPerformance = !!req.sendToPerformance;

    dto.distributorIds = Array.from(new Set((req.distributorIds ?? []).filter((x: any) => x !== null && x !== undefined))).map(Number);
    dto.opCoIds = Array.from(new Set((req.opCoIds ?? []).filter((x: any) => x !== null && x !== undefined))).map(Number);
    dto.industryIds = Array.from(new Set((req.industryIds ?? []).filter((x: any) => x !== null && x !== undefined))).map(Number);

    return dto;
  }


  private toStatusInt(status?: string): number | null {
    if (!status) return null;
    const s = status.toLowerCase();
    switch (s) {
      case 'draft': return 1;
      case 'pending': return 2;
      case 'active': return 3;
      case 'expired': return 4;
      case 'terminated': return 5;
      case 'suspended': return 6;
      default: return null; // e.g., 'expiring', 'all'
    }
  }

  // Assignments API
  getDistributorAssignments(contractId: number): Observable<ContractDistributorAssignment[]> {
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/distributors`)
      .pipe(map(list => (list || []).map(x => this.mapAssignmentDates(x))), catchError(this.handleError));
  }

  addDistributorAssignment(contractId: number, req: CreateContractDistributorAssignmentRequest): Observable<ContractDistributorAssignment> {
    const payload = this.normalizeAssignmentDates(req);
    return this.http.post<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/distributors`, payload)
      .pipe(map(x => this.mapAssignmentDates(x)), catchError(this.handleError));
  }

  removeDistributorAssignment(contractId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/distributors/${id}`)
      .pipe(catchError(this.handleError));
  }

  getManufacturerAssignments(contractId: number): Observable<ContractManufacturerAssignment[]> {
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/manufacturers`)
      .pipe(map(list => (list || []).map(x => this.mapAssignmentDates(x))), catchError(this.handleError));
  }

  addManufacturerAssignment(contractId: number, req: CreateContractManufacturerAssignmentRequest): Observable<ContractManufacturerAssignment> {
    const payload = this.normalizeAssignmentDates(req);
    return this.http.post<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/manufacturers`, payload)
      .pipe(map(x => this.mapAssignmentDates(x)), catchError(this.handleError));
  }

  removeManufacturerAssignment(contractId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/manufacturers/${id}`)
      .pipe(catchError(this.handleError));
  }

  getOpCoAssignments(contractId: number): Observable<ContractOpCoAssignment[]> {
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/opcos`)
      .pipe(map(list => (list || []).map(x => this.mapAssignmentDates(x))), catchError(this.handleError));
  }

  addOpCoAssignment(contractId: number, req: CreateContractOpCoAssignmentRequest): Observable<ContractOpCoAssignment> {
    const payload = this.normalizeAssignmentDates(req);
    return this.http.post<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/opcos`, payload)
      .pipe(map(x => this.mapAssignmentDates(x)), catchError(this.handleError));
  }

  removeOpCoAssignment(contractId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/opcos/${id}`)
      .pipe(catchError(this.handleError));
  }

  getIndustryAssignments(contractId: number): Observable<ContractIndustryAssignment[]> {
    return this.http.get<any[]>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/industries`)
      .pipe(map(list => (list || []).map(x => this.mapAssignmentDates(x))), catchError(this.handleError));
  }

  addIndustryAssignment(contractId: number, req: CreateContractIndustryAssignmentRequest): Observable<ContractIndustryAssignment> {
    const payload = this.normalizeAssignmentDates(req);
    return this.http.post<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/industries`, payload)
      .pipe(map(x => this.mapAssignmentDates(x)), catchError(this.handleError));
  }

  removeIndustryAssignment(contractId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiService.getApiUrl()}/${this.endpoint}/${contractId}/assignments/industries/${id}`)
      .pipe(catchError(this.handleError));
  }

  private mapAssignmentDates<T extends { assignedDate?: any }>(obj: T): T {
    if (obj && obj.assignedDate) {
      (obj as any).assignedDate = new Date(obj.assignedDate);
    }
    return obj;
  }

  private normalizeAssignmentDates(obj: any): any {
    return {
      ...obj,
      assignedDate: obj.assignedDate ? new Date(obj.assignedDate).toISOString() : null
    };
  }



  /**
   * Get contract statistics
   */
  getStatistics(): Observable<{
    total: number;
    active: number;
    expired: number;
    expiringSoon: number;
    suspended: number;
    totalValue: number;
  }> {
    return this.http.get<{
      total: number;
      active: number;
      expired: number;
      expiringSoon: number;
      suspended: number;
      totalValue: number;
    }>(`${this.apiService.getApiUrl()}/${this.endpoint}/statistics`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Get dashboard statistics (real counts from database)
   */
  getDashboardStats(): Observable<any> {
    return this.http.get<any>(`${this.apiService.getApiUrl()}/${this.endpoint}/dashboard-stats`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Handle HTTP errors
   */
  private mapVersionDto(dto: any): ContractVersion {
    return {
      id: dto.Id ?? dto.id,
      contractId: dto.ContractId ?? dto.contractId,
      versionNumber: dto.VersionNumber ?? dto.versionNumber,
      name: dto.Name ?? dto.name ?? '',
      foreignContractId: dto.ForeignContractId ?? dto.foreignContractId ?? null,
      sendToPerformance: (dto.SendToPerformance ?? dto.sendToPerformance) === true,
      isSuspended: (dto.IsSuspended ?? dto.isSuspended) === true,
      suspendedDate: dto.SuspendedDate ? new Date(dto.SuspendedDate) : (dto.suspendedDate ? new Date(dto.suspendedDate) : null),
      internalNotes: dto.InternalNotes ?? dto.internalNotes ?? null,
      // Manufacturer/Entegra metadata snapshot
      manufacturerReferenceNumber: dto.ManufacturerReferenceNumber ?? dto.manufacturerReferenceNumber ?? null,
      manufacturerBillbackName: dto.ManufacturerBillbackName ?? dto.manufacturerBillbackName ?? null,
      manufacturerTermsAndConditions: dto.ManufacturerTermsAndConditions ?? dto.manufacturerTermsAndConditions ?? null,
      manufacturerNotes: dto.ManufacturerNotes ?? dto.manufacturerNotes ?? null,
      contactPerson: dto.ContactPerson ?? dto.contactPerson ?? null,
      entegraContractType: dto.EntegraContractType ?? dto.entegraContractType ?? null,
      entegraVdaProgram: dto.EntegraVdaProgram ?? dto.entegraVdaProgram ?? null,
      startDate: new Date(dto.StartDate ?? dto.startDate),
      endDate: new Date(dto.EndDate ?? dto.endDate),
      createdDate: new Date(dto.CreatedDate ?? dto.createdDate ?? Date.now()),
      createdBy: dto.CreatedBy ?? dto.createdBy ?? null,
      prices: (dto.Prices ?? dto.prices ?? []).map((p: any) => ({
        id: p.Id ?? p.id,
        productId: p.ProductId ?? p.productId,
        productName: p.ProductName ?? p.productName,
        price: p.Price ?? p.price ?? null,
        priceType: p.PriceType ?? p.priceType ?? null,
        uom: p.UOM ?? p.uom ?? null,
        tier: p.Tier ?? p.tier ?? null,
        effectiveFrom: p.EffectiveFrom ? new Date(p.EffectiveFrom) : (p.effectiveFrom ? new Date(p.effectiveFrom) : null),
        effectiveTo: p.EffectiveTo ? new Date(p.EffectiveTo) : (p.effectiveTo ? new Date(p.effectiveTo) : null),

        allowance: p.Allowance ?? p.allowance ?? null,
        commercialDelPrice: p.CommercialDelPrice ?? p.commercialDelPrice ?? null,
        commercialFobPrice: p.CommercialFobPrice ?? p.commercialFobPrice ?? null,
        commodityDelPrice: p.CommodityDelPrice ?? p.commodityDelPrice ?? null,
        commodityFobPrice: p.CommodityFobPrice ?? p.commodityFobPrice ?? null,
        estimatedQty: p.EstimatedQty ?? p.estimatedQty ?? null,
        billbacksAllowed: (p.BillbacksAllowed ?? p.billbacksAllowed) === true,
        pua: p.PUA ?? p.pua ?? null,
        ffsPrice: p.FFSPrice ?? p.ffsPrice ?? null,
        noiPrice: p.NOIPrice ?? p.noiPrice ?? null,
        ptv: p.PTV ?? p.ptv ?? null,
        internalNotes: p.InternalNotes ?? p.internalNotes ?? null
      }))
    };
  }

  private toCreateVersionDto(req: CreateContractVersionRequest): any {
    return {
      name: req.name,
      foreignContractId: req.foreignContractId ?? null,
      sendToPerformance: !!req.sendToPerformance,
      internalNotes: req.internalNotes ?? null,
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: req.manufacturerReferenceNumber ?? null,
      manufacturerBillbackName: req.manufacturerBillbackName ?? null,
      manufacturerTermsAndConditions: req.manufacturerTermsAndConditions ?? null,
      manufacturerNotes: req.manufacturerNotes ?? null,
      contactPerson: req.contactPerson ?? null,
      entegraContractType: req.entegraContractType ?? null,
      entegraVdaProgram: req.entegraVdaProgram ?? null,
      startDate: req.startDate ? new Date(req.startDate).toISOString() : null,
      endDate: req.endDate ? new Date(req.endDate).toISOString() : null,
      prices: (req.prices || []).map(p => ({
        productId: p.productId,
        price: p.price ?? null,
        priceType: p.priceType ?? null,
        uom: p.uom ?? null,
        tier: p.tier ?? null,
        effectiveFrom: p.effectiveFrom ? new Date(p.effectiveFrom).toISOString() : null,
        effectiveTo: p.effectiveTo ? new Date(p.effectiveTo).toISOString() : null,
        allowance: p.allowance ?? null,
        commercialDelPrice: p.commercialDelPrice ?? null,
        commercialFobPrice: p.commercialFobPrice ?? null,
        commodityDelPrice: p.commodityDelPrice ?? null,
        commodityFobPrice: p.commodityFobPrice ?? null,
        estimatedQty: p.estimatedQty ?? null,
        billbacksAllowed: !!p.billbacksAllowed,
        pua: p.pua ?? null,
        ffsPrice: p.ffsPrice ?? null,
        noiPrice: p.noiPrice ?? null,
        ptv: p.ptv ?? null,
        internalNotes: p.internalNotes ?? null
      })),
      sourceVersionId: req.sourceVersionId ?? null
    };
  }

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
        errorMessage = 'Contract not found.';
      } else if (error.status === 400) {
        const modelErrors = error.error?.errors;
        if (modelErrors && typeof modelErrors === 'object') {
          const firstKey = Object.keys(modelErrors)[0];
          const firstVal = (modelErrors as any)[firstKey];
          const firstMsg = Array.isArray(firstVal) ? firstVal[0] : firstVal;
          errorMessage = firstMsg || 'Bad request. Please check your input.';
        } else {
          errorMessage = error.error?.title || error.error?.message || 'Bad request. Please check your input.';
        }
      } else if (error.status === 409) {
        errorMessage = 'Contract number already exists.';
      } else if (error.status === 422) {
        errorMessage = error.error?.message || 'Validation failed.';
      } else if (error.status >= 500) {
        errorMessage = error.error?.message || error.error?.error || 'Server error. Please try again later.';
      } else if (error.error?.message) {
        errorMessage = error.error.message;
      }
    }

    console.error('Contract Service Error:', error);
    return throwError(() => new Error(errorMessage));
  }
}
