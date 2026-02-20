import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors, ValidatorFn, FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ContractService } from '../../services/contract.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { IndustryService } from '../../services/industry.service';
import { DistributorService } from '../../services/distributor.service';
import { OpCoService } from '../../services/opco.service';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';

import { Product } from '../../models/product.model';
import { Contract, CreateContractRequest, UpdateContractRequest, ContractStatus } from '../../models/contract.model';
import { ContractVersion, CreateContractVersionRequest } from '../../models/contract-version.model';
import { BulkRenewalDialogComponent } from '../../components/bulk-renewal-dialog/bulk-renewal-dialog.component';
import { ProductSearchModalComponent } from '../../shared/components/product-search-modal/product-search-modal.component';
import { BulkRenewalResponse } from '../../models/bulk-renewal.model';
import { forkJoin, of } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';

@Component({
  selector: 'app-contract-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterModule, MatDialogModule, ProductSearchModalComponent],
  templateUrl: './contract-form.component.html',
  styleUrls: ['./contract-form.component.css']
})
export class ContractFormComponent implements OnInit, OnChanges {
  form: FormGroup;
  isEditMode = false;
  @Input() readonlyMode = false;
  @Input() value: Contract | null = null;
  @Input() versionOverride: ContractVersion | null = null;
  @Output() duplicateAmend = new EventEmitter<void>();

  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  // Toast + confirmation modal state for suspend/unsuspend
  showActionToast = false;
  actionToastMessage: string | null = null;
  showSuspendModal = false;
  showUnsuspendModal = false;

  validationErrors: string[] = [];
  contract: Contract | null = null;
  distributors: { id: number; name: string }[] = [];
  filteredDistributors: { id: number; name: string }[] = [];
  distributorFilter = '';
  opcos: { id: number; name: string }[] = [];
  filteredOpcos: { id: number; name: string }[] = [];
  opcoFilter = '';
  products: { id: number; name: string }[] = [];
  filteredProducts: { id: number; name: string }[] = [];
  productFilter = '';

  // Pricing rows keyed by productId
  priceRows: Array<{
    productId: number;
    priceType: string;
    uom: string;
    billbacksAllowed: boolean;
    allowance?: number;
    commercialDelPrice?: number;
    commercialFobPrice?: number;
    commodityDelPrice?: number;
    commodityFobPrice?: number;
    estimatedQty?: number;
    pua?: number;
    ffsPrice?: number;
    noiPrice?: number;
    ptv?: number;
    internalNotes?: string;
  }> = [];

  readonly uoms = ['Cases', 'Pounds'];
  // Canonical price types matching backend persistence
  readonly priceTypes = [
    'Contract Price',
    'Contract Price at Time of Purchase',
    'List at Time of Purchase/No Bid',
    'Suspended'
  ];

  private normalizePriceType(input: any): string {
    const s = String(input || '').trim().toLowerCase();
    if (!s) return this.priceTypes[0];
    if (s === 'suspended' || s === 'product suspended') return 'Suspended';
    if (
      s === 'list at time of purchase/no bid' ||
      s === 'list at time of purchase / no bid' ||
      s === 'published list price at time of purchase' ||
      s === 'list at time of purchase' ||
      s === 'no bid'
    ) return 'List at Time of Purchase/No Bid';
    if (s === 'contract price at time of purchase' || s === 'guaranteed price') return 'Contract Price at Time of Purchase';
    if (s === 'contract price') return 'Contract Price';
    const exact = this.priceTypes.find(pt => pt.toLowerCase() === s);
    return exact || this.priceTypes[0];
  }


  manufacturers: { id: number; name: string }[] = [];
  filteredManufacturers: { id: number; name: string }[] = [];
  manufacturerFilter = '';
  industries: { id: number; name: string }[] = [];
  filteredIndustries: { id: number; name: string }[] = [];
  industryFilter = '';

  // Panels and selected badges (match User -> Manufacturers UX)
  selectedManufacturers: { id: number; name: string }[] = [];
  showManufacturerPanel = false;

  selectedIndustries: { id: number; name: string }[] = [];
  showIndustriesPanel = false;

  selectedDistributors: { id: number; name: string }[] = [];
  showDistributorsPanel = false;

  selectedOpcos: { id: number; name: string }[] = [];
  showOpcoPanel = false;

  selectedProducts: { id: number; name: string }[] = [];
  showProductsPanel = false;

  // Add Product Modal state
  showProductModal = false;
  // Legacy modal state retained for compatibility
  modalProductSearch = '';
  modalFilteredProducts: { id: number; name: string }[] = [];
  modalSelectedSet: Set<number> = new Set<number>();
  modalSelectAll = false;
  // New simplified modal bindings
  productSearch = '';
  selectAllProducts = false;
  selectedProductIds: Set<number> = new Set<number>();

  // Add Single Product Modal state
  showAddSingleProductModal = false;
  addSingleProduct = {
    productId: null as number | null,
    priceType: 'Contract Price',
    uom: 'Cases',
    estimatedQty: null as number | null,
    billbacksAllowed: false,
    allowance: null as number | null,
    commercialDelPrice: null as number | null,
    commercialFobPrice: null as number | null,
    commodityDelPrice: null as number | null,
    commodityFobPrice: null as number | null,
    pua: null as number | null,
    ffsPrice: null as number | null,
    noiPrice: null as number | null,
    ptv: null as number | null,
    internalNotes: ''
  };
  addSingleProductError: string | null = null;
  addSingleProductSearch = '';
  showProductSearchForAdd = false;
  selectedAddProduct: Product | null = null;

  get filteredAddSingleProducts(): { id: number; name: string }[] {
    const q = (this.addSingleProductSearch || '').toLowerCase();
    if (!q) return this.products || [];
    return (this.products || []).filter(p => (p.name || '').toLowerCase().includes(q));
  }

  // Store seeded product names from database to preserve them during reload
  private seededProductNames = new Map<number, string>();

  // Versioning
  versions: ContractVersion[] = [];
  selectedVersionNumber: number | null = null;
  actions: string[] = ['Clone', 'Compare'];
  selectedAction: string | null = null;
  isAdmin = false;
  showComparePanel = false;
  compareWithVersionNumber: number | null = null;
  diffResult: any = null;
  showDiffModal = false;

  get compareCandidates(): ContractVersion[] {
    const sel = Number(this.selectedVersionNumber);
    return (this.versions || []).filter(v => Number(v.versionNumber) !== sel);
  }

  // Clone mode (enables selective editing in read-only view)
  cloneMode = false;

