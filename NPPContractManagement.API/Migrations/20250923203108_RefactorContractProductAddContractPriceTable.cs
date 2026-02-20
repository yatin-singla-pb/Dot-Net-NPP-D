using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RefactorContractProductAddContractPriceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractPrice",
                table: "ContractProducts");

            migrationBuilder.CreateTable(
                name: "ContractPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
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
                    BillbacksAllowed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PUA = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    FFSPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    NOIPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    PTV = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    InternalNotes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ContractPrices_ProductId_VersionNumber",
                table: "ContractPrices",
                columns: new[] { "ProductId", "VersionNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "ContractPrice",
                table: "ContractProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
