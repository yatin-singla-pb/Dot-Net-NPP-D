using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class ContractIndustryVersionService : IContractIndustryVersionService
    {
        private readonly IContractIndustryVersionRepository _repo;
        public ContractIndustryVersionService(IContractIndustryVersionRepository repo) { _repo = repo; }

        public async Task<IEnumerable<ContractIndustryVersionDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? industryId = null)
        {
            var items = await _repo.GetAllAsync(contractId, versionNumber, industryId);
            return items.Select(x => new ContractIndustryVersionDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                IndustryId = x.IndustryId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
        }

        public async Task<ContractIndustryVersionDto?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            return x == null ? null : new ContractIndustryVersionDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                IndustryId = x.IndustryId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            };
        }

        public async Task<ContractIndustryVersionDto> CreateAsync(CreateContractIndustryVersionRequest request)
        {
            Validate(request.ContractId, request.VersionNumber);
            var entity = new ContractIndustryVersion
            {
                ContractId = request.ContractId,
                IndustryId = request.IndustryId,
                VersionNumber = request.VersionNumber,
                AssignedBy = request.AssignedBy,
                AssignedDate = request.AssignedDate
            };
            var created = await _repo.CreateAsync(entity);
            return new ContractIndustryVersionDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                IndustryId = created.IndustryId,
                VersionNumber = created.VersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
        }

        public async Task<ContractIndustryVersionDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new ArgumentException("Not found", nameof(id));
            existing.AssignedBy = assignedBy;
            existing.AssignedDate = assignedDate;
            var updated = await _repo.UpdateAsync(existing);
            return new ContractIndustryVersionDto
            {
                Id = updated.Id,
                ContractId = updated.ContractId,
                IndustryId = updated.IndustryId,
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

