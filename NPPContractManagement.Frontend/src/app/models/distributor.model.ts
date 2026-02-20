export interface Distributor {
  id: number;
  name: string;
  status: DistributorStatus;
  description?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  website?: string;
  contactPerson?: string;
  receiveContractProposal: boolean;
  isActive: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;

  // Navigation properties
  opCosCount?: number;
}

export enum DistributorStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Pending = 'Pending'
}

export interface CreateDistributorRequest {
  name: string;
  description?: string;
  status?: DistributorStatus;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  website?: string;
  contactPerson?: string;
  receiveContractProposal?: boolean;
}

export interface UpdateDistributorRequest {
  name?: string;
  description?: string;
  status?: DistributorStatus;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  website?: string;
  contactPerson?: string;
  receiveContractProposal?: boolean;
  isActive?: boolean;
}

export interface DistributorSearchResult {
  items: Distributor[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Helper functions
export class DistributorHelper {
  static getStatusColor(status: DistributorStatus): string {
    switch (status) {
      case DistributorStatus.Active:
        return 'success';
      case DistributorStatus.Inactive:
        return 'danger';
      case DistributorStatus.Pending:
        return 'warning';
      default:
        return 'secondary';
    }
  }

  static getStatusIcon(status: DistributorStatus): string {
    switch (status) {
      case DistributorStatus.Active:
        return 'fa-check-circle';
      case DistributorStatus.Inactive:
        return 'fa-times-circle';
      case DistributorStatus.Pending:
        return 'fa-clock';
      default:
        return 'fa-question-circle';
    }
  }

  static formatAddress(distributor: Distributor): string {
    const parts = [
      distributor.address,
      distributor.city,
      distributor.state,
      distributor.zipCode
    ].filter(part => part && part.trim());
    
    return parts.join(', ');
  }

  static getDisplayName(distributor: Distributor): string {
    return distributor.name;
  }

  static isActive(distributor: Distributor): boolean {
    return distributor.status === DistributorStatus.Active && distributor.isActive;
  }

  static canEdit(distributor: Distributor): boolean {
    return true; // Add business logic here if needed
  }

  static canDelete(distributor: Distributor): boolean {
    return true; // Add business logic here if needed
  }

  static sortByName(distributors: Distributor[]): Distributor[] {
    return distributors.sort((a, b) => a.name.localeCompare(b.name));
  }

  static filterByStatus(distributors: Distributor[], status: DistributorStatus): Distributor[] {
    return distributors.filter(distributor => distributor.status === status);
  }

  static filterByContractProposal(distributors: Distributor[], receiveProposal: boolean): Distributor[] {
    return distributors.filter(distributor => distributor.receiveContractProposal === receiveProposal);
  }

  static searchByText(distributors: Distributor[], searchTerm: string): Distributor[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return distributors;

    return distributors.filter(distributor =>
      distributor.name.toLowerCase().includes(term) ||
      (distributor.contactPerson && distributor.contactPerson.toLowerCase().includes(term)) ||
      (distributor.city && distributor.city.toLowerCase().includes(term)) ||
      (distributor.state && distributor.state.toLowerCase().includes(term))
    );
  }
}
