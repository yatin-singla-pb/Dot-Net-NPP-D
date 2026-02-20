using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class EnsureRejectReasonColumnOnProposalsPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure Proposals table has RejectReason column (DevDb was missing it)
            migrationBuilder.Sql(@"SET @stmt = (
    SELECT IF(
        (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
         WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Proposals' AND COLUMN_NAME = 'RejectReason') = 0,
        'ALTER TABLE `Proposals` ADD COLUMN `RejectReason` varchar(1000) NULL',
        'SELECT 1'
    )
);
PREPARE s1 FROM @stmt; EXECUTE s1; DEALLOCATE PREPARE s1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: we won't drop the column in Down to avoid data loss if present
        }
    }
}
