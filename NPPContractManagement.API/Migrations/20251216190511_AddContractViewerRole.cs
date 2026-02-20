using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContractViewerRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if role already exists before inserting (for dev database compatibility)
            migrationBuilder.Sql(@"
                INSERT INTO `Roles` (`Id`, `CreatedBy`, `CreatedDate`, `Description`, `IsActive`, `ModifiedBy`, `ModifiedDate`, `Name`)
                SELECT 6, 'System', '2025-01-01 00:00:00', 'View contracts and run reports', TRUE, NULL, NULL, 'Contract Viewer'
                WHERE NOT EXISTS (SELECT 1 FROM `Roles` WHERE `Id` = 6);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
