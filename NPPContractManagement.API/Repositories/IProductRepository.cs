using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetBySKUAsync(string sku);
        Task<Product?> GetByGTINAsync(string gtin);
        Task<Product?> GetByUPCAsync(string upc);
        Task<Product?> GetByManufacturerProductCodeAsync(string manufacturerProductCode);
        Task<bool> ExistsBySKUAsync(string sku, int? excludeId = null);
        Task<bool> ExistsByGTINAsync(string gtin, int? excludeId = null);
        Task<bool> ExistsByUPCAsync(string upc, int? excludeId = null);
        Task<IEnumerable<Product>> GetByManufacturerIdAsync(int manufacturerId);
        Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> SearchAsync(string searchTerm, int? manufacturerId = null, ProductStatus? status = null, string? category = null, string? brand = null, int page = 1, int pageSize = 10);
        Task<int> GetCountAsync(int? manufacturerId = null, ProductStatus? status = null, string? category = null, string? brand = null);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetByManufacturerIdsAsync(IEnumerable<int> manufacturerIds);
        Task<IEnumerable<string>> GetDistinctBrandsAsync();
    }
}