import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { ProposalService, Proposal, ProposalType, ProposalStatus, PriceType, ProductProposalStatus } from '../../services/proposal.service';
import { ManufacturerService } from '../../services/manufacturer.service';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-proposal-manufacturer-review',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './proposal-manufacturer-review.component.html'
})
export class ProposalManufacturerReviewComponent implements OnInit {
  loading = false;
  submitting = false;
  error: string | null = null;
  proposalId!: number;
  proposal: Proposal | null = null;
  currentUser: any = null;

  // Lookup data for display
  proposalTypes: ProposalType[] = [];
  proposalStatuses: ProposalStatus[] = [];
  priceTypes: PriceType[] = [];
  productProposalStatuses: ProductProposalStatus[] = [];
  manufacturers: any[] = [];
  products: any[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private manufacturerService: ManufacturerService,
    private productService: ProductService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.proposalId = +this.route.snapshot.params['id'];
    if (!this.proposalId) {
      this.router.navigate(['/admin/proposals']);
      return;
    }
    
    this.currentUser = this.authService.getCurrentUser();
    this.loadData();
  }

  private loadData(): void {
    this.loading = true;
    
    // Load lookup data and proposal data in parallel
    Promise.all([
      this.proposalService.getProposalTypes().toPromise(),
      this.proposalService.getProposalStatuses().toPromise(),
      this.proposalService.getPriceTypes().toPromise(),
      this.proposalService.getProductProposalStatuses().toPromise(),
      this.manufacturerService.getAll().toPromise(),
      this.productService.getAll().toPromise(),
      this.proposalService.getById(this.proposalId).toPromise()
    ]).then(([
      proposalTypes,
      proposalStatuses,
      priceTypes,
      productProposalStatuses,
      manufacturers,
      products,
      proposal
    ]) => {
      this.proposalTypes = proposalTypes || [];
      this.proposalStatuses = proposalStatuses || [];
      this.priceTypes = priceTypes || [];
      this.productProposalStatuses = productProposalStatuses || [];
      this.manufacturers = manufacturers || [];
      this.products = products || [];
      this.proposal = proposal!;
      
      // Check if user has access to this proposal
      if (!this.canAccessProposal()) {
        this.error = 'You do not have permission to review this proposal';
      }
      
      this.loading = false;
    }).catch(error => {
      this.error = 'Failed to load proposal data';
      this.loading = false;
    });
  }

  canAccessProposal(): boolean {
    if (!this.proposal || !this.currentUser) return false;
    
    // Check if user is associated with the proposal's manufacturer
    if (this.proposal.manufacturerId && this.currentUser.manufacturerIds) {
      return this.currentUser.manufacturerIds.includes(this.proposal.manufacturerId);
    }
    
    // Admin users can access all proposals
    return this.currentUser.roles?.includes('Admin') || false;
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

  canSave(): boolean {
    if (!this.proposal) return false;
    const status = this.getProposalStatusName(this.proposal.proposalStatusId);
    return ['Requested', 'Pending'].includes(status) && this.canAccessProposal();
  }

  canSubmit(): boolean {
    if (!this.proposal) return false;
    const status = this.getProposalStatusName(this.proposal.proposalStatusId);
    return ['Requested', 'Pending', 'Saved'].includes(status) && this.canAccessProposal();
  }

  onSave(): void {
    if (!this.proposal || !this.canSave()) return;
    
    this.submitting = true;
    this.error = null;

    // Update status to "Saved"
    const savedStatus = this.proposalStatuses.find(s => s.name === 'Saved');
    if (!savedStatus) {
      this.error = 'Unable to find "Saved" status';
      this.submitting = false;
      return;
    }

    const payload = {
      title: this.proposal.title,
      proposalTypeId: this.proposal.proposalTypeId,
      proposalStatusId: savedStatus.id,
      manufacturerId: this.proposal.manufacturerId,
      startDate: this.proposal.startDate,
      endDate: this.proposal.endDate,
      internalNotes: this.proposal.internalNotes,
      products: this.proposal.products,
      distributorIds: this.proposal.distributorIds,
      industryIds: this.proposal.industryIds,
      opcoIds: this.proposal.opcoIds
    };

    this.proposalService.update(this.proposalId, payload).subscribe({
      next: (result) => {
        this.proposal = result;
        this.submitting = false;
      },
      error: (error) => {
        this.error = error?.error?.message || 'Failed to save proposal';
        this.submitting = false;
      }
    });
  }

  onSubmit(): void {
    if (!this.proposal || !this.canSubmit()) return;
    
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

  getManufacturerProducts(): any[] {
    if (!this.proposal?.products || !this.proposal.manufacturerId) return [];

    return this.proposal.products.filter(product => {
      const productDetails = this.products.find(p => p.id === product.productId);
      return productDetails?.manufacturerId === this.proposal!.manufacturerId;
    });
  }

  isManufacturerProduct(productId: number): boolean {
    if (!this.proposal?.manufacturerId) return false;

    const productDetails = this.products.find(p => p.id === productId);
    return productDetails?.manufacturerId === this.proposal.manufacturerId;
  }
}
