using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Services;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/contracts/{contractId:int}/assignments")]
    [Authorize]
    public class ContractAssignmentsController : ControllerBase
    {
        private readonly IContractAssignmentService _assignmentService;

        public ContractAssignmentsController(IContractAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        // Distributors
        [HttpGet("distributors")]
        public async Task<ActionResult<IEnumerable<ContractDistributorAssignmentDto>>> GetDistributors(int contractId)
        {
            var items = await _assignmentService.GetDistributorsAsync(contractId);
            var result = items.Select(x => new ContractDistributorAssignmentDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                DistributorId = x.DistributorId,
                CurrentVersionNumber = x.CurrentVersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
            return Ok(result);
        }

        [HttpPost("distributors")]
        public async Task<ActionResult<ContractDistributorAssignmentDto>> AddDistributor(int contractId, [FromBody] CreateContractDistributorAssignmentRequest request)
        {
            var created = await _assignmentService.AddDistributorAsync(contractId, request.DistributorId, request.CurrentVersionNumber, request.AssignedBy, request.AssignedDate);
            var dto = new ContractDistributorAssignmentDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                DistributorId = created.DistributorId,
                CurrentVersionNumber = created.CurrentVersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
            return CreatedAtAction(nameof(GetDistributors), new { contractId }, dto);
        }

        [HttpDelete("distributors/{id:int}")]
        public async Task<IActionResult> RemoveDistributor(int contractId, int id)
        {
            await _assignmentService.RemoveDistributorAsync(id);
            return NoContent();
        }

        // Manufacturers
        [HttpGet("manufacturers")]
        public async Task<ActionResult<IEnumerable<ContractManufacturerAssignmentDto>>> GetManufacturers(int contractId)
        {
            var items = await _assignmentService.GetManufacturersAsync(contractId);
            var result = items.Select(x => new ContractManufacturerAssignmentDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                ManufacturerId = x.ManufacturerId,
                CurrentVersionNumber = x.CurrentVersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
            return Ok(result);
        }

        [HttpPost("manufacturers")]
        public async Task<ActionResult<ContractManufacturerAssignmentDto>> AddManufacturer(int contractId, [FromBody] CreateContractManufacturerAssignmentRequest request)
        {
            var created = await _assignmentService.AddManufacturerAsync(contractId, request.ManufacturerId, request.CurrentVersionNumber, request.AssignedBy, request.AssignedDate);
            var dto = new ContractManufacturerAssignmentDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                ManufacturerId = created.ManufacturerId,
                CurrentVersionNumber = created.CurrentVersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
            return CreatedAtAction(nameof(GetManufacturers), new { contractId }, dto);
        }

        [HttpDelete("manufacturers/{id:int}")]
        public async Task<IActionResult> RemoveManufacturer(int contractId, int id)
        {
            await _assignmentService.RemoveManufacturerAsync(id);
            return NoContent();
        }

        // OpCos
        [HttpGet("opcos")]
        public async Task<ActionResult<IEnumerable<ContractOpCoAssignmentDto>>> GetOpCos(int contractId)
        {
            var items = await _assignmentService.GetOpCosAsync(contractId);
            var result = items.Select(x => new ContractOpCoAssignmentDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                OpCoId = x.OpCoId,
                CurrentVersionNumber = x.CurrentVersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
            return Ok(result);
        }

        [HttpPost("opcos")]
        public async Task<ActionResult<ContractOpCoAssignmentDto>> AddOpCo(int contractId, [FromBody] CreateContractOpCoAssignmentRequest request)
        {
            var created = await _assignmentService.AddOpCoAsync(contractId, request.OpCoId, request.CurrentVersionNumber, request.AssignedBy, request.AssignedDate);
            var dto = new ContractOpCoAssignmentDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                OpCoId = created.OpCoId,
                CurrentVersionNumber = created.CurrentVersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
            return CreatedAtAction(nameof(GetOpCos), new { contractId }, dto);
        }

        [HttpDelete("opcos/{id:int}")]
        public async Task<IActionResult> RemoveOpCo(int contractId, int id)
        {
            await _assignmentService.RemoveOpCoAsync(id);
            return NoContent();
        }

        // Industries
        [HttpGet("industries")]
        public async Task<ActionResult<IEnumerable<ContractIndustryAssignmentDto>>> GetIndustries(int contractId)
        {
            var items = await _assignmentService.GetIndustriesAsync(contractId);
            var result = items.Select(x => new ContractIndustryAssignmentDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                IndustryId = x.IndustryId,
                CurrentVersionNumber = x.CurrentVersionNumber,
                AssignedBy = x.AssignedBy,
                AssignedDate = x.AssignedDate
            });
            return Ok(result);
        }

        [HttpPost("industries")]
        public async Task<ActionResult<ContractIndustryAssignmentDto>> AddIndustry(int contractId, [FromBody] CreateContractIndustryAssignmentRequest request)
        {
            var created = await _assignmentService.AddIndustryAsync(contractId, request.IndustryId, request.CurrentVersionNumber, request.AssignedBy, request.AssignedDate);
            var dto = new ContractIndustryAssignmentDto
            {
                Id = created.Id,
                ContractId = created.ContractId,
                IndustryId = created.IndustryId,
                CurrentVersionNumber = created.CurrentVersionNumber,
                AssignedBy = created.AssignedBy,
                AssignedDate = created.AssignedDate
            };
            return CreatedAtAction(nameof(GetIndustries), new { contractId }, dto);
        }

        [HttpDelete("industries/{id:int}")]
        public async Task<IActionResult> RemoveIndustry(int contractId, int id)
        {
            await _assignmentService.RemoveIndustryAsync(id);
            return NoContent();
        }
    }
}

