using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/distributor-product-codes")]
    [Authorize]
    public class DistributorProductCodesController : ControllerBase
    {
        private readonly IDistributorProductCodeService _service;

        public DistributorProductCodesController(IDistributorProductCodeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<object>> Get(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? distributorIds = null,
            [FromQuery] string? productIds = null,
            [FromQuery] int? productStatus = null,
            // Backward-compat single-selects
            [FromQuery] int? distributorId = null,
            [FromQuery] int? productId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string sortDirection = "asc")
        {
            try
            {
                // Parse CSV id lists or fallback to single values
                List<int>? distributorList = null;
                if (!string.IsNullOrWhiteSpace(distributorIds))
                {
                    distributorList = new List<int>();
                    var parts = distributorIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part.Trim(), out var id)) distributorList.Add(id);
                    }
                    if (distributorList.Count == 0) distributorList = null;
                }
                else if (distributorId.HasValue)
                {
                    distributorList = new List<int> { distributorId.Value };
                }

                List<int>? productList = null;
                if (!string.IsNullOrWhiteSpace(productIds))
                {
                    productList = new List<int>();
                    var parts = productIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part.Trim(), out var id)) productList.Add(id);
                    }
                    if (productList.Count == 0) productList = null;
                }
                else if (productId.HasValue)
                {
                    productList = new List<int> { productId.Value };
                }

                var (items, total) = await _service.SearchAsync(searchTerm, distributorList, productList, productStatus, page, pageSize, sortBy, sortDirection);
                return Ok(new
                {
                    items,
                    totalCount = total,
                    pageNumber = page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Distributor Product Codes", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DistributorProductCodeDto>> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { message = "Distributor Product Code not found" });
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the Distributor Product Code", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<DistributorProductCodeDto>> Create([FromBody] CreateDistributorProductCodeDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var created = await _service.CreateAsync(dto, createdBy);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the Distributor Product Code", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult<DistributorProductCodeDto>> Update(int id, [FromBody] UpdateDistributorProductCodeDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var updated = await _service.UpdateAsync(id, dto, modifiedBy);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the Distributor Product Code", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "System Administrator,Contract Manager")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                if (!ok) return NotFound(new { message = "Distributor Product Code not found" });
                return Ok(new { message = "Distributor Product Code deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the Distributor Product Code", error = ex.Message });
            }
        }
    }
}

