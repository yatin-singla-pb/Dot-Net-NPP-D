# Bulk Contract Renewal - Integration Guide

## 🎯 How to Integrate with Contract Listing Page

This guide shows how to add the bulk renewal feature to your existing contract listing component.

---

## Step 1: Update Contract List Component TypeScript

Add the following to your contract list component (e.g., `contract-list.component.ts`):

```typescript
import { MatDialog } from '@angular/material/dialog';
import { BulkRenewalDialogComponent } from '../bulk-renewal-dialog/bulk-renewal-dialog.component';
import { BulkRenewalResponse } from '../../models/bulk-renewal.model';

export class ContractListComponent implements OnInit {
  // Add these properties
  selectedContractIds: Set<number> = new Set();
  selectAll: boolean = false;

  constructor(
    // ... existing dependencies
    private dialog: MatDialog
  ) { }

  // Add these methods

  /**
   * Toggle selection of a single contract
   */
  toggleContractSelection(contractId: number): void {
    if (this.selectedContractIds.has(contractId)) {
      this.selectedContractIds.delete(contractId);
    } else {
      this.selectedContractIds.add(contractId);
    }
    this.updateSelectAllState();
  }

  /**
   * Toggle select all contracts
   */
  toggleSelectAll(): void {
    if (this.selectAll) {
      // Deselect all
      this.selectedContractIds.clear();
    } else {
      // Select all visible contracts
      this.contracts.forEach(contract => {
        this.selectedContractIds.add(contract.id);
      });
    }
    this.selectAll = !this.selectAll;
  }

  /**
   * Update select all checkbox state
   */
  updateSelectAllState(): void {
    this.selectAll = this.contracts.length > 0 && 
                     this.selectedContractIds.size === this.contracts.length;
  }

  /**
   * Check if a contract is selected
   */
  isContractSelected(contractId: number): boolean {
    return this.selectedContractIds.has(contractId);
  }

  /**
   * Open bulk renewal dialog
   */
  openBulkRenewalDialog(): void {
    if (this.selectedContractIds.size === 0) {
      alert('Please select at least one contract');
      return;
    }

    const dialogRef = this.dialog.open(BulkRenewalDialogComponent, {
      width: '600px',
      data: {
        contractIds: Array.from(this.selectedContractIds),
        contractCount: this.selectedContractIds.size
      }
    });

    dialogRef.afterClosed().subscribe((result: BulkRenewalResponse) => {
      if (result) {
        this.handleBulkRenewalResult(result);
      }
    });
  }

  /**
   * Handle bulk renewal result
   */
  handleBulkRenewalResult(result: BulkRenewalResponse): void {
    if (result.success) {
      alert(`Successfully created ${result.successfulProposals} renewal proposal(s)!`);
      this.selectedContractIds.clear();
      this.selectAll = false;
      // Optionally refresh the contract list
      this.loadContracts();
    } else {
      const message = `Created ${result.successfulProposals} of ${result.totalContracts} proposals.\n` +
                     `${result.failedProposals} failed. Check the results for details.`;
      alert(message);
    }
  }

  /**
   * Check if user has permission for bulk renewal
   */
  canUseBulkRenewal(): boolean {
    // Check if user has System Administrator or Contract Manager role
    const userRoles = this.authService.getUserRoles(); // Adjust based on your auth service
    return userRoles.includes('System Administrator') || 
           userRoles.includes('Contract Manager');
  }
}
```

---

## Step 2: Update Contract List Component HTML

Add checkboxes and bulk action button to your template:

```html
<!-- Add this button above the contract table -->
<div class="bulk-actions" *ngIf="canUseBulkRenewal()">
  <button mat-raised-button color="primary" 
          (click)="openBulkRenewalDialog()"
          [disabled]="selectedContractIds.size === 0">
    <mat-icon>autorenew</mat-icon>
    Create Renewal Proposals ({{ selectedContractIds.size }})
  </button>
</div>

<!-- Update your table to include checkboxes -->
<table class="table">
  <thead>
    <tr>
      <!-- Add checkbox column -->
      <th *ngIf="canUseBulkRenewal()">
        <mat-checkbox [(ngModel)]="selectAll" 
                      (change)="toggleSelectAll()">
        </mat-checkbox>
      </th>
      <th>Contract Number</th>
      <th>Manufacturer</th>
      <th>Distributor</th>
      <th>Start Date</th>
      <th>End Date</th>
      <th>Actions</th>
    </tr>
  </thead>
  <tbody>
    <tr *ngFor="let contract of contracts">
      <!-- Add checkbox cell -->
      <td *ngIf="canUseBulkRenewal()">
        <mat-checkbox [checked]="isContractSelected(contract.id)"
                      (change)="toggleContractSelection(contract.id)">
        </mat-checkbox>
      </td>
      <td>{{ contract.contractNumber }}</td>
      <td>{{ contract.manufacturerName }}</td>
      <td>{{ contract.distributorName }}</td>
      <td>{{ contract.startDate | date }}</td>
      <td>{{ contract.endDate | date }}</td>
      <td>
        <!-- Existing actions -->
      </td>
    </tr>
  </tbody>
</table>
```

