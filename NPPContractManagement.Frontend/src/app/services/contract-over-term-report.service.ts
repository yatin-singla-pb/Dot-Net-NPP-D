import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  ContractOverTermReportRequest, 
  ContractOverTermReportResponse 
} from '../models/contract-over-term-report.model';

@Injectable({
  providedIn: 'root'
})
export class ContractOverTermReportService {
  private readonly endpoint = 'Reports';

  constructor(
    private http: HttpClient,
    private apiService: ApiService
  ) { }

  /**
   * Generate Contract Over Term Report
   */
  generateReport(request: ContractOverTermReportRequest): Observable<ContractOverTermReportResponse> {
    return this.http.post<ContractOverTermReportResponse>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/contract-over-term`,
      request
    );
  }

  /**
   * Download Contract Over Term Report as Excel
   */
  downloadExcel(request: ContractOverTermReportRequest): Observable<Blob> {
    return this.http.post(
      `${this.apiService.getApiUrl()}/${this.endpoint}/contract-over-term/excel`,
      request,
      { responseType: 'blob' }
    );
  }
}

