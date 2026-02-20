using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;

namespace NPPContractManagement.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRepository<Role> roleRepository, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                return roles.Select(MapRoleToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                throw;
            }
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                return role != null ? MapRoleToDto(role) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by id {Id}", id);
                throw;
            }
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto, string createdBy)
        {
            try
            {
                // Check if role name already exists
                var existingRole = await _roleRepository.FirstOrDefaultAsync(r => r.Name == createRoleDto.Name);
                if (existingRole != null)
                {
                    throw new InvalidOperationException($"Role '{createRoleDto.Name}' already exists");
                }

                var role = new Role
                {
                    Name = createRoleDto.Name,
                    Description = createRoleDto.Description,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                var createdRole = await _roleRepository.AddAsync(role);
                _logger.LogInformation("Role {RoleName} created successfully by {CreatedBy}", createRoleDto.Name, createdBy);

                return MapRoleToDto(createdRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role {RoleName}", createRoleDto.Name);
                throw;
            }
        }

        public async Task<RoleDto> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto, string modifiedBy)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    throw new InvalidOperationException($"Role with ID {id} not found");
                }

                // Check if role name is taken by another role
                var existingRole = await _roleRepository.FirstOrDefaultAsync(r => r.Name == updateRoleDto.Name && r.Id != id);
                if (existingRole != null)
                {
                    throw new InvalidOperationException($"Role name '{updateRoleDto.Name}' is already taken by another role");
                }

                role.Name = updateRoleDto.Name;
                role.Description = updateRoleDto.Description;
                role.IsActive = updateRoleDto.IsActive;
                role.ModifiedDate = DateTime.UtcNow;
                role.ModifiedBy = modifiedBy;

                var updatedRole = await _roleRepository.UpdateAsync(role);
                _logger.LogInformation("Role {RoleName} updated successfully by {ModifiedBy}", updateRoleDto.Name, modifiedBy);

                return MapRoleToDto(updatedRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return false;
                }

                // Check if role is a system role (prevent deletion of system roles)
                var systemRoles = new[] { "System Administrator", "Contract Manager", "Manufacturer", "Headless", "Contract Viewer" };
                if (systemRoles.Contains(role.Name))
                {
                    throw new InvalidOperationException($"Cannot delete system role '{role.Name}'");
                }

                await _roleRepository.DeleteAsync(role);
                _logger.LogInformation("Role {RoleName} deleted successfully", role.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {Id}", id);
                throw;
            }
        }

        public async Task<bool> ActivateRoleAsync(int id, string modifiedBy)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return false;
                }

                role.IsActive = true;
                role.ModifiedDate = DateTime.UtcNow;
                role.ModifiedBy = modifiedBy;

                await _roleRepository.UpdateAsync(role);
                _logger.LogInformation("Role {RoleName} activated by {ModifiedBy}", role.Name, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating role {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeactivateRoleAsync(int id, string modifiedBy)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return false;
                }

                // Prevent deactivation of System Administrator role
                if (role.Name == "System Administrator")
                {
                    throw new InvalidOperationException("Cannot deactivate System Administrator role");
                }

                role.IsActive = false;
                role.ModifiedDate = DateTime.UtcNow;
                role.ModifiedBy = modifiedBy;

                await _roleRepository.UpdateAsync(role);
                _logger.LogInformation("Role {RoleName} deactivated by {ModifiedBy}", role.Name, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating role {Id}", id);
                throw;
            }
        }

        private RoleDto MapRoleToDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedDate = role.CreatedDate,
                ModifiedDate = role.ModifiedDate,
                CreatedBy = role.CreatedBy,
                ModifiedBy = role.ModifiedBy
            };
        }
    }
}
