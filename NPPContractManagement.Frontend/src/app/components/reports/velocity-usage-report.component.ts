import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { VelocityService } from '../../services/velocity.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { OpCoService } from '../../services/opco.service';
import { IndustryService } from '../../services/industry.service';

interface VelocityUsageAggregate {
  manufacturer?: string;
  product?: string;
  casesPurchased: number;
  minShipmentDate?: Date;
  maxShipmentDate?: Date;
  productId?: string;
  distributorProductCode?: string;
  brand?: string;
  packSize?: string;
  productDescription?: string;
  mfrProductCode?: string;
  gtin?: string;
  avgLandedCost?: number;
  groupKey: string;
}

interface VelocityUsageDetail {
  id: number;
  productId?: string;
  distributorProductCode?: string;
  brand?: string;
  packSize?: string;
  productDescription?: string;
  mfrProductCode?: string;
  gtin?: string;
  manufacturer?: string;
  casesPurchased?: number;
  invoiceDate?: Date;
  invoiceNumber?: string;
  contractPrice?: number;
  landedCost?: number;
  category?: string;
  secondaryCategory?: string;
  tertiaryCategory?: string;
  opCo?: string;
  customerNumber?: string;
  customerName?: string;
}

@Component({
  selector: 'app-velocity-usage-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './velocity-usage-report.component.html',
  styleUrls: ['./velocity-usage-report.component.css']
})
export class VelocityUsageReportComponent implements OnInit {
  // Filters
  startDate: string = '';
  endDate: string = '';
  keyword: string = '';
  selectedManufacturerIds: number[] = [];
  selectedOpCoIds: number[] = [];
  selectedIndustryIds: number[] = [];

  // Dropdown data
  manufacturers: any[] = [];
  opCos: any[] = [];
  industries: any[] = [];

  // Pagination
  currentPage: number = 1;
  pageSize: number = 50;
  totalCount: number = 0;
  totalPages: number = 0;

  // Data
  aggregates: VelocityUsageAggregate[] = [];
  loading: boolean = false;

  // Selection state (persisted across pagination)
  selectedGroupKeys: Set<string> = new Set();

  // Expanded row details
  expandedGroupKey: string | null = null;
  detailRecords: VelocityUsageDetail[] = [];
  loadingDetails: boolean = false;

  // Proposal creation
  showProposalModal: boolean = false;
  proposalTitle: string = '';
  quantityAdjustmentPercent: number | null = null;
  minimumQuantityThreshold: number | null = null;
  proposalDueDate: string = '';
  creatingProposal: boolean = false;

  constructor(
    private velocityService: VelocityService,
    private manufacturerService: ManufacturerService,
    private opCoService: OpCoService,
    private industryService: IndustryService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Set default date range (last 30 days)
    const today = new Date();
    const thirtyDaysAgo = new Date();
    thirtyDaysAgo.setDate(today.getDate() - 30);

    this.endDate = this.formatDate(today);
    this.startDate = this.formatDate(thirtyDaysAgo);

    // Load dropdown data
    this.loadManufacturers();
    this.loadOpCos();
    this.loadIndustries();

    // Load report
    this.loadReport();
  }

  loadManufacturers(): void {
    this.manufacturerService.getAll().subscribe({
      next: (data) => {
        this.manufacturers = data;
      },
      error: (error) => {
        console.error('Error loading manufacturers:', error);
      }
    });
  }

  loadOpCos(): void {
    this.opCoService.getAll().subscribe({
      next: (data) => {
        this.opCos = data;
      },
      error: (error) => {
        console.error('Error loading op-cos:', error);
      }
    });
  }

  loadIndustries(): void {
    this.industryService.getAll().subscribe({
      next: (data) => {
        this.industries = data;
      },
      error: (error) => {
        console.error('Error loading industries:', error);
      }
    });
  }

  loadReport(): void {
    this.loading = true;

    const request = {
      startDate: this.startDate ? new Date(this.startDate) : undefined,
      endDate: this.endDate ? new Date(this.endDate) : undefined,
      keyword: this.keyword || undefined,
      manufacturerIds: this.selectedManufacturerIds.length > 0 ? this.selectedManufacturerIds : undefined,
      opCoIds: this.selectedOpCoIds.length > 0 ? this.selectedOpCoIds : undefined,
      industryIds: this.selectedIndustryIds.length > 0 ? this.selectedIndustryIds : undefined,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    this.velocityService.getUsageReport(request).subscribe({
      next: (response: any) => {
        this.aggregates = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading report:', error);
        this.loading = false;
      }
    });
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadReport();
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadReport();
    }
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadReport();
  }

