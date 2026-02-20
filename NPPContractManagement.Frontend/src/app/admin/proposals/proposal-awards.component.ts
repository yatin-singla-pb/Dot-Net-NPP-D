import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ProposalService, Proposal, ProposalProduct, ProductProposalStatus, PriceType, ProposalStatusHistory, ProposalProductEditHistory } from '../../services/proposal.service';
import { ContractService } from '../../services/contract.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-proposal-awards',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './proposal-awards.component.html'
})
export class ProposalAwardsComponent implements OnInit {
  proposalId!: number;
  proposal: Proposal | null = null;

  // Lookups
  productStatuses: ProductProposalStatus[] = [];
  priceTypes: PriceType[] = [];
  products: any[] = [];

  uoms: string[] = ['Cases', 'Pounds'];

  // History
  statusHistory: ProposalStatusHistory[] = [];
  contractPriceHistory: any[] = [];
  // Proposal edit history (per product)
  editHistory: ProposalProductEditHistory[] = [];


  // UI state
  loading = true;
  saving = false;
  error: string | null = null;
  success: string | null = null;

  // Selection map: productId -> statusName ('Accepted' | 'Rejected' | null)
  selections: Record<number, 'Accepted' | 'Rejected' | null> = {};
  selectedProductId: number | null = null;
  searchTerm = '';

  // Inline editing state for pricing fields
  editingProductId: number | null = null;

  // Product list pagination and filtering
  productSearchTerm = '';
  productFilterPriceType: number | null = null;
  productFilterStatus: number | null = null;
  currentPage = 1;
  pageSize = 10;
  totalProducts = 0;
  Math = Math;
  private _cachedFilteredIndices: number[] | null = null;
  private _lastFilterState = { search: '', priceType: null as number | null, status: null as number | null, arrayLength: 0 };

  // Historical pricing modal
  showHistoricalPricingModal = false;
  selectedProductForHistory: any = null;
  historicalPricing: any[] = [];

  // Proposal edit history modal
  showProposalHistoryModal = false;
  proposalEditHistory: any[] = [];


  // History accordion state
  showPricingHistory = true;
  showEditHistory = false;
  showWorkflowHistory = false;

  // Modal for reject-all
  showRejectAllModal = false;
  rejectReason: string = '';

