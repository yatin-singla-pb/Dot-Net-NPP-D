import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { VelocityService } from './velocity.service';

export interface VelocityExceptionDto {
  id: number;
  jobId: number;
  jobIdStr: string;
  rowIndex: number;
  status: string;
  errorMessage?: string;
  rawData?: string;
  processedAt: string;
  fileName?: string;
  createdBy?: string;
}

export interface VelocityExceptionsResponse {
  data: VelocityExceptionDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface VelocityExceptionsResponseForWidget {
  rows: VelocityExceptionDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class VelocityExceptionService {

  constructor(private velocityService: VelocityService) { }

  /**
   * Get velocity data exceptions with pagination
   */
  getExceptions(page: number = 1, pageSize: number = 5, jobId?: number, startDate?: string, endDate?: string, keyword?: string): Observable<VelocityExceptionsResponse> {
    const request = {
      page,
      pageSize,
      jobId,
      startDate,
      endDate,
      keyword
    };

    return this.velocityService.getExceptions(request);
  }

  /**
   * Get recent velocity data exceptions for dashboard widget
   */
  getRecentExceptions(page: number = 1, pageSize: number = 5): Observable<VelocityExceptionsResponseForWidget> {
    return this.getExceptions(page, pageSize).pipe(
      map(response => ({
        rows: response.data,
        totalCount: response.totalCount,
        page: response.page,
        pageSize: response.pageSize
      }))
    );
  }
}

