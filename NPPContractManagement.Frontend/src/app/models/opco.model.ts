export interface OpCo {
  id: number;
  name: string;
  remoteReferenceCode: string;
  distributorId: number;
  distributorName?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  contactPerson?: string;
  internalNotes?: string;
  status: OpCoStatus;
  isActive: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;
}

export enum OpCoStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Pending = 'Pending'
}

export interface CreateOpCoRequest {
  name: string;
  remoteReferenceCode: string;
  distributorId: number;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  contactPerson?: string;
  internalNotes?: string;
  status?: OpCoStatus;
}

export interface UpdateOpCoRequest {
  name?: string;
  remoteReferenceCode?: string;
  distributorId?: number;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  contactPerson?: string;
  internalNotes?: string;
  status?: OpCoStatus;
  isActive?: boolean;
}

export interface OpCoSearchResult {
  items: OpCo[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Helper functions
export class OpCoHelper {
  static getStatusColor(status: OpCoStatus): string {
    switch (status) {
      case OpCoStatus.Active:
        return 'success';
      case OpCoStatus.Inactive:
        return 'danger';
      case OpCoStatus.Pending:
        return 'warning';
      default:
        return 'secondary';
    }
  }

  static getStatusIcon(status: OpCoStatus): string {
    switch (status) {
      case OpCoStatus.Active:
        return 'fa-check-circle';
      case OpCoStatus.Inactive:
        return 'fa-times-circle';
      case OpCoStatus.Pending:
        return 'fa-clock';
      default:
        return 'fa-question-circle';
    }
  }

  static formatAddress(opco: OpCo): string {
    const parts = [
      opco.address,
      opco.city,
      opco.state,
      opco.zipCode
    ].filter(part => part && part.trim());
    
    return parts.join(', ');
  }

  static getDisplayName(opco: OpCo): string {
    return `${opco.name} (${opco.remoteReferenceCode})`;
  }

  static isActive(opco: OpCo): boolean {
    return opco.status === OpCoStatus.Active && opco.isActive;
  }

  static canEdit(opco: OpCo): boolean {
    return true; // Add business logic here if needed
  }

  static canDelete(opco: OpCo): boolean {
    return true; // Add business logic here if needed
  }

  static sortByName(opcos: OpCo[]): OpCo[] {
    return opcos.sort((a, b) => a.name.localeCompare(b.name));
  }

  static filterByStatus(opcos: OpCo[], status: OpCoStatus): OpCo[] {
    return opcos.filter(opco => opco.status === status);
  }

  static filterByDistributor(opcos: OpCo[], distributorId: number): OpCo[] {
    return opcos.filter(opco => opco.distributorId === distributorId);
  }

  static searchByName(opcos: OpCo[], searchTerm: string): OpCo[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return opcos;

    return opcos.filter(opco =>
      opco.name.toLowerCase().includes(term) ||
      opco.remoteReferenceCode.toLowerCase().includes(term) ||
      (opco.distributorName && opco.distributorName.toLowerCase().includes(term)) ||
      (opco.city && opco.city.toLowerCase().includes(term)) ||
      (opco.state && opco.state.toLowerCase().includes(term))
    );
  }
}
