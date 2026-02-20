using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.Domain.Proposals.Entities;
using NPPContractManagement.API.DTOs.Proposals;
using NPPContractManagement.API.Repositories;
using System.Text.Json;

namespace NPPContractManagement.API.Services
{
    public class ProposalService : IProposalService
    {
        private readonly ApplicationDbContext _db;
        private readonly IProposalRepository _repo;
        private readonly ILogger<ProposalService> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserManufacturerRepository _userManufacturerRepo;

        public ProposalService(ApplicationDbContext db, IProposalRepository repo, ILogger<ProposalService> logger, IEmailService emailService, IUserManufacturerRepository userManufacturerRepo)
        {
            _db = db;
            _repo = repo;
            _logger = logger;
            _emailService = emailService;
            _userManufacturerRepo = userManufacturerRepo;
        }

        public async Task<(IEnumerable<ProposalDto> Items, int TotalCount)> QueryProposalsAsync(
            string? search,
            int page,
            int pageSize,
            IEnumerable<int>? manufacturerIds = null,
            int? proposalStatusId = null,
            int? proposalTypeId = null,
            int? manufacturerId = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            DateTime? endDateFrom = null,
            DateTime? endDateTo = null,
            DateTime? createdDateFrom = null,
            DateTime? createdDateTo = null,
            int? idFrom = null,
            int? idTo = null,
            string? sortBy = null,
            string? sortDirection = null)
        {
            var (items, total) = await _repo.QueryAsync(
                search, page, pageSize, manufacturerIds,
                proposalStatusId, proposalTypeId, manufacturerId,
                startDateFrom, startDateTo, endDateFrom, endDateTo,
                createdDateFrom, createdDateTo, idFrom, idTo,
                sortBy, sortDirection);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var sample = items.FirstOrDefault();
                var statuses = items.Select(i => i.ProposalStatusId).Distinct().OrderBy(x => x).ToArray();
                _logger.LogDebug("[Svc] Query result: total={total}, page={page}, pageSize={pageSize}, batchCount={count}, sampleId={sid}, sampleStatusId={ssid}, statuses=[{statuses}]",
                    total, page, pageSize, items.Count(), sample?.Id, sample?.ProposalStatusId, string.Join(',', statuses));
            }
            return (items.Select(MapToDto).ToList(), total);
        }

        public async Task<ProposalDto?> GetProposalByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<ProposalDto> CreateProposalAsync(ProposalCreateDto dto, string createdBy)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = new Proposal
                {
                    Title = dto.Title,
                    ProposalTypeId = dto.ProposalTypeId,
                    ProposalStatusId = dto.ProposalStatusId,
                    ManufacturerId = dto.ManufacturerId,
                    AmendedContractId = dto.AmendedContractId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    DueDate = dto.DueDate,
                    InternalNotes = dto.InternalNotes,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow
                };

                _db.Proposals.Add(entity);
                await _db.SaveChangesAsync();

                // Distributors
                foreach (var d in dto.DistributorIds.Distinct())
                {
                    _db.ProposalDistributors.Add(new ProposalDistributor { ProposalId = entity.Id, DistributorId = d, CreatedBy = createdBy });
                }
                // Industries
                foreach (var i in dto.IndustryIds.Distinct())
                {
                    _db.ProposalIndustries.Add(new ProposalIndustry { ProposalId = entity.Id, IndustryId = i, CreatedBy = createdBy });
                }
                // OpCos
                foreach (var o in dto.OpcoIds.Distinct())
                {
                    _db.ProposalOpcos.Add(new ProposalOpco { ProposalId = entity.Id, OpCoId = o, CreatedBy = createdBy });
                }

