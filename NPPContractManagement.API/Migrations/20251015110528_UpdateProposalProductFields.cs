using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProposalProductFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackingList",
                table: "ProposalProducts");

            migrationBuilder.RenameColumn(
                name: "ProposedPrice",
                table: "ProposalProducts",
                newName: "CommodityFobPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "Allowance",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BillbacksAllowed",
                table: "ProposalProducts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CommercialDelPrice",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommercialFobPrice",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommodityDelPrice",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerNotes",
                table: "ProposalProducts",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Uom",
                table: "ProposalProducts",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allowance",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "BillbacksAllowed",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "CommercialDelPrice",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "CommercialFobPrice",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "CommodityDelPrice",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "ManufacturerNotes",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "Uom",
                table: "ProposalProducts");

            migrationBuilder.RenameColumn(
                name: "CommodityFobPrice",
                table: "ProposalProducts",
                newName: "ProposedPrice");

            migrationBuilder.AddColumn<string>(
                name: "PackingList",
                table: "ProposalProducts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
