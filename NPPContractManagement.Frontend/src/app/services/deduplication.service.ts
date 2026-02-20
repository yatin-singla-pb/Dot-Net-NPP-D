import { Injectable } from '@angular/core';

export type DeduplicationStrategy = 'collapse_with_metadata' | 'sum_quantities' | 'keep_all';

export interface DeduplicationConfig {
  strategy: DeduplicationStrategy;
  preserveFields?: string[];
  metadataField?: string;
}

export interface ProductLine {
  productId: number;
  priceTypeId?: number | null;
  proposedPrice?: number | null;
  quantity?: number | null;
  packingList?: string | null;
  productProposalStatusId?: number | null;
  metaJson?: string | null;
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class DeduplicationService {
  private defaultConfig: DeduplicationConfig = {
    strategy: 'collapse_with_metadata',
    preserveFields: ['productId', 'priceTypeId', 'proposedPrice', 'quantity', 'productProposalStatusId'],
    metadataField: 'metaJson'
  };

  constructor() {}

  /**
   * Deduplicate product lines based on the specified strategy
   */
  deduplicateProducts(products: ProductLine[], config?: Partial<DeduplicationConfig>): ProductLine[] {
    if (!products || products.length === 0) {
      return [];
    }

    const finalConfig = { ...this.defaultConfig, ...config };

    switch (finalConfig.strategy) {
      case 'collapse_with_metadata':
        return this.collapseWithMetadata(products, finalConfig);
      case 'sum_quantities':
        return this.sumQuantities(products, finalConfig);
      case 'keep_all':
        return [...products]; // No deduplication
      default:
        console.warn(`Unknown deduplication strategy: ${finalConfig.strategy}. Using default.`);
        return this.collapseWithMetadata(products, finalConfig);
    }
  }

  /**
   * Collapse duplicate products and store original packing lists in metadata
   */
  private collapseWithMetadata(products: ProductLine[], config: DeduplicationConfig): ProductLine[] {
    const groupMap = new Map<string, ProductLine[]>();

    // Group products by key fields (excluding packingList and metaJson)
    products.forEach(product => {
      const key = this.generateGroupKey(product, config.preserveFields || []);
      if (!groupMap.has(key)) {
        groupMap.set(key, []);
      }
      groupMap.get(key)!.push(product);
    });

    const result: ProductLine[] = [];

    groupMap.forEach(group => {
      if (group.length === 1) {
        // No duplicates, keep as is
        result.push({ ...group[0] });
      } else {
        // Multiple items, collapse and preserve packing lists
        const representative = { ...group[0] };
        const packingLists = group
          .map(item => item.packingList)
          .filter(pl => pl && pl.trim())
          .filter((pl, index, arr) => arr.indexOf(pl) === index); // Remove duplicates

        if (packingLists.length > 0) {
          const metadata = {
            packing_lists: packingLists,
            original_count: group.length,
            collapsed_at: new Date().toISOString()
          };

          representative[config.metadataField || 'metaJson'] = JSON.stringify(metadata);
        }

        // Use the first non-null packing list as the primary one
        representative.packingList = packingLists[0] || null;

        result.push(representative);
      }
    });

    return result;
  }

  /**
   * Sum quantities for duplicate products
   */
  private sumQuantities(products: ProductLine[], config: DeduplicationConfig): ProductLine[] {
    const groupMap = new Map<string, ProductLine[]>();

    // Group products by key fields (excluding quantity, packingList, and metaJson)
    const keyFields = (config.preserveFields || []).filter(field => field !== 'quantity');
    
    products.forEach(product => {
      const key = this.generateGroupKey(product, keyFields);
      if (!groupMap.has(key)) {
        groupMap.set(key, []);
      }
      groupMap.get(key)!.push(product);
    });

    const result: ProductLine[] = [];

    groupMap.forEach(group => {
      if (group.length === 1) {
        // No duplicates, keep as is
        result.push({ ...group[0] });
      } else {
        // Multiple items, sum quantities
        const representative = { ...group[0] };
        const totalQuantity = group.reduce((sum, item) => sum + (item.quantity || 0), 0);
        
        representative.quantity = totalQuantity;

        // Combine packing lists
        const packingLists = group
          .map(item => item.packingList)
          .filter(pl => pl && pl.trim())
          .filter((pl, index, arr) => arr.indexOf(pl) === index);

        representative.packingList = packingLists.join('; ') || null;

        // Store metadata about the summation
        const metadata = {
          summed_quantities: group.map(item => item.quantity || 0),
          original_count: group.length,
          summed_at: new Date().toISOString()
        };

        representative[config.metadataField || 'metaJson'] = JSON.stringify(metadata);

        result.push(representative);
      }
    });

    return result;
  }

  /**
   * Generate a grouping key for products based on specified fields
   */
  private generateGroupKey(product: ProductLine, fields: string[]): string {
    return fields
      .map(field => {
        const value = product[field];
        return value !== null && value !== undefined ? String(value) : 'null';
      })
      .join('|');
  }

  /**
   * Extract original packing lists from metadata
   */
  extractPackingLists(product: ProductLine, metadataField: string = 'metaJson'): string[] {
    try {
      const metaJson = product[metadataField];
      if (typeof metaJson === 'string') {
        const metadata = JSON.parse(metaJson);
        return metadata.packing_lists || [];
      }
    } catch (error) {
      console.warn('Failed to parse metadata JSON:', error);
    }
    
    // Fallback to current packing list
    return product.packingList ? [product.packingList] : [];
  }

  /**
   * Check if a product line was created through deduplication
   */
  isDeduplicated(product: ProductLine, metadataField: string = 'metaJson'): boolean {
    try {
      const metaJson = product[metadataField];
      if (typeof metaJson === 'string') {
        const metadata = JSON.parse(metaJson);
        return !!(metadata.packing_lists || metadata.summed_quantities);
      }
    } catch (error) {
      // Not a valid metadata JSON
    }
    return false;
  }

  /**
   * Get deduplication statistics for a product line
   */
  getDeduplicationStats(product: ProductLine, metadataField: string = 'metaJson'): any {
    try {
      const metaJson = product[metadataField];
      if (typeof metaJson === 'string') {
        const metadata = JSON.parse(metaJson);
        return {
          originalCount: metadata.original_count || 1,
          strategy: metadata.packing_lists ? 'collapse_with_metadata' : 
                   metadata.summed_quantities ? 'sum_quantities' : 'unknown',
          processedAt: metadata.collapsed_at || metadata.summed_at || null,
          packingLists: metadata.packing_lists || [],
          summedQuantities: metadata.summed_quantities || []
        };
      }
    } catch (error) {
      // Not a valid metadata JSON
    }
    
    return {
      originalCount: 1,
      strategy: 'none',
      processedAt: null,
      packingLists: [],
      summedQuantities: []
    };
  }

  /**
   * Validate deduplication configuration
   */
  validateConfig(config: DeduplicationConfig): boolean {
    const validStrategies: DeduplicationStrategy[] = ['collapse_with_metadata', 'sum_quantities', 'keep_all'];
    
    if (!validStrategies.includes(config.strategy)) {
      console.error(`Invalid deduplication strategy: ${config.strategy}`);
      return false;
    }

    if (config.preserveFields && !Array.isArray(config.preserveFields)) {
      console.error('preserveFields must be an array');
      return false;
    }

    if (config.metadataField && typeof config.metadataField !== 'string') {
      console.error('metadataField must be a string');
      return false;
    }

    return true;
  }

  /**
   * Get the current configuration from environment or localStorage
   */
  getCurrentConfig(): DeduplicationConfig {
    try {
      // Try to get from localStorage first
      const stored = localStorage.getItem('PROPOSAL_DEDUPE_CONFIG');
      if (stored) {
        const config = JSON.parse(stored);
        if (this.validateConfig(config)) {
          return { ...this.defaultConfig, ...config };
        }
      }
    } catch (error) {
      console.warn('Failed to load deduplication config from localStorage:', error);
    }

    // Fallback to environment variable or default
    const envStrategy = localStorage.getItem('PROPOSAL_DEDUPE_STRATEGY') as DeduplicationStrategy;
    if (envStrategy && ['collapse_with_metadata', 'sum_quantities', 'keep_all'].includes(envStrategy)) {
      return { ...this.defaultConfig, strategy: envStrategy };
    }

    return this.defaultConfig;
  }

  /**
   * Save configuration to localStorage
   */
  saveConfig(config: DeduplicationConfig): boolean {
    if (!this.validateConfig(config)) {
      return false;
    }

    try {
      localStorage.setItem('PROPOSAL_DEDUPE_CONFIG', JSON.stringify(config));
      return true;
    } catch (error) {
      console.error('Failed to save deduplication config:', error);
      return false;
    }
  }
}
