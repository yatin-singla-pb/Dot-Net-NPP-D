export interface Manufacturer {
  id: number;
  name: string;
  aka?: string;
  description?: string;
  status: ManufacturerStatus;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  website?: string;
  primaryBrokerId?: number;
  primaryBrokerName?: string;
  contactPerson?: string;
  contactPersonId?: number;
  contactPersonName?: string;
  isActive: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;
}

export enum ManufacturerStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Pending = 'Pending'
}

export interface CreateManufacturerRequest {
  name: string;
  aka?: string;
  description?: string;
  status?: ManufacturerStatus;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  website?: string;
  primaryBrokerId?: number;
  contactPerson?: string;
  contactPersonId?: number;
}

export interface UpdateManufacturerRequest {
  name?: string;
  aka?: string;
  description?: string;
  status?: ManufacturerStatus;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  website?: string;
  primaryBrokerId?: number;
  contactPerson?: string;
  contactPersonId?: number;
  isActive?: boolean;
}

export interface ManufacturerSearchResult {
  items: Manufacturer[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Helper functions
export class ManufacturerHelper {
  static getStatusColor(status: ManufacturerStatus): string {
    switch (status) {
      case ManufacturerStatus.Active:
        return 'success';
      case ManufacturerStatus.Inactive:
        return 'danger';
      case ManufacturerStatus.Pending:
        return 'warning';
      default:
        return 'secondary';
    }
  }

  static getStatusIcon(status: ManufacturerStatus): string {
    switch (status) {
      case ManufacturerStatus.Active:
        return 'fa-check-circle';
      case ManufacturerStatus.Inactive:
        return 'fa-times-circle';
      case ManufacturerStatus.Pending:
        return 'fa-clock';
      default:
        return 'fa-question-circle';
    }
  }

  static formatAddress(manufacturer: Manufacturer): string {
    const parts = [
      manufacturer.address,
      manufacturer.city,
      manufacturer.state,
      manufacturer.zipCode
    ].filter(part => part && part.trim());
    
    return parts.join(', ');
  }

  static getDisplayName(manufacturer: Manufacturer): string {
    return manufacturer.aka ? `${manufacturer.name} (${manufacturer.aka})` : manufacturer.name;
  }

  static isActive(manufacturer: Manufacturer): boolean {
    return manufacturer.status === ManufacturerStatus.Active && manufacturer.isActive;
  }

  static canEdit(manufacturer: Manufacturer): boolean {
    return true; // Add business logic here if needed
  }

  static canDelete(manufacturer: Manufacturer): boolean {
    return true; // Add business logic here if needed
  }

  static sortByName(manufacturers: Manufacturer[]): Manufacturer[] {
    return manufacturers.sort((a, b) => a.name.localeCompare(b.name));
  }

  static filterByStatus(manufacturers: Manufacturer[], status: ManufacturerStatus): Manufacturer[] {
    return manufacturers.filter(manufacturer => manufacturer.status === status);
  }

  static searchByText(manufacturers: Manufacturer[], searchTerm: string): Manufacturer[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return manufacturers;

    return manufacturers.filter(manufacturer =>
      manufacturer.name.toLowerCase().includes(term) ||
      (manufacturer.aka && manufacturer.aka.toLowerCase().includes(term)) ||
      (manufacturer.primaryBrokerName && manufacturer.primaryBrokerName.toLowerCase().includes(term)) ||
      (manufacturer.city && manufacturer.city.toLowerCase().includes(term)) ||
      (manufacturer.state && manufacturer.state.toLowerCase().includes(term))
    );
  }
}