                // Products (dedupe by key commercial fields; packing list removed)
                var grouped = dto.Products
                    .GroupBy(p => new {
                        p.ProductId,
                        p.PriceTypeId,
                        p.Quantity,
                        p.ProductProposalStatusId,
                        p.AmendmentActionId,
                        p.Uom,
                        p.BillbacksAllowed,
                        p.Allowance,
                        p.CommercialDelPrice,
                        p.CommercialFobPrice,
                        p.CommodityDelPrice,
                        p.CommodityFobPrice,
                        p.Pua,
                        p.FfsPrice,
                        p.NoiPrice,
                        p.Ptv,
                        p.InternalNotes,
                        p.ManufacturerNotes
                    })
                    .ToList();
                foreach (var g in grouped)
                {
                    _db.ProposalProducts.Add(new ProposalProduct
                    {
                        ProposalId = entity.Id,
                        ProductId = g.Key.ProductId,
                        PriceTypeId = g.Key.PriceTypeId,
                        Quantity = g.Key.Quantity,
                        AmendmentActionId = g.Key.AmendmentActionId,
                        Uom = g.Key.Uom,
                        BillbacksAllowed = g.Key.BillbacksAllowed,
                        Allowance = g.Key.Allowance,
                        CommercialDelPrice = g.Key.CommercialDelPrice,
                        CommercialFobPrice = g.Key.CommercialFobPrice,
                        CommodityDelPrice = g.Key.CommodityDelPrice,
                        CommodityFobPrice = g.Key.CommodityFobPrice,
                        Pua = g.Key.Pua,
                        FfsPrice = g.Key.FfsPrice,
                        NoiPrice = g.Key.NoiPrice,
                        Ptv = g.Key.Ptv,
                        InternalNotes = g.Key.InternalNotes,
                        ManufacturerNotes = g.Key.ManufacturerNotes,
                        MetaJson = null,
                        ProductProposalStatusId = g.Key.ProductProposalStatusId,
                        CreatedBy = createdBy
                    });
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return MapToDto(entity);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<ProposalDto> UpdateProposalAsync(int id, ProposalUpdateDto dto, string modifiedBy)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var entity = await _db.Proposals.Include(p => p.Products).FirstOrDefaultAsync(p => p.Id == id)
                    ?? throw new ArgumentException("Proposal not found");

                entity.Title = dto.Title;
                entity.ProposalTypeId = dto.ProposalTypeId;
                entity.ProposalStatusId = dto.ProposalStatusId;
                entity.ManufacturerId = dto.ManufacturerId;
                entity.AmendedContractId = dto.AmendedContractId;
                entity.StartDate = dto.StartDate;
                entity.EndDate = dto.EndDate;
                entity.DueDate = dto.DueDate;
                entity.InternalNotes = dto.InternalNotes;
                entity.ModifiedBy = modifiedBy;
                entity.ModifiedDate = DateTime.UtcNow;

                // Capture previous products snapshot for history before we replace them
                var prevProducts = entity.Products.Select(pp => new
                {
                    pp.ProductId,
                    pp.PriceTypeId,
                    pp.Quantity,
                    pp.AmendmentActionId,
                    pp.Uom,
                    pp.BillbacksAllowed,
                    pp.Allowance,
                    pp.CommercialDelPrice,
                    pp.CommercialFobPrice,
                    pp.CommodityDelPrice,
                    pp.CommodityFobPrice,
                    pp.Pua,
                    pp.FfsPrice,
                    pp.NoiPrice,
                    pp.Ptv,
                    pp.ProductProposalStatusId,
                    pp.InternalNotes,
                    pp.ManufacturerNotes
                }).ToList();


                // Replace child collections (simplified)
                _db.ProposalDistributors.RemoveRange(_db.ProposalDistributors.Where(x => x.ProposalId == id));
                _db.ProposalIndustries.RemoveRange(_db.ProposalIndustries.Where(x => x.ProposalId == id));
                _db.ProposalOpcos.RemoveRange(_db.ProposalOpcos.Where(x => x.ProposalId == id));

                // Preserve previous per-product statuses by ProductId
                var prevStatusByProductId = entity.Products
                    .GroupBy(pp => pp.ProductId)
                    .ToDictionary(g => g.Key, g => g.First().ProductProposalStatusId);

                _db.ProposalProducts.RemoveRange(_db.ProposalProducts.Where(x => x.ProposalId == id));
                await _db.SaveChangesAsync();

                foreach (var d in dto.DistributorIds.Distinct())
                    _db.ProposalDistributors.Add(new ProposalDistributor { ProposalId = id, DistributorId = d, CreatedBy = modifiedBy });
                foreach (var i in dto.IndustryIds.Distinct())
                    _db.ProposalIndustries.Add(new ProposalIndustry { ProposalId = id, IndustryId = i, CreatedBy = modifiedBy });
                foreach (var o in dto.OpcoIds.Distinct())
                    _db.ProposalOpcos.Add(new ProposalOpco { ProposalId = id, OpCoId = o, CreatedBy = modifiedBy });

                var grouped = dto.Products
                    .GroupBy(p => new {
                        p.ProductId,
                        p.PriceTypeId,
                        p.Quantity,
                        p.ProductProposalStatusId,
                        p.AmendmentActionId,
                        p.Uom,
                        p.BillbacksAllowed,
                        p.Allowance,
                        p.CommercialDelPrice,
                        p.CommercialFobPrice,
                        p.CommodityDelPrice,
                        p.CommodityFobPrice,
                        p.Pua,
                        p.FfsPrice,
                        p.NoiPrice,
                        p.Ptv,
                        p.InternalNotes,
                        p.ManufacturerNotes
                    })
                    .ToList();
                foreach (var g in grouped)
                {
                    // If no status provided in DTO, carry forward the previous one (if any)
                    int? statusToPersist = g.Key.ProductProposalStatusId;
                    if (statusToPersist == null && prevStatusByProductId.TryGetValue(g.Key.ProductId, out var prevStatus))
                    {
                        statusToPersist = prevStatus;
                    }

                    _db.ProposalProducts.Add(new ProposalProduct
                    {
                        ProposalId = id,
                        ProductId = g.Key.ProductId,
                        PriceTypeId = g.Key.PriceTypeId,
                        Quantity = g.Key.Quantity,
                        AmendmentActionId = g.Key.AmendmentActionId,
                        Uom = g.Key.Uom,
                        BillbacksAllowed = g.Key.BillbacksAllowed,
                        Allowance = g.Key.Allowance,
                        CommercialDelPrice = g.Key.CommercialDelPrice,
                        CommercialFobPrice = g.Key.CommercialFobPrice,
                        CommodityDelPrice = g.Key.CommodityDelPrice,
                        CommodityFobPrice = g.Key.CommodityFobPrice,
                        Pua = g.Key.Pua,
                        FfsPrice = g.Key.FfsPrice,
                        NoiPrice = g.Key.NoiPrice,
                        Ptv = g.Key.Ptv,
                        InternalNotes = g.Key.InternalNotes,
                        ManufacturerNotes = g.Key.ManufacturerNotes,
                        MetaJson = null,
                        ProductProposalStatusId = statusToPersist,
                        CreatedBy = modifiedBy
                    });
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                // Create ProposalProductHistory records for Created/Updated items
                try
                {
                    var currentRows = await _db.ProposalProducts
                        .Where(x => x.ProposalId == id)
                        .ToListAsync();

                    var prevByProd = prevProducts.ToDictionary(p => p.ProductId, p => p);
                    var histories = new List<ProposalProductHistory>();

                    foreach (var pp in currentRows)
                    {
                        var currSnapshot = new
                        {
                            pp.ProductId,
                            pp.PriceTypeId,
                            pp.Quantity,
                            pp.AmendmentActionId,
                            pp.Uom,
                            pp.BillbacksAllowed,
                            pp.Allowance,
                            pp.CommercialDelPrice,
                            pp.CommercialFobPrice,
                            pp.CommodityDelPrice,
                            pp.CommodityFobPrice,
                            pp.Pua,
                            pp.FfsPrice,
                            pp.NoiPrice,
                            pp.Ptv,
                            pp.ProductProposalStatusId,
                            pp.InternalNotes,
                            pp.ManufacturerNotes
                        };
                        string currJson = JsonSerializer.Serialize(currSnapshot);

                        prevByProd.TryGetValue(pp.ProductId, out var prevSnapshot);
                        string? prevJson = prevSnapshot != null ? JsonSerializer.Serialize(prevSnapshot) : null;

                        if (prevJson == null || !string.Equals(prevJson, currJson, StringComparison.Ordinal))
                        {
                            histories.Add(new ProposalProductHistory
                            {
                                ProposalProductId = pp.Id,
                                ChangeType = prevJson == null ? "Created" : "Updated",
                                PreviousJson = prevJson,
                                CurrentJson = currJson,
                                ChangedBy = modifiedBy,
                                ChangedDate = DateTime.UtcNow
                            });
                        }
                    }

                    if (histories.Count > 0)
                    {
                        _db.ProposalProductHistories.AddRange(histories);
                        await _db.SaveChangesAsync();
                    }
                }
                catch
                {
                    // Swallow history errors to avoid blocking main update
                }

                return MapToDto(entity);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<ProposalDto> CloneProposalAsync(int id, string createdBy)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new ArgumentException("Proposal not found");
            var cloneDto = MapToDto(existing);
            cloneDto.Id = 0;
            cloneDto.Title = existing.Title + " (Clone)";
            var createDto = new ProposalCreateDto
            {
                Title = cloneDto.Title,
                ProposalTypeId = cloneDto.ProposalTypeId,
                ProposalStatusId = cloneDto.ProposalStatusId,
                ManufacturerId = cloneDto.ManufacturerId,
                StartDate = cloneDto.StartDate,
                EndDate = cloneDto.EndDate,
                InternalNotes = cloneDto.InternalNotes,
                DistributorIds = cloneDto.DistributorIds,
                IndustryIds = cloneDto.IndustryIds,
                OpcoIds = cloneDto.OpcoIds,
                Products = cloneDto.Products.Select(p => new ProposalProductCreateDto
                {
                    ProductId = p.ProductId,
                    PriceTypeId = p.PriceTypeId,
                    Quantity = p.Quantity,
                    MetaJson = p.MetaJson,
                    ProductProposalStatusId = p.ProductProposalStatusId,
                    Uom = p.Uom,
                    BillbacksAllowed = p.BillbacksAllowed,
                    Allowance = p.Allowance,
                    CommercialDelPrice = p.CommercialDelPrice,
                    CommercialFobPrice = p.CommercialFobPrice,
                    CommodityDelPrice = p.CommodityDelPrice,
                    CommodityFobPrice = p.CommodityFobPrice,
                    Pua = p.Pua,
                    FfsPrice = p.FfsPrice,
                    NoiPrice = p.NoiPrice,
                    Ptv = p.Ptv,
                    InternalNotes = p.InternalNotes,
                    ManufacturerNotes = p.ManufacturerNotes
                }).ToList()
            };
            return await CreateProposalAsync(createDto, createdBy);
        }

        public async Task<bool> SubmitProposalAsync(int id, string userId)
        {
            var entity = await _db.Proposals.FindAsync(id) ?? throw new ArgumentException("Proposal not found");
            var from = entity.ProposalStatusId;
            // Simplified: set Submitted if exists (seed will include)
            var submitted = await _db.ProposalStatuses.FirstOrDefaultAsync(s => s.Name == "Submitted");
            if (submitted == null) return false;
            entity.ProposalStatusId = submitted.Id;
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.UtcNow;
            _db.ProposalStatusHistories.Add(new ProposalStatusHistory { ProposalId = id, FromStatusId = from, ToStatusId = submitted.Id, ChangedBy = userId });
            await _db.SaveChangesAsync();

            // Send email notification to NPP admin users (System Administrator, Contract Manager)
            try
            {
                var adminRoleNames = new[] { "System Administrator", "Contract Manager" };
                var adminUsers = await _db.Users
                    .Where(u => u.IsActive && u.UserRoles.Any(ur => adminRoleNames.Contains(ur.Role.Name)))
                    .Select(u => new { u.Email, u.FirstName, u.LastName })
                    .ToListAsync();

                foreach (var admin in adminUsers)
                {
                    await _emailService.SendProposalSubmittedEmailAsync(
                        admin.Email,
                        $"{admin.FirstName} {admin.LastName}",
                        entity.Title,
                        entity.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send proposal submitted email notifications for proposal {ProposalId}", id);
            }

            return true;
        }

        public async Task<bool> AcceptProductsAsync(int id, string userId)
        {
            var entity = await _db.Proposals.FindAsync(id) ?? throw new ArgumentException("Proposal not found");
            var completed = await _db.ProposalStatuses.FirstOrDefaultAsync(s => s.Name == "Completed");
            if (completed == null) return false;
            var from = entity.ProposalStatusId;
            entity.ProposalStatusId = completed.Id;
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.UtcNow;
            _db.ProposalStatusHistories.Add(new ProposalStatusHistory { ProposalId = id, FromStatusId = from, ToStatusId = completed.Id, ChangedBy = userId });
            await _db.SaveChangesAsync();

            // Send email notification to manufacturer users
            await SendManufacturerNotificationAsync(entity, (email, name) =>
                _emailService.SendProposalAcceptedEmailAsync(email, name, entity.Title, entity.Id));

            return true;
        }

        public async Task<int> BatchCreateAsync(IEnumerable<ProposalCreateDto> items, string createdBy)
        {
            int count = 0;
            foreach (var dto in items)
            {
                await CreateProposalAsync(dto, createdBy);
                count++;
            }
            return count;
        }
        public async Task<bool> RejectProposalAsync(int id, string userId, string? rejectReason)
        {
            var entity = await _db.Proposals.FindAsync(id) ?? throw new ArgumentException("Proposal not found");
            var completed = await _db.ProposalStatuses.FirstOrDefaultAsync(s => s.Name == "Completed");
            if (completed == null) return false;

            var from = entity.ProposalStatusId;
            entity.ProposalStatusId = completed.Id;
            entity.RejectReason = string.IsNullOrWhiteSpace(rejectReason) ? null : rejectReason.Trim();
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.UtcNow;

            _db.ProposalStatusHistories.Add(new ProposalStatusHistory
            {
                ProposalId = id,
                FromStatusId = from,
                ToStatusId = completed.Id,
                ChangedBy = userId,
                Comment = string.IsNullOrWhiteSpace(rejectReason) ? "Rejected" : $"Rejected: {rejectReason}"
            });

            await _db.SaveChangesAsync();

            // Send email notification to manufacturer users
            await SendManufacturerNotificationAsync(entity, (email, name) =>
                _emailService.SendProposalRejectedEmailAsync(email, name, entity.Title, entity.Id, entity.RejectReason));

            return true;
        }

        private async Task SendManufacturerNotificationAsync(Proposal proposal, Func<string, string, Task<bool>> sendEmail)
        {
            if (!proposal.ManufacturerId.HasValue) return;
            try
            {
                var allowedUserIds = await _userManufacturerRepo.GetUserIdsForManufacturerAsync(proposal.ManufacturerId.Value);
                var idSet = new HashSet<int>(allowedUserIds);

                var manufacturerUsers = await _db.Users
                    .Where(u => u.IsActive
                        && u.UserRoles.Any(ur => ur.Role.Name == "Manufacturer")
                        && idSet.Contains(u.Id))
                    .Select(u => new { u.Email, u.FirstName, u.LastName })
                    .ToListAsync();

                foreach (var user in manufacturerUsers)
                {
                    await sendEmail(user.Email, $"{user.FirstName} {user.LastName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send proposal notification emails for proposal {ProposalId}", proposal.Id);
            }
        }


        private static ProposalDto MapToDto(Proposal p)
        {
            return new ProposalDto
            {
                Id = p.Id,
                Title = p.Title,
                ProposalTypeId = p.ProposalTypeId,
                ProposalStatusId = p.ProposalStatusId,
                ProposalStatusName = p.ProposalStatus != null ? p.ProposalStatus.Name : null,
                ManufacturerId = p.ManufacturerId,
                ManufacturerName = p.Manufacturer != null ? p.Manufacturer.Name : null,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                DueDate = p.DueDate,
                InternalNotes = p.InternalNotes,
                RejectReason = p.RejectReason,
                IsActive = p.IsActive,
                AmendedContractId = p.AmendedContractId,
                CreatedDate = p.CreatedDate,
                ModifiedDate = p.ModifiedDate,
                CreatedBy = p.CreatedBy,
                ModifiedBy = p.ModifiedBy,
                Products = p.Products.Select(pp => new ProposalProductDto
                {
                    ProductId = pp.ProductId,
                    PriceTypeId = pp.PriceTypeId,
                    Quantity = pp.Quantity,
                    MetaJson = pp.MetaJson,
                    ProductProposalStatusId = pp.ProductProposalStatusId,
                    AmendmentActionId = pp.AmendmentActionId,
                    Uom = pp.Uom,
                    BillbacksAllowed = pp.BillbacksAllowed,
                    Allowance = pp.Allowance,
                    CommercialDelPrice = pp.CommercialDelPrice,
                    CommercialFobPrice = pp.CommercialFobPrice,
                    CommodityDelPrice = pp.CommodityDelPrice,
                    CommodityFobPrice = pp.CommodityFobPrice,
                    Pua = pp.Pua,
                    FfsPrice = pp.FfsPrice,
                    NoiPrice = pp.NoiPrice,
                    Ptv = pp.Ptv,
                    InternalNotes = pp.InternalNotes,
                    ManufacturerNotes = pp.ManufacturerNotes
                }).ToList(),
                DistributorIds = p.Distributors.Select(d => d.DistributorId).ToList(),
                IndustryIds = p.Industries.Select(i => i.IndustryId).ToList(),
                OpcoIds = p.Opcos.Select(o => o.OpCoId).ToList()
            };
        }
    }
}

