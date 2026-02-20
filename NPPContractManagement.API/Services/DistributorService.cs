using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class DistributorService : IDistributorService
    {
        private readonly IDistributorRepository _distributorRepository;
        private readonly ILogger<DistributorService> _logger;

        public DistributorService(IDistributorRepository distributorRepository, ILogger<DistributorService> logger)
        {
            _distributorRepository = distributorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<DistributorDto>> GetAllDistributorsAsync()
        {
            try
            {
                var distributors = await _distributorRepository.GetAllAsync();
                return distributors.Select(MapDistributorToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all distributors");
                throw;
            }
        }

        public async Task<(IEnumerable<DistributorDto> Distributors, int TotalCount)> SearchDistributorsAsync(string searchTerm, int? status = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", bool? receiveContractProposal = null, string? state = null)
        {
            try
            {
                var statusEnum = status.HasValue ? (DistributorStatus)status.Value : null as DistributorStatus?;
                var distributors = await _distributorRepository.SearchAsync(searchTerm, statusEnum, receiveContractProposal, page, pageSize, sortBy, sortDirection, state);
                var total = await _distributorRepository.GetCountAsync(searchTerm, statusEnum, receiveContractProposal, state);
                return (distributors.Select(MapDistributorToDto), total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching distributors");
                throw;
            }
        }

        public async Task<DistributorDto?> GetDistributorByIdAsync(int id)
        {
            try
            {
                var distributor = await _distributorRepository.GetByIdAsync(id);
                return distributor != null ? MapDistributorToDto(distributor) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting distributor by id {Id}", id);
                throw;
            }
        }

        public async Task<DistributorDto> CreateDistributorAsync(CreateDistributorDto createDistributorDto, string createdBy)
        {
            try
            {
                var distributor = new Distributor
                {
                    Name = createDistributorDto.Name,
                    Description = createDistributorDto.Description,
                    ContactPerson = createDistributorDto.ContactPerson,
                    Email = createDistributorDto.Email,
                    PhoneNumber = createDistributorDto.PhoneNumber,
                    Address = createDistributorDto.Address,
                    City = createDistributorDto.City,
                    State = createDistributorDto.State,
                    ZipCode = createDistributorDto.ZipCode,
                    Country = createDistributorDto.Country,
                    ReceiveContractProposal = createDistributorDto.ReceiveContractProposal,
                    IsRedistributor = createDistributorDto.IsRedistributor,
                    Status = (DistributorStatus)createDistributorDto.Status,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                var createdDistributor = await _distributorRepository.AddAsync(distributor);
                _logger.LogInformation("Distributor {DistributorName} created successfully by {CreatedBy}", createDistributorDto.Name, createdBy);

                return MapDistributorToDto(createdDistributor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating distributor {DistributorName}", createDistributorDto.Name);
                throw;
            }
        }

        public async Task<DistributorDto> UpdateDistributorAsync(int id, UpdateDistributorDto updateDistributorDto, string modifiedBy)
        {
            try
            {
                var distributor = await _distributorRepository.GetByIdAsync(id);
                if (distributor == null)
                {
                    throw new InvalidOperationException($"Distributor with ID {id} not found");
                }

                distributor.Name = updateDistributorDto.Name;
                distributor.Description = updateDistributorDto.Description;
                distributor.ContactPerson = updateDistributorDto.ContactPerson;
                distributor.Email = updateDistributorDto.Email;
                distributor.PhoneNumber = updateDistributorDto.PhoneNumber;
                distributor.Address = updateDistributorDto.Address;
                distributor.City = updateDistributorDto.City;
                distributor.State = updateDistributorDto.State;
                distributor.ZipCode = updateDistributorDto.ZipCode;
                distributor.Country = updateDistributorDto.Country;
                distributor.ReceiveContractProposal = updateDistributorDto.ReceiveContractProposal;
                distributor.IsRedistributor = updateDistributorDto.IsRedistributor;
                distributor.Status = (DistributorStatus)updateDistributorDto.Status;
                distributor.IsActive = updateDistributorDto.IsActive;
                distributor.ModifiedDate = DateTime.UtcNow;
                distributor.ModifiedBy = modifiedBy;

                var updatedDistributor = await _distributorRepository.UpdateAsync(distributor);
                _logger.LogInformation("Distributor {DistributorName} updated successfully by {ModifiedBy}", updateDistributorDto.Name, modifiedBy);

                return MapDistributorToDto(updatedDistributor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating distributor {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteDistributorAsync(int id)
        {
            try
            {
                var distributor = await _distributorRepository.GetByIdAsync(id);
                if (distributor == null)
                {
                    return false;
                }

                await _distributorRepository.DeleteAsync(distributor);
                _logger.LogInformation("Distributor {DistributorName} deleted successfully", distributor.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting distributor {Id}", id);
                throw;
            }
        }

        public async Task<bool> ActivateDistributorAsync(int id, string modifiedBy)
        {
            try
            {
                var distributor = await _distributorRepository.GetByIdAsync(id);
                if (distributor == null)
                {
                    return false;
                }

                distributor.IsActive = true;
                distributor.ModifiedDate = DateTime.UtcNow;
                distributor.ModifiedBy = modifiedBy;

                await _distributorRepository.UpdateAsync(distributor);
                _logger.LogInformation("Distributor {DistributorName} activated by {ModifiedBy}", distributor.Name, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating distributor {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeactivateDistributorAsync(int id, string modifiedBy)
        {
            try
            {
                var distributor = await _distributorRepository.GetByIdAsync(id);
                if (distributor == null)
                {
                    return false;
                }

                distributor.IsActive = false;
                distributor.ModifiedDate = DateTime.UtcNow;
                distributor.ModifiedBy = modifiedBy;

                await _distributorRepository.UpdateAsync(distributor);
                _logger.LogInformation("Distributor {DistributorName} deactivated by {ModifiedBy}", distributor.Name, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating distributor {Id}", id);
                throw;
            }
        }

        private DistributorDto MapDistributorToDto(Distributor distributor)
        {
            return new DistributorDto
            {
                Id = distributor.Id,
                Name = distributor.Name,
                Description = distributor.Description,
                ContactPerson = distributor.ContactPerson,
                Email = distributor.Email,
                PhoneNumber = distributor.PhoneNumber,
                Address = distributor.Address,
                City = distributor.City,
                State = distributor.State,
                ZipCode = distributor.ZipCode,
                Country = distributor.Country,
                ReceiveContractProposal = distributor.ReceiveContractProposal,
                IsRedistributor = distributor.IsRedistributor,
                Status = (int)distributor.Status,
                StatusName = distributor.Status.ToString(),
                IsActive = distributor.IsActive,
                CreatedDate = distributor.CreatedDate,
                ModifiedDate = distributor.ModifiedDate,
                CreatedBy = distributor.CreatedBy,
                ModifiedBy = distributor.ModifiedBy,
                OpCosCount = distributor.OpCos?.Count ?? 0,
                CustomerAccountsCount = distributor.CustomerAccounts?.Count ?? 0
            };
        }
    }
}
