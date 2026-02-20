export interface DistributorProductCode {
  id: number;
  distributorId: number;
  distributorName?: string;
  productId: number;
  productName?: string;
  distributorCode: string;
  catchWeight: boolean;
  eBrand: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;
}

export interface CreateDistributorProductCodeRequest {
  distributorId: number;
  productId: number;
  distributorCode: string;
  catchWeight?: boolean;
  eBrand?: boolean;
}

export interface UpdateDistributorProductCodeRequest {
  distributorId?: number;
  productId?: number;
  distributorCode?: string;
  catchWeight?: boolean;
  eBrand?: boolean;
}

export interface DistributorProductCodeSearchResult {
  items: DistributorProductCode[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

