/**
 * Request for Contract Over Term Report
 */
export interface ContractOverTermReportRequest {
  pointInTime: string; // ISO date string
  maxTermsBack: number;
  contractNumber?: string;
  manufacturerId?: number;
  productId?: number;
  opCoId?: number;
  page: number;
  pageSize: number;
}

/**
 * Response for Contract Over Term Report
 */
export interface ContractOverTermReportResponse {
  rows: ContractOverTermReportRow[];
  totalRows: number;
  maxTermsBack: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Single row in the Contract Over Term Report
 */
export interface ContractOverTermReportRow {
  // Current Term
  contractNumber: string;
  manufacturer: string;
  startDate?: string;
  endDate?: string;
  opCos: string; // Comma-separated
  productCode: string;
  productName: string;
  pricing?: number;
  estimatedVolume?: number;
  actualVolume?: number;
  industry?: string;
  priceType?: string;

  // Previous Terms
  previousTerms: PreviousTermData[];
}

/**
 * Data for a previous contract term
 */
export interface PreviousTermData {
  termNumber: number; // 1, 2, 3, etc.
  startDate?: string;
  endDate?: string;
  pricing?: number;
  estimatedVolume?: number;
  actualVolume?: number;
}

