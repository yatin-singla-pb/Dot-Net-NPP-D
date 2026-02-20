using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContractRelationshipTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                table: "ContractOpCos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "ContractOpCos",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentVersionNumber",
                table: "ContractOpCos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                table: "ContractIndustries",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "ContractIndustries",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentVersionNumber",
                table: "ContractIndustries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AssignedBy",
                table: "ContractDistributors",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "ContractDistributors",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentVersionNumber",
                table: "ContractDistributors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContractManufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    CurrentVersionNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractManufacturers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractManufacturers_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractManufacturers_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ContractDistributors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractDistributors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractDistributors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractDistributors",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractIndustries",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractIndustries",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractIndustries",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractOpCos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractOpCos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractOpCos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.UpdateData(
                table: "ContractOpCos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AssignedBy", "AssignedDate", "CurrentVersionNumber" },
                values: new object[] { null, null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_ContractManufacturers_ContractId_ManufacturerId",
                table: "ContractManufacturers",
                columns: new[] { "ContractId", "ManufacturerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractManufacturers_ManufacturerId",
                table: "ContractManufacturers",
                column: "ManufacturerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractManufacturers");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                table: "ContractOpCos");

            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "ContractOpCos");

            migrationBuilder.DropColumn(
                name: "CurrentVersionNumber",
                table: "ContractOpCos");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                table: "ContractIndustries");

            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "ContractIndustries");

            migrationBuilder.DropColumn(
                name: "CurrentVersionNumber",
                table: "ContractIndustries");

            migrationBuilder.DropColumn(
                name: "AssignedBy",
                table: "ContractDistributors");

            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "ContractDistributors");

            migrationBuilder.DropColumn(
                name: "CurrentVersionNumber",
                table: "ContractDistributors");
        }
    }
}
