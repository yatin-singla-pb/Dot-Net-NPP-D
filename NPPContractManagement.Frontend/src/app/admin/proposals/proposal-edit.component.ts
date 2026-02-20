import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProposalService, ProposalCreateDto, ProposalType, ProposalStatus, PriceType, ProductProposalStatus, Proposal } from '../../services/proposal.service';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';
import { ProductSearchModalComponent } from '../../shared/components/product-search-modal/product-search-modal.component';
import { ExcelExportService } from '../../shared/services/excel-export.service';

@Component({
  selector: 'app-proposal-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, ProductSearchModalComponent],
  templateUrl: './proposal-edit.component.html'
})
export class ProposalEditComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  submitting = false;
  isSubmittingProposal = false;
  error: string | null = null;
  proposalId!: number;
  proposal: Proposal | null = null;
  readOnly = false;
  isManufacturerUser = false;
  Math = Math;

  // Panels & filters for searchable multi-selects (like create/contracts)
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

  // Product list pagination and filtering
  productSearchTerm = '';
  productFilterPriceType: number | null = null;
  productFilterStatus: number | null = null;
  currentPage = 1;
  pageSize = 10;
  totalProducts = 0;
  private _cachedFilteredIndices: number[] | null = null;
  private _lastFilterState = { search: '', priceType: null as number | null, status: null as number | null, arrayLength: 0 };

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
  // UI constants
  // Read-only display copy of per-product statuses (workflow-controlled)
  productStatusesView: (number | null)[] = [];

  uoms: string[] = ['Cases', 'Pounds'];


  // Modal state for confirming product removal
  showRemoveProductModal = false;
  pendingRemoveProductIndex: number | null = null;

  // Modal state for adding a product
  showAddProductModal = false;
  addProductForm: FormGroup | null = null;
  addProductSearchTerm = '';
  addProductError: string | null = null;
  selectedAddProduct: Product | null = null;
  showProductSearchForAdd = false;

  @ViewChild('excelFileInput') excelFileInput!: ElementRef;
  uploadingExcel = false;
  excelUploadError: string | null = null;
  excelFeedback: { type: 'success' | 'warning' | 'danger'; message: string } | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private productService: ProductService,
    private authService: AuthService,
    private http: HttpClient,
    private apiService: ApiService,
    private excelExportService: ExcelExportService
  ) {
    this.createForm();
  }

  ngOnInit(): void {
    this.isManufacturerUser = this.authService.hasRole('Manufacturer');

    this.proposalId = +this.route.snapshot.params['id'];
    if (!this.proposalId) {
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

    // Fetch proposal first; then lookups via anonymous endpoints
    this.proposalService.getById(this.proposalId).toPromise()
      .then((proposal) => {
        this.proposal = proposal || null;
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

        // Initialize filtered manufacturers
        this.applyManufacturerFilter('');

        this.populateForm();
        this.loading = false;
      })
      .catch(() => {
        this.error = 'Failed to load data';
        this.loading = false;
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
    if (this.isManufacturerUser) {
      return;
    }

    const arr: number[] = [...(this.form.get(control)?.value || [])];
    const idx = arr.indexOf(id);
    if (idx >= 0) arr.splice(idx, 1); else arr.push(id);
    this.form.patchValue({ [control]: arr });
  }
  removeFromMulti(control: 'industryIds'|'distributorIds'|'opcoIds', id: number) {
    if (this.isManufacturerUser) {
      return;
    }

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
    // Manufacturer field is always read-only on edit screen
    return;
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

  private getProposalStatusName(id: number): string {
    const status = this.proposalStatuses.find(s => Number(s.id) === Number(id));
    return status ? status.name : `Status ${id}`;
  }

  private updateReadOnlyState(): void {
    if (!this.form) return;

    // Always allow editing (readOnly is always false now)
    this.readOnly = false;

    // Start from everything enabled
    this.form.enable({ emitEvent: false });

    // Always disable manufacturer field (read-only for everyone)
    const manufacturerControl = this.form.get('manufacturerId');
    if (manufacturerControl) {
      manufacturerControl.disable({ emitEvent: false });
    }

    // Manufacturer users: basic information and associations are read-only.
    if (this.isManufacturerUser) {
      const lockedControls = [
        'title',
        'proposalTypeId',
        'proposalStatusId',
        'manufacturerId',
        'startDate',
        'endDate',
        'internalNotes',
        'distributorIds',
        'industryIds',
        'opcoIds'
      ];

      lockedControls.forEach(name => {
        const control = this.form.get(name);
        if (control) {
          control.disable({ emitEvent: false });
        }
      });
    }
  }

  private populateForm(): void {
    if (!this.proposal) return;

    // Clear existing products
    while (this.productsArray.length !== 0) {
      this.productsArray.removeAt(0);
    }
    this.productStatusesView = [];

    // Populate basic fields
    this.form.patchValue({
      title: this.proposal.title,
      proposalTypeId: this.proposal.proposalTypeId,
      proposalStatusId: this.proposal.proposalStatusId,
      manufacturerId: this.proposal.manufacturerId,
      startDate: this.proposal.startDate ? this.proposal.startDate.substring(0, 10) : '',
      endDate: this.proposal.endDate ? this.proposal.endDate.substring(0, 10) : '',
      dueDate: this.proposal.dueDate ? this.proposal.dueDate.substring(0, 10) : '',
      internalNotes: this.proposal.internalNotes,
      distributorIds: this.proposal.distributorIds,
      industryIds: this.proposal.industryIds,
      opcoIds: this.proposal.opcoIds
    });

    // Populate products
    this.proposal.products.forEach(product => {
      const productGroup = this.fb.group({
        productId: [product.productId, [Validators.required]],
        priceTypeId: [product.priceTypeId],
        quantity: [product.quantity, [Validators.min(1)]],
        // Pricing fields
        uom: [product.uom || null],
        billbacksAllowed: [!!product.billbacksAllowed],
        allowance: [product.allowance ?? null, [Validators.min(0)]],
        commercialDelPrice: [product.commercialDelPrice ?? null, [Validators.min(0)]],
        commercialFobPrice: [product.commercialFobPrice ?? null, [Validators.min(0)]],
        commodityDelPrice: [product.commodityDelPrice ?? null, [Validators.min(0)]],
        commodityFobPrice: [product.commodityFobPrice ?? null, [Validators.min(0)]],
        // Additional fields
        pua: [product.pua ?? null, [Validators.min(0)]],
        ffsPrice: [product.ffsPrice ?? null, [Validators.min(0)]],
        noiPrice: [product.noiPrice ?? null],
        ptv: [product.ptv ?? null, [Validators.min(0)]],
        internalNotes: [product.internalNotes || '', [Validators.maxLength(1000)]],
        manufacturerNotes: [product.manufacturerNotes || '', [Validators.maxLength(1000)]]
      });

      // Watch price type changes to enable/disable pricing fields
      productGroup.get('priceTypeId')?.valueChanges.subscribe((priceTypeId) => {
        this.handlePriceTypeChange(productGroup, priceTypeId);
      });

      // Watch allowance changes for this product
      productGroup.get('allowance')?.valueChanges.pipe(
        debounceTime(10),
        distinctUntilChanged()
      ).subscribe((allowanceValue) => {
        this.handleAllowanceChange(productGroup, allowanceValue);
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
        productGroup.get(field)?.valueChanges.pipe(
          debounceTime(10),
          distinctUntilChanged()
        ).subscribe(() => {
          this.handlePricingFieldChange(productGroup);
        });
      });

      this.productsArray.push(productGroup);
      this.productStatusesView.push(product.productProposalStatusId ?? null);
    });

    this.updateReadOnlyState();
    this.initializePricingStates();
  }

  /** Re-apply enable/disable state for all product pricing fields.
   *  Must run AFTER updateReadOnlyState() because form.enable() resets everything.
   *  Unlike the change handlers, this does NOT clear existing values. */
  private initializePricingStates(): void {
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

    const otherFields = ['quantity', 'uom', 'billbacksAllowed'];

    for (let i = 0; i < this.productsArray.length; i++) {
      const productGroup = this.productsArray.at(i) as FormGroup;
      const currentPriceTypeId = productGroup.get('priceTypeId')?.value;
      const currentAllowance = productGroup.get('allowance')?.value;
      const hasPricingValue = pricingFields.some(field => {
        const val = productGroup.get(field)?.value;
        return val != null && val > 0;
      });

      // Check if this is a special price type
      let isSpecialPriceType = false;
      if (currentPriceTypeId) {
        const priceType = this.priceTypes.find(pt => pt.id === currentPriceTypeId);
        const priceTypeName = priceType?.name || '';
        isSpecialPriceType = [
          'Product Discontinued',
          'Product Suspension',
          'Published List Price at time of Purchase'
        ].includes(priceTypeName);
      }

      if (isSpecialPriceType) {
        // Disable all pricing + other fields (values already loaded from API)
        [...pricingFields, 'allowance', ...otherFields].forEach(field => {
          const control = productGroup.get(field);
          if (control) {
            control.disable({ emitEvent: false });
          }
        });
      } else if (currentAllowance != null && currentAllowance > 0) {
        // Allowance has value → disable other pricing fields
        pricingFields.forEach(field => {
          const control = productGroup.get(field);
          if (control) {
            control.disable({ emitEvent: false });
          }
        });
      } else if (hasPricingValue) {
        // A pricing field has value → disable allowance
        const allowanceControl = productGroup.get('allowance');
        if (allowanceControl) {
          allowanceControl.disable({ emitEvent: false });
        }
      }
    }
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

    // Get values from the modal form (including disabled fields)
    const values = this.addProductForm.getRawValue();

    // Check for duplicate product
    const existingProductIds: number[] = [];
    for (let i = 0; i < this.productsArray.length; i++) {
      const pid = this.productsArray.at(i).get('productId')?.value;
      if (pid != null) existingProductIds.push(Number(pid));
    }
    if (existingProductIds.includes(Number(values.productId))) {
      const productName = this.getProductDisplayById(values.productId) || `Product #${values.productId}`;
      this.addProductError = `"${productName}" has already been added to this proposal.`;
      return;
    }

    // Create the real product FormGroup with the entered values
    const productGroup = this.fb.group({
      productId: [values.productId, [Validators.required]],
      priceTypeId: [values.priceTypeId],
      quantity: [values.quantity, [Validators.min(1)]],
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

    this.productsArray.push(productGroup);
    this.productStatusesView.push(null);

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

    // Clear filters so the new product is visible
    this.productSearchTerm = '';
    this.productFilterPriceType = null;
    this.productFilterStatus = null;
    this._cachedFilteredIndices = null;

    // Navigate to the last page where the new product appears
    const totalFiltered = this.productsArray.length;
    this.currentPage = Math.ceil(totalFiltered / this.pageSize);

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
    if (index >= 0 && index < this.productStatusesView.length) {
      this.productStatusesView.splice(index, 1);
    }
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
      // Enable all pricing fields only if no other pricing field has a value
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

    // Check if any pricing field has a value
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

  // Open confirmation modal to remove product
  openRemoveProductModal(index: number): void {
    this.pendingRemoveProductIndex = index;
    this.showRemoveProductModal = true;
  }

  // Close modal without removing
  closeRemoveProductModal(): void {
    this.showRemoveProductModal = false;
    this.pendingRemoveProductIndex = null;
  }

  // Confirm removal, then close
  confirmRemoveProduct(): void {
    if (this.pendingRemoveProductIndex != null) {
      this.removeProduct(this.pendingRemoveProductIndex);
    }
    this.closeRemoveProductModal();
  }

  isFieldInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  isProductFieldInvalid(index: number, field: string): boolean {
    const control = this.productsArray.at(index).get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
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

  // Product display helpers for header
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


  canSubmitProposal(): boolean {
    if (!this.proposal) return false;
    const status = this.getProposalStatusName(this.proposal.proposalStatusId);
    return ['Requested', 'Pending', 'Saved'].includes(status);
  }

  onSave(): void {
    if (this.form.invalid) {
      this.markFormAsTouched();
      return;
    }

    this.submitting = true;
    this.isSubmittingProposal = false;
    this.error = null;

    const payload = this.buildPayload();

    this.proposalService.update(this.proposalId, payload).subscribe({
      next: (result) => {
        this.router.navigate(['/admin/proposals', result.id]);
      },
      error: (error) => {
        this.error = error?.error?.message || 'Failed to save proposal';
        this.submitting = false;
      }
    });
  }

  onSubmitProposal(): void {
    if (this.form.invalid) {
      this.markFormAsTouched();
      return;
    }

    // Validate products before submission
    const validationError = this.validateProductsForSubmission();
    if (validationError) {
      this.error = validationError;
      return;
    }

    this.submitting = true;
    this.isSubmittingProposal = true;
    this.error = null;

    const payload = this.buildPayload();

    // First update the proposal
    this.proposalService.update(this.proposalId, payload).subscribe({
      next: (result) => {
        // Then submit it (changes status to Submitted)
        this.proposalService.submit(this.proposalId).subscribe({
          next: () => {
            this.router.navigate(['/admin/proposals', result.id]);
          },
          error: (error) => {
            this.error = error?.error?.message || 'Failed to submit proposal';
            this.submitting = false;
            this.isSubmittingProposal = false;
          }
        });
      },
      error: (error) => {
        this.error = error?.error?.message || 'Failed to save proposal before submit';
        this.submitting = false;
        this.isSubmittingProposal = false;
      }
    });
  }

  private validateProductsForSubmission(): string | null {
    const productsArray = this.form.get('products') as FormArray;

    if (!productsArray || productsArray.length === 0) {
      return 'At least one product is required to submit the proposal.';
    }

    for (let i = 0; i < productsArray.length; i++) {
      const productGroup = productsArray.at(i) as FormGroup;
      const productId = productGroup.get('productId')?.value;
      const priceTypeId = productGroup.get('priceTypeId')?.value;

      const product = this.products.find(p => p.id === productId);
      const productName = product?.name || `Product ${i + 1}`;

      if (!priceTypeId) {
        return `Price Type is required for ${productName}.`;
      }

      const priceType = this.priceTypes.find(pt => pt.id === priceTypeId);
      const priceTypeName = priceType?.name || '';

      const noPricingRequired = [
        'Product Discontinued',
        'Product Suspension',
        'Published List Price at time of Purchase'
      ].includes(priceTypeName);

      if (noPricingRequired) {
        continue;
      }

      const pricingFieldsData = [
        { name: 'Allowance', control: productGroup.get('allowance') },
        { name: 'Commercial DEL', control: productGroup.get('commercialDelPrice') },
        { name: 'Commercial FOB', control: productGroup.get('commercialFobPrice') },
        { name: 'Commodity DEL', control: productGroup.get('commodityDelPrice') },
        { name: 'Commodity FOB', control: productGroup.get('commodityFobPrice') },
        { name: 'PUA', control: productGroup.get('pua') },
        { name: 'FFS', control: productGroup.get('ffsPrice') },
        { name: 'NOI', control: productGroup.get('noiPrice') },
        { name: 'PTV', control: productGroup.get('ptv') }
      ];

      if (priceTypeName === 'Guaranteed') {
        for (const field of pricingFieldsData) {
          if (field.control?.enabled) {
            const value = field.control.value;
            if (value === null || value === undefined || value === '') {
              return `${field.name} is required for Guaranteed price type on ${productName}.`;
            }
          }
        }
      } else {
        const hasAnyPricing = pricingFieldsData.some(field => {
          if (field.control?.enabled) {
            const value = field.control.value;
            return value !== null && value !== undefined && value !== '';
          }
          return false;
        });

        if (!hasAnyPricing) {
          return `At least one pricing field is required for ${productName}.`;
        }
      }
    }

    return null;
  }

  private buildPayload(): ProposalCreateDto {
    const formValue = this.form.getRawValue();
    return {
      title: formValue.title,
      proposalTypeId: formValue.proposalTypeId,
      proposalStatusId: formValue.proposalStatusId,
      manufacturerId: formValue.manufacturerId || null,
      startDate: formValue.startDate || null,
      endDate: formValue.endDate || null,
      dueDate: formValue.dueDate || null,
      internalNotes: formValue.internalNotes || null,
      products: formValue.products || [],
      distributorIds: formValue.distributorIds || [],
      industryIds: formValue.industryIds || [],
      opcoIds: formValue.opcoIds || []
    };
  }

  private markFormAsTouched(): void {
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
  }

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
    // Check if we need to recalculate
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

    // Recalculate
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

      if (matches) {
        indices.push(i);
      }
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
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
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
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      if (currentPage <= 4) {
        for (let i = 1; i <= 5; i++) {
          pages.push(i);
        }
        pages.push(-1);
        pages.push(totalPages);
      } else if (currentPage >= totalPages - 3) {
        pages.push(1);
        pages.push(-1);
        for (let i = totalPages - 4; i <= totalPages; i++) {
          pages.push(i);
        }
      } else {
        pages.push(1);
        pages.push(-1);
        for (let i = currentPage - 1; i <= currentPage + 1; i++) {
          pages.push(i);
        }
        pages.push(-1);
        pages.push(totalPages);
      }
    }

    return pages;
  }

  onSubmit(): void {
    // Default form submit - just save
    this.onSave();
  }

  onCancel(): void {
    this.router.navigate(['/admin/proposals', this.proposalId]);
  }

  // TrackBy function to prevent unnecessary re-renders
  trackByIndex(index: number, item: any): number {
    return index;
  }

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

    this.excelExportService.exportToExcel(rows, `Proposal_${this.proposalId}_Products`, 'Proposal Products');
  }

  downloadExcelTemplate(): void {
    const manufacturerId = this.proposal?.manufacturerId;
    if (!manufacturerId) return;
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
    if (!this.proposal?.manufacturerId) {
      this.excelFeedback = { type: 'danger', message: 'No manufacturer associated with this proposal.' };
      return;
    }
    this.excelFileInput.nativeElement.click();
  }

  onExcelFileSelected(event: any): void {
    const file = event.target.files[0];
    if (!file) return;
    const manufacturerId = this.proposal?.manufacturerId;
    if (!manufacturerId) {
      this.excelFeedback = { type: 'danger', message: 'No manufacturer on this proposal.' };
      return;
    }

    this.uploadingExcel = true;
    this.excelFeedback = null;

    this.proposalService.importExcel(manufacturerId, file).subscribe({
      next: (response) => {
        this.uploadingExcel = false;

        if (!response.success && response.importedProducts?.length === 0) {
          // Complete failure — no valid rows
          const errors = (response.validationErrors || []).join('; ');
          this.excelFeedback = { type: 'danger', message: `${response.message}${errors ? ' — ' + errors : ''}` };
        } else {
          // Full or partial success
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
      this.productStatusesView.push(null);
      existingProductIds.add(product.productId);
      added++;
    });

    if (added > 0) {
      this.initializePricingStates();
    }

    return { added, skippedDuplicates };
  }
}
