import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ContractOverTermReportService } from '../../services/contract-over-term-report.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { ProductService } from '../../services/product.service';
import { OpCoService } from '../../services/opco.service';
import { 
  ContractOverTermReportRequest, 
  ContractOverTermReportResponse,
  ContractOverTermReportRow 
} from '../../models/contract-over-term-report.model';
import { Manufacturer } from '../../models/manufacturer.model';
import { Product } from '../../models/product.model';
import { OpCo } from '../../models/opco.model';

@Component({
  selector: 'app-contract-over-term-report',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './contract-over-term-report.component.html',
  styleUrls: ['./contract-over-term-report.component.css']
})
export class ContractOverTermReportComponent implements OnInit {
  // Expose Math for template
  Math = Math;

  // Filters
  pointInTime: string = new Date().toISOString().split('T')[0];
  maxTermsBack: number = 3;
  contractNumber: string = '';
  selectedManufacturerId: number | null = null;
  selectedProductId: number | null = null;
  selectedOpCoId: number | null = null;

  // Paging
  currentPage: number = 1;
  pageSize: number = 50;

  // Lookup data
  manufacturers: Manufacturer[] = [];
  products: Product[] = [];
  opCos: OpCo[] = [];

  // Report data
  reportData: ContractOverTermReportResponse | null = null;
  loading = false;
  error: string | null = null;

  // Excel download
  downloadingExcel = false;

  constructor(
    private reportService: ContractOverTermReportService,
    private manufacturerService: ManufacturerService,
    private productService: ProductService,
    private opCoService: OpCoService
  ) { }

  ngOnInit(): void {
    this.loadLookupData();
  }

  private loadLookupData(): void {
    // Load manufacturers
    this.manufacturerService.getAll().subscribe({
      next: (data) => this.manufacturers = data || [],
      error: (err) => console.error('Error loading manufacturers', err)
    });

    // Load products
    this.productService.getAll().subscribe({
      next: (data) => this.products = data || [],
      error: (err) => console.error('Error loading products', err)
    });

    // Load OpCos
    this.opCoService.getAll().subscribe({
      next: (data) => this.opCos = data || [],
      error: (err) => console.error('Error loading OpCos', err)
    });
  }

  onManufacturerChange(): void {
    // Reload products for selected manufacturer
    if (this.selectedManufacturerId) {
      this.productService.getByManufacturer(this.selectedManufacturerId).subscribe({
        next: (data) => this.products = data || [],
        error: (err) => console.error('Error loading products', err)
      });
    } else {
      this.productService.getAll().subscribe({
        next: (data) => this.products = data || [],
        error: (err) => console.error('Error loading products', err)
      });
    }
    this.selectedProductId = null;
  }

  generateReport(): void {
    this.currentPage = 1; // Reset to first page
    this.loadReport();
  }

  loadReport(): void {
    this.loading = true;
    this.error = null;

    const request: ContractOverTermReportRequest = {
      pointInTime: this.pointInTime,
      maxTermsBack: this.maxTermsBack,
      contractNumber: this.contractNumber || undefined,
      manufacturerId: this.selectedManufacturerId || undefined,
      productId: this.selectedProductId || undefined,
      opCoId: this.selectedOpCoId || undefined,
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

  onPageSizeChange(): void {
    this.currentPage = 1;
    this.loadReport();
  }

  getPageNumbers(): number[] {
    if (!this.reportData) return [];
    const totalPages = this.reportData.totalPages || 1;
    const pages: number[] = [];

    // Show max 10 page numbers
    const maxPages = 10;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
    let endPage = Math.min(totalPages, startPage + maxPages - 1);

    if (endPage - startPage < maxPages - 1) {
      startPage = Math.max(1, endPage - maxPages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }

  downloadExcel(): void {
    this.downloadingExcel = true;
    this.error = null;

    // Export all data (no paging for Excel)
    const request: ContractOverTermReportRequest = {
      pointInTime: this.pointInTime,
      maxTermsBack: this.maxTermsBack,
      contractNumber: this.contractNumber || undefined,
      manufacturerId: this.selectedManufacturerId || undefined,
      productId: this.selectedProductId || undefined,
      opCoId: this.selectedOpCoId || undefined,
      page: 1,
      pageSize: 999999 // Get all records for Excel
    };

    this.reportService.downloadExcel(request).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Contract_Over_Term_Report_${new Date().getTime()}.xlsx`;
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

  getTermHeaders(): number[] {
    if (!this.reportData) return [];
    return Array.from({ length: this.reportData.maxTermsBack }, (_, i) => i + 1);
  }

  getPreviousTerm(row: ContractOverTermReportRow, termNumber: number) {
    return row.previousTerms.find(pt => pt.termNumber === termNumber);
  }
}

