using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContractVersionRelationshipTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ContractVersions");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "ContractVersions",
                newName: "AssignedDate");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "ContractVersions",
                newName: "AssignedBy");

            migrationBuilder.CreateTable(
                name: "ContractDistributorsVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    DistributorId = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDistributorsVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractDistributorsVersion_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractDistributorsVersion_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractIndustriesVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractIndustriesVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractIndustriesVersion_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractIndustriesVersion_Industries_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Industries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractManufacturersVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractManufacturersVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractManufacturersVersion_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractManufacturersVersion_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractOpCosVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    OpCoId = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractOpCosVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractOpCosVersion_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractOpCosVersion_OpCos_OpCoId",
                        column: x => x.OpCoId,
                        principalTable: "OpCos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractVersionPrice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    PriceId = table.Column<int>(type: "int", nullable: false),
                    PriceType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Allowance = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CommercialDelPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CommercialFobPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CommodityDelPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CommodityFobPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    UOM = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstimatedQty = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    BillbacksAllowed = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    PUA = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    FFSPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    NOIPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    PTV = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    InternalNotes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractVersionPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractVersionPrice_ContractPrices_PriceId",
                        column: x => x.PriceId,
                        principalTable: "ContractPrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractVersionPrice_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractVersionProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractVersionProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractVersionProduct_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractVersionProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDistributorsVersion_ContractId_DistributorId_Version~",
                table: "ContractDistributorsVersion",
                columns: new[] { "ContractId", "DistributorId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractDistributorsVersion_DistributorId",
                table: "ContractDistributorsVersion",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractIndustriesVersion_ContractId_IndustryId_VersionNumber",
                table: "ContractIndustriesVersion",
                columns: new[] { "ContractId", "IndustryId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractIndustriesVersion_IndustryId",
                table: "ContractIndustriesVersion",
                column: "IndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractManufacturersVersion_ContractId_ManufacturerId_Versi~",
                table: "ContractManufacturersVersion",
                columns: new[] { "ContractId", "ManufacturerId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractManufacturersVersion_ManufacturerId",
                table: "ContractManufacturersVersion",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractOpCosVersion_ContractId_OpCoId_VersionNumber",
                table: "ContractOpCosVersion",
                columns: new[] { "ContractId", "OpCoId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractOpCosVersion_OpCoId",
                table: "ContractOpCosVersion",
                column: "OpCoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractVersionPrice_ContractId_PriceId_VersionNumber",
                table: "ContractVersionPrice",
                columns: new[] { "ContractId", "PriceId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractVersionPrice_PriceId",
                table: "ContractVersionPrice",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractVersionProduct_ContractId_ProductId_VersionNumber",
                table: "ContractVersionProduct",
                columns: new[] { "ContractId", "ProductId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractVersionProduct_ProductId",
                table: "ContractVersionProduct",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractDistributorsVersion");

            migrationBuilder.DropTable(
                name: "ContractIndustriesVersion");

            migrationBuilder.DropTable(
                name: "ContractManufacturersVersion");

            migrationBuilder.DropTable(
                name: "ContractOpCosVersion");

            migrationBuilder.DropTable(
                name: "ContractVersionPrice");

            migrationBuilder.DropTable(
                name: "ContractVersionProduct");

            migrationBuilder.RenameColumn(
                name: "AssignedDate",
                table: "ContractVersions",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                table: "ContractVersions",
                newName: "ModifiedBy");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ContractVersions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ContractVersions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
