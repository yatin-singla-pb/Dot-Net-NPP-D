import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ContractPricingReportService, ContractPricingReportRequest, ContractPricingReportRow, ContractPricingReportResponse } from '../../services/contract-pricing-report.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { ProductService } from '../../services/product.service';
import { OpCoService } from '../../services/opco.service';
import { IndustryService } from '../../services/industry.service';
import { DistributorService } from '../../services/distributor.service';
import { Manufacturer } from '../../models/manufacturer.model';
import { Product } from '../../models/product.model';
import { OpCo } from '../../models/opco.model';
import { Industry } from '../../models/industry.model';
import { Distributor } from '../../models/distributor.model';

@Component({
  selector: 'app-contract-pricing-report',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './contract-pricing-report.component.html',
  styleUrls: ['./contract-pricing-report.component.css']
})
export class ContractPricingReportComponent implements OnInit {
  Math = Math;

  // Filters
  contractNumber: string = '';
  contractName: string = '';
  selectedManufacturerId: number | null = null;
  selectedProductId: number | null = null;
  selectedOpCoId: number | null = null;
  selectedIndustryId: number | null = null;
  selectedDistributorId: number | null = null;
  startDateFrom: string = '';
  startDateTo: string = '';
  endDateFrom: string = '';
  endDateTo: string = '';

  // Pagination
  currentPage: number = 1;
  pageSize: number = 50;

  // Lookup data
  manufacturers: Manufacturer[] = [];
  products: Product[] = [];
  opCos: OpCo[] = [];
  industries: Industry[] = [];
  distributors: Distributor[] = [];

  // Report data
  reportData: ContractPricingReportResponse | null = null;
  loading = false;
  error: string | null = null;

  // Excel download
  downloadingExcel = false;

  constructor(
    private reportService: ContractPricingReportService,
    private manufacturerService: ManufacturerService,
    private productService: ProductService,
    private opCoService: OpCoService,
    private industryService: IndustryService,
    private distributorService: DistributorService
  ) {}

  ngOnInit(): void {
    this.loadLookupData();
  }

  loadLookupData(): void {
    // Load manufacturers
    this.manufacturerService.getAll().subscribe({
      next: (data) => {
        this.manufacturers = data;
      },
      error: (error) => {
        console.error('Error loading manufacturers:', error);
      }
    });

    // Load products
    this.productService.getAll().subscribe({
      next: (data) => {
        this.products = data;
      },
      error: (error) => {
        console.error('Error loading products:', error);
      }
    });

    // Load op-cos
    this.opCoService.getAll().subscribe({
      next: (data) => {
        this.opCos = data;
      },
      error: (error) => {
        console.error('Error loading op-cos:', error);
      }
    });

    // Load industries
    this.industryService.getAll().subscribe({
      next: (data) => {
        this.industries = data;
      },
      error: (error) => {
        console.error('Error loading industries:', error);
      }
    });

    // Load distributors
    this.distributorService.getAll().subscribe({
      next: (data) => {
        this.distributors = data;
      },
      error: (error) => {
        console.error('Error loading distributors:', error);
      }
    });
  }

  generateReport(): void {
    this.currentPage = 1;
    this.loadReport();
  }

  loadReport(): void {
    this.loading = true;
    this.error = null;

    const request: ContractPricingReportRequest = {
      contractNumber: this.contractNumber || undefined,
      contractName: this.contractName || undefined,
      manufacturerId: this.selectedManufacturerId || undefined,
      productId: this.selectedProductId || undefined,
      opCoId: this.selectedOpCoId || undefined,
      industryId: this.selectedIndustryId || undefined,
      distributorId: this.selectedDistributorId || undefined,
      startDateFrom: this.startDateFrom ? new Date(this.startDateFrom) : undefined,
      startDateTo: this.startDateTo ? new Date(this.startDateTo) : undefined,
      endDateFrom: this.endDateFrom ? new Date(this.endDateFrom) : undefined,
      endDateTo: this.endDateTo ? new Date(this.endDateTo) : undefined,
      page: this.currentPage,
      pageSize: this.pageSize
    };

    this.reportService.generateReport(request).subscribe({
      next: (data) => {
        this.reportData = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.message || 'Failed to generate report';
        this.loading = false;
      }
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadReport();
  }

  clearFilters(): void {
    this.contractNumber = '';
    this.contractName = '';
    this.selectedManufacturerId = null;
    this.selectedProductId = null;
    this.selectedOpCoId = null;
    this.selectedIndustryId = null;
    this.selectedDistributorId = null;
    this.startDateFrom = '';
    this.startDateTo = '';
    this.endDateFrom = '';
    this.endDateTo = '';
    this.reportData = null;
  }

  downloadExcel(): void {
    this.downloadingExcel = true;
    this.error = null;

    // Export all data (no paging for Excel)
    const request: ContractPricingReportRequest = {
      contractNumber: this.contractNumber || undefined,
      contractName: this.contractName || undefined,
      manufacturerId: this.selectedManufacturerId || undefined,
      productId: this.selectedProductId || undefined,
      opCoId: this.selectedOpCoId || undefined,
      industryId: this.selectedIndustryId || undefined,
      distributorId: this.selectedDistributorId || undefined,
      startDateFrom: this.startDateFrom ? new Date(this.startDateFrom) : undefined,
      startDateTo: this.startDateTo ? new Date(this.startDateTo) : undefined,
      endDateFrom: this.endDateFrom ? new Date(this.endDateFrom) : undefined,
      endDateTo: this.endDateTo ? new Date(this.endDateTo) : undefined,
      page: 1,
      pageSize: 999999 // Get all records for Excel
    };

    this.reportService.downloadExcel(request).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Contract_Pricing_Report_${new Date().getTime()}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.downloadingExcel = false;
      },
      error: (err) => {
        this.error = err?.error?.message || 'Failed to download Excel file';
        this.downloadingExcel = false;
      }
    });
  }
}

