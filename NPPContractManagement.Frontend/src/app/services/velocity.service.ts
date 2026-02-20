import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { VelocityJob, VelocityJobDetail, VelocityIngestResponse } from '../models/velocity.model';

@Injectable({
  providedIn: 'root'
})
export class VelocityService {
  private readonly endpoint = 'velocity';

  constructor(
    private http: HttpClient,
    private apiService: ApiService
  ) { }

  /**
   * Upload CSV file for velocity data ingestion
   */
  uploadFile(file: File, distributorId: number): Observable<VelocityIngestResponse> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    formData.append('distributorId', distributorId.toString());

    return this.http.post<VelocityIngestResponse>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/ingest`,
      formData
    );
  }

  /**
   * Get job details by job ID
   */
  getJobDetails(jobId: string): Observable<VelocityJobDetail> {
    return this.http.get<VelocityJobDetail>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/jobs/${jobId}`
    );
  }

  /**
   * Get paginated list of jobs
   */
  getJobs(page: number = 1, pageSize: number = 20, status?: string): Observable<any> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (status) {
      params = params.set('status', status);
    }

    return this.http.get<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/jobs`,
      { params }
    );
  }

  /**
   * Download sample CSV template
   */
  downloadTemplate(): Observable<Blob> {
    return this.http.get(
      `${this.apiService.getApiUrl()}/${this.endpoint}/template`,
      { responseType: 'blob' }
    );
  }

  /**
   * Helper to trigger template download in browser
   */
  downloadTemplateFile(): void {
    this.downloadTemplate().subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = 'velocity_template.csv';
      link.click();
      window.URL.revokeObjectURL(url);
    });
  }

  /**
   * Restart a failed or stuck job
   */
  restartJob(jobId: string): Observable<VelocityIngestResponse> {
    return this.http.post<VelocityIngestResponse>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/jobs/${jobId}/restart`,
      {}
    );
  }

  /**
   * Get velocity exceptions (failed job rows)
   */
  getExceptions(request: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/exceptions`,
      request
    );
  }

  /**
   * Perform action on a velocity exception (dismiss, new contract, amendment)
   */
  performExceptionAction(id: number, action: string, notes?: string): Observable<any> {
    return this.http.post<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/exceptions/${id}/action`,
      { action, notes }
    );
  }

  /**
   * Generate velocity usage report
   */
  getUsageReport(request: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/usage-report`,
      request
    );
  }

  /**
   * Get detail records for a specific aggregate group
   */
  getUsageDetails(groupKey: string, reportRequest: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/usage-report/details`,
      { groupKey, reportRequest }
    );
  }

  /**
   * Check if contracts exist for selected products
   */
  checkContracts(groupKeys: string[]): Observable<any> {
    return this.http.post<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/usage-report/check-contracts`,
      groupKeys
    );
  }

  /**
   * Create proposals from velocity data
   */
  createProposalsFromVelocity(request: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiService.getApiUrl()}/${this.endpoint}/usage-report/create-proposals`,
      request
    );
  }
}