---

## Step 3: Add Styles

Add these styles to your component CSS:

```css
.bulk-actions {
  margin-bottom: 16px;
  padding: 12px;
  background-color: #f5f5f5;
  border-radius: 4px;
  display: flex;
  gap: 12px;
  align-items: center;
}

.bulk-actions button {
  display: flex;
  align-items: center;
  gap: 8px;
}

.bulk-actions mat-icon {
  font-size: 20px;
  height: 20px;
  width: 20px;
}
```

---

## Step 4: Import Required Modules

Make sure your component imports the necessary modules:

```typescript
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  // ...
  imports: [
    // ... existing imports
    MatCheckboxModule,
    MatDialogModule,
    MatIconModule,
    MatButtonModule
  ]
})
```

---

## Step 5: Add Result Display Component (Optional)

Create a component to show detailed results:

```typescript
// bulk-renewal-results.component.ts
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BulkRenewalResponse } from '../../models/bulk-renewal.model';

@Component({
  selector: 'app-bulk-renewal-results',
  template: `
    <h2 mat-dialog-title>Bulk Renewal Results</h2>
    <mat-dialog-content>
      <div class="summary">
        <p><strong>Total Contracts:</strong> {{ data.totalContracts }}</p>
        <p><strong>Successful:</strong> {{ data.successfulProposals }}</p>
        <p><strong>Failed:</strong> {{ data.failedProposals }}</p>
      </div>

      <h3>Details</h3>
      <table class="results-table">
        <thead>
          <tr>
            <th>Contract</th>
            <th>Status</th>
            <th>Proposal ID</th>
            <th>Products</th>
            <th>Error</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let result of data.results" 
              [class.success]="result.success"
              [class.error]="!result.success">
            <td>{{ result.contractNumber }}</td>
            <td>
              <mat-icon *ngIf="result.success" color="primary">check_circle</mat-icon>
              <mat-icon *ngIf="!result.success" color="warn">error</mat-icon>
            </td>
            <td>{{ result.proposalId || '-' }}</td>
            <td>{{ result.productCount }}</td>
            <td>{{ result.errorMessage || '-' }}</td>
          </tr>
        </tbody>
      </table>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Close</button>
      <button mat-raised-button color="primary" 
              *ngIf="data.createdProposalIds.length > 0"
              (click)="viewProposals()">
        View Proposals
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .summary {
      background-color: #f5f5f5;
      padding: 16px;
      border-radius: 4px;
      margin-bottom: 16px;
    }
    .results-table {
      width: 100%;
      border-collapse: collapse;
    }
    .results-table th,
    .results-table td {
      padding: 8px;
      text-align: left;
      border-bottom: 1px solid #ddd;
    }
    .success {
      background-color: #e8f5e9;
    }
    .error {
      background-color: #ffebee;
    }
  `]
})
export class BulkRenewalResultsComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: BulkRenewalResponse) {}

  viewProposals(): void {
    // Navigate to proposals page
    // this.router.navigate(['/admin/proposals']);
  }
}
```

Then update the `handleBulkRenewalResult` method to show this dialog:

```typescript
handleBulkRenewalResult(result: BulkRenewalResponse): void {
  this.dialog.open(BulkRenewalResultsComponent, {
    width: '800px',
    data: result
  });
  
  this.selectedContractIds.clear();
  this.selectAll = false;
}
```

---

## 🎯 Summary

After following these steps, your contract listing page will have:

✅ Checkboxes for selecting multiple contracts  
✅ "Select All" functionality  
✅ "Create Renewal Proposals" button  
✅ Bulk renewal dialog integration  
✅ Results display  
✅ Role-based access control  

The feature will be fully functional and ready to use!

