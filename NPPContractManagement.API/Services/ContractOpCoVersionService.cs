using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class ContractOpCoVersionService : IContractOpCoVersionService
    {
        private readonly IContractOpCoVersionRepository _repo;
        public ContractOpCoVersionService(IContractOpCoVersionRepository repo) { _repo = repo; }

        public async Task<IEnumerable<ContractOpCoVersionDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? opCoId = null)
        {
            var items = await _repo.GetAllAsync(contractId, versionNumber, opCoId);
            return items.Select(x => new ContractOpCoVersionDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                OpCoId = x.OpCoId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
        }

        public async Task<ContractOpCoVersionDto?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            return x == null ? null : new ContractOpCoVersionDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                OpCoId = x.OpCoId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            };
        }

        public async Task<ContractOpCoVersionDto> CreateAsync(int contractId, int opCoId, int versionNumber, string? assignedBy, DateTime? assignedDate)
        {
            Validate(contractId, versionNumber);
            if (opCoId <= 0) throw new ArgumentException("OpCoId required", nameof(opCoId));
            var entity = new ContractOpCoVersion
            {
                ContractId = contractId,
                OpCoId = opCoId,
                VersionNumber = versionNumber,
                AssignedBy = assignedBy,
                AssignedDate = assignedDate
            };
            var created = await _repo.CreateAsync(entity);
            return new ContractOpCoVersionDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                OpCoId = created.OpCoId,
                VersionNumber = created.VersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
        }

        public async Task<ContractOpCoVersionDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new ArgumentException("Not found", nameof(id));
            existing.AssignedBy = assignedBy;
            existing.AssignedDate = assignedDate;
            var updated = await _repo.UpdateAsync(existing);
            return new ContractOpCoVersionDto
            {
                Id = updated.Id,
                ContractId = updated.ContractId,
                OpCoId = updated.OpCoId,
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

