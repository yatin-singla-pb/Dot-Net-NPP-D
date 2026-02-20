import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProposalService, ProposalCreateDto, ProposalType, ProposalStatus, PriceType, ProductProposalStatus, Proposal } from '../../services/proposal.service';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-proposal-clone',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './proposal-clone.component.html'
})
export class ProposalCloneComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  submitting = false;
  error: string | null = null;
  sourceProposalId!: number;
  sourceProposal: Proposal | null = null;
  Math = Math;

  // Lookup data
  proposalTypes: ProposalType[] = [];
  proposalStatuses: ProposalStatus[] = [];
  priceTypes: PriceType[] = [];
  productProposalStatuses: ProductProposalStatus[] = [];
  manufacturers: any[] = [];
  distributors: any[] = [];
  industries: any[] = [];
  opcos: any[] = [];
  products: any[] = [];
  uoms: string[] = ['Cases', 'Pounds'];

  // Read-only display copy of per-product statuses
  productStatusesView: (number | null)[] = [];

  // Panels & filters for searchable multi-selects
  showDistributorsPanel = false;
  showIndustriesPanel = false;
  showOpcoPanel = false;
  distributorFilter = '';
  industryFilter = '';
  opcoFilter = '';

  // Product list pagination and filtering
  productSearchTerm = '';
  productFilterPriceType: number | null = null;
  productFilterStatus: number | null = null;
  currentPage = 1;
  pageSize = 10;
  totalProducts = 0;
  private _cachedFilteredIndices: number[] | null = null;
  private _lastFilterState = { search: '', priceType: null as number | null, status: null as number | null, arrayLength: 0 };

  // Modal state for confirming product removal
  showRemoveProductModal = false;
  pendingRemoveProductIndex: number | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private productService: ProductService,
    private authService: AuthService
  ) {
    this.createForm();
  }

  ngOnInit(): void {
    this.sourceProposalId = +this.route.snapshot.params['id'];
    if (!this.sourceProposalId) {
      this.router.navigate(['/admin/proposals']);
      return;
    }

    this.loadData();
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
      products: this.fb.array([])
    });
  }

  get productsArray(): FormArray {
    return this.form.get('products') as FormArray;
  }

  private loadData(): void {
    this.loading = true;
    this.error = null;

    this.proposalService.getById(this.sourceProposalId).toPromise()
      .then((proposal) => {
        this.sourceProposal = proposal || null;
        const manufacturerId = proposal?.manufacturerId ? Number(proposal.manufacturerId) : null;

        return Promise.all([
          this.proposalService.getProposalTypes().toPromise(),
          this.proposalService.getProposalStatuses().toPromise(),
          this.proposalService.getPriceTypes().toPromise(),
          this.proposalService.getProductProposalStatuses().toPromise(),
          this.proposalService.getManufacturers().toPromise(),
          this.proposalService.getDistributors().toPromise(),
          this.proposalService.getIndustries().toPromise(),
          this.proposalService.getOpCos().toPromise(),
          this.productService.getByManufacturers(manufacturerId ? [manufacturerId] : []).toPromise(),
        ]);
      })
      .then(([
        proposalTypes,
        proposalStatuses,
        priceTypes,
        productProposalStatuses,
        manufacturers,
        distributors,
        industries,
        opcos,
        products
      ]) => {
        this.proposalTypes = proposalTypes || [];
        this.proposalStatuses = proposalStatuses || [];
        this.priceTypes = priceTypes || [];
        this.productProposalStatuses = productProposalStatuses || [];
        this.manufacturers = manufacturers || [];
        this.distributors = distributors || [];
        this.industries = industries || [];
        this.opcos = opcos || [];
        this.products = products || [];

        this.populateFormFromSource();
        this.loading = false;
      })
      .catch(() => {
        this.error = 'Failed to load proposal data.';
        this.loading = false;
      });
  }

  private populateFormFromSource(): void {
    if (!this.sourceProposal) return;

    // Clear existing products
    while (this.productsArray.length !== 0) {
      this.productsArray.removeAt(0);
    }
    this.productStatusesView = [];

    // Set default status to "Requested" for the clone
    const requestedStatus = this.proposalStatuses.find(s => s.name === 'Requested');
    const defaultStatusId = requestedStatus ? requestedStatus.id : this.sourceProposal.proposalStatusId;

    // Populate basic fields with modified title
    this.form.patchValue({
      title: `Copy of ${this.sourceProposal.title}`,
      proposalTypeId: this.sourceProposal.proposalTypeId,
      proposalStatusId: defaultStatusId,
      manufacturerId: this.sourceProposal.manufacturerId,
      startDate: this.sourceProposal.startDate ? this.sourceProposal.startDate.substring(0, 10) : '',
      endDate: this.sourceProposal.endDate ? this.sourceProposal.endDate.substring(0, 10) : '',
      dueDate: (this.sourceProposal as any).dueDate ? (this.sourceProposal as any).dueDate.substring(0, 10) : '',
      internalNotes: this.sourceProposal.internalNotes,
      distributorIds: [...this.sourceProposal.distributorIds],
      industryIds: [...this.sourceProposal.industryIds],
      opcoIds: [...this.sourceProposal.opcoIds]
    });

    // Populate products with reset status
    const pendingStatus = this.productProposalStatuses.find(s => s.name === 'Pending');
    const defaultProductStatusId = pendingStatus ? pendingStatus.id : null;

    this.sourceProposal.products.forEach(product => {
      const productGroup = this.fb.group({
        productId: [product.productId, [Validators.required]],
        priceTypeId: [product.priceTypeId],
        productProposalStatusId: [defaultProductStatusId as number | null],
        quantity: [product.quantity, [Validators.min(1)]],
        uom: [product.uom || null],
        billbacksAllowed: [!!product.billbacksAllowed],
        allowance: [product.allowance ?? null, [Validators.min(0)]],
        commercialDelPrice: [product.commercialDelPrice ?? null, [Validators.min(0)]],
        commercialFobPrice: [product.commercialFobPrice ?? null, [Validators.min(0)]],
        commodityDelPrice: [product.commodityDelPrice ?? null, [Validators.min(0)]],
        commodityFobPrice: [product.commodityFobPrice ?? null, [Validators.min(0)]],
        pua: [product.pua ?? null, [Validators.min(0)]],
        ffsPrice: [product.ffsPrice ?? null, [Validators.min(0)]],
        noiPrice: [product.noiPrice ?? null],
        ptv: [product.ptv ?? null, [Validators.min(0)]],
        internalNotes: [product.internalNotes || '', [Validators.maxLength(1000)]],
        manufacturerNotes: [product.manufacturerNotes || '', [Validators.maxLength(1000)]]
      });

      // Watch price type changes
      productGroup.get('priceTypeId')?.valueChanges.subscribe((priceTypeId) => {
        this.handlePriceTypeChange(productGroup, priceTypeId);
      });

      // Watch allowance changes
      productGroup.get('allowance')?.valueChanges.pipe(
        debounceTime(10),
        distinctUntilChanged()
      ).subscribe((allowanceValue) => {
        this.handleAllowanceChange(productGroup, allowanceValue);
      });

      // Watch pricing field changes
      const pricingFields = ['commercialDelPrice', 'commercialFobPrice', 'commodityDelPrice', 'commodityFobPrice', 'pua', 'ffsPrice', 'noiPrice', 'ptv'];
      pricingFields.forEach(field => {
        productGroup.get(field)?.valueChanges.pipe(
          debounceTime(10),
          distinctUntilChanged()
        ).subscribe(() => {
          this.handlePricingFieldChange(productGroup);
        });
      });

      this.productsArray.push(productGroup);
      this.productStatusesView.push(defaultProductStatusId);
    });
  }

  // ===== Multi-select helpers =====
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
  applyPanel(control: 'industryIds'|'distributorIds'|'opcoIds') { this.closePanel(control); }

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

  getManufacturerName(id: number | null): string {
    const m = (this.manufacturers || []).find((x: any) => x.id === Number(id));
    return m?.name || '';
  }

  // ===== Product display helpers =====
  getProductById(id: number): any {
    return (this.products || []).find(p => Number(p.id) === Number(id));
  }

  getProductDisplayById(id?: number | null): string {
    if (id === undefined || id === null) return '';
    const p = this.getProductById(Number(id));
    if (!p) return '';
    const code = (p.manufacturerProductCode ?? '').toString().trim();
    const desc = (p.description ?? p.name ?? '').toString().trim();
    return [code, desc].filter(Boolean).join(' - ') || (p.name ?? '');
  }

  getProductHeaderTitle(id: number): string {
    const p = this.getProductById(id);
    if (!p) return '';
    const parts = [p.manufacturerProductCode, p.brand, p.packSize, p.name || p.description].filter(Boolean);
    return parts.join(' - ');
  }

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

  getProductStatusBadgeClass(status: string): any {
    const s = (status || '').toLowerCase();
    return {
      'bg-success': s === 'active',
      'bg-danger': s === 'discontinued',
      'bg-warning text-dark': s === 'inactive' || s === 'pending'
    };
  }

  // ===== Price type change handlers =====
  handlePriceTypeChange(productGroup: FormGroup, priceTypeId: number | null | undefined): void {
    const pricingFields = ['allowance', 'commercialDelPrice', 'commercialFobPrice', 'commodityDelPrice', 'commodityFobPrice', 'pua', 'ffsPrice', 'noiPrice', 'ptv'];
    const otherFields = ['quantity', 'uom', 'billbacksAllowed'];

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
      otherFields.forEach(field => { productGroup.get(field)?.enable({ emitEvent: false }); });
      return;
    }

    const priceType = this.priceTypes.find(pt => pt.id === priceTypeId);
    const priceTypeName = priceType?.name || '';
    const noPricingRequired = ['Product Discontinued', 'Product Suspension', 'Published List Price at time of Purchase'].includes(priceTypeName);

    if (noPricingRequired) {
      [...pricingFields, ...otherFields].forEach(field => {
        const control = productGroup.get(field);
        if (control) {
          control.clearValidators();
          control.updateValueAndValidity({ emitEvent: false });
          if (field === 'billbacksAllowed') { control.setValue(false, { emitEvent: false }); }
          else { control.setValue(null, { emitEvent: false }); }
          control.disable({ emitEvent: false });
          control.markAsUntouched();
        }
      });
    } else {
      pricingFields.forEach(field => {
        const control = productGroup.get(field);
        if (control) {
          control.enable({ emitEvent: false });
          control.clearValidators();
          control.setValidators([Validators.min(0)]);
          control.updateValueAndValidity({ emitEvent: false });
        }
      });
      otherFields.forEach(field => { productGroup.get(field)?.enable({ emitEvent: false }); });
    }
  }

  handleAllowanceChange(productGroup: FormGroup, allowanceValue: number | null | undefined): void {
    const hasAllowance = allowanceValue != null && allowanceValue > 0;
    const pricingFields = ['commercialDelPrice', 'commercialFobPrice', 'commodityDelPrice', 'commodityFobPrice', 'pua', 'ffsPrice', 'noiPrice', 'ptv'];

    if (hasAllowance) {
      pricingFields.forEach(field => {
        const control = productGroup.get(field);
        if (control && control.enabled) {
          control.setValue(null, { emitEvent: false });
          control.disable({ emitEvent: false });
        }
      });
    } else {
      const hasPricingValue = pricingFields.some(field => {
        const val = productGroup.get(field)?.value;
        return val != null && val > 0;
      });
      if (!hasPricingValue) {
        pricingFields.forEach(field => {
          const control = productGroup.get(field);
          if (control && control.disabled) { control.enable({ emitEvent: false }); }
        });
      }
    }
  }

  handlePricingFieldChange(productGroup: FormGroup): void {
    const pricingFields = ['commercialDelPrice', 'commercialFobPrice', 'commodityDelPrice', 'commodityFobPrice', 'pua', 'ffsPrice', 'noiPrice', 'ptv'];
    const hasPricingValue = pricingFields.some(field => {
      const val = productGroup.get(field)?.value;
      return val != null && val > 0;
    });

    const allowanceControl = productGroup.get('allowance');
    if (hasPricingValue) {
      if (allowanceControl && allowanceControl.enabled) {
        allowanceControl.setValue(null, { emitEvent: false });
        allowanceControl.disable({ emitEvent: false });
      }
    } else {
      if (allowanceControl && allowanceControl.disabled) {
        allowanceControl.enable({ emitEvent: false });
      }
    }
  }

  isPricingDisabledForProduct(productGroup: FormGroup): boolean {
    const priceTypeId = productGroup.get('priceTypeId')?.value;
    if (!priceTypeId) return false;
    const priceType = this.priceTypes.find(pt => pt.id === priceTypeId);
    const priceTypeName = priceType?.name || '';
    return ['Product Discontinued', 'Product Suspension', 'Published List Price at time of Purchase'].includes(priceTypeName);
  }

  onPriceTypeChange(productGroup: FormGroup): void {
    const priceTypeId = productGroup.get('priceTypeId')?.value;
    this.handlePriceTypeChange(productGroup, priceTypeId);
  }

  // ===== Product pagination =====
  get paginatedProducts() {
    const filtered = this.filteredProductIndices;
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return filtered.slice(start, end).map(index => ({
      control: this.productsArray.at(index) as FormGroup,
      index: index
    }));
  }

  get filteredProductIndices(): number[] {
    const currentState = {
      search: this.productSearchTerm,
      priceType: this.productFilterPriceType,
      status: this.productFilterStatus,
      arrayLength: this.productsArray.length
    };

    if (this._cachedFilteredIndices !== null &&
        this._lastFilterState.search === currentState.search &&
        this._lastFilterState.priceType === currentState.priceType &&
        this._lastFilterState.status === currentState.status &&
        this._lastFilterState.arrayLength === currentState.arrayLength) {
      return this._cachedFilteredIndices;
    }

    let indices: number[] = [];
    for (let i = 0; i < this.productsArray.length; i++) {
      const control = this.productsArray.at(i) as FormGroup;
      let matches = true;

      if (this.productSearchTerm) {
        const term = this.productSearchTerm.toLowerCase();
        const productId = control.get('productId')?.value;
        const productName = this.getProductDisplayById(productId) || `Product ${i + 1}`;
        const internalNotes = control.get('internalNotes')?.value || '';
        matches = matches && (productName.toLowerCase().includes(term) || internalNotes.toLowerCase().includes(term));
      }

      if (this.productFilterPriceType !== null) {
        matches = matches && (control.get('priceTypeId')?.value === this.productFilterPriceType);
      }

      if (this.productFilterStatus !== null) {
        matches = matches && (control.get('productProposalStatusId')?.value === this.productFilterStatus);
      }

      if (matches) indices.push(i);
    }

    this.totalProducts = indices.length;
    this._cachedFilteredIndices = indices;
    this._lastFilterState = currentState;
    return indices;
  }

  get totalPages(): number {
    return Math.ceil(this.totalProducts / this.pageSize);
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages) this.currentPage = page;
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.currentPage = 1;
  }

  onProductSearch(term: string): void {
    this.productSearchTerm = term;
    this.currentPage = 1;
    this._cachedFilteredIndices = null;
  }

  onPriceTypeFilter(priceTypeId: number | null): void {
    this.productFilterPriceType = priceTypeId;
    this.currentPage = 1;
    this._cachedFilteredIndices = null;
  }

  onStatusFilter(statusId: number | null): void {
    this.productFilterStatus = statusId;
    this.currentPage = 1;
    this._cachedFilteredIndices = null;
  }

  clearFilters(): void {
    this.productSearchTerm = '';
    this.productFilterPriceType = null;
    this.productFilterStatus = null;
    this.currentPage = 1;
    this._cachedFilteredIndices = null;
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const totalPages = this.totalPages;
    const currentPage = this.currentPage;
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      if (currentPage <= 4) {
        for (let i = 1; i <= 5; i++) pages.push(i);
        pages.push(-1);
        pages.push(totalPages);
      } else if (currentPage >= totalPages - 3) {
        pages.push(1);
        pages.push(-1);
        for (let i = totalPages - 4; i <= totalPages; i++) pages.push(i);
      } else {
        pages.push(1);
        pages.push(-1);
        for (let i = currentPage - 1; i <= currentPage + 1; i++) pages.push(i);
        pages.push(-1);
        pages.push(totalPages);
      }
    }
    return pages;
  }

  // ===== Product add/remove =====
  addProduct(): void {
    const productGroup = this.fb.group({
      productId: [null, [Validators.required]],
      priceTypeId: [null],
      productProposalStatusId: [null as number | null],
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

    productGroup.get('priceTypeId')?.valueChanges.subscribe((priceTypeId) => {
      this.handlePriceTypeChange(productGroup, priceTypeId);
    });

    productGroup.get('allowance')?.valueChanges.pipe(
      debounceTime(10), distinctUntilChanged()
    ).subscribe((allowanceValue) => {
      this.handleAllowanceChange(productGroup, allowanceValue);
    });

    const pricingFields = ['commercialDelPrice', 'commercialFobPrice', 'commodityDelPrice', 'commodityFobPrice', 'pua', 'ffsPrice', 'noiPrice', 'ptv'];
    pricingFields.forEach(field => {
      productGroup.get(field)?.valueChanges.pipe(
        debounceTime(10), distinctUntilChanged()
      ).subscribe(() => {
        this.handlePricingFieldChange(productGroup);
      });
    });

    const pendingStatus = this.productProposalStatuses.find(s => s.name === 'Pending');
    if (pendingStatus) {
      productGroup.patchValue({ productProposalStatusId: pendingStatus.id });
    }

    this.productsArray.push(productGroup);
    this.productStatusesView.push(null);
    this._cachedFilteredIndices = null;
  }

  removeProduct(index: number): void {
    this.productsArray.removeAt(index);
    if (index >= 0 && index < this.productStatusesView.length) {
      this.productStatusesView.splice(index, 1);
    }
    this._cachedFilteredIndices = null;
  }

  openRemoveProductModal(index: number): void {
    this.pendingRemoveProductIndex = index;
    this.showRemoveProductModal = true;
  }

  closeRemoveProductModal(): void {
    this.showRemoveProductModal = false;
    this.pendingRemoveProductIndex = null;
  }

  confirmRemoveProduct(): void {
    if (this.pendingRemoveProductIndex != null) {
      this.removeProduct(this.pendingRemoveProductIndex);
    }
    this.closeRemoveProductModal();
  }

  // ===== Form helpers =====
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

    const formValue = this.form.getRawValue() as any;

    const distributorIds: number[] = Array.from(new Set(((formValue.distributorIds || []) as any[])
      .map((x: any) => Number(x))
      .filter((x: any) => !!x)));
    const industryIds: number[] = Array.from(new Set(((formValue.industryIds || []) as any[])
      .map((x: any) => Number(x))
      .filter((x: any) => !!x)));
    const opcoIds: number[] = Array.from(new Set(((formValue.opcoIds || formValue.opCoIds || []) as any[])
      .map((x: any) => Number(x))
      .filter((x: any) => !!x)));

    const products = ((formValue.products || []) as any[]).map(p => ({
      productId: Number(p.productId),
      priceTypeId: p.priceTypeId != null && p.priceTypeId !== '' ? Number(p.priceTypeId) : null,
      quantity: p.quantity != null && p.quantity !== '' ? Number(p.quantity) : null,
      productProposalStatusId: p.productProposalStatusId != null && p.productProposalStatusId !== '' ? Number(p.productProposalStatusId) : null,
      uom: (p.uom ?? null),
      billbacksAllowed: !!p.billbacksAllowed,
      allowance: p.allowance != null && p.allowance !== '' ? Number(p.allowance) : null,
      commercialDelPrice: p.commercialDelPrice != null && p.commercialDelPrice !== '' ? Number(p.commercialDelPrice) : null,
      commercialFobPrice: p.commercialFobPrice != null && p.commercialFobPrice !== '' ? Number(p.commercialFobPrice) : null,
      commodityDelPrice: p.commodityDelPrice != null && p.commodityDelPrice !== '' ? Number(p.commodityDelPrice) : null,
      commodityFobPrice: p.commodityFobPrice != null && p.commodityFobPrice !== '' ? Number(p.commodityFobPrice) : null,
      pua: p.pua != null && p.pua !== '' ? Number(p.pua) : null,
      ffsPrice: p.ffsPrice != null && p.ffsPrice !== '' ? Number(p.ffsPrice) : null,
      noiPrice: p.noiPrice != null ? !!p.noiPrice : null,
      ptv: p.ptv != null && p.ptv !== '' ? Number(p.ptv) : null,
      internalNotes: (p.internalNotes ?? null),
      manufacturerNotes: (p.manufacturerNotes ?? null)
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

    this.proposalService.create(payload).subscribe({
      next: (result) => {
        this.router.navigate(['/admin/proposals', result.id]);
      },
      error: (error) => {
        this.error = error?.error?.message || 'Failed to clone proposal';
        this.submitting = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/proposals', this.sourceProposalId]);
  }

  getSourceProposalTitle(): string {
    return this.sourceProposal ? this.sourceProposal.title : 'Loading...';
  }

  trackByIndex(index: number, item: any): number {
    return index;
  }
}