  ContractStatus = ContractStatus;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private contractService: ContractService,
    private manufacturerService: ManufacturerService,
    private industryService: IndustryService,
    private distributorService: DistributorService,
    private opcoService: OpCoService,
    private productService: ProductService,
    private authService: AuthService,
    private dialog: MatDialog
  ) {
    this.form = this.createForm();
  }

  ngOnInit(): void {
    this.isAdmin = this.authService.hasRole('System Administrator');
    // allow query param to force readonly (robust for tests without queryParamMap)
    const qpm: any = (this.route as any)?.snapshot?.queryParamMap;
    const ro = qpm?.get ? (qpm.get('readonly') || qpm.get('viewOnly') || qpm.get('mode')) : null;
    if (ro && (ro === '1' || (typeof ro === 'string' && (ro.toLowerCase() === 'true' || ro.toLowerCase() === 'view')))) {
      this.readonlyMode = true;
    }

    const id = this.route.snapshot.paramMap.get('id');

    this.loadBaseLists$().subscribe(() => {
      if (this.value) {
        this.contract = this.value;
        this.populateForm(this.value);
        if (this.readonlyMode && !this.cloneMode) this.form.disable();
        this.loadDropdowns();
      } else if (id) {
        this.isEditMode = true;
        this.readonlyMode = true;
        this.loadDropdowns();
        this.loadContract(+id);
      } else {
        this.loadDropdowns();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['value'] && this.value) {
      this.contract = this.value;
      this.populateForm(this.value);
      if (this.readonlyMode && !this.cloneMode) this.form.disable();
    }
    if (this.readonlyMode && changes['versionOverride'] && this.versionOverride) {
      this.populateFromVersion(this.versionOverride);
      if (!this.cloneMode) this.form.disable();
    }
  }

  private createForm(): FormGroup {
    const fg = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      title: [''], // legacy compatibility
      // Hidden single manufacturerId used for backend payload (first of selected manufacturerIds)
      manufacturerId: [null, [Validators.required]],
      manufacturerIds: [[], [this.minSelectedValidator(1)]],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]],
      foreignContractId: ['', [Validators.maxLength(100)]], // new
      foreignContractID: [''], // legacy
      suspendedDate: [null],
      sendToPerformance: [false],
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: ['', [Validators.required, Validators.maxLength(100)]],
      manufacturerBillbackName: [''],
      manufacturerTermsAndConditions: [''],
      manufacturerNotes: [''],
      contactPerson: [''],
      entegraContractType: [''],
      entegraVdaProgram: [''],
      internalNotes: [''], // new
      notes: [''], // legacy
      distributorIds: [[]],
      opCoIds: [[]],
      industryIds: [[]],
      productIds: [[]]
    });
    fg.setValidators(this.dateValidator());
    return fg;
  }

  private loadBaseLists$() {
    const manufacturers$ = this.manufacturerService.getAllActive().pipe(catchError(err => { console.warn('Manufacturers load failed', err); return of([] as any[]); }));
    const industries$ = this.industryService.getActive().pipe(catchError(err => { console.warn('Industries load failed', err); return of([] as any[]); }));
    const distributors$ = this.distributorService.getPaginated(1, 1000, 'Name', 'asc', undefined, 'Active').pipe(catchError(err => { console.warn('Distributors load failed', err); return of({ items: [] } as any); }));

    return forkJoin({ manufacturers: manufacturers$, industries: industries$, distributors: distributors$ }).pipe(
      tap(({ manufacturers, industries, distributors }: any) => {
        this.manufacturers = (manufacturers || []).map((m: any) => ({ id: m.id, name: m.name }));
        this.filteredManufacturers = [...this.manufacturers];
        this.industries = (industries || []).map((i: any) => ({ id: i.id, name: i.name }));
        this.filteredIndustries = [...this.industries];
        const list = (distributors?.items || []);
        this.distributors = list.map((d: any) => ({ id: d.id, name: d.name }));
        this.filteredDistributors = [...this.distributors];
        this.updateSelectedSnapshots();
      }),
      // map to void
      map(() => void 0)
    );
  }

  private loadDropdowns(): void {
    this.manufacturerService.getAllActive().subscribe({
      next: list => {
        this.manufacturers = list.map(m => ({ id: m.id, name: m.name }));
        this.filteredManufacturers = [...this.manufacturers];
        this.updateSelectedSnapshots();
      },
      error: () => {}
    });
    this.industryService.getActive().subscribe({
      next: list => {
        this.industries = list.map(i => ({ id: (i as any).id, name: (i as any).name }));
        this.filteredIndustries = [...this.industries];
        this.updateSelectedSnapshots();
      },
      error: () => {}
    });

    // Load active distributors (paginated large page)
    this.distributorService.getPaginated(1, 1000, 'Name', 'asc', undefined, 'Active').subscribe({
      next: res => {
        const list = res.items || [];
        this.distributors = list.map(d => ({ id: (d as any).id, name: (d as any).name }));
        this.filteredDistributors = [...this.distributors];
        this.updateSelectedSnapshots();
      },
      error: () => {}
    });

    // Dependent subscriptions
    this.form.get('distributorIds')?.valueChanges.subscribe(ids => this.reloadOpCosForDistributors((ids || []) as number[]));
    this.form.get('manufacturerId')?.valueChanges.subscribe((id: number | null) => {
      // keep manufacturerIds in sync for backward-compat
      const mIds = id ? [Number(id)] : [];
      if (this.form.get('manufacturerIds')) this.form.get('manufacturerIds')?.setValue(mIds as any, { emitEvent: false } as any);
      // In read-only mode do NOT wipe selections or pricing; just refresh badges
      if (this.readonlyMode) { this.updateSelectedSnapshots(); return; }
      // reset selected products and pricing when manufacturer changes (edit/create mode only)
      this.form.get('productIds')?.setValue([]);
      this.priceRows = [];
      this.reloadProductsForManufacturer(id ?? null);
      this.updateSelectedSnapshots();
    });

    // Initial load for current selections
    this.reloadOpCosForDistributors((this.form.get('distributorIds')?.value || []) as number[]);
    const initMfrId: number | null = this.form.get('manufacturerId')?.value ?? null;
    this.reloadProductsForManufacturer(initMfrId);

    // keep price rows in sync with selected products
    this.form.get('productIds')?.valueChanges.subscribe(() => this.ensurePriceRowsForSelectedProducts());
  }

  private ensurePriceRowsForSelectedProducts(): void {
    const selected: number[] = (this.form.get('productIds')?.value || []) as number[];
    // add rows for newly selected
    selected.forEach(pid => {
      if (!this.priceRows.find(r => r.productId === pid)) {
        this.priceRows.push({ productId: pid, priceType: this.priceTypes[0], uom: this.uoms[0], billbacksAllowed: false });
      }
    });
    // remove rows for unselected
    this.priceRows = this.priceRows.filter(r => selected.includes(r.productId));
  }

  private loadContract(id: number): void {
    this.loading = true;
    this.error = null;

    const contract$ = this.contractService.getById(id);
    const manufacturers$ = this.contractService.getManufacturerAssignments(id).pipe(catchError(err => { console.warn('Manufacturers load failed', err); return of([] as any[]); }));
    const versions$ = this.contractService.getVersions(id).pipe(catchError(err => { console.warn('Versions load failed', err); return of([] as ContractVersion[]); }));

    forkJoin({ c: contract$, mfrs: manufacturers$, versions: versions$ }).subscribe({
      next: ({ c, mfrs, versions }) => {
        this.contract = c;
        this.populateForm(c);
        this.updateActions();

        // Seed selected products directly from backend contract payload (Products/ContractProducts)
        const directProducts = ((((c as any)?.Products) || ((c as any)?.products) || []) as Array<any>)
          .map(x => ({ id: Number(x.Id ?? x.id ?? x.ProductId ?? x.productId), name: String(x.Name ?? x.name ?? x.ProductName ?? x.productName ?? (x.Id ?? x.id)) }))
          .filter(x => !!x.id);
        const cpList = (((c as any)?.ContractProducts) || ((c as any)?.contractProducts) || directProducts) as Array<any>;

        if (Array.isArray(cpList) && cpList.length) {
          const currentSel: number[] = (this.form.get('productIds')?.value || []) as number[];

          if (!currentSel.length) {
            const cpIds = Array.from(new Set(cpList.map((x: any) => Number(x.productId ?? x.id)).filter((x: any) => !!x)));
            this.form.get('productIds')?.setValue(cpIds);

            // Seed product master with names so tags render immediately
            const seedMap = new Map<number, { id: number; name: string }>();
            (this.products || []).forEach(p => seedMap.set(p.id, p));
            cpList.forEach((x: any) => {
              const id = Number(x.productId ?? x.id);
              const name = String(x.productName ?? x.name ?? id);
              if (id) seedMap.set(id, { id, name });
            });
            this.products = Array.from(seedMap.values()).sort((a, b) => a.name.localeCompare(b.name));
            this.filteredProducts = [...this.products];
            this.updateSelectedSnapshots();
          }
        }

        this.versions = versions || [];
        const latest = this.versions.length ? Math.max(...this.versions.map(v => Number(v.versionNumber))) : null;
        this.selectedVersionNumber = latest;
        // Always populate UI from selected version (default to highest)
        this.onVersionChange();
        // If only one version, disable compare flow
        if ((this.versions?.length || 0) <= 1) {
          this.showComparePanel = false;
          this.compareWithVersionNumber = null;
        }

        const selectedVersion = this.versions.find(v => v.versionNumber === this.selectedVersionNumber) || null;
        if (selectedVersion && (selectedVersion.prices && selectedVersion.prices.length)) {
          const productIds = Array.from(new Set((selectedVersion.prices || []).map(p => Number(p.productId)).filter(x => !!x)));
          this.form.get('productIds')?.setValue(productIds);
          this.priceRows = productIds.map(pid => {
            const p = (selectedVersion.prices || []).find(pp => Number((pp as any).productId) === pid) as any;
            return {
              productId: pid,
              priceType: this.normalizePriceType(p?.priceType),
              uom: (p?.uom as any) || this.uoms[0],
              billbacksAllowed: !!(p?.billbacksAllowed),
              allowance: p?.allowance ?? undefined,
              commercialDelPrice: p?.commercialDelPrice ?? undefined,
              commercialFobPrice: p?.commercialFobPrice ?? undefined,
              commodityDelPrice: p?.commodityDelPrice ?? undefined,
              commodityFobPrice: p?.commodityFobPrice ?? undefined,
              estimatedQty: p?.estimatedQty ?? undefined,
              pua: p?.pua ?? undefined,
              ffsPrice: p?.ffsPrice ?? undefined,
              noiPrice: p?.noiPrice ?? undefined,
              ptv: p?.ptv ?? undefined,
              internalNotes: p?.internalNotes ?? undefined
            } as any;
          });
          // Seed product list immediately from version so chips render even before API returns
          const map = new Map<number, { id: number; name: string }>();
          (selectedVersion.prices || []).forEach(pp => {
            const idNum = Number(pp.productId);
            if (idNum) map.set(idNum, { id: idNum, name: pp.productName || String(idNum) });
          });
          // merge with any existing
          const mergedMap = new Map<number, { id: number; name: string }>();
          (this.products || []).forEach(p => mergedMap.set(p.id, p));
          map.forEach(v => mergedMap.set(v.id, v));
          this.products = Array.from(mergedMap.values()).sort((a, b) => a.name.localeCompare(b.name));
          this.filteredProducts = [...this.products];
          this.updateSelectedSnapshots();
        } else if (this.contract) {
          // Fallback to latest version pricing/products if version payload lacks prices
          forkJoin({
            vProducts: this.contractService.getContractProducts(this.contract.id).pipe(catchError(_ => of([] as {id:number;name:string}[]))),
            vPrices: this.contractService.getContractPricing(this.contract.id).pipe(catchError(_ => of([] as any[])))
          }).subscribe(({ vProducts, vPrices }) => {
            const seedProducts = (vProducts || []) as { id: number; name: string }[];
            const seedProductIds = Array.from(new Set((vPrices || []).map((p: any) => Number(p.productId)).filter((x: any) => !!x)));
            this.form.get('productIds')?.setValue(seedProductIds);
            this.priceRows = seedProductIds.map(pid => {
              const p = (vPrices as any[]).find(pp => Number((pp as any).productId) === pid) as any;
              return {
                productId: pid,
                priceType: this.normalizePriceType(p?.priceType),
                uom: (p?.uom as any) || this.uoms[0],
                billbacksAllowed: !!(p?.billbacksAllowed),
                allowance: p?.allowance ?? undefined,
                commercialDelPrice: p?.commercialDelPrice ?? undefined,
                commercialFobPrice: p?.commercialFobPrice ?? undefined,
                commodityDelPrice: p?.commodityDelPrice ?? undefined,
                commodityFobPrice: p?.commodityFobPrice ?? undefined,
                estimatedQty: p?.estimatedQty ?? undefined,
                pua: p?.pua ?? undefined,
                ffsPrice: p?.ffsPrice ?? undefined,
                noiPrice: p?.noiPrice ?? undefined,
                ptv: p?.ptv ?? undefined,
                internalNotes: p?.internalNotes ?? undefined
              } as any;
            });
            // Seed products from fallback too
            const m = new Map<number, { id: number; name: string }>();
            seedProducts.forEach(sp => m.set(sp.id, { id: sp.id, name: sp.name }));
            seedProductIds.forEach(id => { if (!m.has(id)) m.set(id, { id, name: String(id) }); });
            this.products = Array.from(m.values()).sort((a, b) => a.name.localeCompare(b.name));
            this.filteredProducts = [...this.products];
            this.updateSelectedSnapshots();
          });
        }
        if (this.readonlyMode && !this.cloneMode) this.form.disable();

        const manufacturerIds = Array.from(new Set((mfrs || []).map((x: any) => Number(x.manufacturerId)).filter((x: any) => !!x)));
        // Safety: if for any reason price rows are still empty, repopulate from the selected version
        if ((!this.priceRows || this.priceRows.length === 0) && this.selectedVersionNumber) {
          const vSel = this.versions.find(v => v.versionNumber === this.selectedVersionNumber);
          if (vSel && (vSel.prices?.length ?? 0) > 0) {
            this.onVersionChange();
          }
        }

        this.form.get('manufacturerIds')?.setValue(manufacturerIds);
        const first = manufacturerIds.length ? manufacturerIds[0] : null;
        this.form.get('manufacturerId')?.setValue(first);

        // Let the reactive subscription in loadDropdowns() handle product loading
        // This ensures consistency with OpCos pattern
        this.loading = false;
      },
      error: err => {
        this.error = 'Failed to load contract.';
        console.error(err);
        this.loading = false;
      }
    });
  }

  private populateForm(c: Contract): void {
    const mfrId = (c as any).manufacturerId;
    this.form.patchValue({
      name: (c as any).name || c.title || '',
      title: c.title || (c as any).name || '',
      manufacturerId: mfrId,
      manufacturerIds: mfrId ? [mfrId] : [],
      startDate: this.toInputDate(c.startDate),
      endDate: this.toInputDate(c.endDate),
      foreignContractId: (c as any).foreignContractId || (c as any).foreignContractID || '',
      foreignContractID: (c as any).foreignContractID || (c as any).foreignContractId || '',
      suspendedDate: (c as any).suspendedDate ? this.toInputDate((c as any).suspendedDate) : null,
      sendToPerformance: c.sendToPerformance,
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: (c as any).manufacturerReferenceNumber || '',
      manufacturerBillbackName: (c as any).manufacturerBillbackName || '',
      manufacturerTermsAndConditions: (c as any).manufacturerTermsAndConditions || '',
      manufacturerNotes: (c as any).manufacturerNotes || '',
      contactPerson: (c as any).contactPerson || '',
      entegraContractType: (c as any).entegraContractType || '',
      entegraVdaProgram: (c as any).entegraVdaProgram || '',
      internalNotes: (c as any).internalNotes || (c as any).notes || '',
      notes: (c as any).notes || (c as any).internalNotes || ''
    });
    const inds = (c.industries || []).map(i => i.industryId);
    const dists = (c.distributors || []).map(d => d.distributorId);
    const opcos = (c.opCos || []).map(o => o.opCoId);
    const product = (c.contractProducts || []).map(o => o.productId);
    this.form.get('industryIds')?.setValue(inds);
    this.form.get('distributorIds')?.setValue(dists);
    this.form.get('opCoIds')?.setValue(opcos);
    this.form.get('productIds')?.setValue(product);

    // If contract payload includes products, seed productIds and product names for chips immediately
    const directProducts = ((((c as any)?.Products) || ((c as any)?.products) || []) as Array<any>)
      .map(x => ({ id: Number(x.Id ?? x.id ?? x.ProductId ?? x.productId), name: String(x.Name ?? x.name ?? x.ProductName ?? x.productName ?? (x.Id ?? x.id)) }))
      .filter(x => !!x.id);
    const cpList = (((c as any)?.ContractProducts) || ((c as any)?.contractProducts) || directProducts) as Array<any>;

    if (Array.isArray(cpList) && cpList.length) {
      const currentSel: number[] = (this.form.get('productIds')?.value || []) as number[];

      if (!currentSel.length) {
        const cpIds = Array.from(new Set(cpList.map((x: any) => Number(x.productId ?? x.id)).filter((x: any) => !!x)));
        this.form.get('productIds')?.setValue(cpIds);
        const map = new Map<number, { id: number; name: string }>();
        (this.products || []).forEach(p => map.set(p.id, p));
        cpList.forEach((x: any) => {
          const id = Number(x.productId ?? x.id);
          const name = String(x.productName ?? x.name ?? id);
          if (id) {
            map.set(id, { id, name });
            // Store the seeded name for later use
            this.seededProductNames.set(id, name);
          }
        });
        this.products = Array.from(map.values()).sort((a, b) => a.name.localeCompare(b.name));
        this.filteredProducts = [...this.products];
      }
    }

    // sync selected badges
    this.updateSelectedSnapshots();
  }

  private populateFromVersion(v: ContractVersion): void {
    if (!v) return;
    this.form.patchValue({
      name: v.name || '',
      title: v.name || '',
      startDate: this.toInputDate(v.startDate),
      endDate: this.toInputDate(v.endDate),
      foreignContractId: v.foreignContractId || '',
      foreignContractID: v.foreignContractId || '',
      suspendedDate: v.suspendedDate ? this.toInputDate(v.suspendedDate) : null,
      sendToPerformance: !!v.sendToPerformance,
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: v.manufacturerReferenceNumber || '',
      manufacturerBillbackName: v.manufacturerBillbackName || '',
      manufacturerTermsAndConditions: v.manufacturerTermsAndConditions || '',
      manufacturerNotes: v.manufacturerNotes || '',
      contactPerson: v.contactPerson || '',
      entegraContractType: v.entegraContractType || '',
      entegraVdaProgram: v.entegraVdaProgram || '',
      internalNotes: v.internalNotes || '',
      notes: v.internalNotes || ''
    });
    // Do not alter selected distributors/opcos/industries here; they are contract-level
    this.updateSelectedSnapshots();
  }

  private toInputDate(d: Date | string): string {
    const date = new Date(d);
    const y = date.getFullYear();
    const m = (date.getMonth() + 1).toString().padStart(2, '0');
    const da = date.getDate().toString().padStart(2, '0');
    return `${y}-${m}-${da}`;
  }
  onVersionChange(): void {
    const verNo = Number(this.selectedVersionNumber);
    if (!verNo || Number.isNaN(verNo)) return;
    // normalize selectedVersionNumber to number to avoid strict-equality mismatches
    this.selectedVersionNumber = verNo;
    // Ensure compare target never equals current selected version
    if (this.showComparePanel && Number(this.compareWithVersionNumber) === verNo) {
      this.compareWithVersionNumber = null;
    }
    const v = this.versions.find(x => Number(x.versionNumber) === verNo) || null;
    if (!v) return;

    // Always refetch the specific version from API to ensure prices are loaded
    if (this.contract && (v as any).id) {
      this.contractService.getVersion(this.contract.id, (v as any).id).subscribe(vFull => {
        this.populateFromVersion(vFull);
        const prices = (vFull.prices || []) as any[];
        const ids = Array.from(new Set(prices.map(p => Number(p.productId)).filter(x => !!x)));
        this.form.get('productIds')?.setValue(ids);
        this.priceRows = ids.map(pid => {
          const p = prices.find(pp => Number((pp as any).productId) === pid) as any;
          return {
            productId: pid,
            priceType: this.normalizePriceType(p?.priceType),
            uom: (p?.uom as any) || this.uoms[0],
            billbacksAllowed: !!(p?.billbacksAllowed),
            allowance: p?.allowance ?? undefined,
            commercialDelPrice: p?.commercialDelPrice ?? undefined,
            commercialFobPrice: p?.commercialFobPrice ?? undefined,
            commodityDelPrice: p?.commodityDelPrice ?? undefined,
            commodityFobPrice: p?.commodityFobPrice ?? undefined,
            estimatedQty: p?.estimatedQty ?? undefined,
            pua: p?.pua ?? undefined,
            ffsPrice: p?.ffsPrice ?? undefined,
            noiPrice: p?.noiPrice ?? undefined,
            ptv: p?.ptv ?? undefined,
            internalNotes: p?.internalNotes ?? undefined
          } as any;
        });
        this.seedProductsFromPrices(prices);
        if (this.readonlyMode && !this.cloneMode) this.form.disable();
      });
      return;
    }

    // Fallback to existing in-memory version data
    this.populateFromVersion(v);
    const versionPrices = (v.prices || []) as any[];
    let productIds = Array.from(new Set(versionPrices.map(p => Number(p.productId)).filter(x => !!x)));

    if (!productIds.length && this.contract) {
      this.contractService.getContractPricing(this.contract.id).subscribe(vPrices => {
        const ids = Array.from(new Set((vPrices || []).map((p: any) => Number(p.productId)).filter((x: any) => !!x)));
        this.form.get('productIds')?.setValue(ids);
        this.priceRows = ids.map(pid => {
          const p = (vPrices as any[]).find(pp => Number((pp as any).productId) === pid) as any;
          return {
            productId: pid,
            priceType: this.normalizePriceType(p?.priceType),
            uom: (p?.uom as any) || this.uoms[0],
            billbacksAllowed: !!(p?.billbacksAllowed),
            allowance: p?.allowance ?? undefined,
            commercialDelPrice: p?.commercialDelPrice ?? undefined,
            commercialFobPrice: p?.commercialFobPrice ?? undefined,
            commodityDelPrice: p?.commodityDelPrice ?? undefined,
            commodityFobPrice: p?.commodityFobPrice ?? undefined,
            estimatedQty: p?.estimatedQty ?? undefined,
            pua: p?.pua ?? undefined,
            ffsPrice: p?.ffsPrice ?? undefined,
            noiPrice: p?.noiPrice ?? undefined,
            ptv: p?.ptv ?? undefined,
            internalNotes: p?.internalNotes ?? undefined
          } as any;
        });
        this.seedProductsFromPrices(vPrices as any[]);
        if (this.readonlyMode && !this.cloneMode) this.form.disable();
      });
      return;
    }

    this.form.get('productIds')?.setValue(productIds);
    this.priceRows = productIds.map(pid => {
      const p = versionPrices.find(pp => Number((pp as any).productId) === pid) as any;
      return {
        productId: pid,
        priceType: this.normalizePriceType(p?.priceType),
        uom: (p?.uom as any) || this.uoms[0],
        billbacksAllowed: !!(p?.billbacksAllowed),
        allowance: p?.allowance ?? undefined,
        commercialDelPrice: p?.commercialDelPrice ?? undefined,
        commercialFobPrice: p?.commercialFobPrice ?? undefined,
        commodityDelPrice: p?.commodityDelPrice ?? undefined,
        commodityFobPrice: p?.commodityFobPrice ?? undefined,
        estimatedQty: p?.estimatedQty ?? undefined,
        pua: p?.pua ?? undefined,
        ffsPrice: p?.ffsPrice ?? undefined,
        noiPrice: p?.noiPrice ?? undefined,
        ptv: p?.ptv ?? undefined,
        internalNotes: p?.internalNotes ?? undefined
      } as any;
    });

    this.seedProductsFromPrices(versionPrices);
    if (this.readonlyMode && !this.cloneMode) this.form.disable();
  }

  private seedProductsFromPrices(prices: any[]): void {
    const map = new Map<number, { id: number; name: string }>();
    (prices || []).forEach(pp => { const id = Number(pp.productId); if (id) map.set(id, { id, name: pp.productName || String(id) }); });
    const merged = new Map<number, { id: number; name: string }>();
    (this.products || []).forEach(p => merged.set(p.id, p));
    map.forEach(v => merged.set(v.id, v));
    this.products = Array.from(merged.values()).sort((a, b) => a.name.localeCompare(b.name));
    this.filteredProducts = [...this.products];
    this.updateSelectedSnapshots();
  }

  onActionChange(action: string): void {
    if (!this.contract || !this.selectedVersionNumber) return;
    if (action === 'Clone') {
      this.enterCloneMode();
      this.showComparePanel = false;
      this.compareWithVersionNumber = null;
      this.showDiffModal = false;
    } else if (action === 'Compare') {
      this.cloneMode = false; // remain in read-only while comparing
      if (this.readonlyMode) this.form.disable({ emitEvent: false });
      this.showComparePanel = true;
      this.compareWithVersionNumber = null;
      if (!this.versions.length) {
        this.contractService.getVersions(this.contract.id).subscribe(vs => this.versions = vs || []);
      }
    } else if (action === 'Create Proposal') {
      this.navigateToProposalCreation();
    } else if (action === 'Suspend') {
      this.openSuspendModal();
    } else if (action === 'Unsuspend') {
      if (!this.isAdmin) {
        this.error = 'You do not have permission to unsuspend contracts.';
        return;
      }
      this.openUnsuspendModal();
    } else {
      this.showComparePanel = false;
      this.compareWithVersionNumber = null;
      this.showDiffModal = false;
    }
  }

  /**
   * Navigate to proposal creation page with prepopulated data from this contract
   */
  navigateToProposalCreation(): void {
    if (!this.contract) return;

    // Navigate to proposal creation with contract ID as query parameter
    // The proposal creation page will handle prepopulating the data
    this.router.navigate(['/admin/proposals/create'], {
      queryParams: {
        renewFromContract: this.contract.id
      }
    });
  }

  private updateActions(): void {
    const base = ['Clone', 'Compare'];
    if (this.contract) {
      // Add Create Proposal option if user has permission
      if (this.canUseBulkRenewal()) {
        base.push('Create Proposal');
      }
      if (!this.contract.isSuspended && this.isAdmin) {
        base.push('Suspend');
      } else if (this.contract.isSuspended && this.isAdmin) {
        base.push('Unsuspend');
      }
    }
    this.actions = base;
  }

  /**
   * Check if user can use bulk renewal feature
   */
  canUseBulkRenewal(): boolean {
    return this.authService.hasRole('System Administrator') ||
           this.authService.hasRole('Contract Manager');
  }

  suspendContract(): void {
    if (!this.contract) return;
    this.submitting = true;
    this.error = null;
    this.contractService.suspend(this.contract.id).subscribe({
      next: () => {
        this.successMessage = null;
        this.actionToastMessage = 'Contract suspended successfully';
        this.showActionToast = true;
        setTimeout(() => this.showActionToast = false, 3000);
        this.closeSuspendModal();
        this.loadContract(this.contract!.id);
      },
      error: (err) => {
        this.error = err?.message || 'Failed to suspend contract';
      },
      complete: () => {
        this.submitting = false;
        this.updateActions();
      }
    });
  }

  unsuspendContract(): void {
    if (!this.contract) return;
    this.submitting = true;
    this.error = null;
    this.contractService.unsuspend(this.contract.id).subscribe({
      next: () => {
        this.successMessage = null;
        this.actionToastMessage = 'Contract unsuspended successfully';
        this.showActionToast = true;
        setTimeout(() => this.showActionToast = false, 3000);
        this.closeUnsuspendModal();
        this.loadContract(this.contract!.id);
      },
      error: (err) => {
        this.error = err?.message || 'Failed to unsuspend contract';
      },
      complete: () => {
        this.submitting = false;
        this.updateActions();
      }
    });
  }

  openSuspendModal(): void { this.showSuspendModal = true; }
  closeSuspendModal(): void { this.showSuspendModal = false; }
  confirmSuspend(): void { this.suspendContract(); }

  openUnsuspendModal(): void { this.showUnsuspendModal = true; }
  closeUnsuspendModal(): void { this.showUnsuspendModal = false; }
  confirmUnsuspend(): void { this.unsuspendContract(); }

  closeActionToast(): void { this.showActionToast = false; }

  private enterCloneMode(): void {
    this.cloneMode = true;
    // Re-enable the form then lock fields that must remain read-only in clone mode
    this.form.enable({ emitEvent: false });
    const toDisable = ['manufacturerId', 'manufacturerIds', 'distributorIds', 'opCoIds', 'industryIds', 'productIds'];
    toDisable.forEach(n => {
      const ctrl = this.form.get(n);
      if (ctrl) ctrl.disable({ emitEvent: false });
    });
  }

  showDifferences(): void {
    const vA = Number(this.selectedVersionNumber);
    const vB = Number(this.compareWithVersionNumber);
    if (!this.contract || !vA || !vB) return;

    const leftMeta = this.versions.find(v => Number(v.versionNumber) === vA);
    const rightMeta = this.versions.find(v => Number(v.versionNumber) === vB);
    if (!leftMeta || !rightMeta) return;

    // Fetch full versions to ensure complete pricing payloads
    forkJoin({
      a: this.contractService.getVersion(this.contract.id, (leftMeta as any).id),
      b: this.contractService.getVersion(this.contract.id, (rightMeta as any).id)
    }).subscribe(({ a, b }) => {
      this.diffResult = this.buildDiffModel(a, b, vA, vB);
      this.showDiffModal = true;
    });
  }

  private buildDiffModel(a: ContractVersion, b: ContractVersion, leftNo?: number, rightNo?: number): any {
    const headerFields: Array<keyof ContractVersion> = [
      'name','foreignContractId','startDate','endDate','isSuspended','sendToPerformance','suspendedDate','internalNotes',
      'manufacturerReferenceNumber','manufacturerBillbackName','manufacturerTermsAndConditions','manufacturerNotes','contactPerson','entegraContractType','entegraVdaProgram'
    ];

    const headerDiff = headerFields.map(f => ({
      field: f,
      from: (a as any)[f],
      to: (b as any)[f]
    })).filter(x => this.formatVal(x.from) !== this.formatVal(x.to));

    // Build product pricing maps
    const aMap = new Map<number, any>();
    (a.prices || []).forEach(p => aMap.set(Number(p.productId), p));
    const bMap = new Map<number, any>();
    (b.prices || []).forEach(p => bMap.set(Number(p.productId), p));

    const allIds = new Set<number>([...Array.from(aMap.keys()), ...Array.from(bMap.keys())]);
    const priceFields = [
      'priceType','uom','billbacksAllowed','allowance','commercialDelPrice','commercialFobPrice',
      'commodityDelPrice','commodityFobPrice','estimatedQty','pua','ffsPrice','noiPrice','ptv','internalNotes'
    ];

    const added: any[] = [];
    const removed: any[] = [];
    const modified: any[] = [];

    const sbs: any[] = [];

    allIds.forEach(id => {
      const ap = aMap.get(id);
      const bp = bMap.get(id);
      if (ap && !bp) {
        removed.push({ productId: id, productName: ap.productName || String(id) });
      } else if (!ap && bp) {
        added.push({ productId: id, productName: bp.productName || String(id) });
      } else if (ap && bp) {
        const fields = priceFields.map(n => ({ name: n, from: (ap as any)[n], to: (bp as any)[n] }))
          .filter(x => this.formatVal(x.from) !== this.formatVal(x.to));
        if (fields.length) modified.push({ productId: id, productName: ap.productName || bp.productName || String(id), fields });
      }

      // Build side-by-side row
      const diffFields = new Set<string>();
      priceFields.forEach(n => {
        const fromV = ap ? (ap as any)[n] : undefined;
        const toV = bp ? (bp as any)[n] : undefined;
        if (this.formatVal(fromV) !== this.formatVal(toV)) diffFields.add(n);
      });
      sbs.push({
        productId: id,
        nameA: ap?.productName || String(id),
        nameB: bp?.productName || String(id),
        a: ap || null,
        b: bp || null,
        diffFields
      });
    });

    return { headerDiff, added, removed, modified, pricingSideBySide: sbs, fields: priceFields, leftVersion: leftNo, rightVersion: rightNo };
  }

  private formatVal(v: any): string {
    if (v === null || v === undefined) return '';
    if (v instanceof Date) return this.toInputDate(v);
    // ISO date strings
    const s = String(v);
    if (/^\d{4}-\d{2}-\d{2}T/.test(s)) return s.slice(0,10);
    return s;
  }

  closeDiffModal(): void {
    this.showDiffModal = false;
  }


  isFieldInvalid(field: string): boolean {
    const ctrl = this.form.get(field);
    return !!(ctrl && ctrl.invalid && (ctrl.dirty || ctrl.touched));
  }

  onSubmit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(c => c.markAsTouched());
      this.validationErrors = this.getValidationErrors();
      return;
    }
    this.validationErrors = [];

    this.submitting = true;
    const v = this.form.value;

    if (this.isEditMode && this.contract) {
      const nameVal = (v.name ?? '').toString().trim();
      const internalNotesVal = (v.internalNotes ?? '').toString().trim();
      const foreignIdVal = (v.foreignContractId ?? '').toString().trim();
      const legacyForeignIdVal = (v.foreignContractID ?? '').toString().trim() || foreignIdVal;

      // Ensure manufacturerId is set from multi-select
      let manufacturerIdVal: number | undefined = v.manufacturerId != null ? Number(v.manufacturerId) : undefined;
      const mIds: number[] = (v.manufacturerIds || []) as number[];
      if ((!manufacturerIdVal || manufacturerIdVal <= 0) && mIds.length) manufacturerIdVal = Number(mIds[0]);

      const distributorIds: number[] = Array.from(new Set(((v.distributorIds ?? []) as number[]).map(Number))).filter(x => !!x);
      const opCoIds: number[] = Array.from(new Set(((v.opCoIds ?? []) as number[]).map(Number))).filter(x => !!x);
      const industryIds: number[] = Array.from(new Set(((v.industryIds ?? []) as number[]).map(Number))).filter(x => !!x);

      const req: UpdateContractRequest = {
        name: nameVal || undefined,
        title: nameVal || (v.title?.trim() || undefined),
        manufacturerId: manufacturerIdVal,
        startDate: v.startDate ? new Date(v.startDate) : undefined,
        endDate: v.endDate ? new Date(v.endDate) : undefined,

        foreignContractId: foreignIdVal || undefined,
        foreignContractID: legacyForeignIdVal || undefined,
        sendToPerformance: !!v.sendToPerformance,
        // Manufacturer/Entegra metadata
        manufacturerReferenceNumber: (v.manufacturerReferenceNumber ?? '').toString().trim() || undefined,
        manufacturerBillbackName: (v.manufacturerBillbackName ?? '').toString().trim() || undefined,
        manufacturerTermsAndConditions: (v.manufacturerTermsAndConditions ?? '').toString().trim() || undefined,
        manufacturerNotes: (v.manufacturerNotes ?? '').toString().trim() || undefined,
        contactPerson: (v.contactPerson ?? '').toString().trim() || undefined,
        entegraContractType: (v.entegraContractType ?? '').toString().trim() || undefined,
        entegraVdaProgram: (v.entegraVdaProgram ?? '').toString().trim() || undefined,
        internalNotes: internalNotesVal,
        notes: internalNotesVal || (v.notes?.trim() || undefined),
        distributorIds,
        opCoIds,
        industryIds
      } as any;
      this.contractService.update(this.contract.id, req).subscribe({
        next: () => {
          this.submitting = false;
          this.router.navigate(['/admin/contracts']);
        },
        error: err => {
          this.error = err?.message || 'Failed to update contract.';
          this.submitting = false;
        }
      });
    } else {
      const nameVal = (v.name ?? '').toString().trim();
      const internalNotesVal = (v.internalNotes ?? '').toString().trim();
      const foreignIdVal = (v.foreignContractId ?? '').toString().trim();

      const distributorIds: number[] = Array.from(new Set(((v.distributorIds ?? []) as number[]).map(Number))).filter(x => !!x);
      const opCoIds: number[] = Array.from(new Set(((v.opCoIds ?? []) as number[]).map(Number))).filter(x => !!x);
      const productIds: number[] = Array.from(new Set(((v.productIds ?? []) as number[]).map(Number))).filter(x => !!x);
      const industryIds: number[] = Array.from(new Set(((v.industryIds ?? []) as number[]).map(Number))).filter(x => !!x);

      this.ensurePriceRowsForSelectedProducts();
      const prices = this.priceRows.map(r => ({
        versionNumber: 1,
        productId: Number(r.productId),
        priceType: r.priceType,
        allowance: r.allowance ?? null,
        commercialDelPrice: r.commercialDelPrice ?? null,
        commercialFobPrice: r.commercialFobPrice ?? null,
        commodityDelPrice: r.commodityDelPrice ?? null,
        commodityFobPrice: r.commodityFobPrice ?? null,
        uom: r.uom,
        estimatedQty: r.estimatedQty ?? null,
        billbacksAllowed: !!r.billbacksAllowed,
        pua: r.pua ?? null,
        ffsPrice: r.ffsPrice ?? null,
        noiPrice: r.noiPrice ?? null,
        ptv: r.ptv ?? null,
        internalNotes: r.internalNotes ?? null
      }));

      const req: CreateContractRequest = {
        name: nameVal || undefined,
        startDate: new Date(v.startDate),
        endDate: new Date(v.endDate),
        foreignContractId: foreignIdVal || undefined,
        sendToPerformance: !!v.sendToPerformance,
        // Manufacturer/Entegra metadata
        manufacturerReferenceNumber: (v.manufacturerReferenceNumber ?? '').toString().trim() || undefined,
        manufacturerBillbackName: (v.manufacturerBillbackName ?? '').toString().trim() || undefined,
        manufacturerTermsAndConditions: (v.manufacturerTermsAndConditions ?? '').toString().trim() || undefined,
        manufacturerNotes: (v.manufacturerNotes ?? '').toString().trim() || undefined,
        contactPerson: (v.contactPerson ?? '').toString().trim() || undefined,
        entegraContractType: (v.entegraContractType ?? '').toString().trim() || undefined,
        entegraVdaProgram: (v.entegraVdaProgram ?? '').toString().trim() || undefined,
        internalNotes: internalNotesVal,
        distributorIds,
        opCoIds,
        industryIds,
        productIds,
        prices
      } as any;
      this.contractService.create(req).subscribe({
        next: () => {
          this.successMessage = 'Contract created successfully!';
          this.submitting = false;
          setTimeout(() => this.router.navigate(['/admin/contracts']), 800);
        },
        error: err => {
          this.error = err?.message || 'Failed to create contract.';
          this.submitting = false;
        }
      });
    }
  }

  private getValidationErrors(): string[] {
    const errors: string[] = [];
    const c = this.form.controls;
    if (c['name']?.invalid) errors.push('Name is required.');
    if (c['manufacturerId']?.invalid) errors.push('Manufacturer is required.');
    if (c['startDate']?.invalid) errors.push('Start Date is required.');
    if (c['endDate']?.invalid) errors.push('End Date is required.');
    if (c['manufacturerReferenceNumber']?.invalid) errors.push('Manufacturer Reference Number is required.');
    if (this.form.errors?.['dateOrder']) errors.push('Start Date cannot be greater than End Date.');
    return errors;
  }

  onCancel(): void {
    this.router.navigate(['/admin/contracts']);
  }

  onDuplicateAmend(): void {
    // Per request: do not create a new version on Duplicate/Amend. No-op.
    this.successMessage = null;
    this.error = null;
    this.successMessage = 'Duplicate/Amend is disabled (no new version will be created).';
    setTimeout(() => this.successMessage = null, 2000);
  }

  getProductName(id: number): string {
    const p = this.products.find(x => x.id === id);
    if (p) return p.name;

    // Check seeded product names from database
    const seededName = this.seededProductNames.get(id);
    if (seededName) return seededName;

    return `${id}`;
  }

  applyManufacturerFilter(term: string): void {
    this.manufacturerFilter = term || '';
    const t = this.manufacturerFilter.toLowerCase();
    this.filteredManufacturers = this.manufacturers.filter(m => (m.name || '').toLowerCase().includes(t));
  }
  selectManufacturer(id: number): void {
    if (this.readonlyMode && !this.cloneMode) return;
    this.form.get('manufacturerId')?.setValue(id);
    this.showManufacturerPanel = false;
    // manufacturerId valueChanges will sync manufacturerIds and refresh dependent data
  }

  applyIndustryFilter(term: string): void {
    this.industryFilter = term || '';
    const t = this.industryFilter.toLowerCase();
    this.filteredIndustries = this.industries.filter(i => (i.name || '').toLowerCase().includes(t));
  }
  applyDistributorFilter(term: string): void {
    this.distributorFilter = term || '';
    const t = this.distributorFilter.toLowerCase();
    this.filteredDistributors = this.distributors.filter(d => (d.name || '').toLowerCase().includes(t));
  }
  applyOpcoFilter(term: string): void {
    this.opcoFilter = term || '';
    const t = this.opcoFilter.toLowerCase();
    this.filteredOpcos = this.opcos.filter(o => (o.name || '').toLowerCase().includes(t));
  }
  applyProductFilter(term: string): void {
    this.productFilter = term || '';
    const t = this.productFilter.toLowerCase();
    this.filteredProducts = this.products.filter(p => (p.name || '').toLowerCase().includes(t));
  }

  parseNum(val: any): number | undefined {
    const n = Number.parseFloat(String(val));
    return Number.isNaN(n) ? undefined : n;
  }

  // --- Pricing field interdependency (Allowance vs Commercial DEL/FOB/PUA) ---
  isPricingDisabledByAllowance(row: any): boolean {
    return row.allowance != null && row.allowance > 0;
  }

  isAllowanceDisabledByPricing(row: any): boolean {
    return (row.commercialDelPrice != null && row.commercialDelPrice > 0)
        || (row.commercialFobPrice != null && row.commercialFobPrice > 0)
        || (row.pua != null && row.pua > 0);
  }

  onAllowanceInput(row: any, value: number | undefined): void {
    row.allowance = value;
    if (value != null && value > 0) {
      row.commercialDelPrice = undefined;
      row.commercialFobPrice = undefined;
      row.pua = undefined;
    }
  }

  onPricingInput(row: any, field: string, value: number | undefined): void {
    (row as any)[field] = value;
    if (value != null && value > 0) {
      row.allowance = undefined;
    }
  }

  // --- Dropdown panel helpers (match User -> Manufacturers UX) ---
  private computeSelected(controlName: string, all: { id: number; name: string }[]): { id: number; name: string }[] {
    const ids: number[] = (this.form.get(controlName)?.value || []) as number[];
    const set = new Set(ids);
    return (all || []).filter(x => set.has(x.id));
  }

  private updateSelectedSnapshots(): void {
    this.selectedManufacturers = this.computeSelected('manufacturerIds', this.manufacturers);
    this.selectedIndustries = this.computeSelected('industryIds', this.industries);
    this.selectedDistributors = this.computeSelected('distributorIds', this.distributors);
    this.selectedOpcos = this.computeSelected('opCoIds', this.opcos);
    this.selectedProducts = this.computeSelected('productIds', this.products);
  }

  isIdSelected(controlName: 'manufacturerIds'|'industryIds'|'distributorIds'|'opCoIds'|'productIds', id: number): boolean {
    const ids: number[] = (this.form.get(controlName)?.value || []) as number[];
    return ids.includes(id);
  }
  toggleId(controlName: 'manufacturerIds'|'industryIds'|'distributorIds'|'opCoIds'|'productIds', id: number): void {
    if (this.readonlyMode && !this.cloneMode) return;
    const ctrl = this.form.get(controlName)!;
    const ids: number[] = [ ...(ctrl.value || []) ];
    const idx = ids.indexOf(id);
    if (idx >= 0) ids.splice(idx, 1); else ids.push(id);
    ctrl.setValue(ids);
    ctrl.markAsTouched();
    this.updateSelectedSnapshots();
    // Force dependent reloads immediately on toggle for responsiveness
    if (controlName === 'manufacturerIds') {
      const first = ids.length ? ids[0] : null;
      this.form.get('manufacturerId')?.setValue(first);
      this.reloadProductsForManufacturers(ids);
    }
    if (controlName === 'distributorIds') {
      this.reloadOpCosForDistributors(ids);
    }
    if (controlName === 'productIds') this.ensurePriceRowsForSelectedProducts();
  }
  removeFromMulti(controlName: 'manufacturerIds'|'industryIds'|'distributorIds'|'opCoIds'|'productIds', id: number): void {
    if (this.readonlyMode && !this.cloneMode) return;
    const ctrl = this.form.get(controlName)!;
    const ids: number[] = [ ...(ctrl.value || []) ].filter(x => x !== id);
    ctrl.setValue(ids);
    ctrl.markAsTouched();
    this.updateSelectedSnapshots();
    if (controlName === 'productIds') this.ensurePriceRowsForSelectedProducts();
  }
  togglePanel(controlName: 'manufacturerIds'|'industryIds'|'distributorIds'|'opCoIds'|'productIds'): void {
    if (this.readonlyMode && !this.cloneMode) return;
    switch (controlName) {
      case 'manufacturerIds':
        this.showManufacturerPanel = !this.showManufacturerPanel; if (this.showManufacturerPanel) this.applyManufacturerFilter(''); break;
      case 'industryIds':
        this.showIndustriesPanel = !this.showIndustriesPanel; if (this.showIndustriesPanel) this.applyIndustryFilter(''); break;
      case 'distributorIds':
        this.showDistributorsPanel = !this.showDistributorsPanel; if (this.showDistributorsPanel) this.applyDistributorFilter(''); break;
      case 'opCoIds':
        this.showOpcoPanel = !this.showOpcoPanel; if (this.showOpcoPanel) this.applyOpcoFilter(''); break;
      case 'productIds':
        this.showProductsPanel = !this.showProductsPanel; if (this.showProductsPanel) this.applyProductFilter(''); break;
    }
  }
  closePanel(controlName: 'manufacturerIds'|'industryIds'|'distributorIds'|'opCoIds'|'productIds'): void {
    switch (controlName) {
      case 'manufacturerIds': this.showManufacturerPanel = false; break;
      case 'industryIds': this.showIndustriesPanel = false; break;
      case 'distributorIds': this.showDistributorsPanel = false; break;
      case 'opCoIds': this.showOpcoPanel = false; break;
      case 'productIds': this.showProductsPanel = false; break;
    }
  }

  applyPanel(controlName: 'manufacturerIds'|'industryIds'|'distributorIds'|'opCoIds'|'productIds'): void {
  this.closePanel(controlName);
  // Force dependent reloads on Apply to be safe
  if (controlName === 'manufacturerIds') {
    const ids = (this.form.get('manufacturerIds')?.value || []) as number[];
    const first = ids.length ? ids[0] : null;
    this.form.get('manufacturerId')?.setValue(first);
    this.reloadProductsForManufacturers(ids);
  }
  if (controlName === 'distributorIds') {
    const ids = (this.form.get('distributorIds')?.value || []) as number[];
    this.reloadOpCosForDistributors(ids);
  }
  if (controlName === 'productIds') {
    this.ensurePriceRowsForSelectedProducts();
  }
  this.updateSelectedSnapshots();
}

  private minSelectedValidator(min: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const arr = control.value as any[];
      if (Array.isArray(arr) && arr.length >= min) return null;
      return { minSelected: { required: min, actual: Array.isArray(arr) ? arr.length : 0 } };
    };
  }

  private dateValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const group = control as FormGroup;
      const start = group.get('startDate')?.value;
      const end = group.get('endDate')?.value;
      let err: ValidationErrors | null = null;
      if (start && end && new Date(start) > new Date(end)) {
        err = { ...(err || {}), dateOrder: true } as ValidationErrors;
      }
      return err;
    };
  }


  private reloadOpCosForDistributors(distributorIds: number[]): void {
    const ids = Array.isArray(distributorIds) ? distributorIds.filter(x => typeof x === 'number') : [];
    if (!ids.length) {
      this.opcos = [];
      this.filteredOpcos = [];
      this.form.get('opCoIds')?.setValue([]);
      this.updateSelectedSnapshots();
      return;
    }
    this.opcoService.getByDistributorIds(ids).subscribe({
      next: list => {
        // dedupe and sort
        const map = new Map<number, { id: number; name: string }>();
        (list || []).forEach(o => map.set((o as any).id, { id: (o as any).id, name: (o as any).name }));
        this.opcos = Array.from(map.values()).sort((a, b) => a.name.localeCompare(b.name));
        this.filteredOpcos = [...this.opcos];
        // prune selections not in available list
        const allowed = new Set(this.opcos.map(o => o.id));
        const current: number[] = (this.form.get('opCoIds')?.value || []) as number[];
        const pruned = current.filter(id => allowed.has(id));
        if (pruned.length !== current.length) this.form.get('opCoIds')?.setValue(pruned);
        this.updateSelectedSnapshots();
      },
      error: () => {}
    });
  }

  private reloadProductsForManufacturer(manufacturerId: number | null): void {
    const id = manufacturerId ? +manufacturerId : 0;
    if (!id) {
      this.products = [];
      this.filteredProducts = [];
      this.form.get('productIds')?.setValue([]);
      this.priceRows = [];
      this.updateSelectedSnapshots();
      return;
    }
    this.productService.getByManufacturer(id).subscribe({
      next: list => {
        const map = new Map<number, { id: number; name: string }>();
        (list || []).forEach(p => map.set((p as any).id, { id: (p as any).id, name: (p as any).name }));
        this.products = Array.from(map.values()).sort((a, b) => a.name.localeCompare(b.name));
        this.filteredProducts = [...this.products];
        // prune selections not in available list
        const allowed = new Set(this.products.map(p => p.id));
        const current: number[] = (this.form.get('productIds')?.value || []) as number[];
        const pruned = current.filter(pid => allowed.has(pid));
        if (pruned.length !== current.length) this.form.get('productIds')?.setValue(pruned);
        this.ensurePriceRowsForSelectedProducts();
        this.updateSelectedSnapshots();
      },
      error: () => {}
    });
  }


  private reloadProductsForManufacturers(manufacturerIds: number[]): void {
    const ids = Array.isArray(manufacturerIds) ? manufacturerIds.filter(x => typeof x === 'number') : [];
    if (!ids.length) {
      this.products = [];
      this.filteredProducts = [];
      this.form.get('productIds')?.setValue([]);
      this.priceRows = [];
      this.updateSelectedSnapshots();
      return;
    }
    this.productService.getByManufacturers(ids).subscribe({
      next: list => {
        const map = new Map<number, { id: number; name: string }>();
        (list || []).forEach(p => map.set((p as any).id, { id: (p as any).id, name: (p as any).name }));

        // Also preserve any seeded products that might not be returned by the API
        const current: number[] = (this.form.get('productIds')?.value || []) as number[];
        current.forEach(pid => {
          if (!map.has(pid)) {
            const seededName = this.seededProductNames.get(pid);
            if (seededName) {
              map.set(pid, { id: pid, name: seededName });
            }
          }
        });

        // Also preserve any products from selected version pricing
        const selectedVersion = this.versions.find(v => v.versionNumber === this.selectedVersionNumber) || null;
        if (selectedVersion?.prices) {
          selectedVersion.prices.forEach((pp: any) => {
            const idNum = Number(pp.productId);
            if (idNum && !map.has(idNum)) {
              map.set(idNum, { id: idNum, name: pp.productName || String(idNum) });
            }
          });
        }

        this.products = Array.from(map.values()).sort((a, b) => a.name.localeCompare(b.name));
        this.filteredProducts = [...this.products];
        const allowed = new Set(this.products.map(p => p.id));
        const pruned = current.filter(pid => allowed.has(pid));
        if (pruned.length !== current.length) this.form.get('productIds')?.setValue(pruned);
        this.ensurePriceRowsForSelectedProducts();
        this.updateSelectedSnapshots();
      },
      error: () => {}
    });
  }
  // --- Add Product Modal handlers ---
  openProductModal(): void {
    if (this.readonlyMode && !this.cloneMode) return;
    const mId: number | null = this.form.get('manufacturerId')?.value ?? null;
    if (!mId) return;
    // ensure product list is loaded for the selected manufacturer
    if (!this.products || !this.products.length) {
      this.reloadProductsForManufacturer(mId);
    }
    const ids: number[] = (this.form.get('productIds')?.value || []) as number[];
    this.modalSelectedSet = new Set<number>(ids.map(Number));
    this.modalProductSearch = '';
    this.refreshModalFilteredProducts();
    this.showProductModal = true;
  }

  closeProductModal(): void {
    this.showProductModal = false;
  }

  // New simplified modal flow
  onAddProductClick(): void {
    if (this.readonlyMode && !this.cloneMode) return;
    const mId: number | null = this.form.get('manufacturerId')?.value ?? null;
    if (!mId) { alert('Please select a manufacturer first.'); return; }
    if (!this.products?.length) this.reloadProductsForManufacturer(mId);
    const ids: number[] = (this.form.get('productIds')?.value || []) as number[];
    this.selectedProductIds = new Set<number>(ids.map(Number));
    this.productSearch = '';
    this.selectAllProducts = false;
    this.showProductModal = true;
  }

  filteredModalProducts(): { id:number; name:string }[] {
    const t = (this.productSearch || '').toLowerCase();
    return (this.products || []).filter(p => (p.name || '').toLowerCase().includes(t));
  }

  toggleSelectAllProducts(): void {
    const list = this.filteredModalProducts();
    if (this.selectAllProducts) list.forEach((p: {id:number; name:string}) => this.selectedProductIds.add(p.id));
    else list.forEach((p: {id:number; name:string}) => this.selectedProductIds.delete(p.id));
  }

  toggleProductSelection(id: number, checked: boolean): void {
    if (checked) this.selectedProductIds.add(id); else this.selectedProductIds.delete(id);
  }

  addSelectedProducts(): void {
    const ids = Array.from(this.selectedProductIds.values());
    this.form.get('productIds')?.setValue(ids);
    this.ensurePriceRowsForSelectedProducts();
    this.updateSelectedSnapshots();
    this.showProductModal = false;
  }

  removeProductRow(productId: number): void {
    if (this.readonlyMode && !this.cloneMode) return;
    this.removeFromMulti('productIds', productId);
  }

  openAddSingleProductModal(): void {
    if (this.readonlyMode && !this.cloneMode) return;
    const mId: number | null = this.form.get('manufacturerId')?.value ?? null;
    if (!mId) { alert('Please select a manufacturer first.'); return; }
    if (!this.products?.length) this.reloadProductsForManufacturer(mId);

    // Reset the modal form
    this.addSingleProduct = {
      productId: null,
      priceType: this.priceTypes[0],
      uom: this.uoms[0],
      estimatedQty: null,
      billbacksAllowed: false,
      allowance: null,
      commercialDelPrice: null,
      commercialFobPrice: null,
      commodityDelPrice: null,
      commodityFobPrice: null,
      pua: null,
      ffsPrice: null,
      noiPrice: null,
      ptv: null,
      internalNotes: ''
    };
    this.addSingleProductError = null;
    this.addSingleProductSearch = '';
    this.selectedAddProduct = null;
    this.showAddSingleProductModal = true;
  }

  openProductSearchForAdd(): void {
    this.showProductSearchForAdd = true;
  }

  onAddProductSelected(product: Product): void {
    this.selectedAddProduct = product;
    this.addSingleProduct.productId = product.id;
    this.showProductSearchForAdd = false;
  }

  clearAddProduct(): void {
    this.selectedAddProduct = null;
    this.addSingleProduct.productId = null;
  }

  cancelAddSingleProductModal(): void {
    this.showAddSingleProductModal = false;
    this.addSingleProductError = null;
  }

  confirmAddSingleProduct(): void {
    if (!this.addSingleProduct.productId) {
      this.addSingleProductError = 'Please select a product.';
      return;
    }

    // Check for duplicate
    const existingIds: number[] = (this.form.get('productIds')?.value || []) as number[];
    if (existingIds.includes(this.addSingleProduct.productId)) {
      this.addSingleProductError = 'This product has already been added.';
      return;
    }

    // Ensure the product is in the products lookup for name display
    if (this.selectedAddProduct && !this.products.find(p => p.id === this.selectedAddProduct!.id)) {
      this.products.push({ id: this.selectedAddProduct.id, name: this.selectedAddProduct.name || this.selectedAddProduct.description || '' });
    }

    // Add the price row FIRST (before setValue) so ensurePriceRowsForSelectedProducts won't create a duplicate
    this.priceRows.push({
      productId: this.addSingleProduct.productId,
      priceType: this.addSingleProduct.priceType || this.priceTypes[0],
      uom: this.addSingleProduct.uom || this.uoms[0],
      billbacksAllowed: this.addSingleProduct.billbacksAllowed || false,
      estimatedQty: this.addSingleProduct.estimatedQty ?? undefined,
      allowance: this.addSingleProduct.allowance ?? undefined,
      commercialDelPrice: this.addSingleProduct.commercialDelPrice ?? undefined,
      commercialFobPrice: this.addSingleProduct.commercialFobPrice ?? undefined,
      commodityDelPrice: this.addSingleProduct.commodityDelPrice ?? undefined,
      commodityFobPrice: this.addSingleProduct.commodityFobPrice ?? undefined,
      pua: this.addSingleProduct.pua ?? undefined,
      ffsPrice: this.addSingleProduct.ffsPrice ?? undefined,
      noiPrice: this.addSingleProduct.noiPrice ?? undefined,
      ptv: this.addSingleProduct.ptv ?? undefined,
      internalNotes: this.addSingleProduct.internalNotes || undefined
    });

    // Now add productId to the form (triggers ensurePriceRowsForSelectedProducts which will skip since row exists)
    const newIds = [...existingIds, this.addSingleProduct.productId];
    this.form.get('productIds')?.setValue(newIds);

    this.updateSelectedSnapshots();
    this.showAddSingleProductModal = false;
    this.addSingleProductError = null;
  }

  openProductSelectionModal(): void {
    const mId: number | null = this.form.get('manufacturerId')?.value ?? null;
    if (!mId) { alert('Please select a manufacturer first.'); return; }
    this.openProductModal();
  }

  onProductModalConfirm(ids: number[]): void {
    this.form.get('productIds')?.setValue(ids || []);
    this.ensurePriceRowsForSelectedProducts();
    this.updateSelectedSnapshots();
    this.showProductModal = false;
  }

  onModalSearch(term: string): void {
    this.modalProductSearch = term || '';
    this.refreshModalFilteredProducts();
  }

  private refreshModalFilteredProducts(): void {
    const t = (this.modalProductSearch || '').toLowerCase();
    this.modalFilteredProducts = (this.products || []).filter(p => (p.name || '').toLowerCase().includes(t));
    this.updateModalSelectAllState();
  }

  private updateModalSelectAllState(): void {
    if (!this.modalFilteredProducts.length) { this.modalSelectAll = false; return; }
    this.modalSelectAll = this.modalFilteredProducts.every(p => this.modalSelectedSet.has(p.id));
  }

  toggleModalProduct(id: number, checked: boolean): void {
    if (checked) this.modalSelectedSet.add(id); else this.modalSelectedSet.delete(id);
    this.updateModalSelectAllState();
  }

  toggleModalSelectAll(checked: boolean): void {
    if (checked) {
      this.modalFilteredProducts.forEach(p => this.modalSelectedSet.add(p.id));
    } else {
      this.modalFilteredProducts.forEach(p => this.modalSelectedSet.delete(p.id));
    }
    this.updateModalSelectAllState();
  }

  applyProductModal(): void {
    const ids = Array.from(this.modalSelectedSet.values());
    this.form.get('productIds')?.setValue(ids);
    this.ensurePriceRowsForSelectedProducts();
    this.updateSelectedSnapshots();
    this.showProductModal = false;
  }


  onSaveAndUpdateClone(): void {
    if (!this.contract) return;
    this.submitting = true;
    const v = this.form.value;

    // Build CreateContractVersionRequest from allowed fields + pricing
    const req: CreateContractVersionRequest = {
      name: (v.name ?? '').toString().trim(),
      foreignContractId: (v.foreignContractId ?? '').toString().trim() || undefined,
      sendToPerformance: !!v.sendToPerformance,

      internalNotes: (v.internalNotes ?? '').toString().trim() || undefined,
      // Manufacturer/Entegra metadata
      manufacturerReferenceNumber: (v.manufacturerReferenceNumber ?? '').toString().trim() || undefined,
      manufacturerBillbackName: (v.manufacturerBillbackName ?? '').toString().trim() || undefined,
      manufacturerTermsAndConditions: (v.manufacturerTermsAndConditions ?? '').toString().trim() || undefined,
      manufacturerNotes: (v.manufacturerNotes ?? '').toString().trim() || undefined,
      contactPerson: (v.contactPerson ?? '').toString().trim() || undefined,
      entegraContractType: (v.entegraContractType ?? '').toString().trim() || undefined,
      entegraVdaProgram: (v.entegraVdaProgram ?? '').toString().trim() || undefined,
      startDate: v.startDate ? new Date(v.startDate) : new Date(),
      endDate: v.endDate ? new Date(v.endDate) : new Date(),
      prices: this.priceRows.map(r => ({
        productId: Number(r.productId),
        priceType: r.priceType,
        uom: r.uom,
        allowance: r.allowance ?? null,
        commercialDelPrice: r.commercialDelPrice ?? null,
        commercialFobPrice: r.commercialFobPrice ?? null,
        commodityDelPrice: r.commodityDelPrice ?? null,
        commodityFobPrice: r.commodityFobPrice ?? null,
        estimatedQty: r.estimatedQty ?? null,
        billbacksAllowed: !!r.billbacksAllowed,
        pua: r.pua ?? null,
        ffsPrice: r.ffsPrice ?? null,
        noiPrice: r.noiPrice ?? null,
        ptv: r.ptv ?? null,
        internalNotes: r.internalNotes ?? null
      }))
    } as any;

    this.contractService.createVersion(this.contract.id, req).subscribe({
      next: (newVersion) => {
        this.submitting = false;
        this.cloneMode = false;
        this.readonlyMode = true;
        this.selectedAction = null;
        this.successMessage = `Version ${newVersion.versionNumber} created successfully.`;
        // Refresh versions and focus the newly created one
        this.contractService.getVersions(this.contract!.id).subscribe(vs => {
          this.versions = vs || [];
          this.selectedVersionNumber = newVersion.versionNumber;
          this.onVersionChange();
        });
      },
      error: err => {
        this.error = err?.message || 'Failed to create version.';
        this.submitting = false;
      }
    });
  }

  // Derive source Proposal ID for existing records that predate FK, using manufacturerReferenceNumber like 'PROPOSAL-123'
  private parseProposalIdFromRef(ref: any): number | null {
    const s = (ref ?? '').toString();
    const m = s.match(/proposal[-\s#:]*(\d+)/i);
    if (m && !isNaN(Number(m[1]))) return Number(m[1]);
    return null;
  }

  public sourceProposalId(): number | null {
    // 1) Direct FK present
    if (this.contract && this.contract.proposalId != null) {
      const n = Number(this.contract.proposalId);
      if (!isNaN(n)) return n;
    }
    // 2) Query param hint (when redirected right after creation)
    const qp = (this.route as any)?.snapshot?.queryParamMap?.get?.('fromProposal');
    if (qp && !isNaN(Number(qp))) return Number(qp);
    // 3) Derive from form or existing value (older rows)
    const formRef = this.form?.get('manufacturerReferenceNumber')?.value;
    const idFromForm = this.parseProposalIdFromRef(formRef);
    if (idFromForm != null) return idFromForm;
    return this.parseProposalIdFromRef(this.contract?.manufacturerReferenceNumber);
  }

}

