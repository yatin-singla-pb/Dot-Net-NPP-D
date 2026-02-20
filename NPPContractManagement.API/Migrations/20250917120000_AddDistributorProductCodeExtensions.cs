using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NPPContractManagement.API.Data;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250917120000_AddDistributorProductCodeExtensions")]
    public partial class AddDistributorProductCodeExtensions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns if not exist
            migrationBuilder.AddColumn<bool>(
                name: "CatchWeight",
                table: "DistributorProductCodes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EBrand",
                table: "DistributorProductCodes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            // Drop old composite index if exists
            migrationBuilder.Sql(@"SET @idx := (SELECT INDEX_NAME FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND INDEX_NAME = 'IX_DistributorProductCodes_DistributorId_ProductId_DistributorCode' LIMIT 1);
                                  SET @sql := IF(@idx IS NOT NULL, 'DROP INDEX IX_DistributorProductCodes_DistributorId_ProductId_DistributorCode ON DistributorProductCodes;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            // Create new unique indexes as per SRS
            migrationBuilder.CreateIndex(
                name: "IX_DistributorProductCodes_DistributorId_DistributorCode",
                table: "DistributorProductCodes",
                columns: new[] { "DistributorId", "DistributorCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistributorProductCodes_DistributorId_ProductId",
                table: "DistributorProductCodes",
                columns: new[] { "DistributorId", "ProductId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new indexes
            migrationBuilder.Sql(@"SET @idx := (SELECT INDEX_NAME FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND INDEX_NAME = 'IX_DistributorProductCodes_DistributorId_DistributorCode' LIMIT 1);
                                  SET @sql := IF(@idx IS NOT NULL, 'DROP INDEX IX_DistributorProductCodes_DistributorId_DistributorCode ON DistributorProductCodes;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            migrationBuilder.Sql(@"SET @idx := (SELECT INDEX_NAME FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND INDEX_NAME = 'IX_DistributorProductCodes_DistributorId_ProductId' LIMIT 1);
                                  SET @sql := IF(@idx IS NOT NULL, 'DROP INDEX IX_DistributorProductCodes_DistributorId_ProductId ON DistributorProductCodes;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            // Recreate old composite unique index (if desired)
            migrationBuilder.CreateIndex(
                name: "IX_DistributorProductCodes_DistributorId_ProductId_DistributorCode",
                table: "DistributorProductCodes",
                columns: new[] { "DistributorId", "ProductId", "DistributorCode" },
                unique: true);

            // Drop added columns
            migrationBuilder.DropColumn(
                name: "CatchWeight",
                table: "DistributorProductCodes");

            migrationBuilder.DropColumn(
                name: "EBrand",
                table: "DistributorProductCodes");
        }
    }
}

