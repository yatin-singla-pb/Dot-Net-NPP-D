import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProposalService, ProposalCreateDto, ProposalType, ProposalStatus, PriceType, ProductProposalStatus } from '../../services/proposal.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { DistributorService } from '../../services/distributor.service';
import { IndustryService } from '../../services/industry.service';
import { OpCoService } from '../../services/opco.service';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { ContractService } from '../../services/contract.service';
import { ContractVersionPrice } from '../../models/contract-version.model';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';
import { ProductSearchModalComponent } from '../../shared/components/product-search-modal/product-search-modal.component';
import { ExcelExportService } from '../../shared/services/excel-export.service';


@Component({
  selector: 'app-proposal-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterModule, ProductSearchModalComponent],
  templateUrl: './proposal-create.component.html'
})
export class ProposalCreateComponent implements OnInit {
  @ViewChild('excelFileInput') excelFileInput!: ElementRef<HTMLInputElement>;

  form!: FormGroup;
  loading = false;
  submitting = false;
  error: string | null = null;

  // Excel upload/download
  uploadingExcel = false;
  excelUploadError: string | null = null;
  excelFeedback: { type: 'success' | 'warning' | 'danger'; message: string } | null = null;

  // Panels & search filters for multi-selects (like contracts)
  showIndustriesPanel = false;
  showDistributorsPanel = false;
  showOpcoPanel = false;
  industryFilter = '';
  distributorFilter = '';
  opcoFilter = '';

  // Manufacturer searchable single-select
  showManufacturerPanel = false;
  manufacturerFilter = '';
  filteredManufacturers: any[] = [];

  // Modal state for product selection (like contracts)
  showProductModal = false;
  productSearch = '';
  selectAllProducts = false;
  selectedProductIds: Set<number> = new Set<number>();

  // Contract selection modal for Amendment type
  showContractModal = false;
  contractSearch = '';
  contracts: any[] = [];
  contractsLoading = false;
  selectedContract: any = null;

  // Amendment associations (read-only)
  amendmentDistributors: Array<{id:number; name:string}> = [];
  amendmentIndustries: Array<{id:number; name:string}> = [];
  amendmentOpcos: Array<{id:number; name:string}> = [];
  amendmentPrices: ContractVersionPrice[] = [];

  isAmendment(): boolean {
    const typeId = this.form?.get('proposalTypeId')?.value;
    const t = this.proposalTypes.find(x => x.id === typeId);
    return t?.name === 'Amendment';
  }

  private computeAmendmentActionForProduct(productId: number): number | null {
    if (!this.isAmendment() || !this.form.get('amendedContractId')?.value) return null;
    const inContract = (this.amendmentProducts || []).some(p => Number(p.id) === Number(productId));
    // 1 = Add, 2 = Update
    return inContract ? 2 : 1;
  }

  amendmentProducts: Array<{id:number; name:string}> = [];
  manufacturerNameLabel: string = '';
  // Role helpers
  isAdminOrNpp(): boolean {
    return this.auth?.hasAnyRole?.(['System Administrator', 'Contract Manager']) ?? false;
  }
  isManufacturerUser(): boolean {
    return this.auth?.hasRole?.('Manufacturer') ?? false;
  }

  amendmentActions = [ { id: 1, name: 'Add' }, { id: 2, name: 'Update' } ];

  getAmendmentPriceForProduct(productId: number | null | undefined): ContractVersionPrice | null {
    if (!productId) return null;
    return this.amendmentPrices.find(p => Number(p.productId) === Number(productId)) || null;
  }

  private ensureProductRowsForContract(): void {
    const existingIds = new Set<number>((this.form.value.products || []).map((p: any) => Number(p.productId)).filter((x: any) => !!x));
    (this.amendmentProducts || []).forEach(p => {
      if (!existingIds.has(p.id)) {
        const fg = this.fb.group({
          productId: [p.id, [Validators.required]],
          priceTypeId: [null],
          quantity: [null, [Validators.min(1)]],
          productProposalStatusId: [null as number | null],
          amendmentActionId: [{ value: this.computeAmendmentActionForProduct(p.id), disabled: true }],
          // Pricing fields
          uom: [null],
          billbacksAllowed: [false],
          allowance: [null, [Validators.min(0)]],
          commercialDelPrice: [null, [Validators.min(0)]],
          commercialFobPrice: [null, [Validators.min(0)]],
          commodityDelPrice: [null, [Validators.min(0)]],
          commodityFobPrice: [null, [Validators.min(0)]],
          // Additional fields
          pua: [null, [Validators.min(0)]],
          ffsPrice: [null, [Validators.min(0)]],
          noiPrice: [null, [Validators.min(0)]],
          ptv: [null, [Validators.min(0)]],
          internalNotes: ['', [Validators.maxLength(1000)]],
          manufacturerNotes: ['', [Validators.maxLength(1000)]]
        });
        const cp = this.getAmendmentPriceForProduct(p.id);
        const defaultQty = (cp?.estimatedQty != null ? cp.estimatedQty : null);
        if (defaultQty != null) {
          fg.patchValue({ quantity: defaultQty as any });
        }
        const requested = this.productProposalStatuses.find(s => s.name === 'Requested');
        if (requested) fg.patchValue({ productProposalStatusId: requested.id });
        this.productsArray.push(fg);
      }


    });
  }


  // Lookup data
  proposalTypes: ProposalType[] = [];
  proposalStatuses: ProposalStatus[] = [];
  priceTypes: PriceType[] = [];
  productProposalStatuses: ProductProposalStatus[] = [];
  manufacturers: any[] = [];
  distributors: any[] = [];
  industries: any[] = [];
  opcos: any[] = [];
  // UI constants
  uoms: string[] = ['Cases', 'Pounds'];

  products: any[] = [];

