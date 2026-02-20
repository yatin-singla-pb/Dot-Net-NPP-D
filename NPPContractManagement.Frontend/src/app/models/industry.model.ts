export interface Industry {
  id: number;
  name: string;
  description?: string;
  status: IndustryStatus;
  isActive: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;
}

export enum IndustryStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Pending = 'Pending'
}

export interface CreateIndustryRequest {
  name: string;
  description?: string;
  status?: IndustryStatus;
}

export interface UpdateIndustryRequest {
  name?: string;
  description?: string;
  status?: IndustryStatus;
  isActive?: boolean;
}

export interface IndustrySearchResult {
  items: Industry[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

export interface IndustryStatistics {
  total: number;
  active: number;
  inactive: number;
  recentlyCreated: number;
}

export interface IndustryValidationResult {
  isValid: boolean;
  message?: string;
}

// Helper functions
export class IndustryHelper {
  static getStatusColor(status: IndustryStatus): string {
    switch (status) {
      case IndustryStatus.Active:
        return 'success';
      case IndustryStatus.Inactive:
        return 'danger';
      case IndustryStatus.Pending:
        return 'warning';
      default:
        return 'secondary';
    }
  }

  static getStatusIcon(status: IndustryStatus): string {
    switch (status) {
      case IndustryStatus.Active:
        return 'fa-check-circle';
      case IndustryStatus.Inactive:
        return 'fa-times-circle';
      case IndustryStatus.Pending:
        return 'fa-clock';
      default:
        return 'fa-question-circle';
    }
  }

  static formatStatus(status: IndustryStatus): string {
    return status.toString();
  }

  static isActive(industry: Industry): boolean {
    return industry.status === IndustryStatus.Active && industry.isActive;
  }

  static canEdit(industry: Industry): boolean {
    return true; // Add business logic here if needed
  }

  static canDelete(industry: Industry): boolean {
    return true; // Add business logic here if needed
  }

  static getDisplayName(industry: Industry): string {
    return industry.name;
  }

  static sortByName(industries: Industry[]): Industry[] {
    return industries.sort((a, b) => a.name.localeCompare(b.name));
  }

  static filterByStatus(industries: Industry[], status: IndustryStatus): Industry[] {
    return industries.filter(industry => industry.status === status);
  }

  static filterActive(industries: Industry[]): Industry[] {
    return industries.filter(industry => this.isActive(industry));
  }

  static searchByName(industries: Industry[], searchTerm: string): Industry[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return industries;

    return industries.filter(industry =>
      industry.name.toLowerCase().includes(term) ||
      (industry.description && industry.description.toLowerCase().includes(term))
    );
  }
}
