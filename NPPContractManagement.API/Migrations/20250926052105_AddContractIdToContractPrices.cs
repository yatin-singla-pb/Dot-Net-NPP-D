using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContractIdToContractPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "ContractPrices",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE ContractPrices cp
JOIN ContractVersionPrice cvp ON cvp.PriceId = cp.Id
SET cp.ContractId = cvp.ContractId
WHERE cp.ContractId IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "ContractId",
                table: "ContractPrices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractPrices_ContractId_VersionNumber",
                table: "ContractPrices",
                columns: new[] { "ContractId", "VersionNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_ContractPrices_Contracts_ContractId",
                table: "ContractPrices",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractPrices_Contracts_ContractId",
                table: "ContractPrices");

            migrationBuilder.DropIndex(
                name: "IX_ContractPrices_ContractId_VersionNumber",
                table: "ContractPrices");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "ContractPrices");
        }
    }
}
