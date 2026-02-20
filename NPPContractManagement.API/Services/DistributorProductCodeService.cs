using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class DistributorProductCodeService : IDistributorProductCodeService
    {
        private readonly IDistributorProductCodeRepository _repo;
        private readonly IDistributorRepository _distributorRepo;
        private readonly IProductRepository _productRepo;
        private readonly ILogger<DistributorProductCodeService> _logger;

        public DistributorProductCodeService(
            IDistributorProductCodeRepository repo,
            IDistributorRepository distributorRepo,
            IProductRepository productRepo,
            ILogger<DistributorProductCodeService> logger)
        {
            _repo = repo;
            _distributorRepo = distributorRepo;
            _productRepo = productRepo;
            _logger = logger;
        }

        public async Task<(IEnumerable<DistributorProductCodeDto> Items, int TotalCount)> SearchAsync(string? searchTerm, IEnumerable<int>? distributorIds = null, IEnumerable<int>? productIds = null, int? productStatus = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc")
        {
            var items = await _repo.SearchAsync(searchTerm, distributorIds, productIds, productStatus, page, pageSize, sortBy, sortDirection);
            var total = await _repo.GetCountAsync(searchTerm, distributorIds, productIds, productStatus);
            return (items.Select(MapToDto), total);
        }

        public async Task<DistributorProductCodeDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<DistributorProductCodeDto> CreateAsync(CreateDistributorProductCodeDto dto, string createdBy)
        {
            // Validate
            await EnsureDistributorExists(dto.DistributorId);
            await EnsureProductExists(dto.ProductId);

            var code = (dto.DistributorCode ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("DistributorCode is required");

            var existing = await _repo.FindByDistributorAndCodeAsync(dto.DistributorId, code);
            if (existing != null) throw new ArgumentException("A code with this Distributor and DistributorCode already exists.");

            // Optional unique mapping per (Distributor, Product)
            var existingMap = await _repo.FindByDistributorAndProductAsync(dto.DistributorId, dto.ProductId);
            if (existingMap != null) throw new ArgumentException("This Distributor already has a code mapped to the specified Product.");

            var entity = new DistributorProductCode
            {
                DistributorId = dto.DistributorId,
                ProductId = dto.ProductId,
                DistributorCode = code,
                CatchWeight = dto.CatchWeight,
                EBrand = dto.EBrand,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task<DistributorProductCodeDto> UpdateAsync(int id, UpdateDistributorProductCodeDto dto, string modifiedBy)
        {
            var entity = await _repo.GetByIdAsync(id) ?? throw new ArgumentException("Distributor Product Code not found");

            var newDistributorId = dto.DistributorId ?? entity.DistributorId;
            var newProductId = dto.ProductId ?? entity.ProductId;
            var newCode = (dto.DistributorCode ?? entity.DistributorCode).Trim();

            await EnsureDistributorExists(newDistributorId);
            await EnsureProductExists(newProductId);

            // Uniqueness validations
            var duplicateCode = await _repo.FindByDistributorAndCodeAsync(newDistributorId, newCode, excludeId: id);
            if (duplicateCode != null) throw new ArgumentException("A code with this Distributor and DistributorCode already exists.");

            var duplicateMap = await _repo.FindByDistributorAndProductAsync(newDistributorId, newProductId, excludeId: id);
            if (duplicateMap != null) throw new ArgumentException("This Distributor already has a code mapped to the specified Product.");

            entity.DistributorId = newDistributorId;
            entity.ProductId = newProductId;
            entity.DistributorCode = newCode;
            if (dto.CatchWeight.HasValue) entity.CatchWeight = dto.CatchWeight.Value;
            if (dto.EBrand.HasValue) entity.EBrand = dto.EBrand.Value;
            entity.ModifiedBy = modifiedBy;
            entity.ModifiedDate = DateTime.UtcNow;
            entity.UpdatedBy = modifiedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(entity);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            await _repo.DeleteAsync(entity);
            return true;
        }

        private DistributorProductCodeDto MapToDto(DistributorProductCode e)
        {
            return new DistributorProductCodeDto
            {
                Id = e.Id,
                DistributorId = e.DistributorId,
                DistributorName = e.Distributor?.Name,
                ProductId = e.ProductId,
                ProductName = e.Product?.Description,
                ProductDescription = e.Product?.Description,
                ManufacturerProductCode = e.Product?.ManufacturerProductCode,
                Brand = e.Product?.Brand,
                ManufacturerName = e.Product?.Manufacturer?.Name,
                DistributorCode = e.DistributorCode,
                CatchWeight = e.CatchWeight,
                EBrand = e.EBrand,
                CreatedDate = e.CreatedDate,
                ModifiedDate = e.ModifiedDate,
                CreatedBy = e.CreatedBy,
                ModifiedBy = e.ModifiedBy
            };
        }

        private async Task EnsureDistributorExists(int distributorId)
        {
            var exists = await _distributorRepo.ExistsAsync(distributorId);
            if (!exists) throw new ArgumentException($"Distributor with ID {distributorId} not found");
        }

        private async Task EnsureProductExists(int productId)
        {
            var exists = await _productRepo.ExistsAsync(productId);
            if (!exists) throw new ArgumentException($"Product with ID {productId} not found");
        }
    }
}

