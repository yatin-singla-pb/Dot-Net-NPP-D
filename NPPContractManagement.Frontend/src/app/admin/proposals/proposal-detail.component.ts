import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { ProposalService, Proposal, ProposalType, ProposalStatus, PriceType, ProductProposalStatus, ProposalCreateDto, ProposalConflictResult } from '../../services/proposal.service';
import { ProductService } from '../../services/product.service';
import { ContractService } from '../../services/contract.service';
import { CreateContractRequest } from '../../models/contract.model';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-proposal-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './proposal-detail.component.html',
  styles: [`
    :host {
      display: block;
      min-height: 100vh;
    }

    :host ::ng-deep #proposal_panels_container {
      transform: none !important;
      z-index: 1 !important;
    }

    :host ::ng-deep .panel,
    :host ::ng-deep .card {
      z-index: 1 !important;
    }

    :host ::ng-deep .basic-info-grid {
      display: grid !important;
      grid-template-columns: 1fr 1fr 1fr !important;
      column-gap: 24px !important;
      align-items: start !important;
      width: 100% !important;
    }

    :host ::ng-deep .info-column {
      min-width: 0;
      overflow: visible;
    }

    :host ::ng-deep .info-label {
      display: block;
      font-weight: 600;
      margin-bottom: 6px;
    }

    :host ::ng-deep .info-value {
      display: block;
      line-height: 1.4;
      white-space: normal;
      word-break: break-word;
      overflow-wrap: anywhere;
    }

    :host ::ng-deep .info-value.multiline {
      max-width: 100%;
      white-space: normal;
      word-break: break-word;
    }

    @media (max-width: 768px) {
      :host ::ng-deep .container-fluid {
        padding-left: 16px !important;
        padding-right: 16px !important;
      }

      :host ::ng-deep .basic-info-grid {
        grid-template-columns: 1fr !important;
        row-gap: 12px !important;
      }
    }
  `]
})
export class ProposalDetailComponent implements OnInit {
  loading = false;
  submitting = false;
  error: string | null = null;
  successMessage: string | null = null;
  proposalId!: number;
  proposal: Proposal | null = null;

  // Lookup data for display
  proposalTypes: ProposalType[] = [];
  proposalStatuses: ProposalStatus[] = [];
  priceTypes: PriceType[] = [];
  productProposalStatuses: ProductProposalStatus[] = [];
  manufacturers: any[] = [];
  distributors: any[] = [];
  industries: any[] = [];
  opcos: any[] = [];
  products: any[] = [];

  // Product list pagination and filtering
  productSearchTerm = '';
  productFilterPriceType: number | null = null;
  productFilterStatus: number | null = null;
  currentPage = 1;
  pageSize = 10;
  totalProducts = 0;
  Math = Math;

  // UX state for confirmation modals and navigation
  showApproveConfirm: boolean = false;
  showRejectConfirm: boolean = false;
  rejectReason: string = '';
  openContractAfterCreate: boolean = true;

  // Conflict detection state
  conflictResult: ProposalConflictResult | null = null;
  conflictLoading: boolean = false;
  conflictProductIds: Set<number> = new Set();

  // Helpers
  canShowReviewBanner(): boolean {
    if (!this.proposal) return false;
    return this.getProposalStatusName(this.proposal.proposalStatusId) === 'Submitted';
  }

  isManufacturer(): boolean {
    return this.authService.hasRole('Manufacturer');
  }

  // Modal open/close helpers
  openApproveConfirm(): void { if (this.canAccept()) { this.showApproveConfirm = true; } }
  cancelApproveConfirm(): void { this.showApproveConfirm = false; }
  confirmApprove(): void { this.showApproveConfirm = false; this.doAccept(); }

  openRejectConfirm(): void { if (this.canReject()) { this.showRejectConfirm = true; } }
  cancelRejectConfirm(): void { this.showRejectConfirm = false; this.rejectReason = ''; }
  confirmReject(): void { this.showRejectConfirm = false; this.doReject(); }

  private normalizePriceType(input: string): string {
    const pt = (input || '').trim().toLowerCase();
    if (!pt) return 'Contract Price';
    if (pt === 'discontinued' || pt === 'product discontinued') return 'Discontinued';
    if (pt === 'suspended' || pt === 'product suspended') return 'Suspended';
    if (pt === 'published list price at time of purchase' || pt === 'list at time of purchase/no bid' || pt === 'list at time of purchase / no bid') return 'List at Time of Purchase/No Bid';
    if (pt === 'guaranteed price') return 'Contract Price at Time of Purchase';
    return 'Contract Price';
  }


  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private productService: ProductService,
    private contractService: ContractService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.proposalId = +this.route.snapshot.params['id'];
    if (!this.proposalId) {
      this.router.navigate(['/admin/proposals']);
      return;
    }