  // Modal state for adding a single product with full details
  showAddProductModal = false;
  addProductForm: FormGroup | null = null;
  addProductSearchTerm = '';
  addProductError: string | null = null;
  selectedAddProduct: Product | null = null;
  showProductSearchForAdd = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private manufacturerService: ManufacturerService,
    private distributorService: DistributorService,
    private industryService: IndustryService,
    private opcoService: OpCoService,
    private productService: ProductService,
    private contractService: ContractService,
    private auth: AuthService,
    private http: HttpClient,
    private apiService: ApiService,
    private excelExportService: ExcelExportService
  ) {
    this.createForm();
  }

  ngOnInit(): void {
    this.loadLookupData();

    // Check if we're renewing from a contract
    this.route.queryParams.subscribe(params => {
      const renewFromContractId = params['renewFromContract'];
      if (renewFromContractId) {
        this.loadContractForRenewal(+renewFromContractId);
      }
    });
  }

  private createForm(): void {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      proposalTypeId: [null, [Validators.required]],
      proposalStatusId: [null, [Validators.required]],
      manufacturerId: [null],
      startDate: [''],
      endDate: [''],
      dueDate: [''],
      internalNotes: ['', [Validators.maxLength(1000)]],
      distributorIds: [[]],
      industryIds: [[]],
      opcoIds: [[]],
      products: this.fb.array([]),
      amendedContractId: [null]
    });
  }

  get productsArray(): FormArray {
    return this.form.get('products') as FormArray;
  }

  private loadLookupData(): void {
    this.loading = true;
    this.error = null;

    // Load all lookup data in parallel using forkJoin (anonymous endpoints)
    forkJoin({
      proposalTypes: this.proposalService.getProposalTypes(),
      proposalStatuses: this.proposalService.getProposalStatuses(),
      priceTypes: this.proposalService.getPriceTypes(),
      productProposalStatuses: this.proposalService.getProductProposalStatuses(),
      manufacturers: this.proposalService.getManufacturers(),
      distributors: this.proposalService.getDistributors(),
      industries: this.proposalService.getIndustries(),
      opcos: this.proposalService.getOpCos()
    }).subscribe({
      next: (data) => {
        this.proposalTypes = data.proposalTypes || [];
        this.proposalStatuses = data.proposalStatuses || [];
        this.priceTypes = data.priceTypes || [];
        this.productProposalStatuses = data.productProposalStatuses || [];
        this.manufacturers = data.manufacturers || [];
        this.distributors = data.distributors || [];
        this.industries = data.industries || [];
        this.opcos = data.opcos || [];

        // Initialize filtered manufacturers
        this.applyManufacturerFilter('');

        // React to Amendment type to open contract modal
        const typeCtrl = this.form.get('proposalTypeId');
        typeCtrl?.valueChanges.subscribe((typeId: number | null) => {
          const t = this.proposalTypes.find(x => x.id === typeId);
          if (t?.name === 'Amendment') {
            this.openContractModal();
          } else {
            // If switching away from Amendment, re-enable fields and clear amendment context
            this.setAmendmentReadonly(false);
            this.form.patchValue({ amendedContractId: null }, { emitEvent: false });
            this.amendmentDistributors = [];
            this.amendmentIndustries = [];
            this.amendmentOpcos = [];
            this.amendmentProducts = [];
            this.amendmentPrices = [];
          }
        });

        this.products = []; // loaded on manufacturer selection

        // Role-based default status
        const requestedStatus = this.proposalStatuses.find(s => s.name === 'Requested');
        const savedStatus = this.proposalStatuses.find(s => s.name === 'Saved');
        if (this.isAdminOrNpp() && requestedStatus) {
          this.form.patchValue({ proposalStatusId: requestedStatus.id });
        } else if (this.isManufacturerUser() && savedStatus) {
          this.form.patchValue({ proposalStatusId: savedStatus.id });
        }

        // Manufacturer users: auto-set and lock Manufacturer, and show read-only name
        if (this.isManufacturerUser()) {
          const ids = (this.auth?.manufacturerIds || []).map((x: any) => Number(x)).filter((n: number) => !isNaN(n));
          if (ids.length) {
            const chosenId = ids[0];
            this.form.patchValue({ manufacturerId: chosenId }, { emitEvent: true });
            const man = (this.manufacturers || []).find((m: any) => Number(m.id) === chosenId);
            this.manufacturerNameLabel = man?.name || `Manufacturer #${chosenId}`;
            this.form.get('manufacturerId')?.disable({ emitEvent: false });
          }
        }

        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load lookup data:', error);
        this.error = 'Failed to load lookup data. Please refresh the page and try again.';
        this.loading = false;
      }
    });

    // Dynamic dependent lookups
    this.form.get('manufacturerId')?.valueChanges.subscribe((manufacturerId: number | null) => {
      const ids = manufacturerId ? [Number(manufacturerId)] : [];
      this.proposalService.getProductsByManufacturers(ids).subscribe({
        next: (prods) => {
          this.products = prods || [];
          // Reset modal selections if manufacturer changed


          this.selectedProductIds.clear();
          this.selectAllProducts = false;
        },
        error: () => this.products = []
      });
    });

    this.form.get('distributorIds')?.valueChanges.subscribe((ids: number[] | null) => {
      const distributorIds = (ids || []).map(x => Number(x));
      this.proposalService.getOpCosByDistributors(distributorIds).subscribe({
        next: (ops) => this.opcos = ops || [],
        error: () => { /* keep previous opcos if filtering fails */ }
      });
    });
  }

  // ===== Multi-select helpers (match contracts UX) =====
  get selectedIndustries() {
    const ids: number[] = this.form.get('industryIds')?.value || [];
    return (this.industries || []).filter((i: any) => ids.includes(i.id));
  }
  get selectedDistributors() {
    const ids: number[] = this.form.get('distributorIds')?.value || [];
    return (this.distributors || []).filter((d: any) => ids.includes(d.id));
  }
  get selectedOpcos() {
    const ids: number[] = this.form.get('opcoIds')?.value || [];
    return (this.opcos || []).filter((o: any) => ids.includes(o.id));
  }

  togglePanel(control: 'industryIds'|'distributorIds'|'opcoIds') {
    if (control === 'industryIds') this.showIndustriesPanel = !this.showIndustriesPanel;
    if (control === 'distributorIds') this.showDistributorsPanel = !this.showDistributorsPanel;
    if (control === 'opcoIds') this.showOpcoPanel = !this.showOpcoPanel;
  }
  closePanel(control: 'industryIds'|'distributorIds'|'opcoIds') {
    if (control === 'industryIds') this.showIndustriesPanel = false;
    if (control === 'distributorIds') this.showDistributorsPanel = false;
    if (control === 'opcoIds') this.showOpcoPanel = false;
  }
  applyPanel(control: 'industryIds'|'distributorIds'|'opcoIds') {
    this.closePanel(control);
  }

  isIdSelected(control: 'industryIds'|'distributorIds'|'opcoIds', id: number): boolean {
    const arr: number[] = this.form.get(control)?.value || [];
    return arr.includes(id);
  }
  toggleId(control: 'industryIds'|'distributorIds'|'opcoIds', id: number) {
    const arr: number[] = [...(this.form.get(control)?.value || [])];
    const idx = arr.indexOf(id);
    if (idx >= 0) arr.splice(idx, 1); else arr.push(id);
    this.form.patchValue({ [control]: arr });
  }
  // ===== Amendment: Contract selection modal logic =====
  openContractModal() {
    this.contractSearch = '';
    this.selectedContract = null;
    this.showContractModal = true;
    this.loadContracts();
  }
  closeContractModal() { this.showContractModal = false; }
  loadContracts() {
    this.contractsLoading = true;
    this.contractService.getPaginated(1, 50, this.contractSearch).subscribe({
      next: (res) => {
        this.contracts = res.items || [];
        this.contractsLoading = false;
      },
      error: () => {
        this.contracts = [];
        this.contractsLoading = false;
      }
    });
  }
  onContractSearchChange(val: string) {
    this.contractSearch = val || '';
    this.loadContracts();
  }
  selectContract(contract: any) {
    this.selectedContract = contract;
  }
  private setAmendmentReadonly(lock: boolean) {
    const fields = ['title','manufacturerId','startDate','endDate','internalNotes'];
    fields.forEach(f => {
      const ctrl = this.form.get(f);
      if (!ctrl) return;
      if (lock && ctrl.enabled) ctrl.disable({ emitEvent: false });
      if (!lock && ctrl.disabled) ctrl.enable({ emitEvent: false });
    });
  }
  confirmContractSelection() {
    if (!this.selectedContract) return;
    const c = this.selectedContract;
    // Patch known fields from contract
    this.form.patchValue({
      title: c.name || c.title || '',
      startDate: c.startDate ? new Date(c.startDate).toISOString().slice(0,10) : '',
      endDate: c.endDate ? new Date(c.endDate).toISOString().slice(0,10) : '',
      internalNotes: c.internalNotes || ''
    }, { emitEvent: false });
    // Set amended contract and load read-only associations
    this.form.patchValue({ amendedContractId: c.id }, { emitEvent: false });
    // Clear any existing product rows; products will be added explicitly via modal for Amendment
    while (this.productsArray.length) { this.productsArray.removeAt(0); }
    this.loadAmendmentAssociations(c.id);



    // Fetch manufacturer assignments to set manufacturerId
    this.contractService.getManufacturerAssignments(c.id).subscribe({
      next: (assignments) => {
        const first = (assignments || [])[0];
        if (first?.manufacturerId) {
          this.form.patchValue({ manufacturerId: first.manufacturerId }, { emitEvent: true });
        }
        // Lock the fields after population
        this.setAmendmentReadonly(true);
      },
      error: () => {
        // Even if assignments fail, lock title/dates/notes; leave manufacturer editable
        this.setAmendmentReadonly(true);
      }
    });

    this.closeContractModal();
  }

  removeFromMulti(control: 'industryIds'|'distributorIds'|'opcoIds', id: number) {
    const arr: number[] = [...(this.form.get(control)?.value || [])].filter(x => x !== id);
    this.form.patchValue({ [control]: arr });
  }

  get filteredIndustries() {
    const q = (this.industryFilter || '').toLowerCase();
    return (this.industries || []).filter((i: any) => !q || i.name?.toLowerCase().includes(q));
  }
  get filteredDistributors() {
    const q = (this.distributorFilter || '').toLowerCase();
    return (this.distributors || []).filter((d: any) => !q || d.name?.toLowerCase().includes(q));
  }
  get filteredOpcos() {
    const q = (this.opcoFilter || '').toLowerCase();
    return (this.opcos || []).filter((o: any) => !q || o.name?.toLowerCase().includes(q));
  }

  applyIndustryFilter(val: string) { this.industryFilter = val; }
  applyDistributorFilter(val: string) { this.distributorFilter = val; }
  applyOpcoFilter(val: string) { this.opcoFilter = val; }

  // --- Manufacturer searchable single-select helpers ---
  toggleManufacturerPanel(): void {
    this.showManufacturerPanel = !this.showManufacturerPanel;
    if (this.showManufacturerPanel) {
      this.applyManufacturerFilter(this.manufacturerFilter);
    }
  }

  closeManufacturerPanel(): void {
    this.showManufacturerPanel = false;
  }

  applyManufacturerFilter(term: string): void {
    this.manufacturerFilter = term || '';
    const t = this.manufacturerFilter.toLowerCase();
    this.filteredManufacturers = (this.manufacturers || []).filter((m: any) => (m.name || '').toLowerCase().includes(t));
  }

  selectManufacturer(id: number): void {
    this.form.get('manufacturerId')?.setValue(id);
    this.showManufacturerPanel = false;
  }

  getManufacturerName(id: number | null): string {
    const m = (this.manufacturers || []).find((x: any) => x.id === Number(id));
    return m?.name || '';
  }

  // Read-only product status helpers for display in template
  getProductStatusLabel(id?: number | null): string {
    if (id === undefined || id === null) return 'Pending';
    const name = this.productProposalStatuses.find(s => Number(s.id) === Number(id))?.name;
    return name || 'Pending';
  }

  statusBadgeClass(id?: number | null): any {
    const nm = (this.getProductStatusLabel(id) || '').toLowerCase();
    return {
      'bg-success': nm === 'accepted',
      'bg-danger': nm === 'rejected',
      'bg-warning text-dark': nm !== 'accepted' && nm !== 'rejected'
    };
  }

  getProductName(id: number): string {
    const p = (this.products || []).find((x: any) => x.id === id);
    return p?.name || `Product ${id}`;
  }

  getProductById(id: number): any {
    return (this.products || []).find((x: any) => x.id === id);
  }

  getProductHeaderTitle(id: number): string {
    const p = this.getProductById(id);
    if (!p) return '';

    const parts = [
      p.manufacturerProductCode,
      p.brand,
      p.packSize,
      p.name || p.description
    ].filter(Boolean);

    return parts.join(' - ');
  }

  getProductHeaderDisplay(id: number): string {
    const p = this.getProductById(id);
    if (!p) return '';

    const parts = [
      p.manufacturerProductCode,
      p.brand,
      p.packSize,
      p.name || p.description
    ].filter(Boolean);

    return parts.join(' - ');
  }

  getProductStatus(id: number): string {
    const p = this.getProductById(id);
    return p?.status || 'Active';
  }

  getProductStatusBadgeClass(status: string): any {
    const s = (status || '').toLowerCase();
    return {
      'bg-success': s === 'active',
      'bg-danger': s === 'discontinued',
      'bg-warning text-dark': s === 'inactive' || s === 'pending'
    };
  }

  // ===== Products modal (match contracts UX) =====
  onAddProductClick() {
    if (!this.form.get('manufacturerId')?.value) return;
    this.productSearch = '';
    this.selectAllProducts = false;
    this.showProductModal = true;
  }
  closeProductModal() { this.showProductModal = false; }

  filteredModalProducts() {
    const q = (this.productSearch || '').toLowerCase();
    return (this.products || []).filter((p: any) => {
      if (!q) return true;
      return (p.name?.toLowerCase().includes(q)) ||
             (p.productName?.toLowerCase().includes(q)) ||
             (p.description?.toLowerCase().includes(q)) ||
             (p.manufacturerProductCode?.toLowerCase().includes(q)) ||
             (p.packSize?.toLowerCase().includes(q));
    });
  }

  private loadAmendmentAssociations(contractId: number) {
    forkJoin({
      dist: this.contractService.getDistributorAssignments(contractId),
      inds: this.contractService.getIndustryAssignments(contractId),
      ops: this.contractService.getOpCoAssignments(contractId),
      prods: this.contractService.getContractProducts(contractId),
      prices: this.contractService.getContractPricing(contractId)
    }).subscribe({
      next: ({ dist, inds, ops, prods, prices }) => {
        const dmap = new Map<number, string>((this.distributors || []).map((d: any) => [d.id, d.name]));
        const imap = new Map<number, string>((this.industries || []).map((i: any) => [i.id, i.name]));
        const omap = new Map<number, string>((this.opcos || []).map((o: any) => [o.id, o.name]));
        this.amendmentDistributors = (dist || []).map((x: any) => ({ id: x.distributorId, name: dmap.get(x.distributorId) || String(x.distributorId) }));
        this.amendmentIndustries = (inds || []).map((x: any) => ({ id: x.industryId, name: imap.get(x.industryId) || String(x.industryId) }));
        this.amendmentOpcos = (ops || []).map((x: any) => ({ id: x.opCoId, name: omap.get(x.opCoId) || String(x.opCoId) }));
        this.amendmentProducts = (prods || []).map((p: any) => ({ id: p.id, name: p.name }));
        this.amendmentPrices = prices || [];

        // Recompute amendment action per existing product lines now that contract data is loaded
        this.productsArray.controls.forEach((grp: any) => {
          const pid = grp.get('productId')?.value;
          if (pid != null) {
            const actionId = this.computeAmendmentActionForProduct(pid);
            grp.get('amendmentActionId')?.setValue(actionId, { emitEvent: false });
          }
        });

        // In Amendment flow, do not auto-add contract products; they will be added explicitly from the modal
      },
      error: () => {
        this.amendmentDistributors = [];
        this.amendmentIndustries = [];
        this.amendmentOpcos = [];
        this.amendmentProducts = [];
        this.amendmentPrices = [];
      }
    });
  }

  toggleProductSelection(productId: number, checked: boolean) {
    if (checked) this.selectedProductIds.add(productId); else this.selectedProductIds.delete(productId);
  }
  toggleSelectAllProducts() {
    if (this.selectAllProducts) {
      this.filteredModalProducts().forEach((p: any) => this.selectedProductIds.add(p.id));
    } else {
      this.filteredModalProducts().forEach((p: any) => this.selectedProductIds.delete(p.id));
    }
  }
  addSelectedProducts() {
    const existingIds = new Set<number>((this.form.value.products || []).map((p: any) => Number(p.productId)).filter((x: any) => !!x));
    this.selectedProductIds.forEach(id => {
      if (!existingIds.has(id)) {
        const fg = this.fb.group({
          productId: [id, [Validators.required]],
          priceTypeId: [null],
          quantity: [null, [Validators.min(1)]],
          productProposalStatusId: [null as number | null],
          amendmentActionId: [{ value: this.computeAmendmentActionForProduct(id), disabled: true }],
          // Pricing fields
          uom: [null],
          billbacksAllowed: [false],
          allowance: [null, [Validators.min(0)]],
          commercialDelPrice: [null, [Validators.min(0)]],
          commercialFobPrice: [null, [Validators.min(0)]],
          commodityDelPrice: [null, [Validators.min(0)]],
          commodityFobPrice: [null, [Validators.min(0)]],
          // Additional fields
          pua: [null, [Validators.min(0)]],
          ffsPrice: [null, [Validators.min(0)]],
          noiPrice: [null, [Validators.min(0)]],
          ptv: [null, [Validators.min(0)]],
          internalNotes: ['', [Validators.maxLength(1000)]],
          manufacturerNotes: ['', [Validators.maxLength(1000)]]
        });
        const cp = this.getAmendmentPriceForProduct(id);
        const defaultQty = (cp?.estimatedQty != null ? cp.estimatedQty : null);
        if (defaultQty != null) {
          fg.patchValue({ quantity: defaultQty as any });
        }
        // Prefill contract-related pricing fields if available (Amendment context)
        if (cp) {
          fg.patchValue({
            uom: (cp.uom ?? null) as any,
            billbacksAllowed: !!cp.billbacksAllowed as any,
            allowance: (cp.allowance ?? null) as any,
            commercialDelPrice: (cp.commercialDelPrice ?? null) as any,
            commercialFobPrice: (cp.commercialFobPrice ?? null) as any,
            commodityDelPrice: (cp.commodityDelPrice ?? null) as any,
            commodityFobPrice: (cp.commodityFobPrice ?? null) as any,
            pua: (cp.pua ?? null) as any,
            ffsPrice: (cp.ffsPrice ?? null) as any,
            noiPrice: (cp.noiPrice ?? null) as any,
            ptv: (cp.ptv ?? null) as any,
            internalNotes: (cp.internalNotes ?? '') as any
          } as any, { emitEvent: false });
        }
        const requested = this.productProposalStatuses.find(s => s.name === 'Requested');
        if (requested) fg.patchValue({ productProposalStatusId: requested.id });

        // Watch allowance changes for this product
        fg.get('allowance')?.valueChanges.pipe(
          debounceTime(10),
          distinctUntilChanged()
        ).subscribe((allowanceValue: number | null) => {
          this.handleAllowanceChange(fg, allowanceValue);
        });

        // Watch pricing field changes for this product
        const pricingFields = [
          'commercialDelPrice',
          'commercialFobPrice',
          'commodityDelPrice',
          'commodityFobPrice',
          'pua',
          'ffsPrice',
          'noiPrice',
          'ptv'
        ];
        pricingFields.forEach(field => {
          fg.get(field)?.valueChanges.pipe(
            debounceTime(10),
            distinctUntilChanged()
          ).subscribe(() => {
            this.handlePricingFieldChange(fg);
          });
        });

        this.productsArray.push(fg);
      }
    });
    this.closeProductModal();
  }

  get filteredAddProducts(): any[] {
    const q = (this.addProductSearchTerm || '').toLowerCase();
    if (!q) return this.products || [];
    return (this.products || []).filter((p: any) =>
      (p.name || '').toLowerCase().includes(q) ||
      (p.description || '').toLowerCase().includes(q) ||
      (p.manufacturerProductCode || '').toLowerCase().includes(q) ||
      (p.packSize || '').toLowerCase().includes(q) ||
      (p.brand || '').toLowerCase().includes(q)
    );
  }

  openAddProductModal(): void {
    this.addProductSearchTerm = '';
    this.addProductError = null;
    this.selectedAddProduct = null;
    this.addProductForm = this.fb.group({
      productId: [null, [Validators.required]],
      priceTypeId: [null],
      quantity: [null, [Validators.min(1)]],
      uom: [null],
      billbacksAllowed: [false],
      allowance: [null, [Validators.min(0)]],
      commercialDelPrice: [null, [Validators.min(0)]],
      commercialFobPrice: [null, [Validators.min(0)]],
      commodityDelPrice: [null, [Validators.min(0)]],
      commodityFobPrice: [null, [Validators.min(0)]],
      pua: [null, [Validators.min(0)]],
      ffsPrice: [null, [Validators.min(0)]],
      noiPrice: [null],
      ptv: [null, [Validators.min(0)]],
      internalNotes: ['', [Validators.maxLength(1000)]],
      manufacturerNotes: ['', [Validators.maxLength(1000)]]
    });

    // Set up pricing interaction subscriptions on the modal form
    this.addProductForm.get('priceTypeId')?.valueChanges.subscribe((priceTypeId) => {
      this.handlePriceTypeChange(this.addProductForm!, priceTypeId);
    });

    this.addProductForm.get('allowance')?.valueChanges.pipe(
      debounceTime(10),
      distinctUntilChanged()
    ).subscribe((allowanceValue) => {
      this.handleAllowanceChange(this.addProductForm!, allowanceValue);
    });

    const pricingFields = [
      'commercialDelPrice', 'commercialFobPrice', 'commodityDelPrice',
      'commodityFobPrice', 'pua', 'ffsPrice', 'noiPrice', 'ptv'
    ];
    pricingFields.forEach(field => {
      this.addProductForm!.get(field)?.valueChanges.pipe(
        debounceTime(10),
        distinctUntilChanged()
      ).subscribe(() => {
        this.handlePricingFieldChange(this.addProductForm!);
      });
    });

    // When product is selected in modal, prefill amendment pricing if applicable
    this.addProductForm.get('productId')?.valueChanges.subscribe((pid: number | null) => {
      if (pid != null && this.isAmendment() && this.form.get('amendedContractId')?.value) {
        const cp = this.getAmendmentPriceForProduct(pid);
        if (cp) {
          this.addProductForm!.patchValue({
            quantity: (cp.estimatedQty ?? null) as any,
            uom: (cp.uom ?? null) as any,
            billbacksAllowed: !!cp.billbacksAllowed as any,
            allowance: (cp.allowance ?? null) as any,
            commercialDelPrice: (cp.commercialDelPrice ?? null) as any,
            commercialFobPrice: (cp.commercialFobPrice ?? null) as any,
            commodityDelPrice: (cp.commodityDelPrice ?? null) as any,
            commodityFobPrice: (cp.commodityFobPrice ?? null) as any,
            pua: (cp.pua ?? null) as any,
            ffsPrice: (cp.ffsPrice ?? null) as any,
            noiPrice: (cp.noiPrice ?? null) as any,
            ptv: (cp.ptv ?? null) as any,
            internalNotes: (cp.internalNotes ?? '') as any
          } as any, { emitEvent: false });
        }
      }
    });

    this.showAddProductModal = true;
  }

  cancelAddProductModal(): void {
    this.showAddProductModal = false;
    this.addProductForm = null;
    this.addProductSearchTerm = '';
    this.addProductError = null;
    this.selectedAddProduct = null;
  }

  openProductSearchForAdd(): void {
    this.showProductSearchForAdd = true;
  }

  onAddProductSelected(product: Product): void {
    this.selectedAddProduct = product;
    this.addProductForm?.patchValue({ productId: product.id });
    this.addProductForm?.get('productId')?.markAsTouched();
    this.showProductSearchForAdd = false;
  }

  clearAddProduct(): void {
    this.selectedAddProduct = null;
    this.addProductForm?.patchValue({ productId: null });
  }

  confirmAddProduct(): void {
    if (!this.addProductForm) return;

    this.addProductError = null;

    if (this.addProductForm.invalid) {
      Object.keys(this.addProductForm.controls).forEach(key => {
        this.addProductForm!.get(key)?.markAsTouched();
      });
      return;
    }

    const values = this.addProductForm.getRawValue();

    // Check for duplicate product
    const existingProductIds: number[] = [];
    for (let i = 0; i < this.productsArray.length; i++) {
      const pid = this.productsArray.at(i).get('productId')?.value;
      if (pid != null) existingProductIds.push(Number(pid));
    }
    if (existingProductIds.includes(Number(values.productId))) {
      const product = this.getProductById(Number(values.productId));
      const productName = product?.name || `Product #${values.productId}`;
      this.addProductError = `"${productName}" has already been added to this proposal.`;
      return;
    }

    // Create the real product FormGroup with the entered values
    const productGroup = this.fb.group({
      productId: [values.productId, [Validators.required]],
      priceTypeId: [values.priceTypeId],
      quantity: [values.quantity, [Validators.min(1)]],
      productProposalStatusId: [null as number | null],
      amendmentActionId: [{ value: this.computeAmendmentActionForProduct(values.productId), disabled: true }],
      uom: [values.uom],
      billbacksAllowed: [values.billbacksAllowed],
      allowance: [values.allowance, [Validators.min(0)]],
      commercialDelPrice: [values.commercialDelPrice, [Validators.min(0)]],
      commercialFobPrice: [values.commercialFobPrice, [Validators.min(0)]],
      commodityDelPrice: [values.commodityDelPrice, [Validators.min(0)]],
      commodityFobPrice: [values.commodityFobPrice, [Validators.min(0)]],
      pua: [values.pua, [Validators.min(0)]],
      ffsPrice: [values.ffsPrice, [Validators.min(0)]],
      noiPrice: [values.noiPrice],
      ptv: [values.ptv, [Validators.min(0)]],
      internalNotes: [values.internalNotes || '', [Validators.maxLength(1000)]],
      manufacturerNotes: [values.manufacturerNotes || '', [Validators.maxLength(1000)]]
    });

    // Set default product status to "Requested"
    const requested = this.productProposalStatuses.find(s => s.name === 'Requested');
    if (requested) {
      productGroup.patchValue({ productProposalStatusId: requested.id });
    }

    // Set up watchers on the real product group
    productGroup.get('priceTypeId')?.valueChanges.subscribe((priceTypeId) => {
      this.handlePriceTypeChange(productGroup, priceTypeId);
    });

    productGroup.get('allowance')?.valueChanges.pipe(
      debounceTime(10),
      distinctUntilChanged()
    ).subscribe((allowanceValue) => {
      this.handleAllowanceChange(productGroup, allowanceValue);
    });

    const pricingFieldNames = [
      'commercialDelPrice', 'commercialFobPrice', 'commodityDelPrice',
      'commodityFobPrice', 'pua', 'ffsPrice', 'noiPrice', 'ptv'
    ];
    pricingFieldNames.forEach(field => {
      productGroup.get(field)?.valueChanges.pipe(
        debounceTime(10),
        distinctUntilChanged()
      ).subscribe(() => {
        this.handlePricingFieldChange(productGroup);
      });
    });

    // Apply the same pricing disable state from the modal form to the real group
    pricingFieldNames.forEach(field => {
      if (this.addProductForm!.get(field)?.disabled) {
        productGroup.get(field)?.disable({ emitEvent: false });
      }
    });
    if (this.addProductForm!.get('allowance')?.disabled) {
      productGroup.get('allowance')?.disable({ emitEvent: false });
    }
    ['quantity', 'uom', 'billbacksAllowed'].forEach(field => {
      if (this.addProductForm!.get(field)?.disabled) {
        productGroup.get(field)?.disable({ emitEvent: false });
      }
    });

    this.productsArray.push(productGroup);

    // Close the modal
    this.showAddProductModal = false;
    this.addProductForm = null;
  }

  isAddProductFieldInvalid(field: string): boolean {
    if (!this.addProductForm) return false;
    const control = this.addProductForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  removeProduct(index: number): void {
    this.productsArray.removeAt(index);
  }

  handleAllowanceChange(productGroup: FormGroup, allowanceValue: number | null | undefined): void {
    const hasAllowance = allowanceValue != null && allowanceValue > 0;
    const pricingFields = [
      'commercialDelPrice',
      'commercialFobPrice',
      'commodityDelPrice',
      'commodityFobPrice',
      'pua',
      'ffsPrice',
      'noiPrice',
      'ptv'
    ];

    if (hasAllowance) {
      // Check if any pricing field is currently enabled (meaning we need to disable them)
      const anyEnabled = pricingFields.some(field => {
        const control = productGroup.get(field);
        return control && control.enabled;
      });

      // Only make changes if there's actually something to change
      if (anyEnabled) {
        pricingFields.forEach(field => {
          const control = productGroup.get(field);
          if (control && control.enabled) {
            control.setValue(null, { emitEvent: false });
            control.disable({ emitEvent: false });
          }
        });
      }
    } else {
      const hasPricingValue = pricingFields.some(field => {
        const val = productGroup.get(field)?.value;
        return val != null && val > 0;
      });

      if (!hasPricingValue) {
        // Check if any pricing field is currently disabled (meaning we need to enable them)
        const anyDisabled = pricingFields.some(field => {
          const control = productGroup.get(field);
          return control && control.disabled;
        });

        // Only make changes if there's actually something to change
        if (anyDisabled) {
          pricingFields.forEach(field => {
            const control = productGroup.get(field);
            if (control && control.disabled) {
              control.enable({ emitEvent: false });
            }
          });
        }
      }
    }
  }

  handlePricingFieldChange(productGroup: FormGroup): void {
    const pricingFields = [
      'commercialDelPrice',
      'commercialFobPrice',
      'commodityDelPrice',
      'commodityFobPrice',
      'pua',
      'ffsPrice',
      'noiPrice',
      'ptv'
    ];

    const hasPricingValue = pricingFields.some(field => {
      const val = productGroup.get(field)?.value;
      return val != null && val > 0;
    });

    const allowanceControl = productGroup.get('allowance');
    if (hasPricingValue) {
      // Only make changes if allowance is currently enabled
      if (allowanceControl && allowanceControl.enabled) {
        allowanceControl.setValue(null, { emitEvent: false });
        allowanceControl.disable({ emitEvent: false });
      }
    } else {
      // Only make changes if allowance is currently disabled
      if (allowanceControl && allowanceControl.disabled) {
        allowanceControl.enable({ emitEvent: false });
      }
    }
  }

  handlePriceTypeChange(productGroup: FormGroup, priceTypeId: number | null | undefined): void {
    const pricingFields = [
      'allowance',
      'commercialDelPrice',
      'commercialFobPrice',
      'commodityDelPrice',
      'commodityFobPrice',
      'pua',
      'ffsPrice',
      'noiPrice',
      'ptv'
    ];

    const otherFields = [
      'quantity',
      'uom',
      'billbacksAllowed'
    ];

    if (!priceTypeId) {
      pricingFields.forEach(field => {
        const control = productGroup.get(field);
        if (control) {
          control.clearValidators();
          control.setValidators([Validators.min(0)]);
          control.updateValueAndValidity({ emitEvent: false });
          control.enable({ emitEvent: false });
        }
      });
      otherFields.forEach(field => {
        productGroup.get(field)?.enable({ emitEvent: false });
      });
      return;
    }

    const priceType = this.priceTypes.find(pt => pt.id === priceTypeId);
    const priceTypeName = priceType?.name || '';

    const noPricingRequired = [
      'Product Discontinued',
      'Product Suspension',
      'Published List Price at time of Purchase'
    ].includes(priceTypeName);

    if (noPricingRequired) {
      [...pricingFields, ...otherFields].forEach(field => {
        const control = productGroup.get(field);
        if (control) {
          control.clearValidators();
          control.updateValueAndValidity({ emitEvent: false });
          if (field === 'billbacksAllowed') {
            control.setValue(false, { emitEvent: false });
          } else {
            control.setValue(null, { emitEvent: false });
          }
          control.disable({ emitEvent: false });
          control.markAsUntouched();
        }
      });
    } else {
      pricingFields.forEach(field => {
        const control = productGroup.get(field);
        if (control) {
          control.setValue(null, { emitEvent: false });
          control.enable({ emitEvent: false });

          if (priceTypeName === 'Guaranteed') {
            control.setValidators([Validators.required, Validators.min(0)]);
          } else {
            control.clearValidators();
            control.setValidators([Validators.min(0)]);
          }
          control.updateValueAndValidity({ emitEvent: false });
        }
      });

      otherFields.forEach(field => {
        productGroup.get(field)?.enable({ emitEvent: false });
      });
    }
  }

  isPricingDisabledForProduct(productGroup: FormGroup): boolean {
    const priceTypeId = productGroup.get('priceTypeId')?.value;
    if (!priceTypeId) {
      return false;
    }

    const priceType = this.priceTypes.find(pt => pt.id === priceTypeId);
    const priceTypeName = priceType?.name || '';

    return [
      'Product Discontinued',
      'Product Suspension',
      'Published List Price at time of Purchase'
    ].includes(priceTypeName);
  }

  onPriceTypeChange(productGroup: FormGroup): void {
    const priceTypeId = productGroup.get('priceTypeId')?.value;
    this.handlePriceTypeChange(productGroup, priceTypeId);
  }

  isFieldInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  isProductFieldInvalid(index: number, field: string): boolean {
    const control = this.productsArray.at(index).get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onSubmit(): void {
    if (this.form.invalid) {
      Object.values(this.form.controls).forEach(control => {
        control.markAsTouched();
        if (control instanceof FormArray) {
          control.controls.forEach(arrayControl => {
            if (arrayControl instanceof FormGroup) {
              Object.values(arrayControl.controls).forEach(c => c.markAsTouched());
            }
          });
        }
      });
      return;
    }

    this.submitting = true;
    this.error = null;

    const formValue = this.form.getRawValue() as any; // include disabled fields

    const distributorIds: number[] = Array.from(new Set(((formValue.distributorIds || []) as any[]).map((x: any) => Number(x)).filter((x: any) => !!x)));
    const industryIds: number[] = Array.from(new Set(((formValue.industryIds || []) as any[]).map((x: any) => Number(x)).filter((x: any) => !!x)));
    const opcoIds: number[] = Array.from(new Set(((formValue.opcoIds || formValue.opCoIds || []) as any[]).map((x: any) => Number(x)).filter((x: any) => !!x)));

    const toNullableNumber = (value: any): number | null => {
      return value === null || value === undefined || value === '' ? null : Number(value);
    };

    const products = ((formValue.products || []) as any[]).map(p => ({
      productId: Number(p.productId),
      priceTypeId: toNullableNumber(p.priceTypeId),
      quantity: toNullableNumber(p.quantity),
      productProposalStatusId: toNullableNumber(p.productProposalStatusId),
      // Pricing fields
      uom: (p.uom ?? null),
      billbacksAllowed: !!p.billbacksAllowed,
      allowance: toNullableNumber(p.allowance),
      commercialDelPrice: toNullableNumber(p.commercialDelPrice),
      commercialFobPrice: toNullableNumber(p.commercialFobPrice),
      commodityDelPrice: toNullableNumber(p.commodityDelPrice),
      commodityFobPrice: toNullableNumber(p.commodityFobPrice),
      // Additional fields
      pua: toNullableNumber(p.pua),
      ffsPrice: toNullableNumber(p.ffsPrice),
      noiPrice: p.noiPrice != null ? !!p.noiPrice : null,
      ptv: toNullableNumber(p.ptv),
      internalNotes: (p.internalNotes ?? null),
      manufacturerNotes: (p.manufacturerNotes ?? null),
      amendmentActionId: (this.isAmendment() && formValue.amendedContractId)
        ? (p.amendmentActionId != null && p.amendmentActionId !== ''
          ? Number(p.amendmentActionId)
          : this.computeAmendmentActionForProduct(p.productId))
        : null
    }));

    const payload: ProposalCreateDto = {
      title: String(formValue.title || '').trim(),
      proposalTypeId: Number(formValue.proposalTypeId),
      proposalStatusId: Number(formValue.proposalStatusId),
      manufacturerId: formValue.manufacturerId ? Number(formValue.manufacturerId) : null,
      startDate: formValue.startDate || null,
      endDate: formValue.endDate || null,
      dueDate: formValue.dueDate || null,
      internalNotes: (formValue.internalNotes || '').toString().trim() || null,
      products,
      distributorIds,
      industryIds,
      opcoIds
    };

    // Attach amendment fields if applicable (proposal-level)
    const isAmendment = this.proposalTypes.find(x => x.id === payload.proposalTypeId)?.name === 'Amendment';
    if (isAmendment) {
      (payload as any).amendedContractId = formValue.amendedContractId ? Number(formValue.amendedContractId) : null;
      if (!(payload as any).amendedContractId) {
        this.error = 'Amendment Contract is required.';
        this.submitting = false;
        return;
      }
    }

    this.proposalService.create(payload).subscribe({
      next: (result) => {
        if (this.isManufacturerUser()) {
          // Manufacturer: return to list to avoid admin-only detail route and continue workflow
          this.router.navigate(['/admin/proposals']);
        } else {
          // Admin/NPP: go to detail view
          this.router.navigate(['/admin/proposals', result.id]);
        }
      },
      error: (error) => {
        this.error = error?.error?.message || 'Failed to create proposal';
        this.submitting = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/proposals']);
  }

  // Excel Download/Upload Methods
  exportProducts(): void {
    if (this.productsArray.length === 0) {
      this.excelFeedback = { type: 'warning', message: 'No products to export.' };
      return;
    }

    const rows = [];
    for (let i = 0; i < this.productsArray.length; i++) {
      const pg = this.productsArray.at(i) as FormGroup;
      const productId = pg.get('productId')?.value;
      const product = this.getProductById(productId);

      rows.push({
        'SKU': product?.manufacturerProductCode || product?.sku || '',
        'Product Name': product?.name || product?.description || '',
        'UOM': pg.get('uom')?.value || '',
        'Billbacks Allowed': pg.get('billbacksAllowed')?.value ? 'Yes' : 'No',
        'Allowance': pg.get('allowance')?.value ?? '',
        'Commercial Del Price': pg.get('commercialDelPrice')?.value ?? '',
        'Commercial FOB Price': pg.get('commercialFobPrice')?.value ?? '',
        'Commodity Del Price': pg.get('commodityDelPrice')?.value ?? '',
        'Commodity FOB Price': pg.get('commodityFobPrice')?.value ?? '',
        'PUA': pg.get('pua')?.value ?? '',
        'FFS Price': pg.get('ffsPrice')?.value ?? '',
        'NOI Price': pg.get('noiPrice')?.value ?? '',
        'PTV': pg.get('ptv')?.value ?? '',
        'Internal Notes': pg.get('internalNotes')?.value || '',
        'Manufacturer Notes': pg.get('manufacturerNotes')?.value || ''
      });
    }

    this.excelExportService.exportToExcel(rows, 'Proposal_Products', 'Proposal Products');
  }

  downloadExcelTemplate(): void {
    const manufacturerId = this.form.get('manufacturerId')?.value;
    if (!manufacturerId) {
      this.excelFeedback = { type: 'danger', message: 'Please select a manufacturer first.' };
      return;
    }
    this.excelFeedback = null;
    this.proposalService.downloadTemplate(manufacturerId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Proposal_Products_Template.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.excelFeedback = { type: 'danger', message: 'Failed to download template.' };
      }
    });
  }

  uploadExcelFile(): void {
    const manufacturerId = this.form.get('manufacturerId')?.value;
    if (!manufacturerId) {
      this.excelFeedback = { type: 'danger', message: 'Please select a manufacturer first.' };
      return;
    }
    this.excelFileInput.nativeElement.click();
  }

  onExcelFileSelected(event: any): void {
    const file = event.target.files[0];
    if (!file) return;

    const manufacturerId = this.form.get('manufacturerId')?.value;
    if (!manufacturerId) {
      this.excelFeedback = { type: 'danger', message: 'Please select a manufacturer first.' };
      return;
    }

    this.uploadingExcel = true;
    this.excelFeedback = null;

    this.proposalService.importExcel(manufacturerId, file).subscribe({
      next: (response) => {
        this.uploadingExcel = false;

        if (!response.success && response.importedProducts?.length === 0) {
          const errors = (response.validationErrors || []).join('; ');
          this.excelFeedback = { type: 'danger', message: `${response.message}${errors ? ' — ' + errors : ''}` };
        } else {
          const result = this.populateProductsFromExcel(response.importedProducts || []);
          const parts: string[] = [];
          if (result.added > 0) parts.push(`${result.added} product(s) imported`);
          if (result.skippedDuplicates > 0) parts.push(`${result.skippedDuplicates} duplicate(s) skipped`);
          if (response.invalidRows > 0) parts.push(`${response.invalidRows} row(s) had errors`);

          const hasWarnings = result.skippedDuplicates > 0 || response.invalidRows > 0;
          if (response.validationErrors?.length > 0) {
            parts.push(response.validationErrors.join('; '));
          }
          this.excelFeedback = {
            type: hasWarnings ? 'warning' : 'success',
            message: parts.join('. ') + '.'
          };
        }
        this.excelFileInput.nativeElement.value = '';
      },
      error: (error) => {
        this.uploadingExcel = false;
        this.excelFeedback = { type: 'danger', message: error?.error?.message || error?.error?.error || error?.message || 'Failed to upload Excel file.' };
        this.excelFileInput.nativeElement.value = '';
      }
    });
  }

  private populateProductsFromExcel(importedProducts: any[]): { added: number; skippedDuplicates: number } {
    const existingProductIds = new Set<number>();
    for (let i = 0; i < this.productsArray.length; i++) {
      existingProductIds.add(this.productsArray.at(i).get('productId')?.value);
    }

    let added = 0;
    let skippedDuplicates = 0;

    importedProducts.forEach(product => {
      if (existingProductIds.has(product.productId)) {
        skippedDuplicates++;
        return;
      }

      const productGroup = this.fb.group({
        productId: [product.productId, [Validators.required]],
        priceTypeId: [null],
        quantity: [null, [Validators.min(1)]],
        productProposalStatusId: [null as number | null],
        amendmentActionId: [{ value: null, disabled: true }],
        uom: [product.uom || null],
        billbacksAllowed: [product.billbacksAllowed || false],
        allowance: [product.allowance || null, [Validators.min(0)]],
        commercialDelPrice: [product.commercialDelPrice || null, [Validators.min(0)]],
        commercialFobPrice: [product.commercialFobPrice || null, [Validators.min(0)]],
        commodityDelPrice: [product.commodityDelPrice || null, [Validators.min(0)]],
        commodityFobPrice: [product.commodityFobPrice || null, [Validators.min(0)]],
        pua: [product.pua || null, [Validators.min(0)]],
        ffsPrice: [product.ffsPrice || null, [Validators.min(0)]],
        noiPrice: [product.noiPrice || null],
        ptv: [product.ptv || null, [Validators.min(0)]],
        internalNotes: [product.internalNotes || null],
        manufacturerNotes: [product.manufacturerNotes || null]
      });

      // Set up pricing watchers (same pattern as confirmAddProduct)
      productGroup.get('priceTypeId')?.valueChanges.subscribe((priceTypeId) => {
        this.handlePriceTypeChange(productGroup, priceTypeId);
      });

      productGroup.get('allowance')?.valueChanges.pipe(
        debounceTime(10),
        distinctUntilChanged()
      ).subscribe((allowanceValue) => {
        this.handleAllowanceChange(productGroup, allowanceValue);
      });

      const pricingFieldNames = [
        'commercialDelPrice', 'commercialFobPrice', 'commodityDelPrice',
        'commodityFobPrice', 'pua', 'ffsPrice', 'noiPrice', 'ptv'
      ];
      pricingFieldNames.forEach(field => {
        productGroup.get(field)?.valueChanges.pipe(
          debounceTime(10),
          distinctUntilChanged()
        ).subscribe(() => {
          this.handlePricingFieldChange(productGroup);
        });
      });

      this.productsArray.push(productGroup);
      existingProductIds.add(product.productId);
      added++;

      // Initialize pricing disable state for imported values
      const allowanceVal = productGroup.get('allowance')?.value;
      if (allowanceVal != null && allowanceVal > 0) {
        pricingFieldNames.forEach(f => productGroup.get(f)?.disable({ emitEvent: false }));
      } else {
        const hasPricing = pricingFieldNames.some(f => {
          const v = productGroup.get(f)?.value;
          return v != null && v > 0;
        });
        if (hasPricing) {
          productGroup.get('allowance')?.disable({ emitEvent: false });
        }
      }
    });

    return { added, skippedDuplicates };
  }

  /**
   * Load contract data for renewal and prepopulate the form
   */
  private loadContractForRenewal(contractId: number): void {
    this.contractService.getById(contractId).subscribe({
      next: (contract) => {
        if (!contract) {
          this.error = 'Contract not found';
          return;
        }

        // Wait for lookup data to load first
        const checkLoaded = setInterval(() => {
          if (!this.loading && this.proposalTypes.length > 0) {
            clearInterval(checkLoaded);
            this.prepopulateFromContract(contract);
          }
        }, 100);
      },
      error: (error) => {
        console.error('Error loading contract:', error);
        this.error = error?.error?.message || 'Failed to load contract for renewal';
      }
    });
  }

  /**
   * Prepopulate form with contract data for renewal
   */
  private prepopulateFromContract(contract: any): void {
    // Helper function to extract numeric ID
    const extractId = (value: any): number | null => {
      if (value == null) return null;
      if (typeof value === 'number') return value;
      if (typeof value === 'string') {
        const num = parseInt(value, 10);
        return isNaN(num) ? null : num;
      }
      if (typeof value === 'object' && value.id != null) {
        return extractId(value.id);
      }
      return null;
    };

    // Extract manufacturer ID - try multiple approaches
    let manufacturerId: number | null = null;

    // Try direct manufacturerId property
    if (contract.manufacturerId) {
      manufacturerId = extractId(contract.manufacturerId);
    }

    // Try manufacturer object
    if (!manufacturerId && contract.manufacturer) {
      manufacturerId = extractId(contract.manufacturer.id || contract.manufacturer);
    }

    // Try manufacturers array
    if (!manufacturerId && contract.manufacturers && Array.isArray(contract.manufacturers) && contract.manufacturers.length > 0) {
      manufacturerId = extractId(contract.manufacturers[0].manufacturerId || contract.manufacturers[0].id);
    }

    // If still no manufacturer, get it from the first product
    if (!manufacturerId && contract.contractProducts && Array.isArray(contract.contractProducts) && contract.contractProducts.length > 0) {
      const firstProductId = extractId(contract.contractProducts[0].productId);
      if (firstProductId) {
        // We'll need to load the product to get its manufacturer
        // Will load products and get manufacturer from there
      }
    }

    // If no manufacturer but we have products, load the first product to get manufacturer
    if (!manufacturerId && contract.contractProducts && contract.contractProducts.length > 0) {
      const firstProductId = extractId(contract.contractProducts[0].productId);
      if (firstProductId) {
        this.productService.getById(firstProductId).subscribe({
          next: (product) => {
            if (product && product.manufacturerId) {
              const mfgId = product.manufacturerId;
              // Continue with prepopulation using the manufacturer from product
              this.finishPrepopulation(contract, mfgId);
            } else {
              this.error = 'Warning: Could not determine manufacturer from products.';
            }
          },
          error: () => {
            this.error = 'Warning: Could not load product to determine manufacturer.';
          }
        });
        return; // Exit and wait for product to load
      }
    }

    // Track missing fields for warning
    const missingFields: string[] = [];

    if (!manufacturerId) {
      missingFields.push('Manufacturer');
    }

    // Extract IDs from navigation properties - handle multiple property name variations
    let distributorIds: number[] = [];
    let industryIds: number[] = [];
    let opcoIds: number[] = [];

    // Try different property names for distributors
    const distributorsProp = contract.distributors || contract.Distributors || contract.contractDistributors || [];
    const industriesProp = contract.industries || contract.Industries || contract.contractIndustries || [];
    const opcosProp = contract.opCos || contract.OpCos || contract.contractOpCos || [];

    if (Array.isArray(distributorsProp)) {
      distributorIds = distributorsProp.map((d: any) => extractId(d.distributorId || d.id)).filter((id): id is number => id != null);
    }
    if (Array.isArray(industriesProp)) {
      industryIds = industriesProp.map((i: any) => extractId(i.industryId || i.id)).filter((id): id is number => id != null);
    }
    if (Array.isArray(opcosProp)) {
      opcoIds = opcosProp.map((o: any) => extractId(o.opCoId || o.id)).filter((id): id is number => id != null);
    }

    // Track missing data
    if (distributorIds.length === 0) {
      missingFields.push('Distributor');
    }
    if (industryIds.length === 0) {
      missingFields.push('Industry');
    }
    if (opcoIds.length === 0) {
      missingFields.push('Op-Co');
    }

    // Check for products
    const hasProducts = contract.products && Array.isArray(contract.products) && contract.products.length > 0;
    if (!hasProducts) {
      missingFields.push('Products');
    }

    // Show warning if fields are missing but continue
    if (missingFields.length > 0) {
      this.error = `Warning: The source contract is missing some fields: ${missingFields.join(', ')}. You can still create the proposal but will need to fill in these fields manually.`;
    }

    // Set proposal type to "New Contract"
    let proposalType = this.proposalTypes.find(t => t.name === 'New Contract');
    if (!proposalType) {
      proposalType = this.proposalTypes.find(t => t.name?.toLowerCase() === 'new contract');
    }
    if (!proposalType) {
      proposalType = this.proposalTypes.find(t => t.name?.toLowerCase().includes('new'));
    }
    if (!proposalType && this.proposalTypes.length > 0) {
      proposalType = this.proposalTypes[0];
    }
    if (proposalType) {
      this.form.patchValue({ proposalTypeId: proposalType.id });
    }

    // Set title with next term
    const nextTerm = this.calculateNextTerm(contract.startDate, contract.endDate);

    // Set default proposal status to Draft
    const draftStatus = this.proposalStatuses.find(s => s.name === 'Draft' || s.name?.toLowerCase() === 'draft');

    this.form.patchValue({
      title: `${contract.name} - New Contract Proposal`,
      manufacturerId: manufacturerId,
      proposalStatusId: draftStatus?.id || (this.proposalStatuses.length > 0 ? this.proposalStatuses[0].id : null),
      startDate: nextTerm.startDate,
      endDate: nextTerm.endDate,
      distributorIds: distributorIds,
      industryIds: industryIds,
      opcoIds: opcoIds
    });

    // Load products for the manufacturer and then load contract products
    if (manufacturerId) {
      this.productService.getByManufacturer(manufacturerId).subscribe({
        next: (products) => {
          this.products = products || [];

          // Load contract products and prices
          this.loadContractProductsForRenewal(contract.id);
        },
        error: (err) => {
          console.error('Failed to load products for manufacturer:', err);
          this.error = 'Failed to load products for manufacturer';
        }
      });
    }
  }

  /**
   * Finish prepopulation with manufacturer ID (fallback when manufacturer is derived from product)
   */
  private finishPrepopulation(contract: any, manufacturerId: number): void {

    const extractId = (value: any): number | null => {
      if (value == null) return null;
      if (typeof value === 'number') return value;
      if (typeof value === 'string') {
        const num = parseInt(value, 10);
        return isNaN(num) ? null : num;
      }
      if (typeof value === 'object' && value.id != null) {
        return extractId(value.id);
      }
      return null;
    };

    let distributorIds: number[] = [];
    let industryIds: number[] = [];
    let opcoIds: number[] = [];

    const distributorsProp = contract.distributors || [];
    const industriesProp = contract.industries || [];
    const opcosProp = contract.opCos || [];

    if (Array.isArray(distributorsProp)) {
      distributorIds = distributorsProp.map((d: any) => extractId(d.distributorId || d.id)).filter((id): id is number => id != null);
    }
    if (Array.isArray(industriesProp)) {
      industryIds = industriesProp.map((i: any) => extractId(i.industryId || i.id)).filter((id): id is number => id != null);
    }
    if (Array.isArray(opcosProp)) {
      opcoIds = opcosProp.map((o: any) => extractId(o.opCoId || o.id)).filter((id): id is number => id != null);
    }

    let proposalType = this.proposalTypes.find(t => t.name === 'New Contract');
    if (!proposalType) {
      proposalType = this.proposalTypes.find(t => t.name?.toLowerCase() === 'new contract');
    }
    if (!proposalType && this.proposalTypes.length > 0) {
      proposalType = this.proposalTypes[0];
    }
    if (proposalType) {
      this.form.patchValue({ proposalTypeId: proposalType.id });
    }

    const nextTerm = this.calculateNextTerm(contract.startDate, contract.endDate);
    const draftStatus = this.proposalStatuses.find(s => s.name === 'Draft' || s.name?.toLowerCase() === 'draft');

    this.form.patchValue({
      title: `${contract.name} - New Contract Proposal`,
      manufacturerId: manufacturerId,
      proposalStatusId: draftStatus?.id || (this.proposalStatuses.length > 0 ? this.proposalStatuses[0].id : null),
      startDate: nextTerm.startDate,
      endDate: nextTerm.endDate,
      distributorIds: distributorIds,
      industryIds: industryIds,
      opcoIds: opcoIds
    });

    this.productService.getByManufacturer(manufacturerId).subscribe({
      next: (products) => {
        this.products = products || [];
        this.loadContractProductsForRenewal(contract.id);
      },
      error: (err) => {
        console.error('Failed to load products:', err);
        this.error = 'Failed to load products for manufacturer';
      }
    });
  }

  /**
   * Calculate next term dates based on current contract dates
   */
  private calculateNextTerm(startDate: string | Date | undefined, endDate: string | Date | undefined): { startDate: string; endDate: string } {
    if (!startDate || !endDate) {
      // Default to next year
      const today = new Date();
      const nextYear = new Date(today.getFullYear() + 1, 0, 1);
      const nextYearEnd = new Date(today.getFullYear() + 1, 11, 31);
      return {
        startDate: nextYear.toISOString().split('T')[0],
        endDate: nextYearEnd.toISOString().split('T')[0]
      };
    }

    const end = new Date(endDate);
    const start = new Date(startDate);
    const duration = end.getTime() - start.getTime();

    // Next term starts the day after current end date
    const nextStart = new Date(end);
    nextStart.setDate(nextStart.getDate() + 1);

    // Next term ends after the same duration
    const nextEnd = new Date(nextStart.getTime() + duration);

    return {
      startDate: nextStart.toISOString().split('T')[0],
      endDate: nextEnd.toISOString().split('T')[0]
    };
  }

  /**
   * Load contract products and prices for renewal
   */
  private loadContractProductsForRenewal(contractId: number): void {
    this.contractService.getContractPricing(contractId).subscribe({
      next: (prices: ContractVersionPrice[]) => {

        // Filter out discontinued products
        const activeProducts = prices.filter((price: ContractVersionPrice) => {
          const product = this.products.find(p => p.id === price.productId);
          const isActive = product && !product.isDiscontinued;
          return isActive;
        });

        // Add products with their current pricing
        activeProducts.forEach((price: ContractVersionPrice) => {
          const productGroup = this.fb.group({
            productId: [price.productId, [Validators.required]],
            priceTypeId: [null],
            quantity: [price.estimatedQty || null, [Validators.min(1)]],
            productProposalStatusId: [null as number | null],
            amendmentActionId: [{ value: null, disabled: true }],
            uom: [price.uom || null],
            billbacksAllowed: [price.billbacksAllowed || false],
            allowance: [price.allowance || null, [Validators.min(0)]],
            commercialDelPrice: [price.commercialDelPrice || null, [Validators.min(0)]],
            commercialFobPrice: [price.commercialFobPrice || null, [Validators.min(0)]],
            commodityDelPrice: [price.commodityDelPrice || null, [Validators.min(0)]],
            commodityFobPrice: [price.commodityFobPrice || null, [Validators.min(0)]],
            pua: [price.pua || null, [Validators.min(0)]],
            ffsPrice: [price.ffsPrice || null, [Validators.min(0)]],
            noiPrice: [price.noiPrice || null, [Validators.min(0)]],
            ptv: [price.ptv || null, [Validators.min(0)]],
            internalNotes: [price.internalNotes || null],
            manufacturerNotes: [null]
          });

          this.productsArray.push(productGroup);
        });
      },
      error: (err) => {
        console.error('Failed to load contract products:', err);
        this.error = 'Failed to load contract products for renewal';
      }
    });
  }

  // TrackBy function to prevent unnecessary re-renders
  trackByIndex(index: number, item: any): number {
    return index;
  }
}
