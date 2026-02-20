using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using System.Data.Common;

namespace NPPContractManagement.API.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/migrations")] // temporary utility
    public class MigrationsUtilityController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private static readonly string[] PriorMigrations = new []
        {
            "20250910221359_InitialCreate",
            "20250911004018_UpdateSchemaForCompleteDataModel",
            "20250911011758_ForceCreateMissingTables",
            "20250911012256_AddSampleData",
            "20250911013146_ForceRecreateMainTables",
            "20250911014502_AddContractSampleData",
            "20250912090000_AddCustomerAccountExtendedFields",
            "20250912113010_AddMemberAccountExtendedFields",
            "20250912115917_EnsureCustomerAccountExtendedFields",
            "20250912122826_AddContractsIndustryForeignSuspended",
            "20250912134941_AddUserContactStatusFields",
            "20250917120000_AddDistributorProductCodeExtensions",
            "20250917121500_AddAuditAliasesForDistributorProductCodes",
            "20250917152223_AddUserManufacturersTable",
            "20250919130000_AddContractVersionPrices",
            "20250923120000_UpdateContractsSchema_AddNameInternalNotes",
            "20250923142931_UpdateContractsSchema_20250923",
            "20250923152942_RemoveObsoleteColumnsFromContracts",
            "20250923160332_AddContractRelationshipTables",
            "20250923181058_UpdateContractVersionsSchema",
            "20250923203108_RefactorContractProductAddContractPriceTable",
            "20250923235108_AddContractVersionRelationshipTables",
            "20250924174303_RemoveDescriptionAndIndustryFromContracts",
            "20250926052105_AddContractIdToContractPrices",
            "20250926062216_AddProductIdToContractVersionPrice",
            "20250929220305_AddCurrentVersionNumberToContractProducts"
        };

        public MigrationsUtilityController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Danger: intentionally AllowAnonymous but gated by a one-time code
        [HttpPost("baseline")]
        [AllowAnonymous]
        public async Task<ActionResult> Baseline([FromQuery] string code)
        {
            if (code != "proposals-baseline-20251002") return Unauthorized();

            // Ensure history table exists
            await _db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (`MigrationId` varchar(150) NOT NULL, `ProductVersion` varchar(32) NOT NULL, PRIMARY KEY (`MigrationId`)) CHARACTER SET=utf8mb4;");

            foreach (var m in PriorMigrations)
            {
                // Insert if missing
                await _db.Database.ExecuteSqlRawAsync("INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ({0}, {1});", m, "9.0.0");
            }

            return Ok(new { message = "Baseline complete", inserted = PriorMigrations.Length });
        }

        // Returns EF migration status (all/applied/pending) for the current database
        [HttpGet("status")]
        [AllowAnonymous]
        public ActionResult Status([FromQuery] string code)
        {
            if (code != "migrations-status-20251110") return Unauthorized();
            var all = _db.Database.GetMigrations();
            var applied = _db.Database.GetAppliedMigrations();
            var pending = _db.Database.GetPendingMigrations();
            return Ok(new { all = all.ToArray(), applied = applied.ToArray(), pending = pending.ToArray() });
        }

        // Checks existence of Proposals-related tables (PascalCase vs snake_case) and Contracts.ProposalId column
        [HttpGet("check-proposals")]
        [AllowAnonymous]
        public async Task<ActionResult> CheckProposals([FromQuery] string code)
        {
            if (code != "migrations-status-20251110") return Unauthorized();

            var pascalTables = new [] { "Proposals", "ProposalProducts", "ProposalDistributors", "ProposalIndustries", "ProposalOpcos", "ProposalStatuses", "ProposalTypes", "PriceTypes", "ProductProposalStatuses", "AmendmentActions", "ProposalStatusHistory", "ProposalProductHistory" };
            var snakeTables = new [] { "proposals", "proposal_products", "proposal_distributors", "proposal_industries", "proposal_opcos", "proposal_statuses", "proposal_types", "price_types", "product_proposal_statuses", "amendment_actions", "proposal_status_history", "proposal_product_history" };

            var result = new Dictionary<string, object?>();

            await using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            async Task<int> CountTableAsync(string table)
            {
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = '{table}'";
                var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return count;
            }

            var pascal = new Dictionary<string, int>();
            var snake = new Dictionary<string, int>();
            foreach (var t in pascalTables) pascal[t] = await CountTableAsync(t);
            foreach (var t in snakeTables) snake[t] = await CountTableAsync(t);

            // Check Contracts.ProposalId column
            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = DATABASE() AND table_name = 'Contracts' AND column_name = 'ProposalId'";
                var hasProposalId = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
                result["contracts"] = new { hasProposalId };
            }

            result["pascal"] = pascal;
            result["snake"] = snake;
            return Ok(result);
        }
    }
}

