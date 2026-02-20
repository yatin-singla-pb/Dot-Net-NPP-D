using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class ContractPriceService : IContractPriceService
    {
        private static readonly HashSet<string> AllowedUoms = new(StringComparer.OrdinalIgnoreCase) { "Cases", "Pounds" };
        private static readonly HashSet<string> AllowedPriceTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            // Canonical allowed values
            "Contract Price",
            "Contract Price at Time of Purchase",
            "List at Time of Purchase/No Bid",
            "Suspended",
            "Discontinued",
            // Backward-compatible legacy variants (accepted but not emitted)
            "List at time of purchase / No Bid",
            "Product Suspended",
            "Product Discontinued"
        };

        private readonly IContractPriceRepository _repo;
        private readonly IProductRepository _productRepo;
        private readonly IContractRepository _contractRepo;
        public ContractPriceService(IContractPriceRepository repo, IProductRepository productRepo, IContractRepository contractRepo)
        {
            _repo = repo; _productRepo = productRepo; _contractRepo = contractRepo;
        }

        public async Task<IEnumerable<ContractPriceDto>> GetAllAsync(int? productId = null, int? versionNumber = null)
        {
            var items = await _repo.GetAllAsync(productId, versionNumber);
            return items.Select(MapToDto);
        }

        public async Task<ContractPriceDto?> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<ContractPriceDto> CreateAsync(CreateContractPriceRequest request, string createdBy)
        {
            if (request.ContractId <= 0) throw new ArgumentException("ContractId is required", nameof(request.ContractId));
            Validate(request.PriceType, request.UOM);
            var product = await _productRepo.GetByIdAsync(request.ProductId);
            if (product == null) throw new ArgumentException("Product not found", nameof(request.ProductId));
            var contract = await _contractRepo.GetByIdAsync(request.ContractId);
            if (contract == null) throw new ArgumentException("Contract not found", nameof(request.ContractId));

            // Validate no duplicate active prices for same product in same contract and version
            var existingPrices = await _repo.GetAllAsync(request.ProductId, request.VersionNumber);
            var duplicatePrice = existingPrices.FirstOrDefault(p =>
                p.ContractId == request.ContractId &&
                p.ProductId == request.ProductId &&
                p.VersionNumber == request.VersionNumber);

            if (duplicatePrice != null)
            {
                throw new ArgumentException(
                    $"A price already exists for Product ID {request.ProductId} in Contract ID {request.ContractId} Version {request.VersionNumber}. " +
                    $"Please update the existing price (ID: {duplicatePrice.Id}) instead of creating a duplicate.",
                    nameof(request.ProductId));
            }

            var entity = new ContractPrice
            {
                VersionNumber = request.VersionNumber,
                ContractId = request.ContractId,
                ProductId = request.ProductId,
                PriceType = request.PriceType,
                Allowance = request.Allowance,
                CommercialDelPrice = request.CommercialDelPrice,
                CommercialFobPrice = request.CommercialFobPrice,
                CommodityDelPrice = request.CommodityDelPrice,
                CommodityFobPrice = request.CommodityFobPrice,
                UOM = request.UOM,
                EstimatedQty = request.EstimatedQty,
                BillbacksAllowed = request.BillbacksAllowed,
                PUA = request.PUA,
                FFSPrice = request.FFSPrice,
                NOIPrice = request.NOIPrice,
                PTV = request.PTV,
                InternalNotes = request.InternalNotes,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _repo.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<ContractPriceDto> UpdateAsync(int id, UpdateContractPriceRequest request, string modifiedBy)
        {
            Validate(request.PriceType, request.UOM);
            var existing = await _repo.GetByIdAsync(id) ?? throw new ArgumentException("ContractPrice not found", nameof(id));

            existing.PriceType = request.PriceType;
            existing.Allowance = request.Allowance;
            existing.CommercialDelPrice = request.CommercialDelPrice;
            existing.CommercialFobPrice = request.CommercialFobPrice;
            existing.CommodityDelPrice = request.CommodityDelPrice;
            existing.CommodityFobPrice = request.CommodityFobPrice;
            existing.UOM = request.UOM;
            existing.EstimatedQty = request.EstimatedQty;
            existing.BillbacksAllowed = request.BillbacksAllowed;
            existing.PUA = request.PUA;
            existing.FFSPrice = request.FFSPrice;
            existing.NOIPrice = request.NOIPrice;
            existing.PTV = request.PTV;
            existing.InternalNotes = request.InternalNotes;
            existing.ModifiedBy = modifiedBy;
            existing.ModifiedDate = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(existing);
            return MapToDto(updated);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

        private static void Validate(string priceType, string uom)
        {
            if (string.IsNullOrWhiteSpace(uom) || !AllowedUoms.Contains(uom))
                throw new ArgumentException("UOM must be 'Cases' or 'Pounds'", nameof(uom));
            if (string.IsNullOrWhiteSpace(priceType) || !AllowedPriceTypes.Contains(priceType))
                throw new ArgumentException("PriceType invalid", nameof(priceType));
        }

        private static ContractPriceDto MapToDto(ContractPrice e) => new()
        {
            Id = e.Id,
            VersionNumber = e.VersionNumber,
            ContractId = e.ContractId,
            ProductId = e.ProductId,
            ProductName = e.Product?.Name,
            PriceType = e.PriceType,
            Allowance = e.Allowance,
            CommercialDelPrice = e.CommercialDelPrice,
            CommercialFobPrice = e.CommercialFobPrice,
            CommodityDelPrice = e.CommodityDelPrice,
            CommodityFobPrice = e.CommodityFobPrice,
            UOM = e.UOM,
            EstimatedQty = e.EstimatedQty,
            BillbacksAllowed = e.BillbacksAllowed,
            PUA = e.PUA,
            FFSPrice = e.FFSPrice,
            NOIPrice = e.NOIPrice,
            PTV = e.PTV,
            InternalNotes = e.InternalNotes,
            CreatedDate = e.CreatedDate,
            CreatedBy = e.CreatedBy
        };
    }
}