  // Approve selected confirmation modal
  showApproveConfirmModal = false;
  openAfterCreation = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private proposalService: ProposalService,
    private contractService: ContractService,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.proposalId = Number(this.route.snapshot.paramMap.get('id'));
    if (!this.isAdmin()) {
      // Manufacturers should not access this page
      this.router.navigate(['/admin/proposals', this.proposalId]);
      return;
    }
    this.load();
  }

  private load(): void {
    this.loading = true;
    this.error = null;

    // 1) Load proposal first to know manufacturer for product lookup
    this.proposalService.getById(this.proposalId).subscribe({
      next: (proposal) => {
        this.proposal = proposal;
        const manufacturerIds = proposal?.manufacturerId ? [Number(proposal.manufacturerId)] : [];

        // 2) Load lookups and history (and products list for names/search)
        forkJoin({
          statuses: this.proposalService.getProductProposalStatuses(),
          pts: this.proposalService.getPriceTypes(),
          history: this.proposalService.getStatusHistory(this.proposalId),
          prodEditHistory: this.proposalService.getProductEditHistory(this.proposalId).pipe(catchError(() => of([]))),
          products: this.proposalService.getProductsByManufacturers(manufacturerIds)
        }).subscribe({
          next: ({ statuses, pts, history, prodEditHistory, products }) => {
            this.productStatuses = statuses || [];
            this.priceTypes = pts || [];
            this.statusHistory = history || [];
            this.editHistory = prodEditHistory || [];
            this.products = products || [];

            // Initialize selections as null (no choice yet)
            (proposal.products || []).forEach(p => this.selections[p.productId] = null);

            // Preselect the first product so history is visible by default
            const first = (proposal.products || [])[0];
            if (first && !this.selectedProductId) {
              this.selectedProductId = first.productId;
              this.loadContractPriceHistory(first.productId);
            }

            this.loading = false;
          },
          error: (err) => {
            this.error = err?.message || 'Failed to load lookups';
            this.loading = false;
          }
        });
      },
      error: (err) => {
        this.error = err?.message || 'Failed to load proposal';
        this.loading = false;
      }
    });
  }

  isAdmin(): boolean {
    return this.auth.hasRole('System Administrator') || this.auth.hasRole('Contract Manager') || this.auth.hasRole('Admin');
  }

  // Helpers
  get acceptedStatusId(): number | null {
    return this.productStatuses.find(s => (s.name || '').toLowerCase() === 'accepted')?.id ?? null;
  }
  get rejectedStatusId(): number | null {
    return this.productStatuses.find(s => (s.name || '').toLowerCase() === 'rejected')?.id ?? null;
  }

  getPriceTypeName(id?: number | null): string {
    if (!id && id !== 0) return '—';
    const pt = this.priceTypes.find(pt => Number(pt.id) === Number(id));
    return pt ? pt.name : `Price Type ${id}`;
  }

  getProductName(id: number): string {
    const prod = this.products.find(p => Number(p.id) === Number(id));
    return prod ? prod.name : `Product ${id}`;
  }

  getProductById(id: number): any {
    return (this.products || []).find(p => Number(p.id) === Number(id));
  }

  getProductHeaderTitle(id: number): string {
    const p = this.getProductById(id);
    if (!p) return '';
    return [p.manufacturerProductCode, p.brand, p.packSize, p.name || p.description].filter(Boolean).join(' - ');
  }

  getProductStatusBadgeClass(status: string): any {
    const s = (status || '').toLowerCase();
    return {
      'bg-success': s === 'active',
      'bg-danger': s === 'discontinued',
      'bg-warning text-dark': s === 'inactive' || s === 'pending'
    };
  }

  formatCurrencyUSD(value: number | null | undefined): string {
    if (value === null || value === undefined) {
      return '—';
    }
    const num = Number(value);
    if (isNaN(num)) {
      return '—';
    }
    return `$${num.toFixed(2)}`;
  }

  get filteredProducts(): ProposalProduct[] {
    const list = this.proposal?.products || [];
    const t = (this.searchTerm || '').trim().toLowerCase();
    if (!t) return list as any;
    return list.filter((p: any) => this.getProductName(p.productId).toLowerCase().includes(t));
  }

  // Selection counters (based on user selections only)
  get acceptedSelectedCount(): number { return Object.values(this.selections).filter(v => v === 'Accepted').length; }
  get rejectedSelectedCount(): number { return Object.values(this.selections).filter(v => v === 'Rejected').length; }
  get pendingSelectedCount(): number { return (this.proposal?.products || []).length - this.acceptedSelectedCount - this.rejectedSelectedCount; }

  // Accept/Reject/Pending counts including existing statuses (for badges/summary)
  get acceptedCountIncludingExisting(): number {
    const acceptedId = this.acceptedStatusId;
    return (this.proposal?.products || []).filter(p => this.selections[p.productId] === 'Accepted' || (!!acceptedId && p.productProposalStatusId === acceptedId)).length;
  }
  get rejectedCountIncludingExisting(): number {
    const rejectedId = this.rejectedStatusId;
    return (this.proposal?.products || []).filter(p => this.selections[p.productId] === 'Rejected' || (!!rejectedId && p.productProposalStatusId === rejectedId)).length;
  }
  get pendingCountIncludingExisting(): number {
    const total = this.totalProducts;
    return total - this.acceptedCountIncludingExisting - this.rejectedCountIncludingExisting;
  }

  selectAll(status: 'Accepted' | 'Rejected'): void {
    (this.proposal?.products || []).forEach(p => this.selections[p.productId] = status);
  }

  clearSelections(): void {
    (this.proposal?.products || []).forEach(p => this.selections[p.productId] = null);
  }
  get filteredEditHistory(): ProposalProductEditHistory[] {
    const list = this.editHistory || [];
    const pid = this.selectedProductId;
    if (!pid) return list.slice(0, 20);
    return list.filter(h => Number(h.productId) === Number(pid)).slice(0, 20);
  }

  formatEditSummary(h: ProposalProductEditHistory): string {
    try {
      const prev = h.previousJson ? JSON.parse(h.previousJson) : {};
      const curr = h.currentJson ? JSON.parse(h.currentJson) : {};
      const parts: string[] = [];
      const add = (label: string, a: any, b: any, map?: (x: any) => any) => {
        const av = map ? map(a) : a; const bv = map ? map(b) : b;
        if (av !== bv) parts.push(`${label}: ${av ?? '—'} → ${bv ?? '—'}`);
      };
      add('Type', prev.priceTypeId, curr.priceTypeId, (id: any) => this.getPriceTypeName(Number(id)));
      add('UOM', prev.uom, curr.uom);
      add('FOB', prev.commercialFobPrice, curr.commercialFobPrice);
      add('DEL', prev.commercialDelPrice, curr.commercialDelPrice);
      add('Allowance', prev.allowance, curr.allowance);
      if (parts.length === 0) return 'No material field changes';

      return parts.join('; ');
    } catch {
      return h.changeType;
    }
  }

  toggleHistory(section: 'pricing' | 'edit' | 'workflow'): void {
    const current = {
      pricing: this.showPricingHistory,
      edit: this.showEditHistory,
      workflow: this.showWorkflowHistory
    };

    this.showPricingHistory = section === 'pricing' ? !current.pricing : false;
    this.showEditHistory = section === 'edit' ? !current.edit : false;
    this.showWorkflowHistory = section === 'workflow' ? !current.workflow : false;
  }

  get paginatedProducts() {
    const filtered = this.filteredProductIndices;
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return filtered.slice(start, end).map(index => this.proposal!.products[index]);
  }

  get filteredProductIndices(): number[] {
    if (!this.proposal?.products) return [];

    // Check if we need to recalculate
    const currentState = {
      search: this.productSearchTerm,
      priceType: this.productFilterPriceType,
      status: this.productFilterStatus,
      arrayLength: this.proposal.products.length
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

    for (let i = 0; i < this.proposal.products.length; i++) {
      const product = this.proposal.products[i];
      let matches = true;

      if (this.productSearchTerm) {
        const term = this.productSearchTerm.toLowerCase();
        const productName = this.getProductName(product.productId) || '';
        const internalNotes = product.internalNotes || '';
        const manufacturerNotes = product.manufacturerNotes || '';
        matches = matches && (productName.toLowerCase().includes(term) ||
                             internalNotes.toLowerCase().includes(term) ||
                             manufacturerNotes.toLowerCase().includes(term));
      }

      if (this.productFilterPriceType !== null) {
        matches = matches && (product.priceTypeId === this.productFilterPriceType);
      }

      if (this.productFilterStatus !== null) {
        matches = matches && (product.productProposalStatusId === this.productFilterStatus);
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

  clearFiltersOnly(): void {
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

  getProductStatusName(statusId: number | null): string {
    if (!statusId) return '';
    return this.productStatuses.find(s => s.id === statusId)?.name || '';
  }

  statusBadgeClass(statusName: string): string {
    switch (statusName?.toLowerCase()) {
      case 'accepted': return 'bg-success';
      case 'rejected': return 'bg-danger';
      case 'pending': return 'bg-warning text-dark';
      default: return 'bg-secondary';
    }
  }

  openHistoricalPricingModal(product: any): void {
    this.selectedProductForHistory = product;
    this.loadHistoricalPricing(product.productId);
    this.showHistoricalPricingModal = true;
  }

  closeHistoricalPricingModal(): void {
    this.showHistoricalPricingModal = false;
    this.selectedProductForHistory = null;
    this.historicalPricing = [];
  }

  private loadHistoricalPricing(productId: number): void {
    this.contractService.getPricesByProduct(productId).subscribe({
      next: (list) => this.historicalPricing = (list || []).slice(0, 20),
      error: () => this.historicalPricing = []
    });
  }

  openProposalHistoryModal(): void {
    this.loadProposalEditHistory();
    this.showProposalHistoryModal = true;
  }

  closeProposalHistoryModal(): void {
    this.showProposalHistoryModal = false;
    this.proposalEditHistory = [];
  }

  private loadProposalEditHistory(): void {
    if (!this.proposal) return;
    this.proposalService.getProductEditHistory(this.proposal.id).subscribe({
      next: (list: any) => this.proposalEditHistory = list || [],
      error: () => this.proposalEditHistory = []
    });
  }

  // Mutual exclusivity: Allowance vs pricing fields
  isAllowanceDisabled(prod: any): boolean {
    return [prod.commercialDelPrice, prod.commercialFobPrice, prod.pua, prod.ffsPrice, prod.noiPrice, prod.ptv]
      .some((v: any) => v != null && v > 0);
  }

  isPricingDisabled(prod: any): boolean {
    return prod.allowance != null && prod.allowance > 0;
  }

  onAllowanceChange(prod: any): void {
    if (prod.allowance != null && prod.allowance > 0) {
      prod.commercialDelPrice = null;
      prod.commercialFobPrice = null;
      prod.pua = null;
      prod.ffsPrice = null;
      prod.noiPrice = null;
      prod.ptv = null;
    }
  }

  onPricingFieldChange(prod: any): void {
    const hasPricingValue = [prod.commercialDelPrice, prod.commercialFobPrice, prod.pua, prod.ffsPrice, prod.noiPrice, prod.ptv]
      .some((v: any) => v != null && v > 0);
    if (hasPricingValue) {
      prod.allowance = null;
    }
  }

  startEditingProduct(productId: number): void {
    this.editingProductId = this.editingProductId === productId ? null : productId;
  }

  onRowClick(p: ProposalProduct & { productName?: string }): void {
    this.selectedProductId = p.productId;
    this.loadContractPriceHistory(p.productId);
  }

  private loadContractPriceHistory(productId: number): void {
    this.contractService.getPricesByProduct(productId).subscribe({
      next: (list) => this.contractPriceHistory = (list || []).slice(0, 20),
      error: () => this.contractPriceHistory = []
    });
  }

  saveSelections(): void {
    if (!this.proposal) return;
    this.saving = true;
    const acceptedId = this.acceptedStatusId;
    const rejectedId = this.rejectedStatusId;

    const updatedProducts = (this.proposal.products || []).map((p: any) => ({
      productId: p.productId,
      priceTypeId: p.priceTypeId ?? null,
      quantity: p.quantity ?? null,
      productProposalStatusId: (this.selections[p.productId] === 'Accepted') ? acceptedId : (this.selections[p.productId] === 'Rejected') ? rejectedId : (p.productProposalStatusId ?? null),
      uom: p.uom ?? null,
      billbacksAllowed: p.billbacksAllowed ?? false,
      allowance: p.allowance ?? null,
      commercialDelPrice: p.commercialDelPrice ?? null,
      commercialFobPrice: p.commercialFobPrice ?? null,
      commodityDelPrice: p.commodityDelPrice ?? null,
      commodityFobPrice: p.commodityFobPrice ?? null,
      pua: p.pua ?? null,
      ffsPrice: p.ffsPrice ?? null,
      noiPrice: p.noiPrice ?? null,
      ptv: p.ptv ?? null,
      internalNotes: p.internalNotes ?? null,
      manufacturerNotes: p.manufacturerNotes ?? null
    }));

    const payload = {
      title: this.proposal.title,
      proposalTypeId: this.proposal.proposalTypeId,
      proposalStatusId: this.proposal.proposalStatusId,
      manufacturerId: this.proposal.manufacturerId ?? null,
      startDate: this.proposal.startDate ?? null,
      endDate: this.proposal.endDate ?? null,
      internalNotes: this.proposal.internalNotes ?? null,
      products: updatedProducts,
      distributorIds: this.proposal.distributorIds || [],
      industryIds: this.proposal.industryIds || [],
      opcoIds: this.proposal.opcoIds || []
    } as any;

    this.proposalService.update(this.proposalId, payload).subscribe({
      next: () => { this.success = 'Selections saved'; this.saving = false; this.load(); },
      error: (err) => { this.error = err?.message || 'Failed to save selections'; this.saving = false; }
    });
  }

  submitAward(): void {
    if (!this.proposal) {
      return;
    }

    const acceptedCount = this.acceptedCountIncludingExisting;
    this.error = null;
    this.conflictDetails = [];

    if (acceptedCount > 0) {
      // At least one product is approved (including existing approvals): create contract with approved ones
      this.confirmApproveSelected();
    } else {
      // No approved products: follow the existing "reject all" flow (no contract)
      this.openRejectAll();
    }
  }

  confirmApproveSelected(): void {
    // Require at least one product selected as Accepted (including existing accepted)
    const acceptedCount = this.acceptedCountIncludingExisting;
    if (!acceptedCount) {
      this.error = 'Select at least one product to approve';
      return;
    }
    this.error = null;
    this.showApproveConfirmModal = true;
  }

  cancelApproveConfirm(): void { this.showApproveConfirmModal = false; }
  proceedApproveSelected(): void {
    this.showApproveConfirmModal = false;
    this.approveSelected();
  }

  private normalizePriceType(input: string): string {
    const pt = (input || '').trim().toLowerCase();
    if (!pt) return 'Contract Price';
    if (pt === 'discontinued' || pt === 'product discontinued') return 'Discontinued';
    if (pt === 'suspended' || pt === 'product suspended') return 'Suspended';
    if (pt === 'published list price at time of purchase' || pt === 'list at time of purchase/no bid' || pt === 'list at time of purchase / no bid') return 'List at Time of Purchase/No Bid';
    if (pt === 'guaranteed price' || pt === 'contract price at time of purchase') return 'Contract Price at Time of Purchase';
    if (pt.includes('list') && pt.includes('no bid')) return 'List at Time of Purchase/No Bid';
    if (pt.includes('contract price')) return 'Contract Price at Time of Purchase';
    return 'Contract Price';
  }

  private approveSelected(): void {
    if (!this.proposal) return;
    this.saving = true;

    // Step 1: persist selections onto the proposal
    const acceptedId = this.acceptedStatusId;
    const rejectedId = this.rejectedStatusId;

    const updatedProducts = (this.proposal.products || []).map((p: any) => ({
      productId: p.productId,
      priceTypeId: p.priceTypeId ?? null,
      quantity: p.quantity ?? null,
      productProposalStatusId: (this.selections[p.productId] === 'Accepted') ? acceptedId : (this.selections[p.productId] === 'Rejected') ? rejectedId : (p.productProposalStatusId ?? null),
      uom: p.uom ?? null,
      billbacksAllowed: p.billbacksAllowed ?? false,
      allowance: p.allowance ?? null,
      commercialDelPrice: p.commercialDelPrice ?? null,
      commercialFobPrice: p.commercialFobPrice ?? null,
      commodityDelPrice: p.commodityDelPrice ?? null,
      commodityFobPrice: p.commodityFobPrice ?? null,
      pua: p.pua ?? null,
      ffsPrice: p.ffsPrice ?? null,
      noiPrice: p.noiPrice ?? null,
      ptv: p.ptv ?? null,
      internalNotes: p.internalNotes ?? null,
      manufacturerNotes: p.manufacturerNotes ?? null
    }));

    const payload = {
      title: this.proposal.title,
      proposalTypeId: this.proposal.proposalTypeId,
      proposalStatusId: this.proposal.proposalStatusId,
      manufacturerId: this.proposal.manufacturerId ?? null,
      startDate: this.proposal.startDate ?? null,
      endDate: this.proposal.endDate ?? null,
      internalNotes: this.proposal.internalNotes ?? null,
      products: updatedProducts,
      distributorIds: this.proposal.distributorIds || [],
      industryIds: this.proposal.industryIds || [],
      opcoIds: this.proposal.opcoIds || []
    } as any;

    this.proposalService.update(this.proposalId, payload).subscribe({
      next: () => {
        // Step 2: create a contract with only accepted products
        const acceptedProducts = updatedProducts.filter((p: any) => p.productProposalStatusId && p.productProposalStatusId === acceptedId);
        const prices = acceptedProducts.map((p: any) => {
          const ptFromId = this.priceTypes.find(pt => Number(pt.id) === Number(p.priceTypeId))?.name || '';
          const mappedPt = this.normalizePriceType(ptFromId);
          return {
            productId: Number(p.productId),
            priceType: mappedPt,
            allowance: p.allowance ?? null,
            commercialDelPrice: p.commercialDelPrice ?? null,
            commercialFobPrice: p.commercialFobPrice ?? null,
            commodityDelPrice: p.commodityDelPrice ?? null,
            commodityFobPrice: p.commodityFobPrice ?? null,
            uom: p.uom,
            estimatedQty: p.estimatedQty ?? null,
            billbacksAllowed: !!p.billbacksAllowed,
            pua: p.pua ?? null,
            ffsPrice: p.ffsPrice ?? null,
            noiPrice: p.noiPrice ?? null,
            ptv: p.ptv ?? null,
            internalNotes: p.internalNotes ?? null
          };
        });

        const req = {
          name: this.proposal?.title || `Proposal ${this.proposalId} Award`,
          startDate: this.proposal?.startDate ? new Date(this.proposal.startDate) : new Date(),
          endDate: this.proposal?.endDate ? new Date(this.proposal.endDate) : new Date(),
          internalNotes: this.proposal?.internalNotes || undefined,
          manufacturerReferenceNumber: `PROPOSAL-${this.proposalId}`,
          distributorIds: this.proposal?.distributorIds || [],
          industryIds: this.proposal?.industryIds || [],
          opCoIds: this.proposal?.opcoIds || [],
          productIds: acceptedProducts.map((p: any) => Number(p.productId)),
          prices
        } as any;

        this.contractService.create(req).subscribe({
          next: (contract) => {
            // Step 3: mark proposal accepted/completed
            this.proposalService.accept(this.proposalId).subscribe({
              next: () => {
                this.saving = false;
                if (this.openAfterCreation) {
                  this.router.navigate(['/admin/contracts', contract?.id], { queryParams: { fromProposal: this.proposalId } });
                } else {
                  this.router.navigate(['/admin/proposals', this.proposalId], { queryParams: { approved: 1 } });
                }
              },
              error: (e) => {
                this.saving = false;
                this.handleAcceptError(e);
              }
            });
          },
          error: (e) => { this.error = e?.error?.message || e?.message || 'Failed to create contract'; this.saving = false; }
        });
      },
      error: (e) => { this.error = e?.error?.message || e?.message || 'Failed to save selections'; this.saving = false; }
    });
  }

  // Conflict details from accept failure
  conflictDetails: Array<{ contractId: number; contractName: string; products: string[] }> = [];

  private handleAcceptError(e: any): void {
    const body = e?.error;
    const message = body?.message || e?.message || 'Failed to accept proposal';
    this.error = message;

    // Extract conflict details from the 400 response and group by contract
    this.conflictDetails = [];
    const conflictResult = body?.conflicts;
    const conflictList = conflictResult?.conflicts;
    if (Array.isArray(conflictList) && conflictList.length) {
      const byContract = new Map<number, { contractId: number; contractName: string; products: string[] }>();
      for (const c of conflictList) {
        const cid = c.conflictingContractId;
        if (!byContract.has(cid)) {
          byContract.set(cid, {
            contractId: cid,
            contractName: c.conflictingContractName || `Contract ${cid}`,
            products: []
          });
        }
        const entry = byContract.get(cid)!;
        const pName = c.productName || `Product ${c.productId}`;
        if (!entry.products.includes(pName)) {
          entry.products.push(pName);
        }
      }
      this.conflictDetails = Array.from(byContract.values());
    }
  }

  // Reject all flow
  openRejectAll(): void { this.showRejectAllModal = true; }
  cancelRejectAll(): void { this.showRejectAllModal = false; this.rejectReason = ''; }
  confirmRejectAll(): void {
    if (!this.proposal) return;
    this.saving = true;

    const rejectedId = this.rejectedStatusId;
    const updatedProducts = (this.proposal.products || []).map((p: any) => ({
      productId: p.productId,
      priceTypeId: p.priceTypeId ?? null,
      quantity: p.quantity ?? null,
      productProposalStatusId: rejectedId,
      uom: p.uom ?? null,
      billbacksAllowed: p.billbacksAllowed ?? false,
      allowance: p.allowance ?? null,
      commercialDelPrice: p.commercialDelPrice ?? null,
      commercialFobPrice: p.commercialFobPrice ?? null,
      commodityDelPrice: p.commodityDelPrice ?? null,
      commodityFobPrice: p.commodityFobPrice ?? null,
      pua: p.pua ?? null,
      ffsPrice: p.ffsPrice ?? null,
      noiPrice: p.noiPrice ?? null,
      ptv: p.ptv ?? null,
      internalNotes: p.internalNotes ?? null,
      manufacturerNotes: p.manufacturerNotes ?? null
    }));

    const payload = {
      title: this.proposal.title,
      proposalTypeId: this.proposal.proposalTypeId,
      proposalStatusId: this.proposal.proposalStatusId,
      manufacturerId: this.proposal.manufacturerId ?? null,
      startDate: this.proposal.startDate ?? null,
      endDate: this.proposal.endDate ?? null,
      internalNotes: this.proposal.internalNotes ?? null,
      products: updatedProducts,
      distributorIds: this.proposal.distributorIds || [],
      industryIds: this.proposal.industryIds || [],
      opcoIds: this.proposal.opcoIds || []
    } as any;

    this.proposalService.update(this.proposalId, payload).subscribe({
      next: () => {
        this.proposalService.reject(this.proposalId, { reason: this.rejectReason || null }).subscribe({
          next: () => { this.saving = false; this.router.navigate(['/admin/proposals', this.proposalId], { queryParams: { rejected: 1 } }); },
          error: (e) => { this.error = e?.message || 'Failed to reject proposal'; this.saving = false; }
        });
      },
      error: (e) => { this.error = e?.message || 'Failed to save selections'; this.saving = false; }
    });
  }
}

