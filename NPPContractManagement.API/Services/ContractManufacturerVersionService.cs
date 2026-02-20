using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class ContractManufacturerVersionService : IContractManufacturerVersionService
    {
        private readonly IContractManufacturerVersionRepository _repo;
        public ContractManufacturerVersionService(IContractManufacturerVersionRepository repo) { _repo = repo; }

        public async Task<IEnumerable<ContractManufacturerVersionDto>> GetAllAsync(int? contractId = null, int? versionNumber = null, int? manufacturerId = null)
        {
            var items = await _repo.GetAllAsync(contractId, versionNumber, manufacturerId);
            return items.Select(x => new ContractManufacturerVersionDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                ManufacturerId = x.ManufacturerId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
        }

        public async Task<ContractManufacturerVersionDto?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            return x == null ? null : new ContractManufacturerVersionDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                ManufacturerId = x.ManufacturerId,
                VersionNumber = x.VersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            };
        }

        public async Task<ContractManufacturerVersionDto> CreateAsync(int contractId, int manufacturerId, int versionNumber, string? assignedBy, DateTime? assignedDate)
        {
            Validate(contractId, versionNumber);
            if (manufacturerId <= 0) throw new ArgumentException("ManufacturerId required", nameof(manufacturerId));
            var entity = new ContractManufacturerVersion
            {
                ContractId = contractId,
                ManufacturerId = manufacturerId,
                VersionNumber = versionNumber,
                AssignedBy = assignedBy,
                AssignedDate = assignedDate
            };
            var created = await _repo.CreateAsync(entity);
            return new ContractManufacturerVersionDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                ManufacturerId = created.ManufacturerId,
                VersionNumber = created.VersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
        }

        public async Task<ContractManufacturerVersionDto> UpdateAsync(int id, string? assignedBy, DateTime? assignedDate)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new ArgumentException("Not found", nameof(id));
            existing.AssignedBy = assignedBy;
            existing.AssignedDate = assignedDate;
            var updated = await _repo.UpdateAsync(existing);
            return new ContractManufacturerVersionDto
            {
                Id = updated.Id,
                ContractId = updated.ContractId,
                ManufacturerId = updated.ManufacturerId,
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

