import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { VelocityService } from '../../services/velocity.service';
import { VelocityJob } from '../../models/velocity.model';
import { ApiService } from '../../services/api.service';
import { DistributorService } from '../../services/distributor.service';

@Component({
  selector: 'app-velocity-reporting',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './velocity-reporting.component.html',
  styleUrls: ['./velocity-reporting.component.css']
})
export class VelocityReportingComponent implements OnInit, OnDestroy {
  selectedFile: File | null = null;
  selectedDistributorId: number | null = null;
  selectedDistributorName: string = '';
  uploading = false;
  uploadProgress = 0;
  uploadError: string | null = null;
  uploadSuccess: string | null = null;

  distributors: any[] = [];
  filteredDistributors: any[] = [];
  distributorFilter: string = '';
  showDistributorPanel = false;
  loadingDistributors = false;

  recentJobs: VelocityJob[] = [];
  loadingJobs = false;
  restartingJobId: string | null = null;

  previewRows: any[] = [];
  previewHeaders: string[] = [];
  showPreview = false;

  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  // Auto-refresh for processing jobs
  private refreshInterval: any;

  constructor(
    private velocityService: VelocityService,
    private http: HttpClient,
    private apiService: ApiService,
    private distributorService: DistributorService
  ) { }

  ngOnInit(): void {
    this.loadDistributors();
    this.loadRecentJobs();
    this.startAutoRefresh();
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
  }

  startAutoRefresh(): void {
    // Auto-refresh every 10 seconds if there are processing/queued jobs
    this.refreshInterval = setInterval(() => {
      if (this.hasActiveJobs()) {
        this.loadRecentJobs();
      }
    }, 10000);
  }

  stopAutoRefresh(): void {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  hasActiveJobs(): boolean {
    return this.recentJobs.some(job =>
      job.status?.toLowerCase() === 'processing' ||
      job.status?.toLowerCase() === 'queued'
    );
  }

  loadDistributors(): void {
    this.loadingDistributors = true;
    this.distributorService.getPaginated(1, 1000, 'Name', 'asc', undefined, 'Active').subscribe({
      next: (resp: any) => {
        this.distributors = (resp?.items || []).filter((d: any) => d.isActive);
        this.filteredDistributors = [...this.distributors];
        this.loadingDistributors = false;
      },
      error: (error) => {
        console.error('Error loading distributors:', error);
        this.uploadError = 'Failed to load distributors';
        this.loadingDistributors = false;
        this.distributors = [];
        this.filteredDistributors = [];
      }
    });
  }

  toggleDistributorPanel(): void {
    this.showDistributorPanel = !this.showDistributorPanel;
    if (this.showDistributorPanel) {
      this.distributorFilter = '';
      this.filteredDistributors = [...this.distributors];
    }
  }

  applyDistributorFilter(value: string): void {
    this.distributorFilter = value;
    const term = value.toLowerCase();
    this.filteredDistributors = this.distributors.filter(d =>
      d.name.toLowerCase().includes(term) ||
      d.id.toString().includes(term)
    );
  }

  selectDistributor(distributor: any): void {
    this.selectedDistributorId = distributor.id;
    this.selectedDistributorName = distributor.name;
  }

  applyDistributorSelection(): void {
    this.showDistributorPanel = false;
  }

  closeDistributorPanel(): void {
    this.showDistributorPanel = false;
    this.distributorFilter = '';
  }

  clearDistributor(): void {
    this.selectedDistributorId = null;
    this.selectedDistributorName = '';
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file type
      const allowedExtensions = ['.csv', '.xlsx', '.xls'];
      const fileName = file.name.toLowerCase();
      const isValidType = allowedExtensions.some(ext => fileName.endsWith(ext));

      if (!isValidType) {
        this.uploadError = 'Invalid file type. Please upload a CSV or Excel file (.csv, .xlsx, .xls)';
        this.selectedFile = null;
        return;
      }

      // Validate file size (10 MB)
      const maxSize = 10 * 1024 * 1024;
      if (file.size > maxSize) {
        this.uploadError = 'File size exceeds 10 MB limit';
        this.selectedFile = null;
        return;
      }

      this.selectedFile = file;
      this.uploadError = null;
      this.uploadSuccess = null;

      // Only preview CSV files (Excel preview would require additional library)
      if (fileName.endsWith('.csv')) {
        this.previewFile(file);
      } else {
        this.showPreview = false;
        this.previewRows = [];
        this.previewHeaders = [];
      }
    }
  }

