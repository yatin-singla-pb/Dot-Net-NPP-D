using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRedistributorFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRedistributor",
                table: "OpCos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRedistributor",
                table: "Distributors",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsRedistributor",
                value: false);

            migrationBuilder.UpdateData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsRedistributor",
                value: false);

            migrationBuilder.UpdateData(
                table: "Distributors",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsRedistributor",
                value: false);

            migrationBuilder.UpdateData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsRedistributor",
                value: false);

            migrationBuilder.UpdateData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsRedistributor",
                value: false);

            migrationBuilder.UpdateData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsRedistributor",
                value: false);

            migrationBuilder.UpdateData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsRedistributor",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRedistributor",
                table: "OpCos");

            migrationBuilder.DropColumn(
                name: "IsRedistributor",
                table: "Distributors");
        }
    }
}
