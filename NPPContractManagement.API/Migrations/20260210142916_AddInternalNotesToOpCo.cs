using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalNotesToOpCo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "OpCos",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 1,
                column: "InternalNotes",
                value: null);

            migrationBuilder.UpdateData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 2,
                column: "InternalNotes",
                value: null);

            migrationBuilder.UpdateData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 3,
                column: "InternalNotes",
                value: null);

            migrationBuilder.UpdateData(
                table: "OpCos",
                keyColumn: "Id",
                keyValue: 4,
                column: "InternalNotes",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "OpCos");
        }
    }
}
