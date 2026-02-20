using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/contract-version/industries")]
    [Authorize]
    public class ContractVersionIndustriesController : ControllerBase
    {
        private readonly IContractIndustryVersionService _service;
        public ContractVersionIndustriesController(IContractIndustryVersionService service) { _service = service; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractIndustryVersionDto>>> Get([FromQuery] int? contractId = null, [FromQuery] int? versionNumber = null, [FromQuery] int? industryId = null)
            => Ok(await _service.GetAllAsync(contractId, versionNumber, industryId));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractIndustryVersionDto>> GetById(int id)
        {
            var x = await _service.GetByIdAsync(id);
            return x == null ? NotFound() : Ok(x);
        }

        [HttpPost]
        public async Task<ActionResult<ContractIndustryVersionDto>> Create([FromBody] CreateContractIndustryVersionRequest request)
            => Ok(await _service.CreateAsync(request));

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ContractIndustryVersionDto>> Update(int id, [FromBody] UpdateAssignedRequest request)
            => Ok(await _service.UpdateAsync(id, request.AssignedBy, request.AssignedDate));

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}

