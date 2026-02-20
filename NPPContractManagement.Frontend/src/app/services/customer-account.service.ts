import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import {
  CustomerAccount, 
  CreateCustomerAccountRequest, 
  UpdateCustomerAccountRequest,
  CustomerAccountSearchResult 
} from '../models/customer-account.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerAccountService {
  private readonly endpoint = 'CustomerAccounts';

  constructor(private apiService: ApiService) {}

  getAllCustomerAccounts(): Observable<CustomerAccount[]> {
    return this.apiService.get<CustomerAccount[]>(this.endpoint);
  }

  getCustomerAccountById(id: number): Observable<CustomerAccount> {
    return this.apiService.get<CustomerAccount>(`${this.endpoint}/${id}`);
  }

  createCustomerAccount(customerAccount: CreateCustomerAccountRequest): Observable<CustomerAccount> {
    const payload = this.toCreateDto(customerAccount);
    return this.apiService.post<CustomerAccount>(this.endpoint, payload);
  }

  updateCustomerAccount(id: number, customerAccount: UpdateCustomerAccountRequest): Observable<CustomerAccount> {
    const payload = this.toUpdateDto(customerAccount);
    return this.apiService.put<CustomerAccount>(`${this.endpoint}/${id}`, payload);
  }

  deleteCustomerAccount(id: number): Observable<any> {
    return this.apiService.delete(`${this.endpoint}/${id}`);
  }

  activateCustomerAccount(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/activate`, {});
  }

  suspendCustomerAccount(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/suspend`, {});
  }

  closeCustomerAccount(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/close`, {});
  }


  searchCustomerAccounts(params: any): Observable<CustomerAccountSearchResult> {
    const query: any = {};
    if (params) {
      // Map Distributors-style params to API expectations
      if (params.pageNumber !== undefined) query.page = params.pageNumber;
      if (params.pageSize !== undefined) query.pageSize = params.pageSize;
      if (params.searchTerm !== undefined) query.searchTerm = params.searchTerm;
      if (params.memberAccountId !== undefined) query.memberAccountId = params.memberAccountId;
      if (params.distributorId !== undefined) query.distributorId = params.distributorId;
      if (params.opCoId !== undefined) query.opCoId = params.opCoId;
      if (params.status !== undefined && params.status !== '') {
        query.status = this.toStatusInt(params.status);
      }
      if (params.isActive !== undefined) query.isActive = params.isActive;
      if (params.industryId !== undefined) query.industryId = params.industryId;
      if (params.association !== undefined && params.association !== '') query.association = params.association;
      if (params.startDate) query.startDate = params.startDate;
      if (params.endDate) query.endDate = params.endDate;
      if (params.tracsAccess !== undefined && params.tracsAccess !== '') query.tracsAccess = params.tracsAccess;
      if (params.toEntegra !== undefined && params.toEntegra !== '') query.toEntegra = params.toEntegra;
      if (params.state !== undefined && params.state !== '') query.state = params.state;
    }
    return this.apiService.get<any>(`${this.endpoint}/search`, query).pipe(
      // Normalize response to CustomerAccountSearchResult shape
      // API returns { data, totalCount, page, pageSize, totalPages }
      // Map data items to model as-is
      // If 'items' already present, keep it
      // Consumers already handle undefined items safely
      // Here we return consistent shape
      // eslint-disable-next-line rxjs/no-explicit-generics
      // eslint-disable-next-line @typescript-eslint/no-unsafe-return
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment
      // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
      map(resp => ({
        items: resp?.items ?? resp?.data ?? [],
        totalCount: resp?.totalCount ?? 0,
        currentPage: resp?.page ?? query.page ?? 1,
        pageSize: resp?.pageSize ?? query.pageSize ?? 10,
        totalPages: resp?.totalPages ?? Math.ceil((resp?.totalCount ?? 0) / (resp?.pageSize ?? (query.pageSize ?? 10)))
      } as CustomerAccountSearchResult))
    );
  }

  private toStatusInt(status: any): number | null {
    if (status === null || status === undefined || status === '') return null;
    const s = typeof status === 'string' ? status.toLowerCase() : status;
    switch (s) {
      case 'active':
      case 1:
        return 1;
      case 'inactive':
      case 2:
        return 2;
      case 'pending':
      case 3:
        return 3;
      case 'suspended':
      case 4:
        return 4;
      case 'closed':
      case 5:
        return 5;
      default:
        return 1; // default Active
    }
  }

  private associationToInt(value: any): number | null {
    if (value === null || value === undefined || value === '') return null;
    if (typeof value === 'number') return value;
    const s = String(value).toLowerCase();
    switch (s) {
      case 'csn': return 1;
      case 'combined': return 2;
      case 'rcdm': return 3;
      case 'semupc': return 4;
      default: return null;
    }
  }

  private emptyToNull(v: any): any {
    return v === '' || v === undefined ? null : v;
  }

  private toCreateDto(req: CreateCustomerAccountRequest): any {
    return {
      memberAccountId: req.memberAccountId,
      distributorId: req.distributorId,
      opCoId: req.opCoId ?? null,
      customerName: req.customerName,
      customerAccountNumber: req.customerAccountNumber,
      address: this.emptyToNull(req.address),
      city: this.emptyToNull(req.city),
      state: this.emptyToNull(req.state),
      zipCode: this.emptyToNull(req.zipCode),
      country: this.emptyToNull(req.country),
      phoneNumber: this.emptyToNull(req.phoneNumber),
      email: this.emptyToNull(req.email),
      // new fields
      salesRep: this.emptyToNull(req.salesRep),
      dso: req.dso ?? null,
      startDate: this.emptyToNull(req.startDate),
      endDate: this.emptyToNull(req.endDate),
      tracsAccess: !!req.tracsAccess,
      markup: this.emptyToNull(req.markup),
      auditDate: this.emptyToNull(req.auditDate),
      toEntegra: !!req.toEntegra,
      dateToEntegra: this.emptyToNull(req.dateToEntegra),
      combinedUniqueID: this.emptyToNull(req.combinedUniqueID),
      internalNotes: this.emptyToNull(req.internalNotes),
      association: this.associationToInt(req.association),
      status: this.toStatusInt(req.status ?? 'Active')
    };
  }

  private toUpdateDto(req: UpdateCustomerAccountRequest): any {
    const dto: any = {};
    if (req.memberAccountId !== undefined) dto.memberAccountId = req.memberAccountId;
    if (req.distributorId !== undefined) dto.distributorId = req.distributorId;
    if (req.opCoId !== undefined) dto.opCoId = req.opCoId;
    if (req.customerName !== undefined) dto.customerName = req.customerName;
    if (req.customerAccountNumber !== undefined) dto.customerAccountNumber = req.customerAccountNumber;
    if (req.address !== undefined) dto.address = this.emptyToNull(req.address);
    if (req.city !== undefined) dto.city = this.emptyToNull(req.city);
    if (req.state !== undefined) dto.state = this.emptyToNull(req.state);
    if (req.zipCode !== undefined) dto.zipCode = this.emptyToNull(req.zipCode);
    if (req.country !== undefined) dto.country = this.emptyToNull(req.country);
    if (req.phoneNumber !== undefined) dto.phoneNumber = this.emptyToNull(req.phoneNumber);
    if (req.email !== undefined) dto.email = this.emptyToNull(req.email);
    if (req.salesRep !== undefined) dto.salesRep = this.emptyToNull(req.salesRep);
    if (req.dso !== undefined) dto.dso = req.dso;
    if (req.startDate !== undefined) dto.startDate = this.emptyToNull(req.startDate);
    if (req.endDate !== undefined) dto.endDate = this.emptyToNull(req.endDate);
    if (req.tracsAccess !== undefined) dto.tracsAccess = !!req.tracsAccess;
    if (req.markup !== undefined) dto.markup = this.emptyToNull(req.markup);
    if (req.auditDate !== undefined) dto.auditDate = this.emptyToNull(req.auditDate);
    if (req.toEntegra !== undefined) dto.toEntegra = !!req.toEntegra;
    if (req.dateToEntegra !== undefined) dto.dateToEntegra = this.emptyToNull(req.dateToEntegra);
    if (req.combinedUniqueID !== undefined) dto.combinedUniqueID = this.emptyToNull(req.combinedUniqueID);
    if (req.internalNotes !== undefined) dto.internalNotes = this.emptyToNull(req.internalNotes);
    if (req.association !== undefined) dto.association = this.associationToInt(req.association);
    if (req.status !== undefined) dto.status = this.toStatusInt(req.status);
    if (req.isActive !== undefined) dto.isActive = req.isActive;
    return dto;
  }

  getCustomerAccountsByMemberAccount(memberAccountId: number): Observable<CustomerAccount[]> {
    return this.apiService.get<CustomerAccount[]>(`${this.endpoint}/by-member-account/${memberAccountId}`);
  }

  getCustomerAccountsByDistributor(distributorId: number): Observable<CustomerAccount[]> {
    return this.apiService.get<CustomerAccount[]>(`${this.endpoint}/by-distributor/${distributorId}`);
  }

  getCustomerAccountsByOpCo(opCoId: number): Observable<CustomerAccount[]> {
    return this.apiService.get<CustomerAccount[]>(`${this.endpoint}/by-opco/${opCoId}`);
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
  getAll(): Observable<CustomerAccount[]> {
    return this.getAllCustomerAccounts();
  }

  getById(id: number): Observable<CustomerAccount> {
    return this.getCustomerAccountById(id);
  }

  create(customerAccount: CreateCustomerAccountRequest): Observable<CustomerAccount> {
    return this.createCustomerAccount(customerAccount);
  }

  update(id: number, customerAccount: UpdateCustomerAccountRequest): Observable<CustomerAccount> {
    return this.updateCustomerAccount(id, customerAccount);
  }

  delete(id: number): Observable<any> {
    return this.deleteCustomerAccount(id);
  }
}
