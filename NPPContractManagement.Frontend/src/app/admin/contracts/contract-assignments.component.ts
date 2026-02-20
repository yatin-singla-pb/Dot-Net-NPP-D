import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { ContractService } from '../../services/contract.service';
import { ContractDistributorAssignment, ContractIndustryAssignment, ContractManufacturerAssignment, ContractOpCoAssignment } from '../../models/contract-assignments.model';

@Component({
  selector: 'app-contract-assignments',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './contract-assignments.component.html',
  styleUrls: ['./contract-assignments.component.css']
})
export class ContractAssignmentsComponent implements OnInit, OnDestroy {
  @Input() readonlyMode = false;
  contractId!: number;

  distributors: ContractDistributorAssignment[] = [];
  manufacturers: ContractManufacturerAssignment[] = [];
  opcos: ContractOpCoAssignment[] = [];
  industries: ContractIndustryAssignment[] = [];

  loading = false;
  error: string | null = null;

  distributorForm!: FormGroup;
  manufacturerForm!: FormGroup;
  opcoForm!: FormGroup;
  industryForm!: FormGroup;

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private contractService: ContractService
  ) {}

  ngOnInit(): void {
    this.contractId = +(this.route.snapshot.paramMap.get('id') || 0);

    this.distributorForm = this.fb.group({
      distributorId: [null, [Validators.required]],
      currentVersionNumber: [1, [Validators.required, Validators.min(1)]],
      assignedBy: [''],
      assignedDate: ['']
    });

    this.manufacturerForm = this.fb.group({
      manufacturerId: [null, [Validators.required]],
      currentVersionNumber: [1, [Validators.required, Validators.min(1)]],
      assignedBy: [''],
      assignedDate: ['']
    });

    this.opcoForm = this.fb.group({
      opCoId: [null, [Validators.required]],
      currentVersionNumber: [1, [Validators.required, Validators.min(1)]],
      assignedBy: [''],
      assignedDate: ['']
    });

    this.industryForm = this.fb.group({
      industryId: [null, [Validators.required]],
      currentVersionNumber: [1, [Validators.required, Validators.min(1)]],
      assignedBy: [''],
      assignedDate: ['']
    });

    // If readonly via query string (defensive)
    const roParam = (this.route as any)?.snapshot?.queryParamMap?.get?.('readonly');
    if (this.readonlyMode || roParam === '1' || (roParam || '').toLowerCase() === 'true') {
      this.readonlyMode = true;
      this.distributorForm.disable();
      this.manufacturerForm.disable();
      this.opcoForm.disable();
      this.industryForm.disable();
    }

    this.reloadAll();
  }

  reloadAll(): void {
    this.loading = true;
    this.contractService.getDistributorAssignments(this.contractId).pipe(takeUntil(this.destroy$)).subscribe({
      next: list => this.distributors = list,
      error: err => this.error = err.message || 'Failed to load distributors'
    });
    this.contractService.getManufacturerAssignments(this.contractId).pipe(takeUntil(this.destroy$)).subscribe({
      next: list => this.manufacturers = list,
      error: err => this.error = err.message || 'Failed to load manufacturers'
    });
    this.contractService.getOpCoAssignments(this.contractId).pipe(takeUntil(this.destroy$)).subscribe({
      next: list => this.opcos = list,
      error: err => this.error = err.message || 'Failed to load opcos'
    });
    this.contractService.getIndustryAssignments(this.contractId).pipe(takeUntil(this.destroy$)).subscribe({
      next: list => { this.industries = list; this.loading = false; },
      error: err => { this.error = err.message || 'Failed to load industries'; this.loading = false; }
    });
  }

  addDistributor(): void {
    if (this.readonlyMode) return;
    if (this.distributorForm.invalid) { this.distributorForm.markAllAsTouched(); return; }
    const val = this.distributorForm.value;
    this.contractService.addDistributorAssignment(this.contractId, {
      distributorId: +val.distributorId,
      currentVersionNumber: +val.currentVersionNumber,
      assignedBy: val.assignedBy || null,
      assignedDate: val.assignedDate || null
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: _ => { this.distributorForm.reset({ currentVersionNumber: 1 }); this.reloadAll(); },
      error: err => this.error = err.message || 'Failed to add distributor'
    });
  }

  removeDistributor(id: number): void {
    if (this.readonlyMode) return;
    this.contractService.removeDistributorAssignment(this.contractId, id)
      .pipe(takeUntil(this.destroy$)).subscribe({ next: _ => this.reloadAll() });
  }

  addManufacturer(): void {
    if (this.readonlyMode) return;
    if (this.manufacturerForm.invalid) { this.manufacturerForm.markAllAsTouched(); return; }
    const val = this.manufacturerForm.value;
    this.contractService.addManufacturerAssignment(this.contractId, {
      manufacturerId: +val.manufacturerId,
      currentVersionNumber: +val.currentVersionNumber,
      assignedBy: val.assignedBy || null,
      assignedDate: val.assignedDate || null
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: _ => { this.manufacturerForm.reset({ currentVersionNumber: 1 }); this.reloadAll(); },
      error: err => this.error = err.message || 'Failed to add manufacturer'
    });
  }

  removeManufacturer(id: number): void {
    if (this.readonlyMode) return;
    this.contractService.removeManufacturerAssignment(this.contractId, id)
      .pipe(takeUntil(this.destroy$)).subscribe({ next: _ => this.reloadAll() });
  }

  addOpCo(): void {
    if (this.readonlyMode) return;
    if (this.opcoForm.invalid) { this.opcoForm.markAllAsTouched(); return; }
    const val = this.opcoForm.value;
    this.contractService.addOpCoAssignment(this.contractId, {
      opCoId: +val.opCoId,
      currentVersionNumber: +val.currentVersionNumber,
      assignedBy: val.assignedBy || null,
      assignedDate: val.assignedDate || null
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: _ => { this.opcoForm.reset({ currentVersionNumber: 1 }); this.reloadAll(); },
      error: err => this.error = err.message || 'Failed to add OpCo'
    });
  }

  removeOpCo(id: number): void {
    if (this.readonlyMode) return;
    this.contractService.removeOpCoAssignment(this.contractId, id)
      .pipe(takeUntil(this.destroy$)).subscribe({ next: _ => this.reloadAll() });
  }

  addIndustry(): void {
    if (this.readonlyMode) return;
    if (this.industryForm.invalid) { this.industryForm.markAllAsTouched(); return; }
    const val = this.industryForm.value;
    this.contractService.addIndustryAssignment(this.contractId, {
      industryId: +val.industryId,
      currentVersionNumber: +val.currentVersionNumber,
      assignedBy: val.assignedBy || null,
      assignedDate: val.assignedDate || null
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: _ => { this.industryForm.reset({ currentVersionNumber: 1 }); this.reloadAll(); },
      error: err => this.error = err.message || 'Failed to add industry'
    });
  }

  removeIndustry(id: number): void {
    if (this.readonlyMode) return;
    this.contractService.removeIndustryAssignment(this.contractId, id)
      .pipe(takeUntil(this.destroy$)).subscribe({ next: _ => this.reloadAll() });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}

