import { TestBed } from '@angular/core/testing';
import { DeduplicationService, ProductLine, DeduplicationStrategy } from './deduplication.service';

describe('DeduplicationService', () => {
  let service: DeduplicationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeduplicationService);
    localStorage.clear();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('collapse_with_metadata strategy', () => {
    it('should keep single products unchanged', () => {
      const products: ProductLine[] = [
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: 'Case of 12'
        }
      ];

      const result = service.deduplicateProducts(products);
      expect(result.length).toBe(1);
      expect(result[0]).toEqual(products[0]);
    });

    it('should collapse duplicate products and preserve packing lists', () => {
      const products: ProductLine[] = [
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: 'Case of 12'
        },
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: 'Case of 24'
        }
      ];

      const result = service.deduplicateProducts(products);
      expect(result.length).toBe(1);
      expect(result[0].productId).toBe(1);
      expect(result[0].packingList).toBe('Case of 12');

      const metadata = JSON.parse(result[0].metaJson!);
      expect(metadata.packing_lists).toEqual(['Case of 12', 'Case of 24']);
      expect(metadata.original_count).toBe(2);
    });

    it('should handle products with different key fields separately', () => {
      const products: ProductLine[] = [
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: 'Case of 12'
        },
        {
          productId: 2,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: 'Case of 12'
        }
      ];

      const result = service.deduplicateProducts(products);
      expect(result.length).toBe(2);
      expect(result[0].productId).toBe(1);
      expect(result[1].productId).toBe(2);
    });
  });

  describe('sum_quantities strategy', () => {
    it('should sum quantities for duplicate products', () => {
      const products: ProductLine[] = [
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: 'Case of 12'
        },
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 3,
          packingList: 'Case of 24'
        }
      ];

      const result = service.deduplicateProducts(products, { strategy: 'sum_quantities' });
      expect(result.length).toBe(1);
      expect(result[0].quantity).toBe(8);
      expect(result[0].packingList).toBe('Case of 12; Case of 24');
      
      const metadata = JSON.parse(result[0].metaJson!);
      expect(metadata.summed_quantities).toEqual([5, 3]);
    });
  });

  describe('keep_all strategy', () => {
    it('should not deduplicate any products', () => {
      const products: ProductLine[] = [
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: 'Case of 12'
        },
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: 'Case of 24'
        }
      ];

      const result = service.deduplicateProducts(products, { strategy: 'keep_all' });
      expect(result.length).toBe(2);
      expect(result).toEqual(products);
    });
  });

  describe('utility methods', () => {
    it('should extract packing lists from metadata', () => {
      const product: ProductLine = {
        productId: 1,
        metaJson: JSON.stringify({
          packing_lists: ['Case of 12', 'Case of 24'],
          original_count: 2
        })
      };

      const packingLists = service.extractPackingLists(product);
      expect(packingLists).toEqual(['Case of 12', 'Case of 24']);
    });

    it('should detect deduplicated products', () => {
      const deduplicatedProduct: ProductLine = {
        productId: 1,
        metaJson: JSON.stringify({
          packing_lists: ['Case of 12', 'Case of 24'],
          original_count: 2
        })
      };

      const regularProduct: ProductLine = {
        productId: 2,
        packingList: 'Case of 12'
      };

      expect(service.isDeduplicated(deduplicatedProduct)).toBe(true);
      expect(service.isDeduplicated(regularProduct)).toBe(false);
    });

    it('should get deduplication statistics', () => {
      const product: ProductLine = {
        productId: 1,
        metaJson: JSON.stringify({
          packing_lists: ['Case of 12', 'Case of 24'],
          original_count: 2,
          collapsed_at: '2024-01-01T00:00:00.000Z'
        })
      };

      const stats = service.getDeduplicationStats(product);
      expect(stats.originalCount).toBe(2);
      expect(stats.strategy).toBe('collapse_with_metadata');
      expect(stats.packingLists).toEqual(['Case of 12', 'Case of 24']);
    });
  });

  describe('configuration management', () => {
    it('should validate valid configurations', () => {
      const validConfig = {
        strategy: 'collapse_with_metadata' as DeduplicationStrategy,
        preserveFields: ['productId', 'priceTypeId'],
        metadataField: 'metaJson'
      };

      expect(service.validateConfig(validConfig)).toBe(true);
    });

    it('should reject invalid configurations', () => {
      const invalidConfig = {
        strategy: 'invalid_strategy' as DeduplicationStrategy,
        preserveFields: ['productId'],
        metadataField: 'metaJson'
      };

      expect(service.validateConfig(invalidConfig)).toBe(false);
    });

    it('should save and load configuration from localStorage', () => {
      const config = {
        strategy: 'sum_quantities' as DeduplicationStrategy,
        preserveFields: ['productId', 'priceTypeId'],
        metadataField: 'customMeta'
      };

      expect(service.saveConfig(config)).toBe(true);
      
      const loadedConfig = service.getCurrentConfig();
      expect(loadedConfig.strategy).toBe('sum_quantities');
      expect(loadedConfig.metadataField).toBe('customMeta');
    });

    it('should fallback to default config when localStorage is invalid', () => {
      localStorage.setItem('PROPOSAL_DEDUPE_CONFIG', 'invalid json');
      
      const config = service.getCurrentConfig();
      expect(config.strategy).toBe('collapse_with_metadata');
    });
  });

  describe('edge cases', () => {
    it('should handle empty product arrays', () => {
      const result = service.deduplicateProducts([]);
      expect(result).toEqual([]);
    });

    it('should handle null/undefined products', () => {
      const result = service.deduplicateProducts(null as any);
      expect(result).toEqual([]);
    });

    it('should handle products with null/undefined values', () => {
      const products: ProductLine[] = [
        {
          productId: 1,
          priceTypeId: null,
          proposedPrice: undefined,
          quantity: 5,
          packingList: null
        }
      ];

      const result = service.deduplicateProducts(products);
      expect(result.length).toBe(1);
      expect(result[0].productId).toBe(1);
    });

    it('should handle products with empty packing lists', () => {
      const products: ProductLine[] = [
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: ''
        },
        {
          productId: 1,
          priceTypeId: 1,
          proposedPrice: 10.00,
          quantity: 5,
          packingList: '   '
        }
      ];

      const result = service.deduplicateProducts(products);
      expect(result.length).toBe(1);
      expect(result[0].metaJson).toBeUndefined();
    });
  });
});
