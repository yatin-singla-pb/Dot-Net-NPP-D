import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApiService } from './api.service';
import {
  MemberAccount,
  CreateMemberAccountRequest,
  UpdateMemberAccountRequest,
  MemberAccountSearchResult,
  MemberAccountStatus
} from '../models/member-account.model';

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
export class MemberAccountService {
  private readonly endpoint = 'member-accounts';

  constructor(private apiService: ApiService) {}

  getAllMemberAccounts(): Observable<MemberAccount[]> {
    return this.apiService.get<MemberAccount[]>(this.endpoint);
  }

  getMemberAccountById(id: number): Observable<MemberAccount> {
    return this.apiService.get<MemberAccount>(`${this.endpoint}/${id}`);
  }

  createMemberAccount(memberAccount: CreateMemberAccountRequest): Observable<MemberAccount> {
    return this.apiService.post<MemberAccount>(this.endpoint, memberAccount);
  }

  updateMemberAccount(id: number, memberAccount: UpdateMemberAccountRequest): Observable<MemberAccount> {
    return this.apiService.put<MemberAccount>(`${this.endpoint}/${id}`, memberAccount);
  }

  deleteMemberAccount(id: number): Observable<any> {
    return this.apiService.delete(`${this.endpoint}/${id}`);
  }

