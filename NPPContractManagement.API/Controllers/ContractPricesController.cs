using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;
using System.Security.Claims;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/contract-prices")]
    [Authorize]
    public class ContractPricesController : ControllerBase
    {
        private readonly IContractPriceService _service;
        public ContractPricesController(IContractPriceService service) { _service = service; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractPriceDto>>> Get([FromQuery] int? productId = null, [FromQuery] int? versionNumber = null)
        {
            var items = await _service.GetAllAsync(productId, versionNumber);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractPriceDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ContractPriceDto>> Create([FromBody] CreateContractPriceRequest request)
        {
            var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var item = await _service.CreateAsync(request, createdBy);
            return Ok(item);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ContractPriceDto>> Update(int id, [FromBody] UpdateContractPriceRequest request)
        {
            var modifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var item = await _service.UpdateAsync(id, request, modifiedBy);
            return Ok(item);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

