using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;
using NPPContractManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace NPPContractManagement.API.Services
{
    public class ContractService : IContractService
    {
        private readonly ApplicationDbContext _db;
        private readonly IContractRepository _contractRepository;
        private readonly IContractVersionRepository _contractVersionRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IDistributorRepository _distributorRepository;
        private readonly IOpCoRepository _opCoRepository;
        private readonly IIndustryRepository _industryRepository;

        private readonly IProductRepository _productRepository;
        private readonly IContractVersionProductRepository _cvProductRepo;
        private readonly IContractDistributorVersionRepository _cvDistributorRepo;
        private readonly IContractIndustryVersionRepository _cvIndustryRepo;
        private readonly IContractOpCoVersionRepository _cvOpCoRepo;
        private readonly IContractManufacturerVersionRepository _cvManufacturerRepo;
        private readonly IContractPriceService _contractPriceService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ContractService> _logger;

        public ContractService(
            ApplicationDbContext db,
            IContractRepository contractRepository,
            IContractVersionRepository contractVersionRepository,
            IManufacturerRepository manufacturerRepository,
            IDistributorRepository distributorRepository,
            IOpCoRepository opCoRepository,
            IIndustryRepository industryRepository,
            IProductRepository productRepository,
            IContractVersionProductRepository cvProductRepo,
            IContractDistributorVersionRepository cvDistributorRepo,
            IContractIndustryVersionRepository cvIndustryRepo,
            IContractOpCoVersionRepository cvOpCoRepo,
            IContractManufacturerVersionRepository cvManufacturerRepo,
            IContractPriceService contractPriceService,
            IEmailService emailService,
            ILogger<ContractService> logger)
        {
            _db = db;
            _contractRepository = contractRepository;
            _contractVersionRepository = contractVersionRepository;
            _manufacturerRepository = manufacturerRepository;
            _distributorRepository = distributorRepository;
            _opCoRepository = opCoRepository;
            _industryRepository = industryRepository;
            _productRepository = productRepository;
            _cvProductRepo = cvProductRepo;
            _cvDistributorRepo = cvDistributorRepo;
            _cvIndustryRepo = cvIndustryRepo;
            _cvOpCoRepo = cvOpCoRepo;
            _cvManufacturerRepo = cvManufacturerRepo;
            _contractPriceService = contractPriceService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<ContractDto>> GetAllContractsAsync()
        {
            var contracts = await _contractRepository.GetAllAsync();
            return contracts.Select(MapToDto);
        }

        public async Task<ContractDto?> GetContractByIdAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            return contract != null ? MapToDto(contract) : null;
        }

        public async Task<ContractDto> CreateContractAsync(CreateContractDto createContractDto, string createdBy)
        {
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // ManufacturerId is no longer part of the payload; manufacturers inferred from selected products

                // Skip unique ContractNumber check (column may not exist in current DB schema)

                // Validate distributors exist
                foreach (var distributorId in createContractDto.DistributorIds)
                {
                    var distributor = await _distributorRepository.GetByIdAsync(distributorId);
                    if (distributor == null)
                    {
                        throw new ArgumentException($"Distributor with ID {distributorId} not found");
                    }
                }

                // Validate OpCos exist
                foreach (var opCoId in createContractDto.OpCoIds)
                {
                    var opCo = await _opCoRepository.GetByIdAsync(opCoId);
                    if (opCo == null)
                    {
                        throw new ArgumentException($"OpCo with ID {opCoId} not found");
                    }
                }

                // Validate industries exist
                foreach (var industryId in createContractDto.IndustryIds)
                {
                    var industry = await _industryRepository.GetByIdAsync(industryId);
                    if (industry == null)
                    {
                        throw new ArgumentException($"Industry with ID {industryId} not found");
                    }
                }

                // Unique constraint previously used Manufacturer; skip here or implement new rules as needed

                var contract = new Contract
                {
                    Name = !string.IsNullOrWhiteSpace(createContractDto.Name) ? createContractDto.Name!.Trim() : string.Empty,
                    StartDate = createContractDto.StartDate,
                    EndDate = createContractDto.EndDate,
                    ForeignContractId = string.IsNullOrWhiteSpace(createContractDto.ForeignContractId) ? null : createContractDto.ForeignContractId!.Trim(),
                    // SuspendedDate is system-managed via Suspend/Unsuspend actions
                    InternalNotes = string.IsNullOrWhiteSpace(createContractDto.InternalNotes) ? null : createContractDto.InternalNotes!.Trim(),
                    // Manufacturer/Entegra metadata
                    ManufacturerReferenceNumber = string.IsNullOrWhiteSpace(createContractDto.ManufacturerReferenceNumber) ? null : createContractDto.ManufacturerReferenceNumber!.Trim(),
                    ManufacturerBillbackName = string.IsNullOrWhiteSpace(createContractDto.ManufacturerBillbackName) ? null : createContractDto.ManufacturerBillbackName!.Trim(),
                    ManufacturerTermsAndConditions = string.IsNullOrWhiteSpace(createContractDto.ManufacturerTermsAndConditions) ? null : createContractDto.ManufacturerTermsAndConditions,
                    ManufacturerNotes = string.IsNullOrWhiteSpace(createContractDto.ManufacturerNotes) ? null : createContractDto.ManufacturerNotes!.Trim(),
                    ContactPerson = string.IsNullOrWhiteSpace(createContractDto.ContactPerson) ? null : createContractDto.ContactPerson!.Trim(),
                    EntegraContractType = string.IsNullOrWhiteSpace(createContractDto.EntegraContractType) ? null : createContractDto.EntegraContractType!.Trim(),
                    EntegraVdaProgram = string.IsNullOrWhiteSpace(createContractDto.EntegraVdaProgram) ? null : createContractDto.EntegraVdaProgram!.Trim(),
                    // IsSuspended is system-managed via Suspend/Unsuspend actions
                    SendToPerformance = createContractDto.SendToPerformance,
                    CurrentVersionNumber = 0,
                        ProposalId = createContractDto.ProposalId,

                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                var createdContract = await _contractRepository.AddAsync(contract);

            // Add distributor relationships
            foreach (var distributorId in createContractDto.DistributorIds)
            {
                var contractDistributor = new ContractDistributor
                {
                    ContractId = createdContract.Id,
                    DistributorId = distributorId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };
                createdContract.ContractDistributors.Add(contractDistributor);
            }

            // Add OpCo relationships
            foreach (var opCoId in createContractDto.OpCoIds)
            {
                var contractOpCo = new ContractOpCo
                {
                    ContractId = createdContract.Id,
                    OpCoId = opCoId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };
                createdContract.ContractOpCos.Add(contractOpCo);
            }

            // Add industry relationships (distinct, 1x each)
            _logger.LogInformation("CreateContract: Received IndustryIds: {Ids}", string.Join(",", createContractDto.IndustryIds ?? new List<int>()));
            foreach (var industryId in (createContractDto.IndustryIds ?? new List<int>()).Distinct())
            {
                var ci = new ContractIndustry
                {
                    ContractId = createdContract.Id,
                    IndustryId = industryId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };
                createdContract.ContractIndustries.Add(ci);
            }
            _logger.LogInformation("CreateContract: Persisting ContractIndustries for ContractId {ContractId}: {Ids}", createdContract.Id, string.Join(",", createdContract.ContractIndustries.Select(ci => ci.IndustryId)));

            // Create initial version (new schema)
            var contractVersion = new ContractVersion
            {
                ContractId = createdContract.Id,
                VersionNumber = 0,
                Name = string.IsNullOrWhiteSpace(createdContract.Name) ? "Initial Version" : createdContract.Name,
                ForeignContractId = createdContract.ForeignContractId,
                SendToPerformance = createdContract.SendToPerformance,
                IsSuspended = createdContract.IsSuspended,
                SuspendedDate = createdContract.SuspendedDate,
                InternalNotes = createdContract.InternalNotes,
                // Manufacturer/Entegra metadata snapshot
                ManufacturerReferenceNumber = createdContract.ManufacturerReferenceNumber,
                ManufacturerBillbackName = createdContract.ManufacturerBillbackName,
                ManufacturerTermsAndConditions = createdContract.ManufacturerTermsAndConditions,
                ManufacturerNotes = createdContract.ManufacturerNotes,
                ContactPerson = createdContract.ContactPerson,
                EntegraContractType = createdContract.EntegraContractType,
                EntegraVdaProgram = createdContract.EntegraVdaProgram,
                StartDate = createdContract.StartDate,
                EndDate = createdContract.EndDate,
                AssignedDate = DateTime.UtcNow,
                AssignedBy = createdBy
            };
            createdContract.ContractVersions.Add(contractVersion);


            // Populate version relationship tables and base product/price data
            var versionNumber = 0;

            // DEBUG: incoming counts before business rules
            _logger.LogDebug("[ContractService] Incoming ProductIds={countProducts}, Prices={countPrices}",
                createContractDto.ProductIds?.Count ?? 0,
                createContractDto.Prices?.Count ?? 0);
            if (createContractDto.Prices != null)
            {
                var preview = string.Join(",", createContractDto.Prices.Take(10).Select(p => $"{p.ProductId}:{p.PriceType}"));
                _logger.LogDebug("[ContractService] Incoming Prices preview (first 10): {preview}", preview);
            }

            var enableRules = !string.Equals(Environment.GetEnvironmentVariable("FEATURE_CONTRACT_PRICE_TYPE_RULES"), "false", StringComparison.OrdinalIgnoreCase);
            _logger.LogDebug("[ContractService] FEATURE_CONTRACT_PRICE_TYPE_RULES enabled={enabled}", enableRules);

            // BUSINESS RULE: Normalize/Filter incoming price requests per proposal price types (apply BEFORE creating entities)
            if (enableRules && createContractDto.Prices != null && createContractDto.Prices.Count > 0)
            {
                var transformed = new List<CreateContractPriceRequest>();
                foreach (var priceReq in createContractDto.Prices)
                {
                    var rawPt = (priceReq.PriceType ?? string.Empty).Trim();
                    var decision = MapPriceTypeWithNearest(rawPt);
                    _logger.LogInformation("[ContractService] PriceType mapping: raw='{raw}', mapped='{mapped}', excluded={excluded}, reason='{reason}'",
                        rawPt, decision.Mapped ?? string.Empty, decision.Excluded, decision.Reason);
                    if (decision.Excluded)
                    {
                        continue;
                    }
                    priceReq.PriceType = decision.Mapped ?? "Contract Price";
                    transformed.Add(priceReq);
                }
                var beforeCount = createContractDto.Prices.Count;
                createContractDto.Prices = transformed;
                var afterCount = createContractDto.Prices.Count;
                var afterPreview = string.Join(",", createContractDto.Prices.Take(10).Select(p => $"{p.ProductId}:{p.PriceType}"));
                _logger.LogDebug("[ContractService] Prices filtered: before={before}, after={after}. Preview: {preview}", beforeCount, afterCount, afterPreview);

                // Align ProductIds to only those that still have kept price lines (avoid orphan products without prices)
                if (createContractDto.ProductIds != null && createContractDto.ProductIds.Count > 0)
                {
                    var beforeProducts = createContractDto.ProductIds.Count;
                    var allowed = new HashSet<int>(createContractDto.Prices.Select(p => p.ProductId));
                    createContractDto.ProductIds = createContractDto.ProductIds.Where(id => allowed.Contains(id)).Distinct().ToList();
                    _logger.LogDebug("[ContractService] ProductIds aligned to price lines: before={before}, after={after}", beforeProducts, createContractDto.ProductIds.Count);
                }

                // Early exit: if no eligible items remain, skip contract creation gracefully
                if (createContractDto.Prices.Count == 0)
                {
                    _logger.LogInformation("[ContractService] No eligible items after price-type rules. Skipping contract creation for '{name}'.", createContractDto.Name);
                    await tx.RollbackAsync();
                    return new ContractDto { Id = 0, Name = createContractDto.Name ?? string.Empty };
                }
            }

            _logger.LogInformation("[ContractService] Proceeding to create contract '{name}' with ProductIds={pCount}, PriceLines={plCount}",
                createContractDto.Name,
                createContractDto.ProductIds?.Count ?? 0,
                createContractDto.Prices?.Count ?? 0);

            // Manufacturer assignments inferred from selected products
            var manufacturerIds = new HashSet<int>();
            if (createContractDto.ProductIds != null)
            {
                foreach (var pid in createContractDto.ProductIds.Distinct())
                {
                    var p = await _productRepository.GetByIdAsync(pid);
                    if (p != null && p.ManufacturerId > 0) manufacturerIds.Add(p.ManufacturerId);
                }
            }
            foreach (var mId in manufacturerIds)
            {
                // Base assignment
                _db.ContractManufacturers.Add(new ContractManufacturer
                {
                    ContractId = createdContract.Id,
                    ManufacturerId = mId,
                    CurrentVersionNumber = versionNumber,
                    AssignedBy = createdBy,
                    AssignedDate = DateTime.UtcNow
                });

                // Version assignment
                await _cvManufacturerRepo.CreateAsync(new ContractManufacturerVersion
                {
                    ContractId = createdContract.Id,
                    ManufacturerId = mId,
                    VersionNumber = versionNumber,
                    AssignedBy = createdBy,
                    AssignedDate = DateTime.UtcNow
                });
            }

            // Distributor version assignments
            foreach (var distributorId in createContractDto.DistributorIds.Distinct())
            {
                await _cvDistributorRepo.CreateAsync(new ContractDistributorVersion
                {
                    ContractId = createdContract.Id,
                    DistributorId = distributorId,
                    VersionNumber = versionNumber,
                    AssignedBy = createdBy,
                    AssignedDate = DateTime.UtcNow
                });
            }

            // OpCo version assignments
            foreach (var opCoId in createContractDto.OpCoIds.Distinct())
            {
                await _cvOpCoRepo.CreateAsync(new ContractOpCoVersion
                {
                    ContractId = createdContract.Id,
                    OpCoId = opCoId,
                    VersionNumber = versionNumber,
                    AssignedBy = createdBy,
                    AssignedDate = DateTime.UtcNow
                });
            }

            // Industry version assignments mirror base table exactly
            foreach (var industryId in createdContract.ContractIndustries.Select(ci => ci.IndustryId).Distinct())
            {
                await _cvIndustryRepo.CreateAsync(new ContractIndustryVersion
                {
                    ContractId = createdContract.Id,
                    IndustryId = industryId,
                    VersionNumber = versionNumber,
                    AssignedBy = createdBy,
                    AssignedDate = DateTime.UtcNow
                });
            }

            // Base ContractProducts and version products
            if (createContractDto.ProductIds != null)
            {
                foreach (var productId in createContractDto.ProductIds.Distinct())
                {
                    var product = await _productRepository.GetByIdAsync(productId);
                    if (product == null) throw new ArgumentException($"Product with ID {productId} not found");

                    createdContract.ContractProducts.Add(new ContractProduct
                    {
                        ContractId = createdContract.Id,
                        ProductId = productId,
                        CurrentVersionNumber = versionNumber,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.UtcNow
                    });

                    await _cvProductRepo.CreateAsync(new ContractVersionProduct
                    {
                        ContractId = createdContract.Id,
                        ProductId = productId,
                        VersionNumber = versionNumber,
                        AssignedBy = createdBy,
                        AssignedDate = DateTime.UtcNow
                    });
                }
            }

            // Base ContractPrices and version price records (stored in ContractVersionPrice table)
            if (createContractDto.Prices != null)
            {
                _logger.LogDebug("[ContractService] Creating {count} ContractPrice lines", createContractDto.Prices.Count);
                foreach (var priceReq in createContractDto.Prices)
                {
                    // Defensive mapping at persistence time to ensure correct PriceType even if earlier steps or client sent blanks
                    var rawPt = (priceReq.PriceType ?? string.Empty).Trim();
                    var decision = MapPriceTypeWithNearest(rawPt);
                    _logger.LogInformation("[ContractService] Final price mapping: productId={pid}, raw='{raw}', mapped='{mapped}', excluded={excluded}, reason='{reason}'",
                        priceReq.ProductId, rawPt, decision.Mapped ?? string.Empty, decision.Excluded, decision.Reason);
                    if (decision.Excluded)
                    {
                        // Skip discontinued at final write barrier
                        continue;
                    }
                    priceReq.PriceType = decision.Mapped ?? priceReq.PriceType ?? "Contract Price";

                    _logger.LogDebug("[ContractService] Price line -> ProductId={pid}, PriceType='{pt}', UOM='{uom}'",
                        priceReq.ProductId, priceReq.PriceType, priceReq.UOM);

                    if (priceReq.VersionNumber <= 0) priceReq.VersionNumber = versionNumber;
                    priceReq.ContractId = createdContract.Id;
                    var createdPrice = await _contractPriceService.CreateAsync(priceReq, createdBy);

                    // Mirror the created ContractPrice into the version table ContractVersionPrice
                    _db.ContractVersionPrices.Add(new ContractVersionPrice
                    {
                        ContractId = createdContract.Id,
                        PriceId = createdPrice.Id,
                        ProductId = createdPrice.ProductId,
                        VersionNumber = createdPrice.VersionNumber,
                        PriceType = createdPrice.PriceType,
                        Allowance = createdPrice.Allowance,
                        CommercialDelPrice = createdPrice.CommercialDelPrice,
                        CommercialFobPrice = createdPrice.CommercialFobPrice,
                        CommodityDelPrice = createdPrice.CommodityDelPrice,
                        CommodityFobPrice = createdPrice.CommodityFobPrice,
                        UOM = createdPrice.UOM,
                        EstimatedQty = createdPrice.EstimatedQty,
                        BillbacksAllowed = createdPrice.BillbacksAllowed,
                        PUA = createdPrice.PUA,
                        FFSPrice = createdPrice.FFSPrice,
                        NOIPrice = createdPrice.NOIPrice,
                        PTV = createdPrice.PTV,
                        InternalNotes = createdPrice.InternalNotes,
                        AssignedBy = createdBy,
                        AssignedDate = DateTime.UtcNow
                    });
                }
            }

            createdContract.CurrentVersionNumber = versionNumber;

            // Audit: Ensure each ContractProduct has a corresponding price line and log the final mapped PriceType
            try
            {
                var priceByProduct = new Dictionary<int, string>(createContractDto.Prices?.Count ?? 0);
                foreach (var pr in (createContractDto.Prices ?? new List<CreateContractPriceRequest>()))
                {
                    var pt = (pr.PriceType ?? string.Empty).Trim();
                    priceByProduct[pr.ProductId] = pt;
                }
                foreach (var cp in (createdContract.ContractProducts ?? new List<ContractProduct>()))
                {
                    if (priceByProduct.TryGetValue(cp.ProductId, out var pt))
                    {
                        _logger.LogInformation("[ContractService] Product→PriceType mapping: productId={pid}, priceType='{pt}'", cp.ProductId, pt);
                    }
                    else
                    {
                        _logger.LogWarning("[ContractService] Product→PriceType mapping: productId={pid} has no price line after filtering (likely discontinued/excluded)", cp.ProductId);
                    }
                }
            }
            catch (Exception exMap)
            {
                _logger.LogWarning(exMap, "[ContractService] Failed to emit Product→PriceType mapping audit");
            }

            await _contractRepository.UpdateAsync(createdContract);
            await tx.CommitAsync();
            return MapToDto(createdContract);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ContractService] CreateContract failed. ProductIds={pCount}, PriceLines={plCount}",
                createContractDto.ProductIds?.Count ?? 0,
                createContractDto.Prices?.Count ?? 0);
            await tx.RollbackAsync();
            throw;
        }
        }


        public async Task<ContractDto> UpdateContractAsync(int id, UpdateContractDto updateContractDto, string modifiedBy)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                throw new ArgumentException("Contract not found", nameof(id));
            }

            // Manufacturer validation not applicable; manufacturer is inferred from products in versions


            // Validate distributors exist
            foreach (var distributorId in updateContractDto.DistributorIds)
            {
                var distributor = await _distributorRepository.GetByIdAsync(distributorId);
                if (distributor == null)
                {
                    throw new ArgumentException($"Distributor with ID {distributorId} not found");
                }
            }

            // Validate OpCos exist
            foreach (var opCoId in updateContractDto.OpCoIds)
            {
                var opCo = await _opCoRepository.GetByIdAsync(opCoId);
                if (opCo == null)
                {
                    throw new ArgumentException($"OpCo with ID {opCoId} not found");
                }
            }

            // Validate industries exist
            foreach (var industryId in updateContractDto.IndustryIds)
            {
                var industry = await _industryRepository.GetByIdAsync(industryId);
                if (industry == null)
                {
                    throw new ArgumentException($"Industry with ID {industryId} not found");
                }
            }

            // Unique constraint previously relied on Manufacturer; skipping here or adjust as needed

            // Update name if provided
            if (!string.IsNullOrWhiteSpace(updateContractDto.Name))
            {
                contract.Name = updateContractDto.Name!.Trim();
            }

            // External refs
            if (!string.IsNullOrWhiteSpace(updateContractDto.ForeignContractId))
            {
                contract.ForeignContractId = updateContractDto.ForeignContractId.Trim();
            }

            // SuspendedDate is system-managed via Suspend/Unsuspend actions
            contract.StartDate = updateContractDto.StartDate;
            contract.EndDate = updateContractDto.EndDate;

            // Internal notes
            if (!string.IsNullOrWhiteSpace(updateContractDto.InternalNotes))
            {
                contract.InternalNotes = updateContractDto.InternalNotes.Trim();
            }

            // Manufacturer/Entegra metadata
            if (!string.IsNullOrWhiteSpace(updateContractDto.ManufacturerReferenceNumber))
                contract.ManufacturerReferenceNumber = updateContractDto.ManufacturerReferenceNumber!.Trim();
            if (!string.IsNullOrWhiteSpace(updateContractDto.ManufacturerBillbackName))
                contract.ManufacturerBillbackName = updateContractDto.ManufacturerBillbackName!.Trim();
            if (!string.IsNullOrWhiteSpace(updateContractDto.ManufacturerTermsAndConditions))
                contract.ManufacturerTermsAndConditions = updateContractDto.ManufacturerTermsAndConditions;
            if (!string.IsNullOrWhiteSpace(updateContractDto.ManufacturerNotes))
                contract.ManufacturerNotes = updateContractDto.ManufacturerNotes!.Trim();
            if (!string.IsNullOrWhiteSpace(updateContractDto.ContactPerson))
                contract.ContactPerson = updateContractDto.ContactPerson!.Trim();
            if (!string.IsNullOrWhiteSpace(updateContractDto.EntegraContractType))
                contract.EntegraContractType = updateContractDto.EntegraContractType!.Trim();
            if (!string.IsNullOrWhiteSpace(updateContractDto.EntegraVdaProgram))
                contract.EntegraVdaProgram = updateContractDto.EntegraVdaProgram!.Trim();

            // Ignored: IsSuspended is managed via explicit Suspend/Unsuspend actions
            // contract.IsSuspended = updateContractDto.IsSuspended;
            contract.SendToPerformance = updateContractDto.SendToPerformance;
            contract.ModifiedDate = DateTime.UtcNow;
            contract.ModifiedBy = modifiedBy;

            // Replace-and-insert pattern with explicit deletes to avoid partial saves
            var distributorIds = (updateContractDto.DistributorIds ?? new List<int>()).Distinct().ToList();
            var opCoIds = (updateContractDto.OpCoIds ?? new List<int>()).Distinct().ToList();
            var industryIds = (updateContractDto.IndustryIds ?? new List<int>()).Distinct().ToList();

            _logger.LogInformation("UpdateContract {Id}: Incoming DistributorIds={Distributors} OpCoIds={OpCos} IndustryIds={Industries}",
                contract.Id,
                string.Join(",", distributorIds),
                string.Join(",", opCoIds),
                string.Join(",", industryIds));

            // Distributors
            var existingCD = await _db.ContractDistributors.Where(x => x.ContractId == contract.Id).ToListAsync();
            _db.ContractDistributors.RemoveRange(existingCD);
            foreach (var distributorId in distributorIds)
            {
                _db.ContractDistributors.Add(new ContractDistributor
                {
                    ContractId = contract.Id,
                    DistributorId = distributorId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = modifiedBy
                });
            }

            // OpCos
            var existingCO = await _db.ContractOpCos.Where(x => x.ContractId == contract.Id).ToListAsync();
            _db.ContractOpCos.RemoveRange(existingCO);
            foreach (var opCoId in opCoIds)
            {
                _db.ContractOpCos.Add(new ContractOpCo
                {
                    ContractId = contract.Id,
                    OpCoId = opCoId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = modifiedBy
                });
            }

            // Industries
            var existingCI = await _db.ContractIndustries.Where(x => x.ContractId == contract.Id).ToListAsync();
            _db.ContractIndustries.RemoveRange(existingCI);
            foreach (var industryId in industryIds)
            {
                _db.ContractIndustries.Add(new ContractIndustry
                {
                    ContractId = contract.Id,
                    IndustryId = industryId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = modifiedBy
                });
            }

            // Mirror to ContractIndustriesVersion for current version
            var currentVersion = contract.CurrentVersionNumber > 0 ? contract.CurrentVersionNumber : 1;
            var existingCIV = await _db.ContractIndustriesVersion
                .Where(v => v.ContractId == contract.Id && v.VersionNumber == currentVersion)
                .ToListAsync();
            _db.ContractIndustriesVersion.RemoveRange(existingCIV);
            foreach (var industryId in industryIds)
            {
                _db.ContractIndustriesVersion.Add(new ContractIndustryVersion
                {
                    ContractId = contract.Id,
                    IndustryId = industryId,
                    VersionNumber = currentVersion,
                    AssignedBy = modifiedBy,
                    AssignedDate = DateTime.UtcNow
                });
            }

            _logger.LogInformation("UpdateContract {Id}: Persisting ContractIndustries: {Ids}", contract.Id, string.Join(",", industryIds));

            await _db.SaveChangesAsync();

            var fresh = await _contractRepository.GetByIdAsync(contract.Id);
            return MapToDto(fresh!);
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return false;
            }

            await _contractRepository.DeleteAsync(id);
            return true;
        }


        public async Task<IEnumerable<ContractDto>> GetContractsByManufacturerIdAsync(int manufacturerId)
        {
            var contracts = await _contractRepository.GetByManufacturerIdAsync(manufacturerId);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetContractsByStatusAsync(int status)
        {
            var contracts = await _contractRepository.GetByStatusAsync((ContractStatus)status);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetContractsByDistributorIdAsync(int distributorId)
        {
            var contracts = await _contractRepository.GetByDistributorIdAsync(distributorId);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetContractsByOpCoIdAsync(int opCoId)
        {
            var contracts = await _contractRepository.GetByOpCoIdAsync(opCoId);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetContractsByIndustryIdAsync(int industryId)
        {
            var contracts = await _contractRepository.GetByIndustryIdAsync(industryId);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetSuspendedContractsAsync()
        {
            var contracts = await _contractRepository.GetSuspendedContractsAsync();
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetContractsForPerformanceAsync()
        {
            var contracts = await _contractRepository.GetContractsForPerformanceAsync();
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetExpiringContractsAsync(DateTime beforeDate)
        {
            var contracts = await _contractRepository.GetExpiringContractsAsync(beforeDate);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetExpiringContractsWithoutProposalsAsync(int daysThreshold)
        {
            var contracts = await _contractRepository.GetExpiringContractsWithoutProposalsAsync(daysThreshold);
            return contracts.Select(MapToDto);
        }

        public async Task<bool> SuspendContractAsync(int id, string modifiedBy)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return false;
            }

            contract.IsSuspended = true;
            contract.SuspendedDate = DateTime.UtcNow;
            contract.ModifiedDate = DateTime.UtcNow;
            contract.ModifiedBy = modifiedBy;

            await _contractRepository.UpdateAsync(contract);
            return true;
        }

        public async Task<bool> UnsuspendContractAsync(int id, string modifiedBy)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return false;
            }

            contract.IsSuspended = false;
            contract.SuspendedDate = null;
            contract.ModifiedDate = DateTime.UtcNow;
            contract.ModifiedBy = modifiedBy;

            await _contractRepository.UpdateAsync(contract);
            return true;
        }

        public async Task<bool> SendToPerformanceAsync(int id, string modifiedBy)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return false;
            }

            contract.SendToPerformance = true;
            contract.ModifiedDate = DateTime.UtcNow;
            contract.ModifiedBy = modifiedBy;

            await _contractRepository.UpdateAsync(contract);
            return true;
        }

        public async Task<bool> RemoveFromPerformanceAsync(int id, string modifiedBy)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                return false;
            }

            contract.SendToPerformance = false;
            contract.ModifiedDate = DateTime.UtcNow;
            contract.ModifiedBy = modifiedBy;

            await _contractRepository.UpdateAsync(contract);
            return true;
        }

        public async Task<ContractDto> CreateNewVersionAsync(int contractId, string changeReason, string createdBy)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
            {
                throw new ArgumentException("Contract not found", nameof(contractId));
            }

            // Compute next version number without relying on Contract.CurrentVersionNumber
            var nextVersionNumber = (contract.ContractVersions.Any() ? contract.ContractVersions.Max(v => v.VersionNumber) : 0) + 1;

            // Create new version (legacy endpoint: set minimal fields with sensible defaults)
            var newVersion = new ContractVersion
            {
                ContractId = contractId,
                VersionNumber = nextVersionNumber,
                Name = !string.IsNullOrWhiteSpace(contract.Name) ? contract.Name : ($"Contract v{nextVersionNumber}"),
                SendToPerformance = contract.SendToPerformance,
                IsSuspended = contract.IsSuspended,
                SuspendedDate = contract.SuspendedDate,
                InternalNotes = contract.InternalNotes,
                // Manufacturer/Entegra metadata snapshot
                ManufacturerReferenceNumber = contract.ManufacturerReferenceNumber,
                ManufacturerBillbackName = contract.ManufacturerBillbackName,
                ManufacturerTermsAndConditions = contract.ManufacturerTermsAndConditions,
                ManufacturerNotes = contract.ManufacturerNotes,
                ContactPerson = contract.ContactPerson,
                EntegraContractType = contract.EntegraContractType,
                EntegraVdaProgram = contract.EntegraVdaProgram,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                AssignedDate = DateTime.UtcNow,
                AssignedBy = createdBy
            };



            contract.ContractVersions.Add(newVersion);
            contract.ModifiedDate = DateTime.UtcNow;
            contract.ModifiedBy = createdBy;

            var updatedContract = await _contractRepository.UpdateAsync(contract);
            return MapToDto(updatedContract);
        }

        public async Task<(IEnumerable<ContractDto> Contracts, int TotalCount)> SearchContractsAsync(string searchTerm, int? manufacturerId = null, int? status = null, int? distributorId = null, int? industryId = null, bool? isSuspended = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10, IEnumerable<int>? allowedManufacturerIds = null)
        {
            var statusEnum = status.HasValue ? (ContractStatus)status.Value : null as ContractStatus?;
            var contracts = await _contractRepository.SearchAsync(searchTerm, manufacturerId, statusEnum, distributorId, industryId, isSuspended, startDate, endDate, page, pageSize, allowedManufacturerIds);
            var totalCount = await _contractRepository.GetCountAsync(manufacturerId, statusEnum, distributorId, industryId, isSuspended, startDate, endDate, allowedManufacturerIds);

            return (contracts.Select(MapToDto), totalCount);
        }

        private static ContractDto MapToDto(Contract contract)
        {
            var manufacturers = contract.ContractManufacturers?.Select(cm => new ManufacturerDto
            {
                Id = cm.Manufacturer.Id,
                Name = cm.Manufacturer.Name,
            }).ToList() ?? new List<ManufacturerDto>();

            // Populate legacy fields for backward compatibility (use first manufacturer)
            var firstManufacturer = manufacturers.FirstOrDefault();

            return new ContractDto
            {
                Id = contract.Id,
                Name = contract.Name,
                ForeignContractId = contract.ForeignContractId,
                SuspendedDate = contract.SuspendedDate,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                InternalNotes = contract.InternalNotes,
                // Manufacturer/Entegra metadata
                ManufacturerReferenceNumber = contract.ManufacturerReferenceNumber,
                ManufacturerBillbackName = contract.ManufacturerBillbackName,
                ManufacturerTermsAndConditions = contract.ManufacturerTermsAndConditions,
                ManufacturerNotes = contract.ManufacturerNotes,
                ContactPerson = contract.ContactPerson,
                EntegraContractType = contract.EntegraContractType,
                EntegraVdaProgram = contract.EntegraVdaProgram,
                IsSuspended = contract.IsSuspended,
                SendToPerformance = contract.SendToPerformance,
                CurrentVersionNumber = contract.CurrentVersionNumber,
                CreatedDate = contract.CreatedDate,
                ModifiedDate = contract.ModifiedDate,
                CreatedBy = contract.CreatedBy,
                ModifiedBy = contract.ModifiedBy,
                ProposalId = contract.ProposalId,
                ProposalTitle = contract.Proposal?.Title,

                // Legacy fields for backward compatibility
                ManufacturerId = firstManufacturer?.Id,
                ManufacturerName = firstManufacturer?.Name,

                Distributors = contract.ContractDistributors?.Select(cd => new DistributorDto
                {
                    Id = cd.Distributor.Id,
                    Name = cd.Distributor.Name,
                }).ToList() ?? new List<DistributorDto>(),
                OpCos = contract.ContractOpCos?.Select(co => new OpCoDto
                {
                    Id = co.OpCo.Id,
                    Name = co.OpCo.Name,
                }).ToList() ?? new List<OpCoDto>(),
                Industries = contract.ContractIndustries?.Select(ci => new IndustryDto
                {
                    Id = ci.Industry.Id,
                    Name = ci.Industry.Name,
                }).ToList() ?? new List<IndustryDto>(),
                Manufacturers = manufacturers,
                Products = contract.ContractProducts?.Select(ci => new ProductDto
                {
                    Id = ci.Product.Id,
                    Name = ci.Product.Name,
                }).ToList() ?? new List<ProductDto>()
            };
        }

        private static ContractVersionDto MapVersionToDto(Models.ContractVersion v)
        {
            return new ContractVersionDto
            {
                Id = v.Id,
                ContractId = v.ContractId,
                VersionNumber = v.VersionNumber,
                // New schema
                Name = v.Name ?? string.Empty,
                ForeignContractId = v.ForeignContractId,
                SendToPerformance = v.SendToPerformance,
                IsSuspended = v.IsSuspended,
                SuspendedDate = v.SuspendedDate,
                InternalNotes = v.InternalNotes,
                // Manufacturer/Entegra metadata
                ManufacturerReferenceNumber = v.ManufacturerReferenceNumber,
                ManufacturerBillbackName = v.ManufacturerBillbackName,
                ManufacturerTermsAndConditions = v.ManufacturerTermsAndConditions,
                ManufacturerNotes = v.ManufacturerNotes,
                ContactPerson = v.ContactPerson,
                EntegraContractType = v.EntegraContractType,
                EntegraVdaProgram = v.EntegraVdaProgram,
                // Common
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                CreatedDate = v.AssignedDate ?? v.StartDate,
                CreatedBy = v.AssignedBy,
                Prices = v.Prices?.Select(p => new ContractVersionPriceDto
                {
                    Id = p.Id,
                    ProductId = p.ProductId,
                    ProductName = p.Product?.Name,
                    Price = p.Price,
                    PriceType = p.PriceType,
                    UOM = p.UOM,
                    Tier = p.Tier,
                    EffectiveFrom = p.EffectiveFrom,
                    EffectiveTo = p.EffectiveTo,

                    // Extended detailed fields
                    Allowance = p.Allowance,
                    CommercialDelPrice = p.CommercialDelPrice,
                    CommercialFobPrice = p.CommercialFobPrice,
                    CommodityDelPrice = p.CommodityDelPrice,
                    CommodityFobPrice = p.CommodityFobPrice,
                    EstimatedQty = p.EstimatedQty,
                    BillbacksAllowed = p.BillbacksAllowed,
                    PUA = p.PUA,
                    FFSPrice = p.FFSPrice,
                    NOIPrice = p.NOIPrice,
                    PTV = p.PTV,
                    InternalNotes = p.InternalNotes
                }).ToList() ?? new List<ContractVersionPriceDto>()
            };
        }

        public async Task<IEnumerable<ContractVersionDto>> GetVersionsAsync(int contractId)
        {
            var versions = await _contractVersionRepository.GetVersionsAsync(contractId);
            return versions.Select(MapVersionToDto);
        }

        public async Task<ContractVersionDto?> GetVersionAsync(int contractId, int versionId)
        {
            var version = await _contractVersionRepository.GetVersionAsync(contractId, versionId);
            return version == null ? null : MapVersionToDto(version);
        }

        public async Task<ContractVersionDto> CreateVersionAsync(int contractId, CreateContractVersionRequest request, string createdBy)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null) throw new ArgumentException("Contract not found", nameof(contractId));

            // Validation
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Name is required", nameof(request.Name));
            if (request.IsSuspended && request.SuspendedDate == null) throw new ArgumentException("SuspendedDate is required when IsSuspended is true", nameof(request.SuspendedDate));

            var version = new Models.ContractVersion
            {
                Name = request.Name.Trim(),
                ForeignContractId = request.ForeignContractId,
                SendToPerformance = request.SendToPerformance,
                IsSuspended = request.IsSuspended,
                SuspendedDate = request.SuspendedDate,
                InternalNotes = request.InternalNotes,
                // Manufacturer/Entegra metadata
                ManufacturerReferenceNumber = request.ManufacturerReferenceNumber,
                ManufacturerBillbackName = request.ManufacturerBillbackName,
                ManufacturerTermsAndConditions = request.ManufacturerTermsAndConditions,
                ManufacturerNotes = request.ManufacturerNotes,
                ContactPerson = request.ContactPerson,
                EntegraContractType = request.EntegraContractType,
                EntegraVdaProgram = request.EntegraVdaProgram,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            // Create the new version first (repository assigns VersionNumber)
            var createdVersion = await _contractVersionRepository.CreateVersionAsync(contractId, version, new List<Models.ContractVersionPrice>(), createdBy);
            var newVn = createdVersion.VersionNumber;

            // Update existing ContractPrices in-place and mirror into ContractVersionPrice
            var affectedPrices = new List<ContractPrice>();
            if (request.Prices != null && request.Prices.Count > 0)
            {
                foreach (var p in request.Prices)
                {
                    var existing = await _db.ContractPrices
                        .FirstOrDefaultAsync(x => x.ContractId == contract.Id && x.ProductId == p.ProductId);

                    if (existing != null)
                    {
                        existing.VersionNumber = newVn;
                        existing.PriceType = p.PriceType ?? existing.PriceType;
                        existing.Allowance = p.Allowance;
                        existing.CommercialDelPrice = p.CommercialDelPrice;
                        existing.CommercialFobPrice = p.CommercialFobPrice;
                        existing.CommodityDelPrice = p.CommodityDelPrice;
                        existing.CommodityFobPrice = p.CommodityFobPrice;
                        existing.UOM = p.UOM ?? existing.UOM;
                        existing.EstimatedQty = p.EstimatedQty;
                        existing.BillbacksAllowed = p.BillbacksAllowed;
                        existing.PUA = p.PUA;
                        existing.FFSPrice = p.FFSPrice;
                        existing.NOIPrice = p.NOIPrice;
                        existing.PTV = p.PTV;
                        existing.InternalNotes = p.InternalNotes;
                        existing.ModifiedBy = createdBy;
                        existing.ModifiedDate = DateTime.UtcNow;
                        affectedPrices.Add(existing);
                    }
                }

                await _db.SaveChangesAsync();

                foreach (var cp in affectedPrices)
                {
                    _db.ContractVersionPrices.Add(new ContractVersionPrice
                    {
                        ContractId = cp.ContractId,
                        PriceId = cp.Id,
                        ProductId = cp.ProductId,
                        VersionNumber = cp.VersionNumber,
                        PriceType = cp.PriceType,
                        Allowance = cp.Allowance,
                        CommercialDelPrice = cp.CommercialDelPrice,
                        CommercialFobPrice = cp.CommercialFobPrice,
                        CommodityDelPrice = cp.CommodityDelPrice,
                        CommodityFobPrice = cp.CommodityFobPrice,
                        UOM = cp.UOM,
                        EstimatedQty = cp.EstimatedQty,
                        BillbacksAllowed = cp.BillbacksAllowed,
                        PUA = cp.PUA,
                        FFSPrice = cp.FFSPrice,
                        NOIPrice = cp.NOIPrice,
                        PTV = cp.PTV,
                        InternalNotes = cp.InternalNotes,
                        AssignedBy = createdBy,
                        AssignedDate = DateTime.UtcNow
                    });
                }

                await _db.SaveChangesAsync();
            }
            else
            {
                // No specific prices provided: bump all contract prices to new version and mirror them
                var allPrices = await _db.ContractPrices
                    .Where(x => x.ContractId == contract.Id)
                    .ToListAsync();

                foreach (var cp in allPrices)
                {
                    cp.VersionNumber = newVn;
                    cp.ModifiedBy = createdBy;
                    cp.ModifiedDate = DateTime.UtcNow;
                }
                await _db.SaveChangesAsync();

                foreach (var cp in allPrices)
                {
                    _db.ContractVersionPrices.Add(new ContractVersionPrice
                    {
                        ContractId = cp.ContractId,
                        PriceId = cp.Id,
                        ProductId = cp.ProductId,
                        VersionNumber = cp.VersionNumber,
                        PriceType = cp.PriceType,
                        Allowance = cp.Allowance,
                        CommercialDelPrice = cp.CommercialDelPrice,
                        CommercialFobPrice = cp.CommercialFobPrice,
                        CommodityDelPrice = cp.CommodityDelPrice,
                        CommodityFobPrice = cp.CommodityFobPrice,
                        UOM = cp.UOM,
                        EstimatedQty = cp.EstimatedQty,
                        BillbacksAllowed = cp.BillbacksAllowed,
                        PUA = cp.PUA,
                        FFSPrice = cp.FFSPrice,
                        NOIPrice = cp.NOIPrice,
                        PTV = cp.PTV,
                        InternalNotes = cp.InternalNotes,
                        AssignedBy = createdBy,
                        AssignedDate = DateTime.UtcNow
                    });
                }

                await _db.SaveChangesAsync();
            }

            // Mirror current assignments into version tables and bump main tables' CurrentVersionNumber
            var now = DateTime.UtcNow;

            // Distributors
            var distIds = await _db.ContractDistributors
                .Where(x => x.ContractId == contract.Id)
                .Select(x => x.DistributorId)
                .ToListAsync();
            if (distIds.Count > 0)
            {
                foreach (var id in distIds.Distinct())
                {
                    _db.ContractDistributorsVersion.Add(new ContractDistributorVersion
                    {
                        ContractId = contract.Id,
                        DistributorId = id,
                        VersionNumber = newVn,
                        AssignedBy = createdBy,
                        AssignedDate = now
                    });
                }
                // Update current version on main rows
                await _db.ContractDistributors
                    .Where(x => x.ContractId == contract.Id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.CurrentVersionNumber, newVn));
            }

            // OpCos
            var opcoIds = await _db.ContractOpCos
                .Where(x => x.ContractId == contract.Id)
                .Select(x => x.OpCoId)
                .ToListAsync();
            if (opcoIds.Count > 0)
            {
                foreach (var id in opcoIds.Distinct())
                {
                    _db.ContractOpCosVersion.Add(new ContractOpCoVersion
                    {
                        ContractId = contract.Id,
                        OpCoId = id,
                        VersionNumber = newVn,
                        AssignedBy = createdBy,
                        AssignedDate = now
                    });
                }
                await _db.ContractOpCos
                    .Where(x => x.ContractId == contract.Id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.CurrentVersionNumber, newVn));
            }

            // Industries
            var industryIds = await _db.ContractIndustries
                .Where(x => x.ContractId == contract.Id)
                .Select(x => x.IndustryId)
                .ToListAsync();
            if (industryIds.Count > 0)
            {
                foreach (var id in industryIds.Distinct())
                {
                    _db.ContractIndustriesVersion.Add(new ContractIndustryVersion
                    {
                        ContractId = contract.Id,
                        IndustryId = id,
                        VersionNumber = newVn,
                        AssignedBy = createdBy,
                        AssignedDate = now
                    });
                }
                await _db.ContractIndustries
                    .Where(x => x.ContractId == contract.Id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.CurrentVersionNumber, newVn));
            }

            // Manufacturers
            var manufacturerIds = await _db.ContractManufacturers
                .Where(x => x.ContractId == contract.Id)
                .Select(x => x.ManufacturerId)
                .ToListAsync();
            if (manufacturerIds.Count > 0)
            {
                foreach (var id in manufacturerIds.Distinct())
                {
                    _db.ContractManufacturersVersion.Add(new ContractManufacturerVersion
                    {
                        ContractId = contract.Id,
                        ManufacturerId = id,
                        VersionNumber = newVn,
                        AssignedBy = createdBy,
                        AssignedDate = now
                    });
                }
                await _db.ContractManufacturers
                    .Where(x => x.ContractId == contract.Id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.CurrentVersionNumber, newVn));
            }

            // Products (prefer provided request.Prices, otherwise use current ContractProducts)
            var productIds = new HashSet<int>();
            if (request.Prices != null && request.Prices.Count > 0)
            {
                foreach (var p in request.Prices) if (p.ProductId > 0) productIds.Add(p.ProductId);
            }
            if (productIds.Count == 0)
            {
                var cprod = await _db.ContractProducts
                    .Where(x => x.ContractId == contract.Id)
                    .Select(x => x.ProductId)
                    .ToListAsync();
                foreach (var id in cprod) productIds.Add(id);
            }
            if (productIds.Count > 0)
            {
                foreach (var id in productIds)
                {
                    _db.ContractVersionProducts.Add(new ContractVersionProduct
                    {
                        ContractId = contract.Id,
                        ProductId = id,
                        VersionNumber = newVn,
                        AssignedBy = createdBy,
                        AssignedDate = now
                    });
                }

                // Update current version on base ContractProducts now that snapshot is taken
                await _db.ContractProducts
                    .Where(x => x.ContractId == contract.Id && productIds.Contains(x.ProductId))
                    .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.CurrentVersionNumber, newVn));
            }

            await _db.SaveChangesAsync();

            // Update parent contract header fields and mark new version current
            contract.Name = request.Name.Trim();
            contract.ForeignContractId = request.ForeignContractId;
            contract.SendToPerformance = request.SendToPerformance;
            contract.IsSuspended = request.IsSuspended;
            contract.SuspendedDate = request.SuspendedDate;
            contract.InternalNotes = request.InternalNotes;
            // Manufacturer/Entegra metadata
            contract.ManufacturerReferenceNumber = request.ManufacturerReferenceNumber;
            contract.ManufacturerBillbackName = request.ManufacturerBillbackName;
            contract.ManufacturerTermsAndConditions = request.ManufacturerTermsAndConditions;
            contract.ManufacturerNotes = request.ManufacturerNotes;
            contract.ContactPerson = request.ContactPerson;
            contract.EntegraContractType = request.EntegraContractType;
            contract.EntegraVdaProgram = request.EntegraVdaProgram;
            contract.StartDate = request.StartDate;
            contract.EndDate = request.EndDate;
            contract.CurrentVersionNumber = createdVersion.VersionNumber;
            contract.ModifiedDate = DateTime.UtcNow;
            contract.ModifiedBy = createdBy;
            await _contractRepository.UpdateAsync(contract);

            return MapVersionToDto(createdVersion);
        }

        public async Task<ContractVersionDto> UpdateVersionAsync(int contractId, int versionId, UpdateContractVersionRequest request, string modifiedBy)
        {
            var existing = await _contractVersionRepository.GetVersionAsync(contractId, versionId);
            if (existing == null) throw new ArgumentException("Contract version not found", nameof(versionId));

            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Name is required", nameof(request.Name));
            if (request.IsSuspended && request.SuspendedDate == null) throw new ArgumentException("SuspendedDate is required when IsSuspended is true", nameof(request.SuspendedDate));

            existing.Name = request.Name.Trim();
            existing.ForeignContractId = request.ForeignContractId;
            existing.SendToPerformance = request.SendToPerformance;
            existing.IsSuspended = request.IsSuspended;
            existing.SuspendedDate = request.SuspendedDate;
            existing.InternalNotes = request.InternalNotes;
            // Manufacturer/Entegra metadata
            existing.ManufacturerReferenceNumber = request.ManufacturerReferenceNumber;
            existing.ManufacturerBillbackName = request.ManufacturerBillbackName;
            existing.ManufacturerTermsAndConditions = request.ManufacturerTermsAndConditions;
            existing.ManufacturerNotes = request.ManufacturerNotes;
            existing.ContactPerson = request.ContactPerson;
            existing.EntegraContractType = request.EntegraContractType;
            existing.EntegraVdaProgram = request.EntegraVdaProgram;
            existing.StartDate = request.StartDate;
            existing.EndDate = request.EndDate;

            var updated = await _contractVersionRepository.UpdateVersionAsync(existing, modifiedBy);
            return MapVersionToDto(updated);
        }

        public async Task<ContractVersionDto> CloneVersionByNumberAsync(int contractId, int sourceVersionNumber, string createdBy)
        {
            var versions = await _contractVersionRepository.GetVersionsAsync(contractId);
            var source = versions.FirstOrDefault(v => v.VersionNumber == sourceVersionNumber);
            if (source == null) throw new ArgumentException("Source contract version not found", nameof(sourceVersionNumber));

            var req = new CreateContractVersionRequest
            {
                Name = source.Name,
                ForeignContractId = source.ForeignContractId,
                SendToPerformance = source.SendToPerformance,
                IsSuspended = source.IsSuspended,
                SuspendedDate = source.SuspendedDate,
                InternalNotes = source.InternalNotes,
                // Manufacturer/Entegra metadata
                ManufacturerReferenceNumber = source.ManufacturerReferenceNumber,
                ManufacturerBillbackName = source.ManufacturerBillbackName,
                ManufacturerTermsAndConditions = source.ManufacturerTermsAndConditions,
                ManufacturerNotes = source.ManufacturerNotes,
                ContactPerson = source.ContactPerson,
                EntegraContractType = source.EntegraContractType,
                EntegraVdaProgram = source.EntegraVdaProgram,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                SourceVersionId = source.Id,
                Prices = source.Prices?.Select(p => new CreateContractVersionPriceRequest
                {
                    ProductId = p.ProductId,
                    Price = p.Price,
                    PriceType = p.PriceType,
                    UOM = p.UOM,
                    Tier = p.Tier,
                    EffectiveFrom = p.EffectiveFrom,
                    EffectiveTo = p.EffectiveTo
                }).ToList() ?? new List<CreateContractVersionPriceRequest>()
            };

            return await CreateVersionAsync(contractId, req, createdBy);
        }

        public async Task<object> CompareVersionsAsync(int contractId, int versionANumber, int versionBNumber)
        {
            var versions = await _contractVersionRepository.GetVersionsAsync(contractId);
            var a = versions.FirstOrDefault(v => v.VersionNumber == versionANumber);
            var b = versions.FirstOrDefault(v => v.VersionNumber == versionBNumber);
            if (a == null || b == null) throw new ArgumentException("One or both versions not found");

            Func<DateTime?, string?> fmt = d => d?.ToString("yyyy-MM-dd");

            var headerDiff = new List<object>();
            void AddHeader(string name, object? va, object? vb)
            {
                if ((va == null && vb == null) || Equals(va, vb)) return;
                headerDiff.Add(new { field = name, a = va, b = vb });
            }
            AddHeader("Name", a.Name, b.Name);
            AddHeader("ForeignContractId", a.ForeignContractId, b.ForeignContractId);
            AddHeader("SendToPerformance", a.SendToPerformance, b.SendToPerformance);
            AddHeader("IsSuspended", a.IsSuspended, b.IsSuspended);
            AddHeader("SuspendedDate", fmt(a.SuspendedDate), fmt(b.SuspendedDate));
            AddHeader("StartDate", a.StartDate.ToString("yyyy-MM-dd"), b.StartDate.ToString("yyyy-MM-dd"));
            AddHeader("EndDate", a.EndDate.ToString("yyyy-MM-dd"), b.EndDate.ToString("yyyy-MM-dd"));

            // Prices
            var priceMapA = (a.Prices ?? new List<ContractVersionPrice>()).ToDictionary(p => p.ProductId);
            var priceMapB = (b.Prices ?? new List<ContractVersionPrice>()).ToDictionary(p => p.ProductId);
            var allProductIds = priceMapA.Keys.Union(priceMapB.Keys).Distinct().ToList();
            var prices = new List<object>();
            foreach (var pid in allProductIds)
            {
                priceMapA.TryGetValue(pid, out var pa);
                priceMapB.TryGetValue(pid, out var pb);
                var status = pa == null ? "added" : pb == null ? "removed" :
                    ((pa.Price != pb.Price) || (pa.PriceType != pb.PriceType) || (pa.UOM != pb.UOM) || (pa.Tier != pb.Tier) ? "modified" : "same");
                if (status == "same") continue;
                prices.Add(new
                {
                    productId = pid,
                    a = pa == null ? null : new { pa.Price, pa.PriceType, pa.UOM, pa.Tier },
                    b = pb == null ? null : new { pb.Price, pb.PriceType, pb.UOM, pb.Tier },
                    status
                });
            }

            // Version assignments
            var mfrA = await _cvManufacturerRepo.GetAllAsync(contractId, a.VersionNumber, null);
            var mfrB = await _cvManufacturerRepo.GetAllAsync(contractId, b.VersionNumber, null);
            var distA = await _cvDistributorRepo.GetAllAsync(contractId, a.VersionNumber, null);
            var distB = await _cvDistributorRepo.GetAllAsync(contractId, b.VersionNumber, null);
            var opcoA = await _cvOpCoRepo.GetAllAsync(contractId, a.VersionNumber, null);
            var opcoB = await _cvOpCoRepo.GetAllAsync(contractId, b.VersionNumber, null);
            var indA = await _cvIndustryRepo.GetAllAsync(contractId, a.VersionNumber, null);
            var indB = await _cvIndustryRepo.GetAllAsync(contractId, b.VersionNumber, null);
            var prodA = await _cvProductRepo.GetAllAsync(contractId, a.VersionNumber, null);
            var prodB = await _cvProductRepo.GetAllAsync(contractId, b.VersionNumber, null);

            object DiffIds<T>(IEnumerable<T> aa, IEnumerable<T> bb, Func<T, int> sel)
            {
                var sa = new HashSet<int>(aa.Select(sel));
                var sb = new HashSet<int>(bb.Select(sel));
                var added = sb.Except(sa).ToArray();
                var removed = sa.Except(sb).ToArray();
                return new { added, removed };
            }

            var assignments = new
            {
                manufacturers = DiffIds(mfrA, mfrB, x => x.ManufacturerId),
                distributors = DiffIds(distA, distB, x => x.DistributorId),
                opcos = DiffIds(opcoA, opcoB, x => x.OpCoId),
                industries = DiffIds(indA, indB, x => x.IndustryId),
                products = DiffIds(prodA, prodB, x => x.ProductId)
            };

            return new
            {
                contractId,
                versionA = a.VersionNumber,
                versionB = b.VersionNumber,
                header = headerDiff,
                prices,
                assignments
            };
        }



        // --- PriceType nearest-match helpers -------------------------------------------------------
        private static string NormalizeToken(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            s = s.ToLowerInvariant();
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (char.IsLetter(ch)) sb.Append(ch);
            }
            return sb.ToString();
        }

        private static int LevenshteinDistance(string a, string b)
        {
            if (a == b) return 0;
            if (a.Length == 0) return b.Length;
            if (b.Length == 0) return a.Length;
            var dp = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }
            return dp[a.Length, b.Length];
        }

        private static double Similarity(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return 1.0;
            var dist = LevenshteinDistance(a, b);
            var maxLen = Math.Max(a.Length, b.Length);
            if (maxLen == 0) return 1.0;
            return 1.0 - (double)dist / maxLen;
        }

        private (string? Mapped, bool Excluded, string Reason) MapPriceTypeWithNearest(string raw)
        {
            var norm = NormalizeToken(raw ?? string.Empty);
            // Strong keyword-based decisions first
            if (norm.Contains("discontinu")) return ("Discontinued", false, "keyword 'discontinu' detected");
            if (norm.Contains("suspend")) return ("Suspended", false, "keyword 'suspend' detected");
            if (norm.Contains("guarante")) return ("Contract Price at Time of Purchase", false, "keyword 'guarante' detected");
            if ((norm.Contains("publish") || norm.Contains("public")) && norm.Contains("list") && (norm.Contains("timeofpurchase") || norm.Contains("purchase") || norm.Contains("nobid")))
                return ("List at Time of Purchase/No Bid", false, "published/list/purchase tokens detected");

            // Fuzzy nearest match among canonical targets
            var candidates = new (string Norm, string? Canonical, string Key)[]
            {
                ("discontinued", "Discontinued", "discontinued"),
                ("suspended", "Suspended", "suspended"),
                ("publishedlistpriceattimeofpurchase", "List at Time of Purchase/No Bid", "published-list"),
                ("listattimeofpurchasenobid", "List at Time of Purchase/No Bid", "list-no-bid"),
                ("guaranteedprice", "Contract Price at Time of Purchase", "guaranteed"),
                ("contractpriceattimeofpurchase", "Contract Price at Time of Purchase", "contract-at-purchase"),
                ("contractprice", "Contract Price", "contract")
            };

            // Compute similarities
            var sims = new List<(int idx, double sim)>();
            for (int i = 0; i < candidates.Length; i++)
            {
                sims.Add((i, Similarity(norm, candidates[i].Norm)));
            }
            sims.Sort((a,b) => b.sim.CompareTo(a.sim));

            // Thresholds
            const double excludeStrong = 0.80; // only exclude via fuzzy if very close
            const double mapThreshold = 0.60;   // otherwise map if reasonably close

            if (sims.Count > 0)
            {
                var (bestIdx, bestSim) = sims[0];
                var bestCand = candidates[bestIdx];

                // If best is 'discontinued' but not strong, do NOT exclude; try next best
                if (bestCand.Canonical == null && bestSim < excludeStrong)
                {
                    // try next best non-discontinued
                    var next = sims.FirstOrDefault(t => candidates[t.idx].Canonical != null && t.sim >= mapThreshold);
                    if (next.idx != 0 || next.sim >= mapThreshold)
                    {
                        var nc = candidates[next.idx];
                        return (nc.Canonical!, false, $"fuzzy→{nc.Key} ({next.sim:F2})");
                    }
                    return ("Contract Price", false, $"low-similarity {bestSim:F2}→default");
                }

                if (bestCand.Canonical == null)
                {
                    // strong fuzzy discontinue
                    return (null, true, $"fuzzy→{bestCand.Key} ({bestSim:F2})");
                }

                if (bestSim < mapThreshold)
                    return ("Contract Price", false, $"low-similarity {bestSim:F2}→default");

                return (bestCand.Canonical, false, $"fuzzy→{bestCand.Key} ({bestSim:F2})");
            }

            // Ultimate fallback
            return ("Contract Price", false, "fallback default");
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalContracts = await _db.Contracts.CountAsync();
            var now = DateTime.UtcNow;
            var activeContracts = await _db.Contracts.CountAsync(c => !c.IsSuspended && c.StartDate <= now && c.EndDate >= now);
            var totalIndustries = await _db.Industries.CountAsync(i => i.IsActive);
            var totalDistributors = await _db.Distributors.CountAsync(d => d.IsActive);
            var totalOpCos = await _db.OpCos.CountAsync(o => o.IsActive);
            var totalManufacturers = await _db.Manufacturers.CountAsync(m => m.IsActive);

            return new DashboardStatsDto
            {
                TotalContracts = totalContracts,
                ActiveContracts = activeContracts,
                TotalIndustries = totalIndustries,
                TotalDistributors = totalDistributors,
                TotalOpCos = totalOpCos,
                TotalManufacturers = totalManufacturers
            };
        }

        public async Task<int> SendExpiryNotificationsAsync(int daysThreshold)
        {
            var contracts = await _contractRepository.GetExpiringContractsWithoutProposalsAsync(daysThreshold);
            var expiringList = contracts.ToList();
            if (!expiringList.Any()) return 0;

            // Get admin users (System Administrator + Contract Manager)
            var adminUsers = await _db.Users
                .Where(u => u.IsActive
                    && u.UserRoles.Any(ur => ur.Role.Name == "System Administrator" || ur.Role.Name == "Contract Manager"))
                .Select(u => new { u.Email, u.FirstName, u.LastName })
                .ToListAsync();

            if (!adminUsers.Any()) return 0;

            int sentCount = 0;
            var now = DateTime.UtcNow;

            foreach (var contract in expiringList)
            {
                var daysUntilExpiry = (int)Math.Ceiling((contract.EndDate - now).TotalDays);
                foreach (var admin in adminUsers)
                {
                    try
                    {
                        await _emailService.SendContractExpiryNotificationEmailAsync(
                            admin.Email,
                            $"{admin.FirstName} {admin.LastName}",
                            contract.Name ?? $"Contract #{contract.Id}",
                            contract.Id,
                            contract.EndDate,
                            daysUntilExpiry);
                        sentCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send expiry notification for contract {ContractId} to {Email}", contract.Id, admin.Email);
                    }
                }
            }

            return sentCount;
        }

    }
}
