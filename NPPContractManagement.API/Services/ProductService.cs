using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IManufacturerRepository _manufacturerRepository;

        public ProductService(IProductRepository productRepository, IManufacturerRepository manufacturerRepository)
        {
            _productRepository = productRepository;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, string createdBy)
        {
            // Validate manufacturer exists
            var manufacturer = await _manufacturerRepository.GetByIdAsync(createProductDto.ManufacturerId);
            if (manufacturer == null)
            {
                throw new ArgumentException("Manufacturer not found", nameof(createProductDto.ManufacturerId));
            }

            // Validate unique SKU if provided
            if (!string.IsNullOrWhiteSpace(createProductDto.SKU))
            {
                var exists = await _productRepository.ExistsBySKUAsync(createProductDto.SKU);
                if (exists)
                {
                    throw new ArgumentException("SKU already exists", nameof(createProductDto.SKU));
                }
            }

            // Validate unique GTIN if provided
            if (!string.IsNullOrWhiteSpace(createProductDto.GTIN))
            {
                var exists = await _productRepository.ExistsByGTINAsync(createProductDto.GTIN);
                if (exists)
                {
                    throw new ArgumentException("GTIN already exists", nameof(createProductDto.GTIN));
                }
            }

            // Validate unique UPC if provided
            if (!string.IsNullOrWhiteSpace(createProductDto.UPC))
            {
                var exists = await _productRepository.ExistsByUPCAsync(createProductDto.UPC);
                if (exists)
                {
                    throw new ArgumentException("UPC already exists", nameof(createProductDto.UPC));
                }
            }

            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                ManufacturerProductCode = createProductDto.ManufacturerProductCode,
                GTIN = createProductDto.GTIN,
                UPC = createProductDto.UPC,
                SKU = createProductDto.SKU,
                PackSize = createProductDto.PackSize,
                ManufacturerId = createProductDto.ManufacturerId,
                Category = createProductDto.Category,
                SubCategory = createProductDto.SubCategory,
                Brand = createProductDto.Brand,
                TertiaryCategory = createProductDto.TertiaryCategory,
                AlwaysList = createProductDto.AlwaysList,
                Notes = createProductDto.Notes,
                Status = (ProductStatus)createProductDto.Status,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            var createdProduct = await _productRepository.AddAsync(product);
            return MapToDto(createdProduct);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto, string modifiedBy)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Product not found", nameof(id));
            }

            // Validate manufacturer exists
            var manufacturer = await _manufacturerRepository.GetByIdAsync(updateProductDto.ManufacturerId);
            if (manufacturer == null)
            {
                throw new ArgumentException("Manufacturer not found", nameof(updateProductDto.ManufacturerId));
            }

            // Validate unique SKU if provided
            if (!string.IsNullOrWhiteSpace(updateProductDto.SKU))
            {
                var exists = await _productRepository.ExistsBySKUAsync(updateProductDto.SKU, id);
                if (exists)
                {
                    throw new ArgumentException("SKU already exists", nameof(updateProductDto.SKU));
                }
            }

            // Validate unique GTIN if provided
            if (!string.IsNullOrWhiteSpace(updateProductDto.GTIN))
            {
                var exists = await _productRepository.ExistsByGTINAsync(updateProductDto.GTIN, id);
                if (exists)
                {
                    throw new ArgumentException("GTIN already exists", nameof(updateProductDto.GTIN));
                }
            }

            // Validate unique UPC if provided
            if (!string.IsNullOrWhiteSpace(updateProductDto.UPC))
            {
                var exists = await _productRepository.ExistsByUPCAsync(updateProductDto.UPC, id);
                if (exists)
                {
                    throw new ArgumentException("UPC already exists", nameof(updateProductDto.UPC));
                }
            }

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.ManufacturerProductCode = updateProductDto.ManufacturerProductCode;
            product.GTIN = updateProductDto.GTIN;
            product.UPC = updateProductDto.UPC;
            product.SKU = updateProductDto.SKU;
            product.PackSize = updateProductDto.PackSize;
            product.ManufacturerId = updateProductDto.ManufacturerId;
            product.Category = updateProductDto.Category;
            product.SubCategory = updateProductDto.SubCategory;
            product.Brand = updateProductDto.Brand;
            product.TertiaryCategory = updateProductDto.TertiaryCategory;
            product.AlwaysList = updateProductDto.AlwaysList;
            product.Notes = updateProductDto.Notes;
            product.Status = (ProductStatus)updateProductDto.Status;
            product.IsActive = updateProductDto.IsActive;
            product.ModifiedDate = DateTime.UtcNow;
            product.ModifiedBy = modifiedBy;

            var updatedProduct = await _productRepository.UpdateAsync(product);
            return MapToDto(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            await _productRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ActivateProductAsync(int id, string modifiedBy)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            product.IsActive = true;
            product.Status = ProductStatus.Active;
            product.ModifiedDate = DateTime.UtcNow;
            product.ModifiedBy = modifiedBy;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeactivateProductAsync(int id, string modifiedBy)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            product.IsActive = false;
            product.Status = ProductStatus.Inactive;
            product.ModifiedDate = DateTime.UtcNow;
            product.ModifiedBy = modifiedBy;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<ProductDto?> GetProductBySKUAsync(string sku)
        {
            var product = await _productRepository.GetBySKUAsync(sku);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto?> GetProductByGTINAsync(string gtin)
        {
            var product = await _productRepository.GetByGTINAsync(gtin);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto?> GetProductByUPCAsync(string upc)
        {
            var product = await _productRepository.GetByUPCAsync(upc);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto?> GetProductByManufacturerProductCodeAsync(string manufacturerProductCode)
        {
            var product = await _productRepository.GetByManufacturerProductCodeAsync(manufacturerProductCode);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByManufacturerIdAsync(int manufacturerId)
        {
            var products = await _productRepository.GetByManufacturerIdAsync(manufacturerId);
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByStatusAsync(int status)
        {
            var products = await _productRepository.GetByStatusAsync((ProductStatus)status);
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _productRepository.GetByCategoryAsync(category);
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return products.Select(MapToDto);
        }

        public async Task<(IEnumerable<ProductDto> Products, int TotalCount)> SearchProductsAsync(string searchTerm, int? manufacturerId = null, int? status = null, string? category = null, string? brand = null, int page = 1, int pageSize = 10)
        {
            var products = await _productRepository.SearchAsync(searchTerm, manufacturerId, status.HasValue ? (ProductStatus)status.Value : null, category, brand, page, pageSize);
            var totalCount = await _productRepository.GetCountAsync(manufacturerId, status.HasValue ? (ProductStatus)status.Value : null, category, brand);

            return (products.Select(MapToDto), totalCount);
        }

        public async Task<IEnumerable<string>> GetDistinctBrandsAsync()
        {
            return await _productRepository.GetDistinctBrandsAsync();
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ManufacturerProductCode = product.ManufacturerProductCode,
                GTIN = product.GTIN,
                UPC = product.UPC,
                SKU = product.SKU,
                PackSize = product.PackSize,
                ManufacturerId = product.ManufacturerId,
                ManufacturerName = product.Manufacturer?.Name,
                Category = product.Category,
                SubCategory = product.SubCategory,
                Brand = product.Brand,
                TertiaryCategory = product.TertiaryCategory,
                AlwaysList = product.AlwaysList,
                Notes = product.Notes,
                Status = (int)product.Status,
                StatusName = product.Status.ToString(),
                IsActive = product.IsActive,
                CreatedDate = product.CreatedDate,
                ModifiedDate = product.ModifiedDate,
                CreatedBy = product.CreatedBy,
                ModifiedBy = product.ModifiedBy
            };
        }

	        public async Task<IEnumerable<ProductDto>> GetProductsByManufacturerIdsAsync(IEnumerable<int> manufacturerIds)
	        {
	            var ids = (manufacturerIds ?? Enumerable.Empty<int>()).Distinct().ToList();
	            if (ids.Count == 0) return Enumerable.Empty<ProductDto>();
	            var products = await _productRepository.GetByManufacturerIdsAsync(ids);
	            return products
	                .GroupBy(p => p.Id)
	                .Select(g => MapToDto(g.First()));
	        }

    }
}
