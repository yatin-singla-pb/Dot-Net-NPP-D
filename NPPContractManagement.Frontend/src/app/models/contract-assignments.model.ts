export interface ContractDistributorAssignment {
  id: number;
  contractId: number;
  distributorId: number;
  currentVersionNumber: number;
  assignedBy?: string | null;
  assignedDate?: string | Date | null;
}

export interface CreateContractDistributorAssignmentRequest {
  distributorId: number;
  currentVersionNumber: number;
  assignedBy?: string | null;
  assignedDate?: string | Date | null;
}

export interface ContractManufacturerAssignment {
  id: number;
  contractId: number;
  manufacturerId: number;
  currentVersionNumber: number;
  assignedBy?: string | null;
  assignedDate?: string | Date | null;
}

export interface CreateContractManufacturerAssignmentRequest {
  manufacturerId: number;
  currentVersionNumber: number;
  assignedBy?: string | null;
  assignedDate?: string | Date | null;
}

export interface ContractOpCoAssignment {
  id: number;
  contractId: number;
  opCoId: number;
  currentVersionNumber: number;
  assignedBy?: string | null;
  assignedDate?: string | Date | null;
}

export interface CreateContractOpCoAssignmentRequest {
  opCoId: number;
  currentVersionNumber: number;
  assignedBy?: string | null;
  assignedDate?: string | Date | null;
}

export interface ContractIndustryAssignment {
  id: number;
  contractId: number;
  industryId: number;
  currentVersionNumber: number;
  assignedBy?: string | null;
  assignedDate?: string | Date | null;
}

export interface CreateContractIndustryAssignmentRequest {
  industryId: number;
  currentVersionNumber: number;
  assignedBy?: string | null;
  assignedDate?: string | Date | null;
}

