import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface ContractPricingReportRequest {
  contractNumber?: string;
  manufacturerId?: number;
  productId?: number;
  opCoId?: number;
  industryId?: number;
  contractName?: string;
  distributorId?: number;
  startDateFrom?: Date;
  startDateTo?: Date;
  endDateFrom?: Date;
  endDateTo?: Date;
  page: number;
  pageSize: number;
}

export interface ContractPricingReportRow {
  contractId: number;
  contractNumber: string;
  manufacturer: string;
  startDate?: Date;
  endDate?: Date;
  opCos: string;
  industry?: string;
  contractVersionNumber: number;
  effectiveDate?: Date;
  productCode: string;
  productName: string;
  pricingVersionNumber: number;
  allowance?: number;
  commercialDelivered?: number;
  commodityDelivered?: number;
  commercialFOB?: number;
  commodityFOB?: number;
  uom: string;
  estimatedVolume?: number;
  actualVolume?: number;
}

export interface ContractPricingReportResponse {
  rows: ContractPricingReportRow[];
  totalRows: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class ContractPricingReportService {
  private readonly endpoint = 'Reports';

  constructor(
    private http: HttpClient,
    private apiService: ApiService
  ) { }

  /**
   * Generate Contract Pricing Report
   */
  generateReport(request: ContractPricingReportRequest): Observable<ContractPricingReportResponse> {
    return this.http.post<ContractPricingReportResponse>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/contract-pricing`,
      request
    );
  }

  /**
   * Download Contract Pricing Report as Excel
   */
  downloadExcel(request: ContractPricingReportRequest): Observable<Blob> {
    return this.http.post(
      `${this.apiService.getApiUrl()}/${this.endpoint}/contract-pricing/excel`,
      request,
      { responseType: 'blob' }
    );
  }
}

