import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { VelocityService } from '../../services/velocity.service';
import { VelocityJobDetail, VelocityJobRow } from '../../models/velocity.model';

@Component({
  selector: 'app-velocity-job-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './velocity-job-details.component.html',
  styleUrls: ['./velocity-job-details.component.css']
})
export class VelocityJobDetailsComponent implements OnInit, OnDestroy {
  jobId: string = '';
  jobDetails: VelocityJobDetail | null = null;
  loading = false;
  error: string | null = null;
  restarting = false;
  restartSuccess: string | null = null;
  restartError: string | null = null;

  // Pagination for rows
  currentPage = 1;
  pageSize = 50;
  totalRows = 0;
  paginatedRows: VelocityJobRow[] = [];

  // Filtering
  statusFilter: string = 'all';

  // Auto-refresh for processing jobs
  private refreshInterval: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private velocityService: VelocityService
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.jobId = params['jobId'];
      if (this.jobId) {
        this.loadJobDetails();
        this.startAutoRefresh();
      }
    });
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
  }

  startAutoRefresh(): void {
    // Auto-refresh every 5 seconds if job is processing or queued
    this.refreshInterval = setInterval(() => {
      if (this.shouldAutoRefresh()) {
        this.loadJobDetails();
      }
    }, 5000);
  }

  stopAutoRefresh(): void {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  shouldAutoRefresh(): boolean {
    if (!this.jobDetails) return false;
    const status = this.jobDetails.status?.toLowerCase();
    return status === 'processing' || status === 'queued';
  }

  loadJobDetails(): void {
    this.loading = true;
    this.error = null;

    this.velocityService.getJobDetails(this.jobId).subscribe({
      next: (response) => {
        this.jobDetails = response;
        this.totalRows = response.rows?.length || 0;
        this.applyFilter();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading job details:', error);
        console.error('Error status:', error.status);
        console.error('Error message:', error.message);
        console.error('Error details:', error.error);

        if (error.status === 401) {
          this.error = 'Unauthorized. Please log in again.';
        } else if (error.status === 404) {
          this.error = `Job ${this.jobId} not found`;
        } else {
          this.error = error.error?.message || error.message || 'An error occurred while retrieving the job';
        }
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    if (!this.jobDetails) return;

    let filteredRows = this.jobDetails.rows || [];

    if (this.statusFilter !== 'all') {
      filteredRows = filteredRows.filter(row => 
        row.status.toLowerCase() === this.statusFilter.toLowerCase()
      );
    }

    this.totalRows = filteredRows.length;
    
    // Paginate
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedRows = filteredRows.slice(startIndex, endIndex);
  }

  onFilterChange(status: string): void {
    this.statusFilter = status;
    this.currentPage = 1;
    this.applyFilter();
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'success':
        return 'badge bg-success';
      case 'failed':
      case 'error':
        return 'badge bg-danger';
      case 'processing':
        return 'badge bg-primary';
      case 'skipped':
        return 'badge bg-warning text-dark';
      default:
        return 'badge bg-secondary';
    }
  }

  getJobStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'completed':
        return 'badge bg-success';
      case 'processing':
        return 'badge bg-primary';
      case 'queued':
        return 'badge bg-secondary';
      case 'failed':
        return 'badge bg-danger';
      case 'partialsuccess':
        return 'badge bg-warning text-dark';
      default:
        return 'badge bg-secondary';
    }
  }

  getTotalPages(): number {
    return Math.ceil(this.totalRows / this.pageSize);
  }

  nextPage(): void {
    if (this.currentPage < this.getTotalPages()) {
      this.currentPage++;
      this.applyFilter();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.applyFilter();
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/velocity']);
  }

  refresh(): void {
    this.loadJobDetails();
  }

  canRestartJob(): boolean {
    if (!this.jobDetails) return false;
    const status = this.jobDetails.status?.toLowerCase();
    return status === 'failed' || status === 'queued' ||
           (status === 'processing' && this.isJobStuck());
  }

  isJobStuck(): boolean {
    if (!this.jobDetails) return false;
    if (this.jobDetails.status?.toLowerCase() !== 'processing') return false;

    const startedAt = this.jobDetails.startedAt ? new Date(this.jobDetails.startedAt) : null;
    if (!startedAt) return false;

    const now = new Date();
    const minutesElapsed = (now.getTime() - startedAt.getTime()) / (1000 * 60);
    return minutesElapsed > 30;
  }

  restartJob(): void {
    if (!this.jobDetails) return;

    if (!confirm(`Are you sure you want to restart job ${this.jobDetails.jobId}?\n\nThis will reset the job and start processing from the beginning.`)) {
      return;
    }

    this.restarting = true;
    this.restartError = null;
    this.restartSuccess = null;

    this.velocityService.restartJob(this.jobDetails.jobId).subscribe({
      next: (response) => {
        this.restarting = false;
        this.restartSuccess = response.message || 'Job restarted successfully';
        // Reload job details after a short delay
        setTimeout(() => {
          this.loadJobDetails();
          this.restartSuccess = null;
        }, 2000);
      },
      error: (error) => {
        this.restarting = false;
        this.restartError = error.error?.message || 'Failed to restart job';
        console.error('Error restarting job:', error);
      }
    });
  }

  getProgressPercentage(): number {
    if (!this.jobDetails || !this.jobDetails.totalRows || this.jobDetails.totalRows === 0) return 0;
    const processed = (this.jobDetails.successRows || 0) + (this.jobDetails.failedRows || 0);
    return Math.round((processed / this.jobDetails.totalRows) * 100);
  }

  getElapsedTime(): string {
    if (!this.jobDetails || !this.jobDetails.startedAt) return '-';

    const startedAt = new Date(this.jobDetails.startedAt);
    const endTime = this.jobDetails.completedAt ? new Date(this.jobDetails.completedAt) : new Date();
    const elapsedMs = endTime.getTime() - startedAt.getTime();

    const seconds = Math.floor(elapsedMs / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);

    if (hours > 0) {
      return `${hours}h ${minutes % 60}m`;
    } else if (minutes > 0) {
      return `${minutes}m ${seconds % 60}s`;
    } else {
      return `${seconds}s`;
    }
  }
}

