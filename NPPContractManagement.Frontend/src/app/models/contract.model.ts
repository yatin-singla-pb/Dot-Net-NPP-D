export interface Contract {
  id: number;

  // New schema
  name?: string;
  // Legacy (deprecated)
  title?: string;
  manufacturerId?: number;
  manufacturerName?: string;
  // External refs
  foreignContractId?: string;
  foreignContractID?: string;
  suspendedDate?: Date;
  // Source proposal
  proposalId?: number;
  proposalTitle?: string;

  status?: ContractStatus;
  startDate: Date;
  endDate: Date;
  totalValue?: number;
  // Notes
  internalNotes?: string;
  notes?: string;
  // Manufacturer/Entegra metadata
  manufacturerReferenceNumber?: string;
  manufacturerBillbackName?: string;
  manufacturerTermsAndConditions?: string;
  manufacturerNotes?: string;
  contactPerson?: string;
  entegraContractType?: string;
  entegraVdaProgram?: string;

  isSuspended: boolean;
  sendToPerformance: boolean;
  currentVersionNumber: number;
  isActive: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;

  // Navigation properties
  distributors?: ContractDistributor[];
  opCos?: ContractOpCo[];
  industries?: ContractIndustry[];
  contractProducts?: ContractProduct[];

}
export interface ContractProduct {
  productId: number;
  productName?: string;
}


export enum ContractStatus {
  Draft = 'Draft',
  Pending = 'Pending',
  Active = 'Active',
  Expired = 'Expired',
  Terminated = 'Terminated',
  Suspended = 'Suspended'
}

export interface ContractDistributor {
  contractId: number;
  distributorId: number;
  distributorName?: string;
  createdDate: Date;
  createdBy?: string;
}

export interface ContractOpCo {
  contractId: number;
  opCoId: number;
  opCoName?: string;
  createdDate: Date;
  createdBy?: string;
}

export interface ContractIndustry {
  contractId: number;
  industryId: number;
  industryName?: string;
  createdDate: Date;
  createdBy?: string;
}

export interface CreateContractRequest {

  // New schema
  name?: string;
  // Manufacturer/Entegra metadata
  manufacturerReferenceNumber?: string;
  manufacturerBillbackName?: string;
  manufacturerTermsAndConditions?: string;
  manufacturerNotes?: string;
  contactPerson?: string;
  entegraContractType?: string;
  entegraVdaProgram?: string;

  startDate: Date;
  endDate: Date;
  // Notes
  internalNotes?: string;
  sendToPerformance?: boolean;
  // Source proposal
  proposalId?: number;

  // External refs
  foreignContractId?: string;

  distributorIds?: number[];
  opCoIds?: number[];
  industryIds?: number[];

  productIds?: number[];
  prices?: CreateContractPriceInput[];
}

export interface CreateContractPriceInput {
  versionNumber?: number; // defaults to highest version on server
  productId: number;
  priceType: string; // "Contract Price" | "List at time of purchase / No Bid" | "Product Suspended"
  allowance?: number;
  commercialDelPrice?: number;
  commercialFobPrice?: number;
  commodityDelPrice?: number;
  commodityFobPrice?: number;
  uom: string; // "Cases" | "Pounds"
  estimatedQty?: number;
  billbacksAllowed?: boolean;
  pua?: number;
  ffsPrice?: number;
  noiPrice?: number;
  ptv?: number;
  internalNotes?: string;
}


export interface UpdateContractRequest {

  // New schema
  name?: string;
  // Legacy
  title?: string;
  manufacturerId?: number;
  startDate?: Date;
  endDate?: Date;
  // External refs
  foreignContractId?: string;
  foreignContractID?: string;
  // Notes
  internalNotes?: string;
  notes?: string;
  sendToPerformance?: boolean;
  distributorIds?: number[];
  opCoIds?: number[];
  industryIds?: number[];
  // Manufacturer/Entegra metadata
  manufacturerReferenceNumber?: string;
  manufacturerBillbackName?: string;
  manufacturerTermsAndConditions?: string;
  manufacturerNotes?: string;
  contactPerson?: string;
  entegraContractType?: string;
  entegraVdaProgram?: string;

}

