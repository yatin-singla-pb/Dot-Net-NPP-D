export interface ContractVersionPrice {
  id: number;
  productId: number;
  productName?: string;
  price?: number | null;
  priceType?: string | null;
  uom?: string | null;
  tier?: string | null;
  effectiveFrom?: Date | null;
  effectiveTo?: Date | null;

  // Extended detailed fields from ContractVersionPrice table
  allowance?: number | null;
  commercialDelPrice?: number | null;
  commercialFobPrice?: number | null;
  commodityDelPrice?: number | null;
  commodityFobPrice?: number | null;
  estimatedQty?: number | null;
  billbacksAllowed?: boolean;
  pua?: number | null;
  ffsPrice?: number | null;
  noiPrice?: number | null;
  ptv?: number | null;
  internalNotes?: string | null;
}

export interface ContractVersion {
  id: number;
  contractId: number;
  versionNumber: number;
  name: string;
  foreignContractId?: string | null;
  sendToPerformance: boolean;
  isSuspended: boolean;
  suspendedDate?: Date | null;
  internalNotes?: string | null;
  // Manufacturer/Entegra metadata snapshot
  manufacturerReferenceNumber?: string | null;
  manufacturerBillbackName?: string | null;
  manufacturerTermsAndConditions?: string | null;
  manufacturerNotes?: string | null;
  contactPerson?: string | null;
  entegraContractType?: string | null;
  entegraVdaProgram?: string | null;
  startDate: Date;
  endDate: Date;
  createdDate: Date;
  createdBy?: string | null;
  prices: ContractVersionPrice[];
}

export interface CreateContractVersionPriceRequest {
  productId: number;
  price?: number | null;
  priceType?: string | null;
  uom?: string | null;
  tier?: string | null;
  effectiveFrom?: Date | null;
  effectiveTo?: Date | null;
  // Extended detailed fields to support clone Save & Update
  allowance?: number | null;
  commercialDelPrice?: number | null;
  commercialFobPrice?: number | null;
  commodityDelPrice?: number | null;
  commodityFobPrice?: number | null;
  estimatedQty?: number | null;
  billbacksAllowed?: boolean;
  pua?: number | null;
  ffsPrice?: number | null;
  noiPrice?: number | null;
  ptv?: number | null;
  internalNotes?: string | null;
}

export interface CreateContractVersionRequest {
  name: string;
  foreignContractId?: string | null;
  sendToPerformance?: boolean;
  internalNotes?: string | null;
  // Manufacturer/Entegra metadata
  manufacturerReferenceNumber?: string | null;
  manufacturerBillbackName?: string | null;
  manufacturerTermsAndConditions?: string | null;
  manufacturerNotes?: string | null;
  contactPerson?: string | null;
  entegraContractType?: string | null;
  entegraVdaProgram?: string | null;
  startDate: Date;
  endDate: Date;
  prices: CreateContractVersionPriceRequest[];
  sourceVersionId?: number;
}

