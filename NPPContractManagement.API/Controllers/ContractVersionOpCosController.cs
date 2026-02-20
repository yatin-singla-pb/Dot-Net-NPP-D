using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/contract-version/opcos")]
    [Authorize]
    public class ContractVersionOpCosController : ControllerBase
    {
        private readonly IContractOpCoVersionService _service;
        public ContractVersionOpCosController(IContractOpCoVersionService service) { _service = service; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractOpCoVersionDto>>> Get([FromQuery] int? contractId = null, [FromQuery] int? versionNumber = null, [FromQuery] int? opCoId = null)
            => Ok(await _service.GetAllAsync(contractId, versionNumber, opCoId));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractOpCoVersionDto>> GetById(int id)
        {
            var x = await _service.GetByIdAsync(id);
            return x == null ? NotFound() : Ok(x);
        }

        public record CreateRequest(int ContractId, int OpCoId, int VersionNumber, string? AssignedBy, DateTime? AssignedDate);

        [HttpPost]
        public async Task<ActionResult<ContractOpCoVersionDto>> Create([FromBody] CreateRequest req)
            => Ok(await _service.CreateAsync(req.ContractId, req.OpCoId, req.VersionNumber, req.AssignedBy, req.AssignedDate));

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ContractOpCoVersionDto>> Update(int id, [FromBody] UpdateAssignedRequest request)
            => Ok(await _service.UpdateAsync(id, request.AssignedBy, request.AssignedDate));

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}

