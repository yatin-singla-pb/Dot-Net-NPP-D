using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePriceAndProductProposalSeed_20251009 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Normalize Price Types to the required set
            migrationBuilder.Sql(@"INSERT INTO PriceTypes (Id, Name, IsActive) VALUES
                (1,'Guaranteed Price',1),
                (2,'Published List Price at Time of Purchase',1),
                (3,'Product Suspended',1),
                (4,'Product Discontinued',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");

            // Normalize Product Proposal Statuses to the required set
            migrationBuilder.Sql(@"INSERT INTO ProductProposalStatuses (Id, Name, IsActive) VALUES
                (1,'Requested',1),
                (2,'Accepted',1),
                (3,'Rejected',1),
                (4,'Proposed',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Price Types to previous set
            migrationBuilder.Sql(@"INSERT INTO PriceTypes (Id, Name, IsActive) VALUES
                (1,'Commercial',1),
                (2,'Commodity',1),
                (3,'FFS',1),
                (4,'NOI',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");

            // Revert Product Proposal Statuses to previous set (and remove Proposed if unused)
            migrationBuilder.Sql(@"INSERT INTO ProductProposalStatuses (Id, Name, IsActive) VALUES
                (1,'Pending',1),
                (2,'Accepted',1),
                (3,'Rejected',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");
            // Best-effort cleanup of the extra id if it exists
            migrationBuilder.Sql("DELETE FROM ProductProposalStatuses WHERE Id = 4;");
        }
    }
}
