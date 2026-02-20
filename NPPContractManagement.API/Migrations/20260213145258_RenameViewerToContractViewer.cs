using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameViewerToContractViewer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure AlwaysList column is nullable before seed data updates (needed for LocalDb where column may still be NOT NULL)
            migrationBuilder.Sql(@"ALTER TABLE `Products` MODIFY COLUMN `AlwaysList` tinyint(1) NULL;");

            // Rename 'Viewer' role to 'Contract Viewer' (idempotent: only if 'Viewer' exists and 'Contract Viewer' does not)
            migrationBuilder.Sql(@"UPDATE `Roles` SET `Name` = 'Contract Viewer', `Description` = 'View contracts and run reports' WHERE `Name` = 'Viewer' AND NOT EXISTS (SELECT 1 FROM (SELECT `Name` FROM `Roles` WHERE `Name` = 'Contract Viewer') AS t);");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "AlwaysList",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "AlwaysList",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "AlwaysList",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "AlwaysList",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "AlwaysList",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "AlwaysList",
                value: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "AlwaysList",
                value: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "AlwaysList",
                value: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "AlwaysList",
                value: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "AlwaysList",
                value: false);
        }
    }
}
