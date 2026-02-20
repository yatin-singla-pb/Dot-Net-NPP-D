using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeManufacturerPrimaryBrokerToUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryBroker",
                table: "Manufacturers");

            migrationBuilder.AddColumn<int>(
                name: "PrimaryBrokerId",
                table: "Manufacturers",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrimaryBrokerId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PrimaryBrokerId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PrimaryBrokerId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_PrimaryBrokerId",
                table: "Manufacturers",
                column: "PrimaryBrokerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Manufacturers_Users_PrimaryBrokerId",
                table: "Manufacturers",
                column: "PrimaryBrokerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manufacturers_Users_PrimaryBrokerId",
                table: "Manufacturers");

            migrationBuilder.DropIndex(
                name: "IX_Manufacturers_PrimaryBrokerId",
                table: "Manufacturers");

            migrationBuilder.DropColumn(
                name: "PrimaryBrokerId",
                table: "Manufacturers");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryBroker",
                table: "Manufacturers",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrimaryBroker",
                value: "John Smith");

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PrimaryBroker",
                value: "Jane Doe");

            migrationBuilder.UpdateData(
                table: "Manufacturers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PrimaryBroker",
                value: "Mike Johnson");
        }
    }
}
