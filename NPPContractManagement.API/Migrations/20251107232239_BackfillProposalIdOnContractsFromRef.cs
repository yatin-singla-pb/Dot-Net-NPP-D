using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class BackfillProposalIdOnContractsFromRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill Contracts.ProposalId for legacy rows using ManufacturerReferenceNumber patterns
            // 1) Exact pattern: PROPOSAL-<id>
            migrationBuilder.Sql(@"
UPDATE Contracts c
JOIN Proposals p
  ON LOWER(c.ManufacturerReferenceNumber) = CONCAT('proposal-', CAST(p.Id AS CHAR))
SET c.ProposalId = p.Id
WHERE c.ProposalId IS NULL;
");

            // 2) Flexible patterns: PROPOSAL <sep> <id>, e.g., PROPOSAL 123, PROPOSAL:#123, PROPOSAL:123
            migrationBuilder.Sql(@"
UPDATE Contracts c
JOIN Proposals p
  ON LOWER(c.ManufacturerReferenceNumber) REGEXP CONCAT('proposal[-[:space:]#:]*', CAST(p.Id AS CHAR), '([^0-9]|$)')
SET c.ProposalId = p.Id
WHERE c.ProposalId IS NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert only rows that match our derivation patterns to avoid touching unrelated data
            migrationBuilder.Sql(@"
UPDATE Contracts c
JOIN Proposals p
  ON LOWER(c.ManufacturerReferenceNumber) = CONCAT('proposal-', CAST(p.Id AS CHAR))
SET c.ProposalId = NULL
WHERE c.ProposalId = p.Id;
");

            migrationBuilder.Sql(@"
UPDATE Contracts c
JOIN Proposals p
  ON LOWER(c.ManufacturerReferenceNumber) REGEXP CONCAT('proposal[-[:space:]#:]*', CAST(p.Id AS CHAR), '([^0-9]|$)')
SET c.ProposalId = NULL
WHERE c.ProposalId = p.Id;
");
        }
    }
}
