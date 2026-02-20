export type CustomerAssociation = 'CSN' | 'Combined' | 'RCDM' | 'SEMUPC';

export interface CustomerAccount {
  id: number;
  memberAccountId: number;
  memberAccountName?: string;
  distributorId: number;
  distributorName?: string;
  opCoId: number;
  opCoName?: string;
  customerName: string;
  customerAccountNumber: string;
  status: CustomerAccountStatus | number;
  statusName?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  // New fields
  salesRep?: string;
  dso?: string;
  startDate?: string;
  endDate?: string;
  tracsAccess: boolean;
  markup?: string;
  auditDate?: string;
  toEntegra?: boolean;
  dateToEntegra?: string;
  combinedUniqueID?: string;
  internalNotes?: string;
  association?: CustomerAssociation;
  associationName?: string;
  isActive: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;
}

export enum CustomerAccountStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Pending = 'Pending',
  Suspended = 'Suspended',
  Closed = 'Closed',
  Billing = 'Billing',
  Test = 'Test',
  Prospect = 'Prospect',
  Rebate = 'Rebate',
  USDA = 'USDA'
}

export interface CreateCustomerAccountRequest {
  memberAccountId: number;
  distributorId: number;
  opCoId: number | null;
  customerName: string;
  customerAccountNumber: string;
  status?: CustomerAccountStatus;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  // New fields
  salesRep?: string;
  dso?: string;
  startDate?: string;
  endDate?: string;
  tracsAccess: boolean;
  markup?: string;
  auditDate?: string;
  toEntegra?: boolean;
  dateToEntegra?: string;
  combinedUniqueID?: string;
  internalNotes?: string;
  association?: number | CustomerAssociation;
}

export interface UpdateCustomerAccountRequest {
  memberAccountId?: number;
  distributorId?: number;
  opCoId?: number | null;
  customerName?: string;
  customerAccountNumber?: string;
  status?: CustomerAccountStatus;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;
  email?: string;
  // New fields
  salesRep?: string;
  dso?: string;
  startDate?: string;
  endDate?: string;
  tracsAccess?: boolean;
  markup?: string;
  auditDate?: string;
  toEntegra?: boolean;
  dateToEntegra?: string;
  combinedUniqueID?: string;
  internalNotes?: string;
  association?: number | CustomerAssociation;
  isActive?: boolean;
}

export interface CustomerAccountSearchResult {
  items: CustomerAccount[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Helper functions
export class CustomerAccountHelper {
  static getStatusColor(status: CustomerAccountStatus): string {
    switch (status) {
      case CustomerAccountStatus.Active:
        return 'success';
      case CustomerAccountStatus.Inactive:
        return 'secondary';
      case CustomerAccountStatus.Pending:
        return 'warning';
      case CustomerAccountStatus.Suspended:
        return 'danger';
      case CustomerAccountStatus.Closed:
        return 'dark';
      default:
        return 'secondary';
    }
  }

  static getStatusIcon(status: CustomerAccountStatus): string {
    switch (status) {
      case CustomerAccountStatus.Active:
        return 'fa-check-circle';
      case CustomerAccountStatus.Inactive:
        return 'fa-pause-circle';
      case CustomerAccountStatus.Pending:
        return 'fa-clock';
      case CustomerAccountStatus.Suspended:
        return 'fa-ban';
      case CustomerAccountStatus.Closed:
        return 'fa-times-circle';
      default:
        return 'fa-question-circle';
    }
  }

  static formatCurrency(amount?: number): string {
    if (!amount) return '$0.00';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  static formatAddress(customerAccount: CustomerAccount): string {
    const parts = [
      customerAccount.address,
      customerAccount.city,
      customerAccount.state,
      customerAccount.zipCode
    ].filter(part => part && (part as string).trim());

    return parts.join(', ');
  }

  static getDisplayName(customerAccount: CustomerAccount): string {
    return `${customerAccount.customerAccountNumber} - ${customerAccount.customerName}`;
  }


  static isActive(customerAccount: CustomerAccount): boolean {
    return customerAccount.status === CustomerAccountStatus.Active && customerAccount.isActive;
  }

  static isSuspended(customerAccount: CustomerAccount): boolean {
    return customerAccount.status === CustomerAccountStatus.Suspended;
  }

  static isClosed(customerAccount: CustomerAccount): boolean {
    return customerAccount.status === CustomerAccountStatus.Closed;
  }

  static canEdit(customerAccount: CustomerAccount): boolean {
    return customerAccount.status !== CustomerAccountStatus.Closed;
  }

  static canDelete(customerAccount: CustomerAccount): boolean {
    return customerAccount.status === CustomerAccountStatus.Pending;
  }

  static canSuspend(customerAccount: CustomerAccount): boolean {
    return customerAccount.status === CustomerAccountStatus.Active;
  }

  static canActivate(customerAccount: CustomerAccount): boolean {
    return customerAccount.status === CustomerAccountStatus.Suspended ||
           customerAccount.status === CustomerAccountStatus.Pending;
  }

  static canClose(customerAccount: CustomerAccount): boolean {
    return customerAccount.status !== CustomerAccountStatus.Closed;
  }

  static sortByAccountNumber(customerAccounts: CustomerAccount[]): CustomerAccount[] {
    return customerAccounts.sort((a, b) => a.customerAccountNumber.localeCompare(b.customerAccountNumber));
  }

  static sortByCustomerName(customerAccounts: CustomerAccount[]): CustomerAccount[] {
    return customerAccounts.sort((a, b) => a.customerName.localeCompare(b.customerName));
  }

  static filterByStatus(customerAccounts: CustomerAccount[], status: CustomerAccountStatus): CustomerAccount[] {
    return customerAccounts.filter(account => account.status === status);
  }

  static filterByMemberAccount(customerAccounts: CustomerAccount[], memberAccountId: number): CustomerAccount[] {
    return customerAccounts.filter(account => account.memberAccountId === memberAccountId);
  }

  static filterByDistributor(customerAccounts: CustomerAccount[], distributorId: number): CustomerAccount[] {
    return customerAccounts.filter(account => account.distributorId === distributorId);
  }

  static filterByOpCo(customerAccounts: CustomerAccount[], opCoId: number): CustomerAccount[] {
    return customerAccounts.filter(account => account.opCoId === opCoId);
  }

  static searchByText(customerAccounts: CustomerAccount[], searchTerm: string): CustomerAccount[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return customerAccounts;

    return customerAccounts.filter(account =>
      account.customerAccountNumber.toLowerCase().includes(term) ||
      account.customerName.toLowerCase().includes(term) ||
      (account.memberAccountName && (account.memberAccountName as string).toLowerCase().includes(term)) ||
      (account.distributorName && (account.distributorName as string).toLowerCase().includes(term)) ||
      (account.opCoName && (account.opCoName as string).toLowerCase().includes(term)) ||
      (account.city && (account.city as string).toLowerCase().includes(term)) ||
      (account.state && (account.state as string).toLowerCase().includes(term))
    );
  }
}
