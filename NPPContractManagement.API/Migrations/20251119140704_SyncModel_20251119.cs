using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_20251119 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Contracts_AmendedContractId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_AmendedContractId",
                table: "Proposals");

            migrationBuilder.AddColumn<int>(
                name: "AmendedContractId1",
                table: "Proposals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_AmendedContractId1",
                table: "Proposals",
                column: "AmendedContractId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Contracts_AmendedContractId1",
                table: "Proposals",
                column: "AmendedContractId1",
                principalTable: "Contracts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Contracts_AmendedContractId1",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_AmendedContractId1",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "AmendedContractId1",
                table: "Proposals");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_AmendedContractId",
                table: "Proposals",
                column: "AmendedContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Contracts_AmendedContractId",
                table: "Proposals",
                column: "AmendedContractId",
                principalTable: "Contracts",
                principalColumn: "Id");
        }
    }
}