export interface ContractSearchResult {
  items: Contract[];
  totalCount: number;

  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Helper functions
export class ContractHelper {
  static getStatusColor(status: ContractStatus): string {
    switch (status) {
      case ContractStatus.Active:
        return 'success';
      case ContractStatus.Draft:
        return 'secondary';
      case ContractStatus.Pending:
        return 'warning';
      case ContractStatus.Expired:
        return 'danger';
      case ContractStatus.Terminated:
        return 'danger';
      case ContractStatus.Suspended:
        return 'warning';
      default:
        return 'secondary';
    }
  }

  static getStatusIcon(status: ContractStatus): string {
    switch (status) {
      case ContractStatus.Active:
        return 'fa-check-circle';
      case ContractStatus.Draft:
        return 'fa-edit';
      case ContractStatus.Pending:
        return 'fa-clock';
      case ContractStatus.Expired:
        return 'fa-calendar-times';
      case ContractStatus.Terminated:
        return 'fa-times-circle';
      case ContractStatus.Suspended:
        return 'fa-pause-circle';
      default:
        return 'fa-question-circle';
    }
  }

  static formatCurrency(amount?: number): string {
    if (!amount) return 'N/A';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  static formatDateRange(startDate: Date, endDate: Date): string {
    const start = new Date(startDate).toLocaleDateString();
    const end = new Date(endDate).toLocaleDateString();
    return `${start} - ${end}`;
  }

  static isExpired(contract: Contract): boolean {
    return new Date(contract.endDate) < new Date();
  }

  static isExpiringSoon(contract: Contract, daysThreshold: number = 30): boolean {
    const endDate = new Date(contract.endDate);
    const today = new Date();
    const diffTime = endDate.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays <= daysThreshold && diffDays > 0;
  }

  static getDaysUntilExpiry(contract: Contract): number {
    const endDate = new Date(contract.endDate);
    const today = new Date();
    const diffTime = endDate.getTime() - today.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  static getDisplayName(contract: Contract): string {
    return (contract.name && contract.name.trim().length > 0) ? contract.name : `#${contract.id}`;
  }

  static isActive(contract: Contract): boolean {
    return contract.status === ContractStatus.Active &&
           contract.isActive &&
           !contract.isSuspended &&
           !this.isExpired(contract);
  }

  static canEdit(contract: Contract): boolean {
    return contract.status !== ContractStatus.Terminated;
  }

  static canDelete(contract: Contract): boolean {
    return contract.status === ContractStatus.Draft;
  }

  static canSuspend(contract: Contract): boolean {
    return contract.status === ContractStatus.Active && !contract.isSuspended;
  }

  static canActivate(contract: Contract): boolean {
    return (contract.status === ContractStatus.Pending || contract.isSuspended) &&
           !this.isExpired(contract);
  }

  static sortByContractNumber(contracts: Contract[]): Contract[] {
    return contracts.sort((a, b) => {
      const an = (a.name || '').toLowerCase();
      const bn = (b.name || '').toLowerCase();
      const cmp = an.localeCompare(bn);
      return cmp !== 0 ? cmp : a.id - b.id;
    });
  }

  static sortByEndDate(contracts: Contract[]): Contract[] {
    return contracts.sort((a, b) => new Date(a.endDate).getTime() - new Date(b.endDate).getTime());
  }

  static filterByStatus(contracts: Contract[], status: ContractStatus): Contract[] {
    return contracts.filter(contract => contract.status === status);
  }

  static filterByManufacturer(contracts: Contract[], manufacturerId: number): Contract[] {
    return contracts.filter(contract => contract.manufacturerId === manufacturerId);
  }

  static filterExpiring(contracts: Contract[], daysThreshold: number = 30): Contract[] {
    return contracts.filter(contract => this.isExpiringSoon(contract, daysThreshold));
  }

  static searchByText(contracts: Contract[], searchTerm: string): Contract[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return contracts;

    return contracts.filter(contract =>
      (contract.name && contract.name.toLowerCase().includes(term)) ||
      (contract.title && contract.title.toLowerCase().includes(term)) ||
      (contract.internalNotes && contract.internalNotes.toLowerCase().includes(term)) ||
      (contract.manufacturerName && contract.manufacturerName.toLowerCase().includes(term))
    );
  }
}
