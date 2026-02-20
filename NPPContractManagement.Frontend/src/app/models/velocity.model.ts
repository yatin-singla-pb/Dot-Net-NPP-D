export interface VelocityJob {
  id: number;
  jobId: string;
  status: string;
  fileName?: string;
  sftpFileUrl?: string;
  totalRows: number;
  processedRows: number;
  successRows: number;
  failedRows: number;
  createdAt: Date;
  startedAt?: Date;
  completedAt?: Date;
  createdBy?: string;
  errorMessage?: string;
  detailsUrl?: string;
}

export interface VelocityJobDetail extends VelocityJob {
  rows: VelocityJobRow[];
}

export interface VelocityJobRow {
  id: number;
  rowIndex: number;
  status: string;
  errorMessage?: string;
  rawData?: string;
  velocityShipmentId?: number;
  processedAt: Date;
}

export interface VelocityIngestResponse {
  jobId: string;
  status: string;
  detailsUrl?: string;
  message?: string;
}

export interface VelocityShipment {
  id: number;
  distributorId: number;
  distributorName?: string;
  shipmentId: string;
  sku: string;
  quantity: number;
  shippedAt: Date;
  origin?: string;
  destination?: string;
  velocityJobId?: number;
  rowIndex?: number;
  createdAt: Date;
  createdBy?: string;
}

