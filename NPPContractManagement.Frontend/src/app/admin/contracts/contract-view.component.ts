import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { Contract } from '../../models/contract.model';
import { ContractVersion } from '../../models/contract-version.model';
import { ContractService } from '../../services/contract.service';
import { ContractFormComponent } from './contract-form.component';
import { ContractAssignmentsComponent } from './contract-assignments.component';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-contract-view',
  standalone: true,
  imports: [CommonModule, RouterModule, ContractFormComponent, ContractAssignmentsComponent],
  templateUrl: './contract-view.component.html',
  styleUrls: ['./contract-view.component.css']
})
export class ContractViewComponent implements OnInit, OnDestroy {
  contractId!: number;
  contract: Contract | null = null;
  versions: ContractVersion[] = [];
  selectedVersion: ContractVersion | null = null;
  loading = false;
  error: string | null = null;
  isAdmin = false;

  // Toast for "created from proposal"
  createdFromProposalId: number | null = null;
  showCreatedFromProposalToast = false;

  // Confirmation modals and action toast
  showSuspendModal = false;
  showUnsuspendModal = false;
  showActionToast = false;
  actionToastMessage: string | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private contractService: ContractService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.contractId = +(this.route.snapshot.paramMap.get('id') || 0);
    if (!this.contractId) { this.error = 'Invalid contract id'; return; }

    this.isAdmin = this.authService.hasRole('System Administrator');

    // Toast when navigated from proposal
    const fromProposal = this.route.snapshot.queryParamMap.get('fromProposal');
    if (fromProposal) {
      this.createdFromProposalId = +fromProposal;
      this.showCreatedFromProposalToast = true;
      setTimeout(() => this.showCreatedFromProposalToast = false, 5000);
    }

    this.loadData();
  }

  private loadData(): void {
    this.loading = true;
    this.contractService.getById(this.contractId).pipe(takeUntil(this.destroy$)).subscribe({
      next: c => { this.contract = c; this.loading = false; },
      error: err => { this.error = err.message || 'Failed to load contract'; this.loading = false; }
    });
    this.contractService.getVersions(this.contractId).pipe(takeUntil(this.destroy$)).subscribe({
      next: v => {
        this.versions = v;
        this.selectedVersion = v.length ? v[0] : null; // assume API returns desc by VersionNumber
      },
      error: err => { this.error = err.message || 'Failed to load versions'; }
    });
  }

  selectVersion(v: ContractVersion): void { this.selectedVersion = v; }

  onDuplicateAmend(): void {
    const sourceVersionId = this.selectedVersion?.id;
    this.router.navigate([`/admin/contracts/${this.contractId}/versions/new`], { queryParams: { sourceVersionId } });
  }

  onSuspend(): void {
    if (!this.contract) return;
    this.openSuspendModal();
  }

  onUnsuspend(): void {
    if (!this.contract) return;
    this.openUnsuspendModal();
  }

  openSuspendModal(): void { this.showSuspendModal = true; }
  closeSuspendModal(): void { this.showSuspendModal = false; }
  confirmSuspend(): void {
    this.loading = true;
    this.contractService.suspend(this.contractId).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.actionToastMessage = 'Contract suspended successfully';
        this.showActionToast = true;
        setTimeout(() => this.showActionToast = false, 3000);
        this.closeSuspendModal();
        this.loadData();
      },
      error: err => { this.error = err.message || 'Failed to suspend contract'; this.loading = false; }
    });
  }

  openUnsuspendModal(): void { this.showUnsuspendModal = true; }
  closeUnsuspendModal(): void { this.showUnsuspendModal = false; }
  confirmUnsuspend(): void {
    this.loading = true;
    this.contractService.unsuspend(this.contractId).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.actionToastMessage = 'Contract unsuspended successfully';
        this.showActionToast = true;
        setTimeout(() => this.showActionToast = false, 3000);
        this.closeUnsuspendModal();
        this.loadData();
      },
      error: err => { this.error = err.message || 'Failed to unsuspend contract'; this.loading = false; }
    });
  }

  closeActionToast(): void { this.showActionToast = false; }

  closeCreatedFromProposalToast(): void { this.showCreatedFromProposalToast = false; }

  trackByVersion = (_: number, v: ContractVersion) => v.id;

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}