  toggleRowSelection(groupKey: string): void {
    if (this.selectedGroupKeys.has(groupKey)) {
      this.selectedGroupKeys.delete(groupKey);
    } else {
      this.selectedGroupKeys.add(groupKey);
    }
  }

  isRowSelected(groupKey: string): boolean {
    return this.selectedGroupKeys.has(groupKey);
  }

  selectAll(): void {
    this.aggregates.forEach(agg => {
      this.selectedGroupKeys.add(agg.groupKey);
    });
  }

  deselectAll(): void {
    this.aggregates.forEach(agg => {
      this.selectedGroupKeys.delete(agg.groupKey);
    });
  }

  toggleRowDetails(groupKey: string): void {
    if (this.expandedGroupKey === groupKey) {
      this.expandedGroupKey = null;
      this.detailRecords = [];
    } else {
      this.expandedGroupKey = groupKey;
      this.loadDetailRecords(groupKey);
    }
  }

  isRowExpanded(groupKey: string): boolean {
    return this.expandedGroupKey === groupKey;
  }

  loadDetailRecords(groupKey: string): void {
    this.loadingDetails = true;

    const reportRequest = {
      startDate: this.startDate ? new Date(this.startDate) : undefined,
      endDate: this.endDate ? new Date(this.endDate) : undefined,
      keyword: this.keyword || undefined,
      manufacturerIds: this.selectedManufacturerIds.length > 0 ? this.selectedManufacturerIds : undefined,
      opCoIds: this.selectedOpCoIds.length > 0 ? this.selectedOpCoIds : undefined,
      industryIds: this.selectedIndustryIds.length > 0 ? this.selectedIndustryIds : undefined
    };

    this.velocityService.getUsageDetails(groupKey, reportRequest).subscribe({
      next: (details: VelocityUsageDetail[]) => {
        this.detailRecords = details;
        this.loadingDetails = false;
      },
      error: (error) => {
        console.error('Error loading details:', error);
        this.loadingDetails = false;
      }
    });
  }

  openProposalModal(): void {
    if (this.selectedGroupKeys.size === 0) {
      alert('Please select at least one row to create a proposal.');
      return;
    }

    this.showProposalModal = true;
    this.proposalTitle = `Velocity Usage Proposal - ${new Date().toLocaleDateString()}`;

    // Set default due date (30 days from now)
    const dueDate = new Date();
    dueDate.setDate(dueDate.getDate() + 30);
    this.proposalDueDate = this.formatDate(dueDate);
  }

  closeProposalModal(): void {
    this.showProposalModal = false;
    this.proposalTitle = '';
    this.quantityAdjustmentPercent = null;
    this.minimumQuantityThreshold = null;
    this.proposalDueDate = '';
  }

  createProposals(): void {
    this.creatingProposal = true;

    const request = {
      selectedGroupKeys: Array.from(this.selectedGroupKeys),
      quantityAdjustmentPercent: this.quantityAdjustmentPercent,
      minimumQuantityThreshold: this.minimumQuantityThreshold,
      proposalDueDate: this.proposalDueDate ? new Date(this.proposalDueDate) : undefined,
      proposalTitle: this.proposalTitle
    };

    this.velocityService.createProposalsFromVelocity(request).subscribe({
      next: (proposalIds: number[]) => {
        this.creatingProposal = false;
        this.closeProposalModal();
        alert(`Successfully created ${proposalIds.length} proposal(s)!`);

        // Navigate to first proposal if only one was created
        if (proposalIds.length === 1) {
          this.router.navigate(['/admin/proposals', proposalIds[0]]);
        } else {
          this.router.navigate(['/admin/proposals']);
        }
      },
      error: (error) => {
        console.error('Error creating proposals:', error);
        this.creatingProposal = false;
        alert('Error creating proposals. Please try again.');
      }
    });
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  formatDateTime(date: Date | string | undefined): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString();
  }

  formatCurrency(value: number | undefined): string {
    if (value === undefined || value === null) return '';
    return `$${value.toFixed(2)}`;
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

