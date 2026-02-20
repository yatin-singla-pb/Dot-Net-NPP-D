import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { BulkRenewalService } from '../../services/bulk-renewal.service';
import { BulkRenewalRequest, PricingAdjustment } from '../../models/bulk-renewal.model';

export interface BulkRenewalDialogData {
  contractIds: number[];
  contractCount: number;
}

@Component({
  selector: 'app-bulk-renewal-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatIconModule
  ],
  templateUrl: './bulk-renewal-dialog.component.html',
  styleUrls: ['./bulk-renewal-dialog.component.css']
})
export class BulkRenewalDialogComponent implements OnInit {
  percentageChange: number = 0;
  minimumQuantityThreshold?: number;
  applyToAllProducts: boolean = true;
  proposalDueDate?: Date;
  processing: boolean = false;
  errorMessage?: string;

  constructor(
    public dialogRef: MatDialogRef<BulkRenewalDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: BulkRenewalDialogData,
    private bulkRenewalService: BulkRenewalService
  ) { }

  ngOnInit(): void {
    // Set default due date to 30 days from now
    const defaultDueDate = new Date();
    defaultDueDate.setDate(defaultDueDate.getDate() + 30);
    this.proposalDueDate = defaultDueDate;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    this.processing = true;
    this.errorMessage = undefined;

    const request: BulkRenewalRequest = {
      contractIds: this.data.contractIds,
      proposalDueDate: this.proposalDueDate,
      additionalProductIds: []
    };

    // Only add pricing adjustment if percentage change is not zero
    if (this.percentageChange !== 0) {
      request.pricingAdjustment = {
        percentageChange: this.percentageChange,
        minimumQuantityThreshold: this.minimumQuantityThreshold,
        applyToAllProducts: this.applyToAllProducts
      };
    }

    this.bulkRenewalService.createBulkRenewal(request).subscribe({
      next: (response) => {
        this.processing = false;
        this.dialogRef.close(response);
      },
      error: (error) => {
        this.processing = false;
        this.errorMessage = error.error?.message || 'An error occurred while creating renewal proposals';
        console.error('Bulk renewal error:', error);
      }
    });
  }

  get isValid(): boolean {
    return this.data.contractIds.length > 0;
  }

  get adjustmentDescription(): string {
    if (this.percentageChange === 0) {
      return 'No price adjustment';
    }

    const direction = this.percentageChange > 0 ? 'increase' : 'decrease';
    const absChange = Math.abs(this.percentageChange);
    
    let desc = `${absChange}% ${direction}`;
    
    if (!this.applyToAllProducts && this.minimumQuantityThreshold) {
      desc += ` (only for quantities >= ${this.minimumQuantityThreshold})`;
    }
    
    return desc;
  }
}

