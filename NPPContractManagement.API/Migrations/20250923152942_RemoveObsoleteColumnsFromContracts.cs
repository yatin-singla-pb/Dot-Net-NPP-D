using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveObsoleteColumnsFromContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FKs first
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Distributors_DistributorId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Manufacturers_ManufacturerId",
                table: "Contracts");

            // Drop indexes next
            migrationBuilder.DropIndex(
                name: "IX_Contracts_DistributorId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ManufacturerId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ContractNumber",
                table: "Contracts");

            // Drop columns
            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DistributorId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Terms",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contracts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add columns with original types
            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "Contracts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Contracts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "DistributorId",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManufacturerId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "Contracts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Terms",
                table: "Contracts",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Contracts",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractNumber",
                table: "Contracts",
                column: "ContractNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_DistributorId",
                table: "Contracts",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ManufacturerId",
                table: "Contracts",
                column: "ManufacturerId");

            // Re-add FKs
            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Distributors_DistributorId",
                table: "Contracts",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Manufacturers_ManufacturerId",
                table: "Contracts",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
