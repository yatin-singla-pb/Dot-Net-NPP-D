using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIdToContractVersionPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ContractVersionPrice",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE ContractVersionPrice cvp
JOIN ContractPrices cp ON cp.Id = cvp.PriceId
SET cvp.ProductId = cp.ProductId
WHERE cvp.ProductId IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ContractVersionPrice",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractVersionPrice_ContractId_ProductId_VersionNumber",
                table: "ContractVersionPrice",
                columns: new[] { "ContractId", "ProductId", "VersionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractVersionPrice_ProductId",
                table: "ContractVersionPrice",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractVersionPrice_Products_ProductId",
                table: "ContractVersionPrice",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractVersionPrice_Products_ProductId",
                table: "ContractVersionPrice");

            migrationBuilder.DropIndex(
                name: "IX_ContractVersionPrice_ContractId_ProductId_VersionNumber",
                table: "ContractVersionPrice");

            migrationBuilder.DropIndex(
                name: "IX_ContractVersionPrice_ProductId",
                table: "ContractVersionPrice");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ContractVersionPrice");
        }
    }
}
