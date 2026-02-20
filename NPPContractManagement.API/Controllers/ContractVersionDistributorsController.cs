using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/contract-version/distributors")]
    [Authorize]
    public class ContractVersionDistributorsController : ControllerBase
    {
        private readonly IContractDistributorVersionService _service;
        public ContractVersionDistributorsController(IContractDistributorVersionService service) { _service = service; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDistributorVersionDto>>> Get([FromQuery] int? contractId = null, [FromQuery] int? versionNumber = null, [FromQuery] int? distributorId = null)
            => Ok(await _service.GetAllAsync(contractId, versionNumber, distributorId));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractDistributorVersionDto>> GetById(int id)
        {
            var x = await _service.GetByIdAsync(id);
            return x == null ? NotFound() : Ok(x);
        }

        public record CreateRequest(int ContractId, int DistributorId, int VersionNumber, string? AssignedBy, DateTime? AssignedDate);

        [HttpPost]
        public async Task<ActionResult<ContractDistributorVersionDto>> Create([FromBody] CreateRequest req)
            => Ok(await _service.CreateAsync(req.ContractId, req.DistributorId, req.VersionNumber, req.AssignedBy, req.AssignedDate));

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ContractDistributorVersionDto>> Update(int id, [FromBody] UpdateAssignedRequest request)
            => Ok(await _service.UpdateAsync(id, request.AssignedBy, request.AssignedDate));

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}

