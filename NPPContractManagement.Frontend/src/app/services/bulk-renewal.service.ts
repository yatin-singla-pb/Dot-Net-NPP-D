import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { BulkRenewalRequest, BulkRenewalResponse, ContractRenewalSummary } from '../models/bulk-renewal.model';

@Injectable({
  providedIn: 'root'
})
export class BulkRenewalService {
  private readonly endpoint = 'bulkRenewal';

  constructor(
    private http: HttpClient,
    private apiService: ApiService
  ) { }

  /**
   * Create multiple proposal requests from selected contracts
   */
  createBulkRenewal(request: BulkRenewalRequest): Observable<BulkRenewalResponse> {
    return this.http.post<BulkRenewalResponse>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/create`,
      request
    );
  }

  /**
   * Alias for createBulkRenewal for backward compatibility
   */
  createRenewalProposals(request: BulkRenewalRequest): Observable<BulkRenewalResponse> {
    return this.createBulkRenewal(request);
  }

  /**
   * Validate contracts can be renewed
   */
  validateContracts(contractIds: number[]): Observable<{ [key: number]: string }> {
    return this.http.post<{ [key: number]: string }>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/validate`,
      contractIds
    );
  }
}

