import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { ContractService } from '../../services/contract.service';
import { ProductService } from '../../services/product.service';
import { CreateContractVersionRequest, ContractVersion } from '../../models/contract-version.model';

@Component({
  selector: 'app-contract-edit-version',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './contract-edit-version.component.html',
  styleUrls: ['./contract-edit-version.component.css']
})
export class ContractEditVersionComponent implements OnInit, OnDestroy {
  contractId!: number;
  sourceVersionId?: number;

  loading = false;
  submitting = false;
  error: string | null = null;
  success: string | null = null;

  products: { id: number; name: string }[] = [];

  form!: FormGroup;

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private contractService: ContractService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.contractId = +(this.route.snapshot.paramMap.get('id') || 0);
    this.sourceVersionId = +(this.route.snapshot.queryParamMap.get('sourceVersionId') || 0) || undefined;

    // build form after DI available
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      foreignContractId: [''],
      sendToPerformance: [false],
      internalNotes: [''],
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: [''],
      manufacturerBillbackName: [''],
      manufacturerTermsAndConditions: [''],
      manufacturerNotes: [''],
      contactPerson: [''],
      entegraContractType: [''],
      entegraVdaProgram: [''],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      prices: this.fb.array([])
    });

    this.loadProducts();

    if (this.sourceVersionId) {
      this.loading = true;
      this.contractService.getVersion(this.contractId, this.sourceVersionId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (v: ContractVersion) => {
            this.loading = false;
            this.form.patchValue({
              name: v.name,
              foreignContractId: v.foreignContractId || '',
              sendToPerformance: !!v.sendToPerformance,
              internalNotes: v.internalNotes || '',
              // Manufacturer/Entegra metadata
              manufacturerReferenceNumber: v.manufacturerReferenceNumber || '',
              manufacturerBillbackName: v.manufacturerBillbackName || '',
              manufacturerTermsAndConditions: v.manufacturerTermsAndConditions || '',
              manufacturerNotes: v.manufacturerNotes || '',
              contactPerson: v.contactPerson || '',
              entegraContractType: v.entegraContractType || '',
              entegraVdaProgram: v.entegraVdaProgram || '',
              startDate: this.toDateInput(v.startDate),
              endDate: this.toDateInput(v.endDate)
            });
            (v.prices || []).forEach(p => this.addPrice({
              productId: p.productId,
              price: p.price ?? null,
              priceType: p.priceType || '',
              uom: p.uom || '',
              tier: p.tier || '',
              effectiveFrom: p.effectiveFrom ? this.toDateInput(p.effectiveFrom) : '',
              effectiveTo: p.effectiveTo ? this.toDateInput(p.effectiveTo) : '',
              pua: p.pua ?? null,
              ffsPrice: p.ffsPrice ?? null,
              noiPrice: p.noiPrice ?? null,
              ptv: p.ptv ?? null,
              internalNotes: p.internalNotes ?? null
            }));
          },
          error: err => { this.loading = false; this.error = err.message || 'Failed to load version'; }
        });
    }
  }

  get prices(): FormArray { return this.form.get('prices') as FormArray; }

  addPrice(init?: any): void {
    this.prices.push(this.fb.group({
      productId: [init?.productId || null, Validators.required],
      price: [init?.price ?? null],
      priceType: [init?.priceType || ''],
      uom: [init?.uom || ''],
      tier: [init?.tier || ''],
      effectiveFrom: [init?.effectiveFrom || ''],
      effectiveTo: [init?.effectiveTo || ''],
      // New fields
      pua: [init?.pua ?? null],
      ffsPrice: [init?.ffsPrice ?? null],
      noiPrice: [init?.noiPrice ?? null],
      ptv: [init?.ptv ?? null],
      internalNotes: [init?.internalNotes || '']
    }));
  }

  removePrice(i: number): void { this.prices.removeAt(i); }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.submitting = true;

    const val = this.form.value;
    const payload: CreateContractVersionRequest = {
      name: val.name,
      foreignContractId: val.foreignContractId || null,
      sendToPerformance: !!val.sendToPerformance,
      internalNotes: val.internalNotes || null,
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: (val.manufacturerReferenceNumber || '').toString().trim() || null,
      manufacturerBillbackName: (val.manufacturerBillbackName || '').toString().trim() || null,
      manufacturerTermsAndConditions: (val.manufacturerTermsAndConditions || '').toString().trim() || null,
      manufacturerNotes: (val.manufacturerNotes || '').toString().trim() || null,
      contactPerson: (val.contactPerson || '').toString().trim() || null,
      entegraContractType: (val.entegraContractType || '').toString().trim() || null,
      entegraVdaProgram: (val.entegraVdaProgram || '').toString().trim() || null,
      startDate: new Date(val.startDate),
      endDate: new Date(val.endDate),
      prices: (val.prices || []).map((p: any) => ({
        productId: +p.productId,
        price: p.price !== '' ? +p.price : null,
        priceType: p.priceType || null,
        uom: p.uom || null,
        tier: p.tier || null,
        effectiveFrom: p.effectiveFrom ? new Date(p.effectiveFrom) : null,
        effectiveTo: p.effectiveTo ? new Date(p.effectiveTo) : null,
        pua: p.pua !== '' ? +p.pua : null,
        ffsPrice: p.ffsPrice !== '' ? +p.ffsPrice : null,
        noiPrice: p.noiPrice !== '' ? +p.noiPrice : null,
        ptv: p.ptv !== '' ? +p.ptv : null,
        internalNotes: (p.internalNotes || '').toString().trim() || null
      })),
      sourceVersionId: this.sourceVersionId
    };

    this.contractService.createVersion(this.contractId, payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: _ => {
          this.submitting = false;
          this.router.navigate([`/admin/contracts/view/${this.contractId}`]);
        },
        error: err => {
          this.submitting = false;
          this.error = err.message || 'Failed to save version';
        }
      });
  }

  cancel(): void { this.router.navigate([`/admin/contracts/view/${this.contractId}`]); }

  private toDateInput(d: Date | string): string {
    const dt = new Date(d);
    return new Date(dt.getTime() - dt.getTimezoneOffset() * 60000).toISOString().slice(0,10);
  }

  private loadProducts(): void {
    this.productService.getPaginated(1, 500).pipe(takeUntil(this.destroy$)).subscribe({
      next: res => this.products = (res.items || []).map((p: any) => ({ id: p.id, name: p.name })),
      error: _ => {}
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}

