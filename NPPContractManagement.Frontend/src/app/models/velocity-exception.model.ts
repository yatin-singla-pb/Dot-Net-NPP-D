export interface VelocityException {
  id: number;
  contractId?: number;
  contractName?: string;
  productId?: number;
  productName?: string;
  distributorId?: number;
  distributorName?: string;
  exceptionType: VelocityExceptionType;
  description: string;
  severity: ExceptionSeverity;
  detectedDate: Date;
  resolvedDate?: Date;
  status: ExceptionStatus;
  assignedTo?: string;
  notes?: string;
}

export enum VelocityExceptionType {
  MissingData = 'Missing Data',
  DataMismatch = 'Data Mismatch',
  InvalidFormat = 'Invalid Format',
  DuplicateEntry = 'Duplicate Entry',
  OutOfRange = 'Out of Range'
}

export enum ExceptionSeverity {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical'
}

export enum ExceptionStatus {
  Open = 'Open',
  InProgress = 'In Progress',
  Resolved = 'Resolved',
  Ignored = 'Ignored'
}

