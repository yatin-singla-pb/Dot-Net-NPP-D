using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingProposalHistoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Repair migration: ensure critical Proposal tables exist in PascalCase even if
            // prior rename/create migrations were marked applied but did not actually run.
            // MySQL supports CREATE TABLE IF NOT EXISTS, which makes this idempotent.

            // 1) AmendmentActions (lookup)
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `AmendmentActions` (
              `Id` int NOT NULL AUTO_INCREMENT,
              `Name` varchar(100) NOT NULL,
              `IsActive` tinyint(1) NOT NULL,
              CONSTRAINT `PK_AmendmentActions` PRIMARY KEY (`Id`)
            ) CHARACTER SET = utf8mb4;");
            migrationBuilder.Sql(@"INSERT INTO `AmendmentActions` (`Id`,`Name`,`IsActive`) VALUES
              (1,'Add',1),(2,'Update',1),(3,'Remove',1)
              ON DUPLICATE KEY UPDATE `Name`=VALUES(`Name`), `IsActive`=VALUES(`IsActive`);");

            // 2) ProposalStatusHistory (audit trail of status changes)
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `ProposalStatusHistory` (
              `Id` int NOT NULL AUTO_INCREMENT,
              `ProposalId` int NOT NULL,
              `FromStatusId` int NOT NULL,
              `ToStatusId` int NOT NULL,
              `Comment` varchar(500) NULL,
              `ChangedDate` datetime(6) NOT NULL,
              `ChangedBy` varchar(100) NOT NULL,
              CONSTRAINT `PK_ProposalStatusHistory` PRIMARY KEY (`Id`),
              CONSTRAINT `FK_ProposalStatusHistory_Proposals_ProposalId` FOREIGN KEY (`ProposalId`) REFERENCES `Proposals` (`Id`) ON DELETE CASCADE,
              CONSTRAINT `FK_ProposalStatusHistory_ProposalStatuses_FromStatusId` FOREIGN KEY (`FromStatusId`) REFERENCES `ProposalStatuses` (`Id`) ON DELETE CASCADE,
              CONSTRAINT `FK_ProposalStatusHistory_ProposalStatuses_ToStatusId` FOREIGN KEY (`ToStatusId`) REFERENCES `ProposalStatuses` (`Id`) ON DELETE CASCADE
            ) CHARACTER SET = utf8mb4;");

            // 3) ProposalProductHistory (audit trail for proposal products)
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `ProposalProductHistory` (
              `Id` int NOT NULL AUTO_INCREMENT,
              `ProposalProductId` int NOT NULL,
              `ChangeType` varchar(50) NOT NULL,
              `PreviousJson` longtext NULL,
              `CurrentJson` longtext NULL,
              `ChangedDate` datetime(6) NOT NULL,
              `ChangedBy` varchar(100) NOT NULL,
              CONSTRAINT `PK_ProposalProductHistory` PRIMARY KEY (`Id`),
              CONSTRAINT `FK_ProposalProductHistory_ProposalProducts_ProposalProductId` FOREIGN KEY (`ProposalProductId`) REFERENCES `ProposalProducts` (`Id`) ON DELETE CASCADE
            ) CHARACTER SET = utf8mb4;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop only the tables created by this repair migration (safe rollback)
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS `ProposalProductHistory`;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS `ProposalStatusHistory`;");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS `AmendmentActions`;");
        }
    }
}