  async previewFile(file: File): Promise<void> {
    try {
      const text = await file.text();
      const lines = text.split('\n').filter(line => line.trim());
      
      if (lines.length === 0) {
        this.uploadError = 'File is empty';
        return;
      }

      // Parse header
      this.previewHeaders = lines[0].split(',').map(h => h.trim());
      
      // Parse first 10 data rows
      this.previewRows = [];
      for (let i = 1; i < Math.min(11, lines.length); i++) {
        const values = lines[i].split(',').map(v => v.trim());
        const row: any = {};
        this.previewHeaders.forEach((header, index) => {
          row[header] = values[index] || '';
        });
        this.previewRows.push(row);
      }

      this.showPreview = true;
    } catch (error) {
      this.uploadError = 'Error reading file';
      console.error('Error previewing file:', error);
    }
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      this.uploadError = 'Please select a file';
      return;
    }

    if (!this.selectedDistributorId) {
      this.uploadError = 'Please select a distributor';
      return;
    }

    this.uploading = true;
    this.uploadError = null;
    this.uploadSuccess = null;
    this.uploadProgress = 0;

    this.velocityService.uploadFile(this.selectedFile, this.selectedDistributorId).subscribe({
      next: (response) => {
        this.uploading = false;
        this.uploadProgress = 100;
        this.uploadSuccess = response.message || 'File uploaded successfully';
        this.selectedFile = null;
        this.selectedDistributorId = null;
        this.showPreview = false;
        this.previewRows = [];
        this.previewHeaders = [];

        // Reset file input
        const fileInput = document.getElementById('fileInput') as HTMLInputElement;
        if (fileInput) {
          fileInput.value = '';
        }

        // Reload jobs
        this.loadRecentJobs();
      },
      error: (error) => {
        this.uploading = false;
        this.uploadProgress = 0;
        this.uploadError = error.error?.message || 'Error uploading file';
        console.error('Upload error:', error);
      }
    });
  }

  loadRecentJobs(): void {
    this.loadingJobs = true;
    this.velocityService.getJobs(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.recentJobs = response.data || [];
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || 0;
        this.loadingJobs = false;
      },
      error: (error) => {
        console.error('Error loading jobs:', error);
        this.loadingJobs = false;
      }
    });
  }

  downloadTemplate(): void {
    this.velocityService.downloadTemplateFile();
  }

  getStatusClass(status: string): string {
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

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadRecentJobs();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadRecentJobs();
    }
  }

  canRestartJob(job: VelocityJob): boolean {
    if (!job || !job.status) return false;
    const status = job.status.toLowerCase();
    // Show restart for failed, queued, or processing jobs
    // This allows users to manually restart if something seems wrong
    return status === 'failed' || status === 'queued' || status === 'processing';
  }

  isJobStuck(job: VelocityJob): boolean {
    // Consider a job stuck if it's been processing for more than 30 minutes
    if (job.status?.toLowerCase() !== 'processing') return false;

    const startedAt = job.startedAt ? new Date(job.startedAt) : null;
    if (!startedAt) return false;

    const now = new Date();
    const minutesElapsed = (now.getTime() - startedAt.getTime()) / (1000 * 60);
    return minutesElapsed > 30;
  }

  restartJob(job: VelocityJob): void {
    if (!confirm(`Are you sure you want to restart job ${job.jobId}?`)) {
      return;
    }

    this.restartingJobId = job.jobId;
    this.velocityService.restartJob(job.jobId).subscribe({
      next: (response) => {
        this.restartingJobId = null;
        this.uploadSuccess = response.message || 'Job restarted successfully';
        this.uploadError = null;
        // Reload jobs to show updated status
        setTimeout(() => this.loadRecentJobs(), 1000);
      },
      error: (error) => {
        this.restartingJobId = null;
        this.uploadError = error.error?.message || 'Failed to restart job';
        this.uploadSuccess = null;
        console.error('Error restarting job:', error);
      }
    });
  }

  getJobProgressPercentage(job: VelocityJob): number {
    if (!job.totalRows || job.totalRows === 0) return 0;
    const processed = (job.successRows || 0) + (job.failedRows || 0);
    return Math.round((processed / job.totalRows) * 100);
  }

  getJobStatusText(job: VelocityJob): string {
    const status = job.status?.toLowerCase();
    if (status === 'processing') {
      const progress = this.getJobProgressPercentage(job);
      return `Processing (${progress}%)`;
    }
    if (status === 'queued') {
      return 'Queued - Waiting to start';
    }
    return job.status || 'Unknown';
  }
}

