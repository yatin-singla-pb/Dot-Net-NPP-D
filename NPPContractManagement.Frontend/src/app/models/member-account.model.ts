export interface MemberAccount {
  id: number;
  memberNumber: string;
  facilityName: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;

  industryId: number;
  industryName?: string;
  w9: boolean;
  w9Date?: Date;
  taxId?: string;
  businessType?: string;

  status: MemberAccountStatus;
  isActive: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;

  // Extended fields
  lopDate?: Date;
  internalNotes?: string;
  clientGroupEnrollment?: number;
  salesforceAccountName: string;
  vmapNumber?: string;
  vmSupplierName?: string;
  vmSupplierSite?: string;
  payType?: PayType;
  payTypeName?: string;

  parentMemberAccountNumber?: string;
  entegraGPONumber?: string;
  clientGroupNumber?: string;
  entegraIdNumber?: string;
  auditDate?: Date;


  // Navigation properties
  customerAccountsCount?: number;
}

export enum MemberAccountStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Pending = 'Pending',
  Suspended = 'Suspended'
}

export enum PayType {
  ACH = 'ACH',
  Check = 'Check',
  Wire = 'Wire'
}


export interface CreateMemberAccountRequest {
  memberNumber: string;
  facilityName: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;

  industryId: number;
  w9?: boolean;
  w9Date?: Date;
  taxId?: string;
  businessType?: string;
  // Extended fields
  lopDate?: Date;
  internalNotes?: string;
  clientGroupEnrollment?: number;
  salesforceAccountName: string;
  vmapNumber?: string;
  vmSupplierName?: string;
  vmSupplierSite?: string;
  payType?: PayType | number;
  parentMemberAccountNumber?: string;
  entegraGPONumber?: string;
  clientGroupNumber?: string;
  entegraIdNumber?: string;
  status?: MemberAccountStatus | number;
  isActive?: boolean;
}

export interface UpdateMemberAccountRequest {
  memberNumber?: string;
  facilityName?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  phoneNumber?: string;

  industryId?: number;
  w9?: boolean;
  w9Date?: Date;
  taxId?: string;
  businessType?: string;
  // Extended fields
  lopDate?: Date;
  internalNotes?: string;
  clientGroupEnrollment?: number;
  salesforceAccountName?: string;
  vmapNumber?: string;
  vmSupplierName?: string;
  vmSupplierSite?: string;
  payType?: PayType | number;
  parentMemberAccountNumber?: string;
  entegraGPONumber?: string;
  clientGroupNumber?: string;
  entegraIdNumber?: string;
  auditDate?: Date;
  status?: MemberAccountStatus | number;
  isActive?: boolean;
}

export interface MemberAccountSearchResult {
  items: MemberAccount[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Helper functions
export class MemberAccountHelper {
  static getStatusColor(status: MemberAccountStatus): string {
    switch (status) {
      case MemberAccountStatus.Active:
        return 'success';
      case MemberAccountStatus.Inactive:
        return 'danger';
      case MemberAccountStatus.Pending:
        return 'warning';
      case MemberAccountStatus.Suspended:
        return 'secondary';
      default:
        return 'secondary';
    }
  }

  static getStatusIcon(status: MemberAccountStatus): string {
    switch (status) {
      case MemberAccountStatus.Active:
        return 'fa-check-circle';
      case MemberAccountStatus.Inactive:
        return 'fa-pause-circle';
      case MemberAccountStatus.Pending:
        return 'fa-clock';
      case MemberAccountStatus.Suspended:
        return 'fa-ban';
      default:
        return 'fa-question-circle';
    }
  }

  static formatAddress(memberAccount: MemberAccount): string {
    const parts = [
      memberAccount.address,
      memberAccount.city,
      memberAccount.state,
      memberAccount.zipCode
    ].filter(part => part && part.trim());

    return parts.join(', ');
  }

  static getDisplayName(memberAccount: MemberAccount): string {
    return `${memberAccount.memberNumber} - ${memberAccount.facilityName}`;
  }

  static getW9Status(memberAccount: MemberAccount): string {
    if (!memberAccount.w9) return 'Not Submitted';
    if (memberAccount.w9Date) {
      return `Submitted ${new Date(memberAccount.w9Date).toLocaleDateString()}`;
    }
    return 'Submitted';
  }

  static getW9StatusColor(memberAccount: MemberAccount): string {
    return memberAccount.w9 ? 'success' : 'warning';
  }

  static isActive(memberAccount: MemberAccount): boolean {
    return memberAccount.status === MemberAccountStatus.Active && memberAccount.isActive;
  }

  static isSuspended(memberAccount: MemberAccount): boolean {
    return memberAccount.status === MemberAccountStatus.Suspended;
  }

  static canEdit(memberAccount: MemberAccount): boolean {
    return memberAccount.status !== MemberAccountStatus.Suspended;
  }

  static canDelete(memberAccount: MemberAccount): boolean {
    return memberAccount.status === MemberAccountStatus.Pending;
  }

  static canSuspend(memberAccount: MemberAccount): boolean {
    return memberAccount.status === MemberAccountStatus.Active;
  }

  static canActivate(memberAccount: MemberAccount): boolean {
    return memberAccount.status === MemberAccountStatus.Suspended ||
           memberAccount.status === MemberAccountStatus.Pending;
  }

  static sortByMemberNumber(memberAccounts: MemberAccount[]): MemberAccount[] {
    return memberAccounts.sort((a, b) => a.memberNumber.localeCompare(b.memberNumber));
  }

  static sortByFacilityName(memberAccounts: MemberAccount[]): MemberAccount[] {
    return memberAccounts.sort((a, b) => a.facilityName.localeCompare(b.facilityName));
  }

  static filterByStatus(memberAccounts: MemberAccount[], status: MemberAccountStatus): MemberAccount[] {
    return memberAccounts.filter(account => account.status === status);
  }

  static filterByIndustry(memberAccounts: MemberAccount[], industryId: number): MemberAccount[] {
    return memberAccounts.filter(account => account.industryId === industryId);
  }

  static filterByW9Status(memberAccounts: MemberAccount[], hasW9: boolean): MemberAccount[] {
    return memberAccounts.filter(account => account.w9 === hasW9);
  }

  static searchByText(memberAccounts: MemberAccount[], searchTerm: string): MemberAccount[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return memberAccounts;

    return memberAccounts.filter(account =>
      account.memberNumber.toLowerCase().includes(term) ||
      account.facilityName.toLowerCase().includes(term) ||

      (account.industryName && account.industryName.toLowerCase().includes(term)) ||
      (account.city && account.city.toLowerCase().includes(term)) ||
      (account.state && account.state.toLowerCase().includes(term)) ||
      (account.salesforceAccountName && account.salesforceAccountName.toLowerCase().includes(term)) ||
      (account.vmSupplierName && account.vmSupplierName.toLowerCase().includes(term)) ||
      (account.parentMemberAccountNumber && account.parentMemberAccountNumber.toLowerCase().includes(term)) ||
      (account.entegraIdNumber && account.entegraIdNumber.toLowerCase().includes(term)) ||
      (account.vmapNumber && account.vmapNumber.toLowerCase().includes(term)) ||
      (account.vmSupplierSite && account.vmSupplierSite.toLowerCase().includes(term)) ||
      (account.entegraGPONumber && account.entegraGPONumber.toLowerCase().includes(term)) ||
      (account.clientGroupNumber && account.clientGroupNumber.toLowerCase().includes(term))
    );
  }
}
