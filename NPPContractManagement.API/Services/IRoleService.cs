using NPPContractManagement.API.DTOs;

namespace NPPContractManagement.API.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto, string createdBy);
        Task<RoleDto> UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto, string modifiedBy);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> ActivateRoleAsync(int id, string modifiedBy);
        Task<bool> DeactivateRoleAsync(int id, string modifiedBy);
    }
}
