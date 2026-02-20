using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class ContractDistributorVersionService : IContractDistributorVersionService
    {
        private readonly IContractDistributorVersionRepository _repo;
        public ContractDistributorVersionService(IContractDistributorVersionRepository repo) { _repo = repo; }

        public async Task<IEnumerable<ContractDistributorVersionDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? distributorId = null)
        {
            var items = await _repo.GetAllAsync(contractId, versionNumber, distributorId);
            return items.Select(x => new ContractDistributorVersionDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                DistributorId = x.DistributorId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
        }

        public async Task<ContractDistributorVersionDto?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            return x == null ? null : new ContractDistributorVersionDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                DistributorId = x.DistributorId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            };
        }

        public async Task<ContractDistributorVersionDto> CreateAsync(int contractId, int distributorId, int versionNumber, string? assignedBy, DateTime? assignedDate)
        {
            Validate(contractId, versionNumber);
            if (distributorId <= 0) throw new ArgumentException("DistributorId required", nameof(distributorId));
            var entity = new ContractDistributorVersion
            {
                ContractId = contractId,
                DistributorId = distributorId,
                VersionNumber = versionNumber,
                AssignedBy = assignedBy,
                AssignedDate = assignedDate
            };
            var created = await _repo.CreateAsync(entity);
            return new ContractDistributorVersionDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                DistributorId = created.DistributorId,
                VersionNumber = created.VersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
        }

        public async Task<ContractDistributorVersionDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new ArgumentException("Not found", nameof(id));
            existing.AssignedBy = assignedBy;
            existing.AssignedDate = assignedDate;
            var updated = await _repo.UpdateAsync(existing);
            return new ContractDistributorVersionDto
            {
                Id = updated.Id,
                ContractId = updated.ContractId,
                DistributorId = updated.DistributorId,
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

