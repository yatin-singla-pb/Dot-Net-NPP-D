using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class ApplyOrphanedChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename 'Distributor' role to 'Headless' (idempotent)
            migrationBuilder.Sql(@"UPDATE `Roles` SET `Name` = 'Headless', `Description` = 'Headless contact with no login access' WHERE `Name` = 'Distributor' AND NOT EXISTS (SELECT 1 FROM (SELECT `Name` FROM `Roles` WHERE `Name` = 'Headless') AS t);");

            // Add velocity exception action columns (idempotent via IF NOT EXISTS)
            migrationBuilder.Sql(@"SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'VelocityJobRows' AND COLUMN_NAME = 'action_status');
SET @sql = IF(@col_exists = 0, 'ALTER TABLE `VelocityJobRows` ADD COLUMN `action_status` varchar(30) NULL', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            migrationBuilder.Sql(@"SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'VelocityJobRows' AND COLUMN_NAME = 'action_notes');
SET @sql = IF(@col_exists = 0, 'ALTER TABLE `VelocityJobRows` ADD COLUMN `action_notes` text NULL', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            migrationBuilder.Sql(@"SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'VelocityJobRows' AND COLUMN_NAME = 'action_taken_by');
SET @sql = IF(@col_exists = 0, 'ALTER TABLE `VelocityJobRows` ADD COLUMN `action_taken_by` varchar(100) NULL', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            migrationBuilder.Sql(@"SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'VelocityJobRows' AND COLUMN_NAME = 'action_taken_at');
SET @sql = IF(@col_exists = 0, 'ALTER TABLE `VelocityJobRows` ADD COLUMN `action_taken_at` datetime(6) NULL', 'SELECT 1');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert 'Headless' role back to 'Distributor'
            migrationBuilder.Sql(@"UPDATE `Roles` SET `Name` = 'Distributor', `Description` = 'Distributor user access' WHERE `Name` = 'Headless' AND NOT EXISTS (SELECT 1 FROM (SELECT `Name` FROM `Roles` WHERE `Name` = 'Distributor') AS t);");

            migrationBuilder.DropColumn(name: "action_status", table: "VelocityJobRows");
            migrationBuilder.DropColumn(name: "action_notes", table: "VelocityJobRows");
            migrationBuilder.DropColumn(name: "action_taken_by", table: "VelocityJobRows");
            migrationBuilder.DropColumn(name: "action_taken_at", table: "VelocityJobRows");
        }
    }
}
