using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using NPPContractManagement.API.DTOs;
using NPPContractManagement.API.Models;
using NPPContractManagement.API.Repositories;
using NPPContractManagement.API.Services;
using Xunit;

namespace NPPContractManagement.API.Tests
{
    public class ContractVersionWorkflowTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;
            var ctx = new ApplicationDbContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        }

        private static (ApplicationDbContext ctx, Contract contract) SeedBasicContract(ApplicationDbContext ctx)
        {
            var manu = new Manufacturer { Name = "M1" };
            var industry = new Industry { Name = "I1" };
            var product = new Product { Name = "P1", SKU = "SKU1", Manufacturer = manu };
            ctx.Manufacturers.Add(manu);
            ctx.Industries.Add(industry);
            ctx.Products.Add(product);

            var contract = new Contract
            {
                ContractNumber = "C-100",
                Title = "Base Contract",
                Description = "Desc",
                Manufacturer = manu,
                Industry = industry,
                Status = ContractStatus.Draft,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddYears(1),
                CreatedBy = "seed",
                CreatedDate = DateTime.UtcNow,
                CurrentVersionNumber = 1
            };
            ctx.Contracts.Add(contract);
            ctx.SaveChanges();

            var v1 = new ContractVersion
            {
                ContractId = contract.Id,
                VersionNumber = 1,
                Title = "v1",
                IsCurrentVersion = true,
                CreatedBy = "seed",
                CreatedDate = DateTime.UtcNow
            };
            ctx.ContractVersions.Add(v1);
            ctx.SaveChanges();
            return (ctx, contract);
        }

        private static ContractService CreateService(ApplicationDbContext ctx)
        {
            var contractRepo = new ContractRepository(ctx);
            var versionRepo = new ContractVersionRepository(ctx);
            var manuRepo = new ManufacturerRepository(ctx);
            var distRepo = new DistributorRepository(ctx);
            var opcoRepo = new OpCoRepository(ctx);
            var industryRepo = new IndustryRepository(ctx);
            return new ContractService(contractRepo, versionRepo, manuRepo, distRepo, opcoRepo, industryRepo);
        }

        [Fact]
        public async Task Create_New_Version_With_Prices()
        {
            using var ctx = CreateContext();
            var (_, contract) = SeedBasicContract(ctx);
            var service = CreateService(ctx);

            var prodId = ctx.Products.Select(p => p.Id).First();

            var req = new CreateContractVersionRequest
            {
                Title = "v2",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddYears(1),
                ChangeReason = "Amend",
                Prices =
                {
                    new CreateContractVersionPriceRequest { ProductId = prodId, Price = 12.34m, PriceType = "Net", UOM = "EA" }
                }
            };

            var created = await service.CreateVersionAsync(contract.Id, req, "tester");

            created.VersionNumber.Should().Be(2);
            created.Prices.Should().HaveCount(1);
            created.Prices[0].Price.Should().Be(12.34m);

            // verify persisted
            var v2 = await ctx.ContractVersions.Include(v => v.Prices).FirstOrDefaultAsync(v => v.ContractId == contract.Id && v.VersionNumber == 2);
            v2.Should().NotBeNull();
            v2!.Prices.Should().HaveCount(1);
        }

        [Fact]
        public async Task Duplicate_From_Source_Version_When_No_Prices_Provided()
        {
            using var ctx = CreateContext();
            var (_, contract) = SeedBasicContract(ctx);
            var service = CreateService(ctx);

            var prodId = ctx.Products.Select(p => p.Id).First();

            // add one price to v1
            var v1 = await ctx.ContractVersions.FirstAsync(v => v.ContractId == contract.Id && v.VersionNumber == 1);
            ctx.ContractVersionPrices.Add(new ContractVersionPrice
            {
                ContractVersionId = v1.Id,
                ProductId = prodId,
                Price = 9.99m,
                PriceType = "Net",
                UOM = "EA",
                CreatedOn = DateTime.UtcNow
            });
            await ctx.SaveChangesAsync();

            var req = new CreateContractVersionRequest
            {
                Title = "v2",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddYears(1),
                ChangeReason = "Duplicate",
                SourceVersionId = v1.Id
            };

            var created = await service.CreateVersionAsync(contract.Id, req, "tester");

            created.VersionNumber.Should().Be(2);
            created.Prices.Should().HaveCount(1);
            created.Prices[0].Price.Should().Be(9.99m);
        }
    }
}

