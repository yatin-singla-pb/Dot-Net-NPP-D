using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ILogger<ManufacturerService> _logger;

        public ManufacturerService(IManufacturerRepository manufacturerRepository, ILogger<ManufacturerService> logger)
        {
            _manufacturerRepository = manufacturerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ManufacturerDto>> GetAllManufacturersAsync()
        {
            try
            {
                var manufacturers = await _manufacturerRepository.GetAllAsync();
                return manufacturers.Select(MapManufacturerToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all manufacturers");
                throw;
            }
        }
        public async Task<(IEnumerable<ManufacturerDto> Manufacturers, int TotalCount)> SearchManufacturersAsync(string searchTerm, int? status = null, int page = 1, int pageSize = 10, string? sortBy = null, string sortDirection = "asc", string? state = null, int? primaryBrokerId = null)
        {
            try
            {
                var statusEnum = status.HasValue ? (Models.ManufacturerStatus)status.Value : null as Models.ManufacturerStatus?;
                var manufacturers = await _manufacturerRepository.SearchAsync(searchTerm ?? string.Empty, statusEnum, page, pageSize, sortBy, sortDirection, state, primaryBrokerId);
                var total = await _manufacturerRepository.GetCountAsync(searchTerm ?? string.Empty, statusEnum, state, primaryBrokerId);
                return (manufacturers.Select(MapManufacturerToDto), total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching manufacturers");
                throw;
            }
        }


        public async Task<ManufacturerDto?> GetManufacturerByIdAsync(int id)
        {
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
                return manufacturer != null ? MapManufacturerToDto(manufacturer) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting manufacturer by id {Id}", id);
                throw;
            }
        }

        public async Task<ManufacturerDto> CreateManufacturerAsync(CreateManufacturerDto createManufacturerDto, string createdBy)
        {
            try
            {
                var manufacturer = new Manufacturer
                {
                    Name = createManufacturerDto.Name,
                    AKA = createManufacturerDto.AKA,
                    Description = createManufacturerDto.Description,
                    ContactPerson = createManufacturerDto.ContactPerson,
                    ContactPersonId = createManufacturerDto.ContactPersonId,
                    Email = createManufacturerDto.Email,
                    PhoneNumber = createManufacturerDto.PhoneNumber,
                    Address = createManufacturerDto.Address,
                    City = createManufacturerDto.City,
                    State = createManufacturerDto.State,
                    ZipCode = createManufacturerDto.ZipCode,
                    Country = createManufacturerDto.Country,
                    PrimaryBrokerId = createManufacturerDto.PrimaryBrokerId,
                    Status = (Models.ManufacturerStatus)(createManufacturerDto.Status == 0 ? 1 : createManufacturerDto.Status),
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                var createdManufacturer = await _manufacturerRepository.AddAsync(manufacturer);
                _logger.LogInformation("Manufacturer {ManufacturerName} created successfully by {CreatedBy}", createManufacturerDto.Name, createdBy);

                return MapManufacturerToDto(createdManufacturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating manufacturer {ManufacturerName}", createManufacturerDto.Name);
                throw;
            }
        }

        public async Task<ManufacturerDto> UpdateManufacturerAsync(int id, UpdateManufacturerDto updateManufacturerDto, string modifiedBy)
        {
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
                if (manufacturer == null)
                {
                    throw new InvalidOperationException($"Manufacturer with ID {id} not found");
                }

                manufacturer.Name = updateManufacturerDto.Name;
                manufacturer.AKA = updateManufacturerDto.AKA;
                manufacturer.Description = updateManufacturerDto.Description;
                manufacturer.ContactPerson = updateManufacturerDto.ContactPerson;
                manufacturer.ContactPersonId = updateManufacturerDto.ContactPersonId;
                manufacturer.Email = updateManufacturerDto.Email;
                manufacturer.PhoneNumber = updateManufacturerDto.PhoneNumber;
                manufacturer.Address = updateManufacturerDto.Address;
                manufacturer.City = updateManufacturerDto.City;
                manufacturer.State = updateManufacturerDto.State;
                manufacturer.ZipCode = updateManufacturerDto.ZipCode;
                manufacturer.Country = updateManufacturerDto.Country;
                manufacturer.PrimaryBrokerId = updateManufacturerDto.PrimaryBrokerId;
                manufacturer.Status = (Models.ManufacturerStatus)(updateManufacturerDto.Status == 0 ? (int)manufacturer.Status : updateManufacturerDto.Status);
                manufacturer.IsActive = updateManufacturerDto.IsActive;
                manufacturer.ModifiedDate = DateTime.UtcNow;
                manufacturer.ModifiedBy = modifiedBy;

                var updatedManufacturer = await _manufacturerRepository.UpdateAsync(manufacturer);
                _logger.LogInformation("Manufacturer {ManufacturerName} updated successfully by {ModifiedBy}", updateManufacturerDto.Name, modifiedBy);

                return MapManufacturerToDto(updatedManufacturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating manufacturer {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteManufacturerAsync(int id)
        {
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
                if (manufacturer == null)
                {
                    return false;
                }

                // Soft delete: mark inactive instead of physical deletion
                manufacturer.IsActive = false;
                manufacturer.ModifiedDate = DateTime.UtcNow;
                manufacturer.ModifiedBy = manufacturer.ModifiedBy ?? "System";

                await _manufacturerRepository.UpdateAsync(manufacturer);
                _logger.LogInformation("Manufacturer {ManufacturerName} soft-deleted (IsActive=false)", manufacturer.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting manufacturer {Id}", id);
                throw;
            }
        }

        public async Task<bool> ActivateManufacturerAsync(int id, string modifiedBy)
        {
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
                if (manufacturer == null)
                {
                    return false;
                }

                manufacturer.IsActive = true;
                manufacturer.ModifiedDate = DateTime.UtcNow;
                manufacturer.ModifiedBy = modifiedBy;

                await _manufacturerRepository.UpdateAsync(manufacturer);
                _logger.LogInformation("Manufacturer {ManufacturerName} activated by {ModifiedBy}", manufacturer.Name, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating manufacturer {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeactivateManufacturerAsync(int id, string modifiedBy)
        {
            try
            {
                var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
                if (manufacturer == null)
                {
                    return false;
                }

                manufacturer.IsActive = false;
                manufacturer.ModifiedDate = DateTime.UtcNow;
                manufacturer.ModifiedBy = modifiedBy;

                await _manufacturerRepository.UpdateAsync(manufacturer);
                _logger.LogInformation("Manufacturer {ManufacturerName} deactivated by {ModifiedBy}", manufacturer.Name, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating manufacturer {Id}", id);
                throw;
            }
        }

        private ManufacturerDto MapManufacturerToDto(Manufacturer manufacturer)
        {
            return new ManufacturerDto
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                AKA = manufacturer.AKA,
                Description = manufacturer.Description,
                ContactPerson = manufacturer.ContactPerson,
                ContactPersonId = manufacturer.ContactPersonId,
                ContactPersonName = manufacturer.ContactPersonUser != null ? ($"{manufacturer.ContactPersonUser.FirstName} {manufacturer.ContactPersonUser.LastName}").Trim() : null,
                Email = manufacturer.Email,
                PhoneNumber = manufacturer.PhoneNumber,
                Address = manufacturer.Address,
                City = manufacturer.City,
                State = manufacturer.State,
                ZipCode = manufacturer.ZipCode,
                Country = manufacturer.Country,
                PrimaryBrokerId = manufacturer.PrimaryBrokerId,
                PrimaryBrokerName = manufacturer.PrimaryBroker != null ? ($"{manufacturer.PrimaryBroker.FirstName} {manufacturer.PrimaryBroker.LastName}").Trim() : null,
                Status = (int)manufacturer.Status,
                StatusName = manufacturer.Status.ToString(),
                IsActive = manufacturer.IsActive,
                CreatedDate = manufacturer.CreatedDate,
                ModifiedDate = manufacturer.ModifiedDate,
                CreatedBy = manufacturer.CreatedBy,
                ModifiedBy = manufacturer.ModifiedBy
            };
        }
    }
}