    // Show success message if redirected after contract creation
    const created = this.route.snapshot.queryParamMap.get('contractCreated');
    if (created) {
      this.successMessage = 'Contract created successfully';
    }

    this.loadData();
  }

  private loadData(): void {
    this.loading = true;
    this.error = null;

    // First fetch the proposal; then load lookups (anonymous lookup endpoints)
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
        this.loading = false;

        // Auto-load conflicts when proposal is in Submitted status
        if (this.proposal && this.getProposalStatusName(this.proposal.proposalStatusId) === 'Submitted') {
          this.loadConflicts();
        }
      })
      .catch((error) => {
        this.error = 'Failed to load proposal data';
        this.loading = false;
      });
  }

  getProposalTypeName(id: number): string {
    const type = this.proposalTypes.find(t => t.id === id);
    return type ? type.name : `Type ${id}`;
  }

  getProposalStatusName(id: number): string {
    const status = this.proposalStatuses.find(s => s.id === id);
    return status ? status.name : `Status ${id}`;
  }

  getPriceTypeName(id: number): string {
    const priceType = this.priceTypes.find(pt => pt.id === id);
    return priceType ? priceType.name : `Price Type ${id}`;
  }

  getProductProposalStatusName(id: number): string {
    const status = this.productProposalStatuses.find(s => s.id === id);
    return status ? status.name : `Status ${id}`;
  }

  getManufacturerName(id: number): string {
    const manufacturer = this.manufacturers.find(m => m.id === id);
    return manufacturer ? manufacturer.name : `Manufacturer ${id}`;
  }

  getProductName(id: number): string {
    const product = this.products.find(p => p.id === id);
    return product ? product.name : `Product ${id}`;
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

  getProductProposalStatusBadgeClass(id: number): any {
    const statusName = this.getProductProposalStatusName(id);
    const nm = (statusName || '').toLowerCase();
    return {
      'bg-success': nm === 'accepted',
      'bg-danger': nm === 'rejected',
      'bg-warning text-dark': nm !== 'accepted' && nm !== 'rejected'
    };
  }

  getDistributorNames(): string[] {
    return this.proposal?.distributorIds.map(id => {
      const distributor = this.distributors.find(d => d.id === id);
      return distributor ? distributor.name : `Distributor ${id}`;
    }) || [];
  }

  getIndustryNames(): string[] {
    return this.proposal?.industryIds.map(id => {
      const industry = this.industries.find(i => i.id === id);
      return industry ? industry.name : `Industry ${id}`;
    }) || [];
  }

  getOpcoNames(): string[] {
    return this.proposal?.opcoIds.map(id => {
      const opco = this.opcos.find(o => o.id === id);
      return opco ? opco.name : `OpCo ${id}`;
    }) || [];
  }

  getDistributorObjects(): any[] {
    return this.proposal?.distributorIds.map(id => {
      return this.distributors.find(d => d.id === id);
    }).filter(Boolean) || [];
  }

  getIndustryObjects(): any[] {
    return this.proposal?.industryIds.map(id => {
      return this.industries.find(i => i.id === id);
    }).filter(Boolean) || [];
  }

  getOpcoObjects(): any[] {
    return this.proposal?.opcoIds.map(id => {
      return this.opcos.find(o => o.id === id);
    }).filter(Boolean) || [];
  }

  onEdit(): void {
    this.router.navigate(['/admin/proposals', this.proposalId, 'edit']);
  }

  onClone(): void {
    this.router.navigate(['/admin/proposals', this.proposalId, 'clone']);
  }

  onSubmit(): void {
    if (!this.proposal) return;

    this.submitting = true;
    this.error = null;

    this.proposalService.submit(this.proposalId).subscribe({
      next: (success) => {
        if (success) {
          // Reload the proposal to get updated status
          this.loadData();
        }
        this.submitting = false;
      },
      error: (error) => {
        this.error = error?.error?.message || 'Failed to submit proposal';
        this.submitting = false;
      }
    });
  }

  onAccept(): void {
    // Open confirmation modal; actual approve happens in confirmApprove()
    if (!this.canAccept()) return;
    this.showApproveConfirm = true;
  }

  private doAccept(): void {
    if (!this.proposal) return;

    this.submitting = true;
    this.error = null;
    this.successMessage = null;

    // Ensure all products are marked Accepted before accepting proposal
    const accepted = this.productProposalStatuses.find(s => s.name === 'Accepted');
    const acceptedId = accepted?.id ?? null;

    const p = this.proposal;
    const payload: ProposalCreateDto = {
      title: p.title,
      proposalTypeId: p.proposalTypeId,
      proposalStatusId: p.proposalStatusId,
      manufacturerId: p.manufacturerId ?? null,
      startDate: p.startDate ?? null,
      endDate: p.endDate ?? null,
      dueDate: p.dueDate ?? null,
      internalNotes: p.internalNotes ?? null,
      distributorIds: p.distributorIds ?? [],
      industryIds: p.industryIds ?? [],
      opcoIds: p.opcoIds ?? [],
      products: (p.products || []).map(prod => ({
        productId: prod.productId,
        priceTypeId: prod.priceTypeId ?? null,
        quantity: prod.quantity ?? null,
        productProposalStatusId: acceptedId ?? prod.productProposalStatusId ?? null,
        // Pricing fields
        uom: prod.uom ?? null,
        billbacksAllowed: !!prod.billbacksAllowed,
        allowance: prod.allowance ?? null,
        commercialDelPrice: prod.commercialDelPrice ?? null,
        commercialFobPrice: prod.commercialFobPrice ?? null,
        commodityDelPrice: prod.commodityDelPrice ?? null,
        commodityFobPrice: prod.commodityFobPrice ?? null,
        // Additional fields
        pua: prod.pua ?? null,
        ffsPrice: prod.ffsPrice ?? null,
        noiPrice: prod.noiPrice ?? null,
        ptv: prod.ptv ?? null,
        internalNotes: prod.internalNotes ?? null,
        manufacturerNotes: prod.manufacturerNotes ?? null,
      }))
    };

    // Update products to Accepted, then create contract, then mark proposal Completed
    this.proposalService.update(this.proposalId, payload).subscribe({
      next: (updated) => {
        const acceptedIdFinal = acceptedId ?? this.productProposalStatuses.find(s => (s.name || '').toLowerCase() === 'accepted')?.id ?? 2;
        const products = (updated?.products || this.proposal?.products || []).filter((prod: any) => Number(prod.productProposalStatusId ?? acceptedIdFinal) === Number(acceptedIdFinal));

        const start = this.proposal?.startDate ? new Date(this.proposal.startDate) : new Date();
        const end = this.proposal?.endDate ? new Date(this.proposal.endDate) : new Date(new Date().setMonth(new Date().getMonth() + 6));

        const req: CreateContractRequest = {
          name: this.proposal?.title || `Contract from Proposal #${this.proposalId}`,
          manufacturerReferenceNumber: `PROPOSAL-${this.proposalId}`,
          startDate: start,
          endDate: end,
          proposalId: this.proposalId,

          internalNotes: this.proposal?.internalNotes || undefined,
          distributorIds: this.proposal?.distributorIds || [],
          industryIds: this.proposal?.industryIds || [],
          opCoIds: this.proposal?.opcoIds || [],
          productIds: products.map((p: any) => Number(p.productId)),
          prices: products.map((p: any) => {
            const ptFromId = this.priceTypes.find(pt => Number(pt.id) === Number(p.priceTypeId))?.name || '';
            const rawPt = (ptFromId || '').toString();
            const mappedPt = this.normalizePriceType(rawPt);
            return {
              productId: Number(p.productId),
              priceType: mappedPt,
              allowance: p.allowance ?? undefined,
              commercialDelPrice: p.commercialDelPrice ?? undefined,
              commercialFobPrice: p.commercialFobPrice ?? undefined,
              commodityDelPrice: p.commodityDelPrice ?? undefined,
              commodityFobPrice: p.commodityFobPrice ?? undefined,
              uom: (p.uom || 'Cases'),
              estimatedQty: p.quantity ?? undefined,
              billbacksAllowed: !!p.billbacksAllowed,
              pua: p.pua ?? undefined,
              ffsPrice: p.ffsPrice ?? undefined,
              noiPrice: p.noiPrice ?? undefined,
              ptv: p.ptv ?? undefined,
              internalNotes: p.internalNotes ?? undefined
            };
          })
        } as CreateContractRequest;

        this.contractService.create(req).subscribe({
          next: (contract) => {
            this.proposalService.accept(this.proposalId).subscribe({
              next: () => {
                if (this.openContractAfterCreate && contract?.id) {
                  this.router.navigate(['/admin/contracts', contract.id], { queryParams: { fromProposal: this.proposalId } });
                } else {
                  this.successMessage = 'Proposal accepted';
                  this.router.navigate(['/admin/proposals', this.proposalId], { queryParams: { contractCreated: 1 } });
                }
                this.submitting = false;
              },
              error: (error) => { this.error = error?.error?.message || 'Failed to complete proposal'; this.submitting = false; }
            });
          },
          error: (err) => {
            // If contract creation fails, still mark as accepted to avoid blocking, but show error
            console.error('[ProposalDetail] Contract creation failed', err);
            this.proposalService.accept(this.proposalId).subscribe({
              next: () => { this.error = 'Contract creation failed, proposal accepted without contract.'; this.loadData(); this.submitting = false; },
              error: (error) => { this.error = error?.error?.message || 'Failed to accept proposal'; this.submitting = false; }
            });
          }
        });
      },
      error: (error) => {
        this.error = error?.error?.message || 'Failed to update proposal before accept';
        this.submitting = false;
      }
    });
  }

  onReject(): void {
    // Open confirmation modal; actual reject happens in confirmReject()
    if (!this.canReject()) return;
    this.showRejectConfirm = true;
  }

  private doReject(): void {
    if (!this.proposal) return;

    this.submitting = true;
    this.error = null;
    this.successMessage = null;

    // Set all products to Rejected, then mark proposal Completed via accept endpoint (which just completes without contract)
    const rejected = this.productProposalStatuses.find(s => s.name === 'Rejected');
    const rejectedId = rejected?.id ?? null;

    const p = this.proposal;
    const payload: ProposalCreateDto = {
      title: p.title,
      proposalTypeId: p.proposalTypeId,
      proposalStatusId: p.proposalStatusId,
      manufacturerId: p.manufacturerId ?? null,
      startDate: p.startDate ?? null,
      endDate: p.endDate ?? null,
      internalNotes: p.internalNotes ?? null,
      distributorIds: p.distributorIds ?? [],
      industryIds: p.industryIds ?? [],
      opcoIds: p.opcoIds ?? [],
      products: (p.products || []).map(prod => ({
        productId: prod.productId,
        priceTypeId: prod.priceTypeId ?? null,
        quantity: prod.quantity ?? null,
        productProposalStatusId: rejectedId ?? prod.productProposalStatusId ?? null,
        uom: prod.uom ?? null,
        billbacksAllowed: !!prod.billbacksAllowed,
        allowance: prod.allowance ?? null,
        commercialDelPrice: prod.commercialDelPrice ?? null,
        commercialFobPrice: prod.commercialFobPrice ?? null,
        commodityDelPrice: prod.commodityDelPrice ?? null,
        commodityFobPrice: prod.commodityFobPrice ?? null,
        pua: prod.pua ?? null,
        ffsPrice: prod.ffsPrice ?? null,
        noiPrice: prod.noiPrice ?? null,
        ptv: prod.ptv ?? null,
        internalNotes: prod.internalNotes ?? null,
        manufacturerNotes: prod.manufacturerNotes ?? null,
      }))
    };

    this.proposalService.update(this.proposalId, payload).subscribe({
      next: () => {
        // Use accept endpoint to move proposal to Completed without creating a contract
        this.proposalService.reject(this.proposalId, { reason: this.rejectReason || null }).subscribe({
          next: () => { this.successMessage = 'Proposal rejected'; this.loadData(); this.submitting = false; },
          error: (error) => { this.error = error?.error?.message || 'Failed to reject proposal'; this.submitting = false; }
        });
      },
      error: (error) => {
        this.error = error?.error?.message || 'Failed to update proposal before rejection';
        this.submitting = false;
      }
    });
  }

  // --- Conflict detection methods ---

  loadConflicts(): void {
    this.conflictLoading = true;
    this.proposalService.getConflicts(this.proposalId).subscribe({
      next: (result) => {
        this.conflictResult = result;
        this.conflictProductIds = new Set(
          (result.conflicts || []).map(c => c.productId)
        );
        this.conflictLoading = false;
      },
      error: (err) => {
        console.error('[ProposalDetail] Failed to load conflicts', err);
        this.conflictLoading = false;
      }
    });
  }

  hasProductConflict(productId: number): boolean {
    return this.conflictProductIds.has(productId);
  }

  suspendConflictingContract(contractId: number): void {
    if (!confirm('Suspend this conflicting contract? This will deactivate its pricing.')) return;
    this.contractService.suspend(contractId).subscribe({
      next: () => {
        this.loadConflicts(); // Re-check after suspension
      },
      error: (err) => {
        this.error = err?.error?.message || 'Failed to suspend contract';
      }
    });
  }

  canEdit(): boolean {
    if (!this.proposal) return false;
    if (this.authService.isContractViewer()) return false;
    const status = this.getProposalStatusName(this.proposal.proposalStatusId);
    return ['Requested', 'Pending', 'Saved', 'Submitted'].includes(status);
  }

  canAccept(): boolean {
    if (!this.proposal) return false;
    if (this.authService.hasRole('Manufacturer')) return false;
    if (this.authService.isContractViewer()) return false;
    const status = this.getProposalStatusName(this.proposal.proposalStatusId);
    // Admin/NPP can approve when proposal is Submitted (regardless of per-product statuses)
    if (status !== 'Submitted') return false;
    // Optional safety: require there to be at least one product to approve
    const hasAnyProduct = (this.proposal.products || []).length > 0;
    if (!hasAnyProduct) return false;
    // Block acceptance when pricing conflicts exist
    if (this.conflictResult?.hasConflicts) return false;
    return true;
  }

  canReject(): boolean {
    if (!this.proposal) return false;
    if (this.authService.hasRole('Manufacturer')) return false;
    if (this.authService.isContractViewer()) return false;
    const status = this.getProposalStatusName(this.proposal.proposalStatusId);
    return status === 'Submitted';
  }

  getStatusBadgeClass(statusId: number): string {
    const status = this.getProposalStatusName(statusId);
    switch (status) {
      case 'Requested': return 'bg-info';
      case 'Pending': return 'bg-warning';
      case 'Saved': return 'bg-secondary';
      case 'Submitted': return 'bg-primary';
      case 'Completed': return 'bg-success';
      default: return 'bg-light text-dark';
    }
  }

  formatDate(dateString: string | null): string {
    if (!dateString) return 'Not set';
    return new Date(dateString).toLocaleDateString();
  }

  formatCurrency(amount: number | null): string {
    if (amount === null || amount === undefined) return 'Not set';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  getTotalProposedValue(): number {
    if (!this.proposal?.products) return 0;
    return this.proposal.products.reduce((total, product) => {
      const price = this.getDisplayPrice(product) || 0;
      const quantity = product.quantity || 0;
      return total + (price * quantity);
    }, 0);
  }

  getDisplayPrice(product: any): number | null {
    if (!product) return null;
    const price = product.commercialDelPrice ?? product.commercialFobPrice ?? product.ffsPrice ?? product.noiPrice ?? null;
    return price != null ? Number(price) : null;
  }

  get paginatedProducts() {
    const filtered = this.filteredProducts;
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return filtered.slice(start, end);
  }

  get filteredProducts() {
    if (!this.proposal?.products) return [];

    let filtered = this.proposal.products;

    if (this.productSearchTerm) {
      const term = this.productSearchTerm.toLowerCase();
      filtered = filtered.filter(product => {
        const productName = this.getProductName(product.productId) || '';
        const internalNotes = product.internalNotes || '';
        const manufacturerNotes = product.manufacturerNotes || '';
        return productName.toLowerCase().includes(term) ||
               internalNotes.toLowerCase().includes(term) ||
               manufacturerNotes.toLowerCase().includes(term);
      });
    }

    if (this.productFilterPriceType !== null) {
      filtered = filtered.filter(product => product.priceTypeId === this.productFilterPriceType);
    }

    if (this.productFilterStatus !== null) {
      filtered = filtered.filter(product => product.productProposalStatusId === this.productFilterStatus);
    }

    this.totalProducts = filtered.length;
    return filtered;
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
  }

  onPriceTypeFilter(priceTypeId: number | null): void {
    this.productFilterPriceType = priceTypeId;
    this.currentPage = 1;
  }

  onStatusFilter(statusId: number | null): void {
    this.productFilterStatus = statusId;
    this.currentPage = 1;
  }

  clearFilters(): void {
    this.productSearchTerm = '';
    this.productFilterPriceType = null;
    this.productFilterStatus = null;
    this.currentPage = 1;
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
}
