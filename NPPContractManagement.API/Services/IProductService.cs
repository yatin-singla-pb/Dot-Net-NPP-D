using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, string createdBy);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto, string modifiedBy);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> ActivateProductAsync(int id, string modifiedBy);
        Task<bool> DeactivateProductAsync(int id, string modifiedBy);
        Task<ProductDto?> GetProductBySKUAsync(string sku);
        Task<ProductDto?> GetProductByGTINAsync(string gtin);
        Task<ProductDto?> GetProductByUPCAsync(string upc);
        Task<ProductDto?> GetProductByManufacturerProductCodeAsync(string manufacturerProductCode);
        Task<IEnumerable<ProductDto>> GetProductsByManufacturerIdAsync(int manufacturerId);
        Task<IEnumerable<ProductDto>> GetProductsByStatusAsync(int status);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
        Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
        Task<(IEnumerable<ProductDto> Products, int TotalCount)> SearchProductsAsync(string searchTerm, int? manufacturerId = null, int? status = null, string? category = null, string? brand = null, int page = 1, int pageSize = 10);
        Task<IEnumerable<ProductDto>> GetProductsByManufacturerIdsAsync(IEnumerable<int> manufacturerIds);
        Task<IEnumerable<string>> GetDistinctBrandsAsync();

    }
}
