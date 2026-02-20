using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class ContractVersionProductService : IContractVersionProductService
    {
        private readonly IContractVersionProductRepository _repo;
        public ContractVersionProductService(IContractVersionProductRepository repo) { _repo = repo; }

        public async Task<IEnumerable<ContractVersionProductDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? productId = null)
        {
            var items = await _repo.GetAllAsync(contractId, versionNumber, productId);
            return items.Select(x => new ContractVersionProductDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                ProductId = x.ProductId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
        }

        public async Task<ContractVersionProductDto?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            return x == null ? null : new ContractVersionProductDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                ProductId = x.ProductId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            };
        }

        public async Task<ContractVersionProductDto> CreateAsync(int contractId, int productId, int versionNumber, string? assignedBy, DateTime? assignedDate)
        {
            Validate(contractId, versionNumber);
            if (productId <= 0) throw new ArgumentException("ProductId required", nameof(productId));
            var entity = new ContractVersionProduct
            {
                ContractId = contractId,
                ProductId = productId,
                VersionNumber = versionNumber,
                AssignedBy = assignedBy,
                AssignedDate = assignedDate
            };
            var created = await _repo.CreateAsync(entity);
            return new ContractVersionProductDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                ProductId = created.ProductId,
                VersionNumber = created.VersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
        }

        public async Task<ContractVersionProductDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new ArgumentException("Not found", nameof(id));
            existing.AssignedBy = assignedBy;
            existing.AssignedDate = assignedDate;
            var updated = await _repo.UpdateAsync(existing);
            return new ContractVersionProductDto
            {
                Id = updated.Id,
                ContractId = updated.ContractId,
                ProductId = updated.ProductId,
                VersionNumber = updated.VersionNumber,
                AssignedBy = updated.AssignedBy,
                AssignedDate = updated.AssignedDate
            };
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

        private static void Validate(int contractId, int versionNumber)
        {
            if (contractId <= 0) throw new ArgumentException("ContractId required", nameof(contractId));
            if (versionNumber <= 0) throw new ArgumentException("VersionNumber required", nameof(versionNumber));
        }
    }
}