  activateMemberAccount(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/activate`, {});
  }

  suspendMemberAccount(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/suspend`, {});
  }

  updateW9Status(id: number, w9: boolean, w9Date?: Date): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/w9-status`, { w9, w9Date });
  }

  searchMemberAccounts(params: any): Observable<MemberAccountSearchResult> {
    return this.apiService.get<MemberAccountSearchResult>(`${this.endpoint}/search`, params);
  }

  getMemberAccountsByIndustry(industryId: number): Observable<MemberAccount[]> {
    return this.apiService.get<MemberAccount[]>(`${this.endpoint}/by-industry/${industryId}`);
  }

  getMemberAccountsWithoutW9(): Observable<MemberAccount[]> {
    return this.apiService.get<MemberAccount[]>(`${this.endpoint}/without-w9`);
  }

  exportToExcel(params?: any): Observable<Blob> {
    return this.apiService.getBlob(`${this.endpoint}/export`, params);
  }

  bulkDelete(ids: number[]): Observable<any> {
    return this.apiService.post(`${this.endpoint}/bulk-delete`, { ids });
  }

  bulkUpdateStatus(ids: number[], status: string): Observable<any> {
    return this.apiService.post(`${this.endpoint}/bulk-update-status`, { ids, status });
  }

  // Alias methods for compatibility
  getAll(): Observable<MemberAccount[]> {
    return this.getAllMemberAccounts();
  }

  getById(id: number): Observable<MemberAccount> {
    return this.getMemberAccountById(id);
  }

  create(memberAccount: CreateMemberAccountRequest): Observable<MemberAccount> {
    return this.createMemberAccount(memberAccount);
  }

  getPaginated(
    pageNumber: number,
    pageSize: number,
    sortBy?: string,
    sortDirection: 'asc' | 'desc' = 'asc',
    searchTerm?: string,
    status?: string,
    industryId?: number,
    state?: string
  ): Observable<PaginatedResult<MemberAccount>> {
    const params: any = {
      pageNumber,
      pageSize,
      sortDirection
    };
    if (sortBy) params.sortBy = sortBy;
    if (searchTerm) params.searchTerm = searchTerm;
    if (status) params.status = status;
    if (industryId) params.industryId = industryId;
    if (state) params.state = state;

    return this.apiService.get<any>(this.endpoint, params).pipe(
      map(response => {
        const items = (response.Items || response.items || []).map((dto: any) => this.mapDtoToMemberAccount(dto));
        return {
          items,
          totalCount: response.TotalCount || response.totalCount || items.length,
          pageNumber: response.PageNumber || response.pageNumber || pageNumber,
          pageSize: response.PageSize || response.pageSize || pageSize,
          totalPages: response.TotalPages || response.totalPages || Math.ceil((response.TotalCount || items.length) / (response.PageSize || pageSize))
        } as PaginatedResult<MemberAccount>;
      }),
      catchError(err => throwError(() => err))
    );
  }

  private mapDtoToMemberAccount(dto: any): MemberAccount {
    // Map status
    let status: MemberAccountStatus = MemberAccountStatus.Active;
    if (dto.StatusName || dto.statusName) {
      const name = dto.StatusName || dto.statusName;
      switch (name) {
        case 'Active': status = MemberAccountStatus.Active; break;
        case 'Inactive': status = MemberAccountStatus.Inactive; break;
        case 'Pending': status = MemberAccountStatus.Pending; break;
        case 'Suspended': status = MemberAccountStatus.Suspended; break;
        default: status = MemberAccountStatus.Active;
      }
    } else {
      const val = dto.Status || dto.status || 1;
      switch (val) {
        case 1: status = MemberAccountStatus.Active; break;
        case 2: status = MemberAccountStatus.Inactive; break;
        case 3: status = MemberAccountStatus.Pending; break;
        case 4: status = MemberAccountStatus.Suspended; break;
        default: status = MemberAccountStatus.Active;
      }
    }

    return {
      id: dto.Id || dto.id,
      memberNumber: dto.MemberNumber || dto.memberNumber,
      facilityName: dto.FacilityName || dto.facilityName,
      address: dto.Address || dto.address,
      city: dto.City || dto.city,
      state: dto.State || dto.state,
      zipCode: dto.ZipCode || dto.zipCode,
      country: dto.Country || dto.country,
      phoneNumber: dto.PhoneNumber || dto.phoneNumber,

      industryId: dto.IndustryId || dto.industryId,
      industryName: dto.IndustryName || dto.industryName,
      w9: dto.W9 ?? dto.w9 ?? false,
      w9Date: dto.W9Date || dto.w9Date ? new Date(dto.W9Date || dto.w9Date) : undefined,
      taxId: dto.TaxId || dto.taxId,
      businessType: dto.BusinessType || dto.businessType,
      status: status,
      isActive: dto.IsActive !== undefined ? dto.IsActive : (dto.isActive ?? true),
      createdDate: new Date(dto.CreatedDate || dto.createdDate || Date.now()),
      modifiedDate: dto.ModifiedDate || dto.modifiedDate ? new Date(dto.ModifiedDate || dto.modifiedDate) : undefined,
      createdBy: dto.CreatedBy || dto.createdBy,
      modifiedBy: dto.ModifiedBy || dto.modifiedBy,
      // Extended fields
      lopDate: dto.LopDate || dto.lopDate ? new Date(dto.LopDate || dto.lopDate) : undefined,
      internalNotes: dto.InternalNotes || dto.internalNotes,
      clientGroupEnrollment: (() => { const v = (dto.ClientGroupEnrollment ?? dto.clientGroupEnrollment); return (typeof v === 'number') ? v : (v === true ? 1 : (v === false ? 0 : undefined)); })(),
      salesforceAccountName: dto.SalesforceAccountName || dto.salesforceAccountName,
      vmapNumber: dto.VMAPNumber || dto.vmapNumber,
      vmSupplierName: dto.VMSupplierName || dto.vmSupplierName,
      vmSupplierSite: dto.VMSupplierSite || dto.vmSupplierSite,
      payType: dto.PayType || dto.payType,
      payTypeName: dto.PayTypeName || dto.payTypeName,
      parentMemberAccountNumber: dto.ParentMemberAccountNumber || dto.parentMemberAccountNumber,
      entegraGPONumber: dto.EntegraGPONumber || dto.entegraGPONumber,
      clientGroupNumber: dto.ClientGroupNumber || dto.clientGroupNumber,
      entegraIdNumber: dto.EntegraIdNumber || dto.entegraIdNumber,
      auditDate: dto.AuditDate || dto.auditDate ? new Date(dto.AuditDate || dto.auditDate) : undefined,
      customerAccountsCount: dto.CustomerAccountsCount || dto.customerAccountsCount
    } as MemberAccount;
  }


  update(id: number, memberAccount: UpdateMemberAccountRequest): Observable<MemberAccount> {
    return this.updateMemberAccount(id, memberAccount);
  }

  delete(id: number): Observable<any> {
    return this.deleteMemberAccount(id);
  }
}
