export interface BulkRenewalRequest {
  contractIds: number[];
  pricingAdjustment?: PricingAdjustment;
  proposalDueDate?: Date;
  additionalProductIds?: number[];
  createdBy?: string;
}

export interface PricingAdjustment {
  percentageChange: number;
  minimumQuantityThreshold?: number;
  applyToAllProducts: boolean;
}

export interface BulkRenewalResponse {
  totalContracts: number;
  successfulProposals: number;
  failedProposals: number;
  createdProposalIds: number[];
  results: ContractRenewalResult[];
  success: boolean;
  message: string;
}

export interface ContractRenewalResult {
  contractId: number;
  contractNumber?: string;
  success: boolean;
  proposalId?: number;
  errorMessage?: string;
  productCount: number;
  additionalProductCount: number;
}

export interface ContractRenewalSummary {
  id: number;
  contractNumber?: string;
  manufacturerName?: string;
  distributorName?: string;
  startDate: Date;
  endDate: Date;
  activeProductCount: number;
  discontinuedProductCount: number;
  canRenew: boolean;
  renewalBlockReason?: string;
}

