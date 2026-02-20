using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContactPersonIdToManufacturers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactPersonId",
                table: "Manufacturers",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContactPersonId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 2,
                column: "ContactPersonId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 3,
                column: "ContactPersonId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_ContactPersonId",
                table: "Manufacturers",
                column: "ContactPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Manufacturers_Users_ContactPersonId",
                table: "Manufacturers",
                column: "ContactPersonId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manufacturers_Users_ContactPersonId",
                table: "Manufacturers");

            migrationBuilder.DropIndex(
                name: "IX_Manufacturers_ContactPersonId",
                table: "Manufacturers");

            migrationBuilder.DropColumn(
                name: "ContactPersonId",
                table: "Manufacturers");
        }
    }
}
