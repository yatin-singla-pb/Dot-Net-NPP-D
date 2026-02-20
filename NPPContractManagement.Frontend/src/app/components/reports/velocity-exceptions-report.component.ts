import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VelocityService } from '../../services/velocity.service';

interface VelocityException {
  id: number;
  jobId: number;
  jobIdStr: string;
  rowIndex: number;
  status: string;
  errorMessage?: string;
  rawData?: string;
  processedAt: Date;
  fileName?: string;
  createdBy?: string;
  opCo?: string;
  customerNumber?: string;
  customerName?: string;
  invoiceNumber?: string;
  invoiceDate?: string;
  productNumber?: string;
  brand?: string;
  description?: string;
  manufacturerName?: string;
  qty?: string;
  actionStatus?: string;
  actionNotes?: string;
  actionTakenBy?: string;
  actionTakenAt?: string;
}

interface VelocityExceptionsResponse {
  data: VelocityException[];
  totalCount: number;
  page: number;
  pageSize: number;
}

@Component({
  selector: 'app-velocity-exceptions-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './velocity-exceptions-report.component.html',
  styleUrls: ['./velocity-exceptions-report.component.css']
})
export class VelocityExceptionsReportComponent implements OnInit {
  // Filters
  jobId: number | null = null;
  startDate: string = '';
  endDate: string = '';
  keyword: string = '';

  // Pagination
  currentPage: number = 1;
  pageSize: number = 50;
  totalCount: number = 0;
  totalPages: number = 0;

  // Data
  exceptions: VelocityException[] = [];
  loading: boolean = false;

  // Expanded row
  expandedRowId: number | null = null;

  constructor(private velocityService: VelocityService) {}

  ngOnInit(): void {
    // Set default date range (last 30 days)
    const today = new Date();
    const thirtyDaysAgo = new Date();
    thirtyDaysAgo.setDate(today.getDate() - 30);

    this.endDate = this.formatDate(today);
    this.startDate = this.formatDate(thirtyDaysAgo);

    this.loadExceptions();
  }

  loadExceptions(): void {
    this.loading = true;

    const request = {
      jobId: this.jobId,
      startDate: this.startDate ? new Date(this.startDate) : undefined,
      endDate: this.endDate ? new Date(this.endDate) : undefined,
      keyword: this.keyword || undefined,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    this.velocityService.getExceptions(request).subscribe({
      next: (response: VelocityExceptionsResponse) => {
        this.exceptions = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading exceptions:', error);
        this.loading = false;
      }
    });
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadExceptions();
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadExceptions();
    }
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadExceptions();
  }

  toggleRow(exceptionId: number): void {
    if (this.expandedRowId === exceptionId) {
      this.expandedRowId = null;
    } else {
      this.expandedRowId = exceptionId;
    }
  }

  isRowExpanded(exceptionId: number): boolean {
    return this.expandedRowId === exceptionId;
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  formatDateTime(date: Date | string): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleString();
  }

  dismissException(exception: VelocityException): void {
    const notes = prompt('Enter notes for dismissal (optional):') || '';
    this.velocityService.performExceptionAction(exception.id, 'Dismissed', notes).subscribe({
      next: () => {
        exception.actionStatus = 'Dismissed';
        exception.actionNotes = notes;
      },
      error: (err) => console.error('Error dismissing exception:', err)
    });
  }

  markAsNewContract(exception: VelocityException): void {
    const notes = prompt('Enter notes (optional):') || '';
    this.velocityService.performExceptionAction(exception.id, 'New Contract', notes).subscribe({
      next: () => {
        exception.actionStatus = 'New Contract';
        exception.actionNotes = notes;
      },
      error: (err) => console.error('Error marking as new contract:', err)
    });
  }

  markAsAmendment(exception: VelocityException): void {
    const notes = prompt('Enter notes (optional):') || '';
    this.velocityService.performExceptionAction(exception.id, 'Amendment', notes).subscribe({
      next: () => {
        exception.actionStatus = 'Amendment';
        exception.actionNotes = notes;
      },
      error: (err) => console.error('Error marking as amendment:', err)
    });
  }

  getActionBadgeClass(status: string): string {
    switch (status) {
      case 'Dismissed': return 'bg-secondary';
      case 'New Contract': return 'bg-info';
      case 'Amendment': return 'bg-warning text-dark';
      default: return 'bg-light text-dark';
    }
  }

  getActionLabel(status: string): string {
    return status || 'Pending';
  }

  getPaginationPages(): number[] {
    const pages: number[] = [];
    const maxPagesToShow = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);

    if (endPage - startPage < maxPagesToShow - 1) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }
}

