using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/contract-version/manufacturers")]
    [Authorize]
    public class ContractVersionManufacturersController : ControllerBase
    {
        private readonly IContractManufacturerVersionService _service;
        public ContractVersionManufacturersController(IContractManufacturerVersionService service) { _service = service; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractManufacturerVersionDto>>> Get([FromQuery] int? contractId = null, [FromQuery] int? versionNumber = null, [FromQuery] int? manufacturerId = null)
            => Ok(await _service.GetAllAsync(contractId, versionNumber, manufacturerId));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractManufacturerVersionDto>> GetById(int id)
        {
            var x = await _service.GetByIdAsync(id);
            return x == null ? NotFound() : Ok(x);
        }

        public record CreateRequest(int ContractId, int ManufacturerId, int VersionNumber, string? AssignedBy, DateTime? AssignedDate);

        [HttpPost]
        public async Task<ActionResult<ContractManufacturerVersionDto>> Create([FromBody] CreateRequest req)
            => Ok(await _service.CreateAsync(req.ContractId, req.ManufacturerId, req.VersionNumber, req.AssignedBy, req.AssignedDate));

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ContractManufacturerVersionDto>> Update(int id, [FromBody] UpdateAssignedRequest request)
            => Ok(await _service.UpdateAsync(id, request.AssignedBy, request.AssignedDate));

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}

