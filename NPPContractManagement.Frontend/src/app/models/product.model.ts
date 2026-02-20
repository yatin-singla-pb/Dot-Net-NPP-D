export interface Product {
  id: number;
  name?: string;
  manufacturerProductCode: string;
  gtin?: string;
  upc?: string;
  description: string;
  packSize?: string;
  manufacturerId: number;
  manufacturerName?: string;
  status: ProductStatus;
  category?: string;
  subCategory?: string;
  brand?: string;
  tertiaryCategory?: string;
  alwaysList?: boolean;
  notes?: string;
  wholesalePrice?: number;
  retailPrice?: number;
  weight?: number;
  dimensions?: string;
  isActive: boolean;
  createdDate: Date;
  modifiedDate?: Date;
  createdBy?: string;
  modifiedBy?: string;
}

export enum ProductStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Discontinued = 'Discontinued',
  Pending = 'Pending'
}

export interface CreateProductRequest {
  name: string;
  manufacturerProductCode: string;
  gtin?: string;
  upc?: string;
  description: string;
  packSize?: string;
  manufacturerId: number;
  status?: ProductStatus;
  category?: string;
  subCategory?: string;
  brand?: string;
  tertiaryCategory?: string;
  alwaysList?: boolean;
  notes?: string;

  wholesalePrice?: number;
  retailPrice?: number;
  weight?: number;
  dimensions?: string;
}

export interface UpdateProductRequest {
  name?: string;
  manufacturerProductCode?: string;
  gtin?: string;
  upc?: string;
  description?: string;
  brand?: string;
  tertiaryCategory?: string;
  alwaysList?: boolean;
  notes?: string;

  packSize?: string;
  manufacturerId?: number;
  status?: ProductStatus;
  category?: string;
  subCategory?: string;
  wholesalePrice?: number;
  retailPrice?: number;
  weight?: number;
  dimensions?: string;
}

export interface ProductSearchResult {
  items: Product[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Helper functions
export class ProductHelper {
  static getStatusColor(status: ProductStatus): string {
    switch (status) {
      case ProductStatus.Active:
        return 'success';
      case ProductStatus.Inactive:
        return 'secondary';
      case ProductStatus.Discontinued:
        return 'danger';
      case ProductStatus.Pending:
        return 'warning';
      default:
        return 'secondary';
    }
  }

  static getStatusIcon(status: ProductStatus): string {
    switch (status) {
      case ProductStatus.Active:
        return 'fa-check-circle';
      case ProductStatus.Inactive:
        return 'fa-pause-circle';
      case ProductStatus.Discontinued:
        return 'fa-times-circle';
      case ProductStatus.Pending:
        return 'fa-clock';
      default:
        return 'fa-question-circle';
    }
  }

  static formatCurrency(amount?: number): string {
    if (!amount) return 'N/A';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  static formatWeight(weight?: number): string {
    if (!weight) return 'N/A';
    return `${weight} lbs`;
  }

  static getDisplayName(product: Product): string {
    return `${product.manufacturerProductCode} - ${product.description}`;
  }

  static getShortDescription(product: Product, maxLength: number = 50): string {
    if (product.description.length <= maxLength) {
      return product.description;
    }
    return product.description.substring(0, maxLength) + '...';
  }

  static isActive(product: Product): boolean {
    return product.status === ProductStatus.Active && product.isActive;
  }

  static isDiscontinued(product: Product): boolean {
    return product.status === ProductStatus.Discontinued;
  }

  static canEdit(product: Product): boolean {
    return product.status !== ProductStatus.Discontinued;
  }

  static canDelete(product: Product): boolean {
    return product.status === ProductStatus.Pending;
  }

  static sortByCode(products: Product[]): Product[] {
    return products.sort((a, b) => a.manufacturerProductCode.localeCompare(b.manufacturerProductCode));
  }

  static sortByDescription(products: Product[]): Product[] {
    return products.sort((a, b) => a.description.localeCompare(b.description));
  }

  static filterByStatus(products: Product[], status: ProductStatus): Product[] {
    return products.filter(product => product.status === status);
  }

  static filterByManufacturer(products: Product[], manufacturerId: number): Product[] {
    return products.filter(product => product.manufacturerId === manufacturerId);
  }

  static filterByCategory(products: Product[], category: string): Product[] {
    return products.filter(product => product.category === category);
  }

  static searchByText(products: Product[], searchTerm: string): Product[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return products;

    return products.filter(product =>
      product.manufacturerProductCode.toLowerCase().includes(term) ||
      product.description.toLowerCase().includes(term) ||
      (product.gtin && product.gtin.toLowerCase().includes(term)) ||
      (product.upc && product.upc.toLowerCase().includes(term)) ||
      (product.manufacturerName && product.manufacturerName.toLowerCase().includes(term)) ||
      (product.category && product.category.toLowerCase().includes(term))
    );
  }
}
