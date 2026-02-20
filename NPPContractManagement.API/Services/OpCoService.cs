using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class OpCoService : IOpCoService
    {
        private readonly IOpCoRepository _opCoRepository;
        private readonly IDistributorRepository _distributorRepository;

        public OpCoService(IOpCoRepository opCoRepository, IDistributorRepository distributorRepository)
        {
            _opCoRepository = opCoRepository;
            _distributorRepository = distributorRepository;
        }

        public async Task<IEnumerable<OpCoDto>> GetAllOpCosAsync()
        {
            var opCos = await _opCoRepository.GetAllAsync();
            return opCos.Select(MapToDto);
        }

        public async Task<OpCoDto?> GetOpCoByIdAsync(int id)
        {
            var opCo = await _opCoRepository.GetByIdAsync(id);
            return opCo != null ? MapToDto(opCo) : null;
        }

        public async Task<OpCoDto> CreateOpCoAsync(CreateOpCoDto createOpCoDto, string createdBy)
        {
            // Validate distributor exists
            var distributor = await _distributorRepository.GetByIdAsync(createOpCoDto.DistributorId);
            if (distributor == null)
            {
                throw new ArgumentException("Distributor not found", nameof(createOpCoDto.DistributorId));
            }

            // Validate unique remote reference code if provided
            if (!string.IsNullOrWhiteSpace(createOpCoDto.RemoteReferenceCode))
            {
                var exists = await _opCoRepository.ExistsByRemoteReferenceCodeAsync(createOpCoDto.RemoteReferenceCode);
                if (exists)
                {
                    throw new ArgumentException("Remote reference code already exists", nameof(createOpCoDto.RemoteReferenceCode));
                }
            }

            var opCo = new OpCo
            {
                Name = createOpCoDto.Name,
                RemoteReferenceCode = createOpCoDto.RemoteReferenceCode,
                DistributorId = createOpCoDto.DistributorId,
                Address = createOpCoDto.Address,
                City = createOpCoDto.City,
                State = createOpCoDto.State,
                ZipCode = createOpCoDto.ZipCode,
                Country = createOpCoDto.Country,
                PhoneNumber = createOpCoDto.PhoneNumber,
                Email = createOpCoDto.Email,
                ContactPerson = createOpCoDto.ContactPerson,
                InternalNotes = createOpCoDto.InternalNotes,
                IsRedistributor = createOpCoDto.IsRedistributor,
                Status = (OpCoStatus)createOpCoDto.Status,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            var createdOpCo = await _opCoRepository.AddAsync(opCo);
            return MapToDto(createdOpCo);
        }

        public async Task<OpCoDto> UpdateOpCoAsync(int id, UpdateOpCoDto updateOpCoDto, string modifiedBy)
        {
            var opCo = await _opCoRepository.GetByIdAsync(id);
            if (opCo == null)
            {
                throw new ArgumentException("OpCo not found", nameof(id));
            }

            // Validate distributor exists
            var distributor = await _distributorRepository.GetByIdAsync(updateOpCoDto.DistributorId);
            if (distributor == null)
            {
                throw new ArgumentException("Distributor not found", nameof(updateOpCoDto.DistributorId));
            }

            // Validate unique remote reference code if provided
            if (!string.IsNullOrWhiteSpace(updateOpCoDto.RemoteReferenceCode))
            {
                var exists = await _opCoRepository.ExistsByRemoteReferenceCodeAsync(updateOpCoDto.RemoteReferenceCode, id);
                if (exists)
                {
                    throw new ArgumentException("Remote reference code already exists", nameof(updateOpCoDto.RemoteReferenceCode));
                }
            }

            opCo.Name = updateOpCoDto.Name;
            opCo.RemoteReferenceCode = updateOpCoDto.RemoteReferenceCode;
            opCo.DistributorId = updateOpCoDto.DistributorId;
            opCo.Address = updateOpCoDto.Address;
            opCo.City = updateOpCoDto.City;
            opCo.State = updateOpCoDto.State;
            opCo.ZipCode = updateOpCoDto.ZipCode;
            opCo.Country = updateOpCoDto.Country;
            opCo.PhoneNumber = updateOpCoDto.PhoneNumber;
            opCo.Email = updateOpCoDto.Email;
            opCo.ContactPerson = updateOpCoDto.ContactPerson;
            opCo.InternalNotes = updateOpCoDto.InternalNotes;
            opCo.IsRedistributor = updateOpCoDto.IsRedistributor;
            opCo.Status = (OpCoStatus)updateOpCoDto.Status;
            opCo.IsActive = updateOpCoDto.IsActive;
            opCo.ModifiedDate = DateTime.UtcNow;
            opCo.ModifiedBy = modifiedBy;

            var updatedOpCo = await _opCoRepository.UpdateAsync(opCo);
            return MapToDto(updatedOpCo);
        }

        public async Task<bool> DeleteOpCoAsync(int id)
        {
            var opCo = await _opCoRepository.GetByIdAsync(id);
            if (opCo == null)
            {
                return false;
            }

            // Check if OpCo has customer accounts
            if (opCo.CustomerAccounts?.Any() == true)
            {
                throw new InvalidOperationException("Cannot delete OpCo with existing customer accounts");
            }

            await _opCoRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ActivateOpCoAsync(int id, string modifiedBy)
        {
            var opCo = await _opCoRepository.GetByIdAsync(id);
            if (opCo == null)
            {
                return false;
            }

            opCo.IsActive = true;
            opCo.Status = OpCoStatus.Active;
            opCo.ModifiedDate = DateTime.UtcNow;
            opCo.ModifiedBy = modifiedBy;

            await _opCoRepository.UpdateAsync(opCo);
            return true;
        }

        public async Task<bool> DeactivateOpCoAsync(int id, string modifiedBy)
        {
            var opCo = await _opCoRepository.GetByIdAsync(id);
            if (opCo == null)
            {
                return false;
            }

            opCo.IsActive = false;
            //opCo.Status = OpCoStatus.Inactive;
            opCo.ModifiedDate = DateTime.UtcNow;
            opCo.ModifiedBy = modifiedBy;

            await _opCoRepository.UpdateAsync(opCo);
             return true;
        }

        public async Task<IEnumerable<OpCoDto>> GetOpCosByDistributorIdAsync(int distributorId)
        {
            var opCos = await _opCoRepository.GetByDistributorIdAsync(distributorId);
            return opCos.Select(MapToDto);
        }

        public async Task<IEnumerable<OpCoDto>> GetOpCosByStatusAsync(int status)
        {
            var opCos = await _opCoRepository.GetByStatusAsync((OpCoStatus)status);
            return opCos.Select(MapToDto);
        }

        public async Task<OpCoDto?> GetOpCoByRemoteReferenceCodeAsync(string remoteReferenceCode)
        {
            var opCo = await _opCoRepository.GetByRemoteReferenceCodeAsync(remoteReferenceCode);
            return opCo != null ? MapToDto(opCo) : null;
        }

        public async Task<(IEnumerable<OpCoDto> OpCos, int TotalCount)> SearchOpCosAsync(string? searchTerm, int? distributorId = null, int? status = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? remoteReferenceCode = null)
        {
            var opCos = await _opCoRepository.SearchAsync(searchTerm, distributorId, status.HasValue ? (OpCoStatus)status.Value : null, page, pageSize, sortBy, sortDirection, remoteReferenceCode);
            var totalCount = await _opCoRepository.GetCountAsync(searchTerm, distributorId, status.HasValue ? (OpCoStatus)status.Value : null, remoteReferenceCode);

            return (opCos.Select(MapToDto), totalCount);
        }

        private static OpCoDto MapToDto(OpCo opCo)
        {
            return new OpCoDto
            {
                Id = opCo.Id,
                Name = opCo.Name,
                RemoteReferenceCode = opCo.RemoteReferenceCode,
                DistributorId = opCo.DistributorId,
                DistributorName = opCo.Distributor?.Name,
                Address = opCo.Address,
                City = opCo.City,
                State = opCo.State,
                ZipCode = opCo.ZipCode,
                Country = opCo.Country,
                PhoneNumber = opCo.PhoneNumber,
                Email = opCo.Email,
                ContactPerson = opCo.ContactPerson,
                InternalNotes = opCo.InternalNotes,
                IsRedistributor = opCo.IsRedistributor,
                Status = (int)opCo.Status,
                StatusName = opCo.Status.ToString(),
                IsActive = opCo.IsActive,
                CreatedDate = opCo.CreatedDate,
                ModifiedDate = opCo.ModifiedDate,
                CreatedBy = opCo.CreatedBy,
                ModifiedBy = opCo.ModifiedBy,
                CustomerAccountsCount = opCo.CustomerAccounts?.Count ?? 0,
                CustomerAccounts = opCo.CustomerAccounts?.Select(ca => new CustomerAccountSummaryDto
                {
                    Id = ca.Id,
                    CustomerName = ca.CustomerName,
                    CustomerAccountNumber = ca.CustomerAccountNumber,
                    StatusName = ((CustomerAccountStatus)ca.Status).ToString()
                }).ToList()
            };
        }

        public async Task<IEnumerable<OpCoDto>> GetOpCosByDistributorIdsAsync(IEnumerable<int> distributorIds)
        {
            var ids = (distributorIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (ids.Count == 0) return Enumerable.Empty<OpCoDto>();
            var opCos = await _opCoRepository.GetByDistributorIdsAsync(ids);
            return opCos
                .GroupBy(o => o.Id)
                .Select(g => MapToDto(g.First()));
        }

    }
}