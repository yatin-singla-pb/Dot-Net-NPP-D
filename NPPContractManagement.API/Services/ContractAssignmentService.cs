using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Models;

namespace NPPContractManagement.API.Services
{
    public class ContractAssignmentService : IContractAssignmentService
    {
        private readonly ApplicationDbContext _context;

        public ContractAssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Distributors
        public async Task<List<ContractDistributor>> GetDistributorsAsync(int contractId)
        {
            return await _context.ContractDistributors
                .Include(x => x.Distributor)
                .Where(x => x.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<ContractDistributor> AddDistributorAsync(int contractId, int distributorId, int currentVersionNumber, string? assignedBy, DateTime? assignedDate)
        {
            var exists = await _context.ContractDistributors.AnyAsync(x => x.ContractId == contractId && x.DistributorId == distributorId);
            if (exists)
            {
                throw new InvalidOperationException("Distributor already assigned to this contract.");
            }

            var entity = new ContractDistributor
            {
                ContractId = contractId,
                DistributorId = distributorId,
                CurrentVersionNumber = currentVersionNumber,
                AssignedBy = assignedBy,
                AssignedDate = assignedDate
            };
            _context.ContractDistributors.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task RemoveDistributorAsync(int id)
        {
            var entity = await _context.ContractDistributors.FindAsync(id);
            if (entity != null)
            {
                _context.ContractDistributors.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // Manufacturers
        public async Task<List<ContractManufacturer>> GetManufacturersAsync(int contractId)
        {
            return await _context.ContractManufacturers
                .Include(x => x.Manufacturer)
                .Where(x => x.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<ContractManufacturer> AddManufacturerAsync(int contractId, int manufacturerId, int currentVersionNumber, string? assignedBy, DateTime? assignedDate)
        {
            var exists = await _context.ContractManufacturers.AnyAsync(x => x.ContractId == contractId && x.ManufacturerId == manufacturerId);
            if (exists)
            {
                throw new InvalidOperationException("Manufacturer already assigned to this contract.");
            }

            var entity = new ContractManufacturer
            {
                ContractId = contractId,
                ManufacturerId = manufacturerId,
                CurrentVersionNumber = currentVersionNumber,
                AssignedBy = assignedBy,
                AssignedDate = assignedDate
            };
            _context.ContractManufacturers.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task RemoveManufacturerAsync(int id)
        {
            var entity = await _context.ContractManufacturers.FindAsync(id);
            if (entity != null)
            {
                _context.ContractManufacturers.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // OpCos
        public async Task<List<ContractOpCo>> GetOpCosAsync(int contractId)
        {
            return await _context.ContractOpCos
                .Include(x => x.OpCo)
                .Where(x => x.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<ContractOpCo> AddOpCoAsync(int contractId, int opCoId, int currentVersionNumber, string? assignedBy, DateTime? assignedDate)
        {
            var exists = await _context.ContractOpCos.AnyAsync(x => x.ContractId == contractId && x.OpCoId == opCoId);
            if (exists)
            {
                throw new InvalidOperationException("OpCo already assigned to this contract.");
            }

            var entity = new ContractOpCo
            {
                ContractId = contractId,
                OpCoId = opCoId,
                CurrentVersionNumber = currentVersionNumber,
                AssignedBy = assignedBy,
                AssignedDate = assignedDate
            };
            _context.ContractOpCos.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task RemoveOpCoAsync(int id)
        {
            var entity = await _context.ContractOpCos.FindAsync(id);
            if (entity != null)
            {
                _context.ContractOpCos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // Industries
        public async Task<List<ContractIndustry>> GetIndustriesAsync(int contractId)
        {
            return await _context.ContractIndustries
                .Include(x => x.Industry)
                .Where(x => x.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<ContractIndustry> AddIndustryAsync(int contractId, int industryId, int currentVersionNumber, string? assignedBy, DateTime? assignedDate)
        {
            var exists = await _context.ContractIndustries.AnyAsync(x => x.ContractId == contractId && x.IndustryId == industryId);
            if (exists)
            {
                throw new InvalidOperationException("Industry already assigned to this contract.");
            }

            var entity = new ContractIndustry
            {
                ContractId = contractId,
                IndustryId = industryId,
                CurrentVersionNumber = currentVersionNumber,
                AssignedBy = assignedBy,
                AssignedDate = assignedDate
            };
            _context.ContractIndustries.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task RemoveIndustryAsync(int id)
        {
            var entity = await _context.ContractIndustries.FindAsync(id);
            if (entity != null)
            {
                _context.ContractIndustries.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}

