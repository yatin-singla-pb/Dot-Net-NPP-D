using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NPPContractManagement.API.Data;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250917121500_AddAuditAliasesForDistributorProductCodes")]
    public partial class AddAuditAliasesForDistributorProductCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add CreatedAt if missing, and backfill from CreatedDate
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'CreatedAt');
                                  SET @sql := IF(@col = 0, 'ALTER TABLE DistributorProductCodes ADD COLUMN CreatedAt datetime(6) NULL;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'CreatedAt');
                                  SET @sql := IF(@col = 1, 'UPDATE DistributorProductCodes SET CreatedAt = IFNULL(CreatedDate, NOW()) WHERE CreatedAt IS NULL;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            // Add UpdatedAt if missing, and backfill from ModifiedDate
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedAt');
                                  SET @sql := IF(@col = 0, 'ALTER TABLE DistributorProductCodes ADD COLUMN UpdatedAt datetime(6) NULL;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedAt');
                                  SET @sql := IF(@col = 1, 'UPDATE DistributorProductCodes SET UpdatedAt = ModifiedDate WHERE UpdatedAt IS NULL;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            // Add UpdatedBy if missing, and backfill from ModifiedBy
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedBy');
                                  SET @sql := IF(@col = 0, 'ALTER TABLE DistributorProductCodes ADD COLUMN UpdatedBy varchar(100) NULL;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedBy');
                                  SET @sql := IF(@col = 1, 'UPDATE DistributorProductCodes SET UpdatedBy = ModifiedBy WHERE UpdatedBy IS NULL;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop columns if they exist (MySQL-safe)
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'CreatedAt');
                                  SET @sql := IF(@col = 1, 'ALTER TABLE DistributorProductCodes DROP COLUMN CreatedAt;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedAt');
                                  SET @sql := IF(@col = 1, 'ALTER TABLE DistributorProductCodes DROP COLUMN UpdatedAt;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");
            migrationBuilder.Sql(@"SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'DistributorProductCodes' AND COLUMN_NAME = 'UpdatedBy');
                                  SET @sql := IF(@col = 1, 'ALTER TABLE DistributorProductCodes DROP COLUMN UpdatedBy;', 'SELECT 1');
                                  PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");
        }
    }
}

