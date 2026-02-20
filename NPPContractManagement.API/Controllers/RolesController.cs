using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return StatusCode(500, new { message = "An error occurred while retrieving roles" });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new { message = "Role not found" });
                }

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the role" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "System Administrator")]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = GetCurrentUserName();
                var role = await _roleService.CreateRoleAsync(createRoleDto, currentUser);

                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, new { message = "An error occurred while creating the role" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "System Administrator")]
        public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = GetCurrentUserName();
                var role = await _roleService.UpdateRoleAsync(id, updateRoleDto, currentUser);

                return Ok(role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the role" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "System Administrator")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Role not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the role" });
            }
        }

        [HttpPost("{id}/activate")]
        [Authorize(Roles = "System Administrator")]
        public async Task<ActionResult> ActivateRole(int id)
        {
            try
            {
                var currentUser = GetCurrentUserName();
                var result = await _roleService.ActivateRoleAsync(id, currentUser);
                if (!result)
                {
                    return NotFound(new { message = "Role not found" });
                }

                return Ok(new { message = "Role activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating role {Id}", id);
                return StatusCode(500, new { message = "An error occurred while activating the role" });
            }
        }

        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "System Administrator")]
        public async Task<ActionResult> DeactivateRole(int id)
        {
            try
            {
                var currentUser = GetCurrentUserName();
                var result = await _roleService.DeactivateRoleAsync(id, currentUser);
                if (!result)
                {
                    return NotFound(new { message = "Role not found" });
                }

                return Ok(new { message = "Role deactivated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating role {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deactivating the role" });
            }
        }

        private string GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        }
    }
}
