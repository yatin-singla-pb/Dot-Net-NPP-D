using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalProductExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FfsPrice",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "ProposalProducts",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "NoiPrice",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ptv",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pua",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FfsPrice",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "NoiPrice",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "Ptv",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "Pua",
                table: "ProposalProducts");
        }
    }
}
