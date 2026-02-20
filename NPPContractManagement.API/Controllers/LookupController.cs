using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/lookup")]
    [AllowAnonymous]
    public class LookupController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public LookupController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("proposal-types")]
        public async Task<ActionResult<object>> GetProposalTypes()
        {
            var items = await _db.ProposalTypes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name, isActive = x.IsActive })
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("proposal-statuses")]
        public async Task<ActionResult<object>> GetProposalStatuses()
        {
            var items = await _db.ProposalStatuses
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name, isActive = x.IsActive })
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("price-types")]
        public async Task<ActionResult<object>> GetPriceTypes()
        {
            var items = await _db.PriceTypes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name, isActive = x.IsActive })
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("product-proposal-statuses")]
        public async Task<ActionResult<object>> GetProductProposalStatuses()
        {
            var items = await _db.ProductProposalStatuses
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name, isActive = x.IsActive })
                .ToListAsync();
            return Ok(items);
        }

        // Additional anonymous lookups used by the Create Proposal form
        [HttpGet("manufacturers")]
        public async Task<ActionResult<object>> GetManufacturers()
        {
            var items = await _db.Manufacturers
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name, isActive = x.IsActive })
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("distributors")]
        public async Task<ActionResult<object>> GetDistributors()
        {
            var items = await _db.Distributors
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name, isActive = x.IsActive })
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("industries")]
        public async Task<ActionResult<object>> GetIndustries()
        {
            var items = await _db.Industries
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name, isActive = x.IsActive })
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("opcos")]
        public async Task<ActionResult<object>> GetOpCos()
        {
            var items = await _db.OpCos
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name, distributorId = x.DistributorId, isActive = x.IsActive })
                .ToListAsync();
            return Ok(items);
        }

        [HttpPost("products/by-manufacturers")]
        public async Task<ActionResult<object>> GetProductsByManufacturers([FromBody] List<int> manufacturerIds)
        {
            manufacturerIds = manufacturerIds ?? new List<int>();
            var items = await _db.Products
                .AsNoTracking()
                .Where(p => manufacturerIds.Count == 0 || manufacturerIds.Contains(p.ManufacturerId))
                .OrderBy(p => p.Name)
                .Select(p => new {
                    id = p.Id,
                    name = p.Name,
                    productName = p.Name,
                    description = p.Description,
                    manufacturerId = p.ManufacturerId,
                    isActive = p.IsActive,
                    manufacturerProductCode = p.ManufacturerProductCode ?? "",
                    packSize = p.PackSize ?? "",
                    brand = p.Brand ?? "",
                    gtin = p.GTIN ?? "",
                    notes = p.Notes ?? "",
                    status = p.Status.ToString()
                })
                .ToListAsync();
            return Ok(items);
        }

        [HttpPost("opcos/by-distributors")]
        public async Task<ActionResult<object>> GetOpCosByDistributors([FromBody] List<int> distributorIds)
        {
            distributorIds = distributorIds ?? new List<int>();
            var items = await _db.OpCos
                .AsNoTracking()
                .Where(o => distributorIds.Count == 0 || distributorIds.Contains(o.DistributorId))
                .OrderBy(o => o.Name)
                .Select(o => new { id = o.Id, name = o.Name, distributorId = o.DistributorId, isActive = o.IsActive })
                .ToListAsync();
            return Ok(items);
        }
    }
}

