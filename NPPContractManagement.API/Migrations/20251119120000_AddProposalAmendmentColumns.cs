using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NPPContractManagement.API.Data;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20251119120000_AddProposalAmendmentColumns")]
    public partial class AddProposalAmendmentColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmendedContractId",
                table: "Proposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AmendmentActionId",
                table: "ProposalProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_AmendedContractId",
                table: "Proposals",
                column: "AmendedContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalProducts_AmendmentActionId",
                table: "ProposalProducts",
                column: "AmendmentActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Contracts_AmendedContractId",
                table: "Proposals",
                column: "AmendedContractId",
                principalTable: "Contracts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalProducts_AmendmentActions_AmendmentActionId",
                table: "ProposalProducts",
                column: "AmendmentActionId",
                principalTable: "AmendmentActions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Contracts_AmendedContractId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalProducts_AmendmentActions_AmendmentActionId",
                table: "ProposalProducts");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_AmendedContractId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_ProposalProducts_AmendmentActionId",
                table: "ProposalProducts");

            migrationBuilder.DropColumn(
                name: "AmendedContractId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "AmendmentActionId",
                table: "ProposalProducts");
        }
    }
}

