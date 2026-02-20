using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMemberAccountPayTypeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PayType",
                table: "MemberAccounts",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PayType",
                value: null);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "PayType",
                value: null);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "PayType",
                value: null);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "PayType",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PayType",
                table: "MemberAccounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PayType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "PayType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "PayType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "PayType",
                value: 1);
        }
    }
}
