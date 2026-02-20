using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMemberAccountClientGroupEnrollmentToNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClientGroupEnrollment",
                table: "MemberAccounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClientGroupEnrollment",
                value: null);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClientGroupEnrollment",
                value: null);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClientGroupEnrollment",
                value: null);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClientGroupEnrollment",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "ClientGroupEnrollment",
                table: "MemberAccounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClientGroupEnrollment",
                value: false);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClientGroupEnrollment",
                value: false);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClientGroupEnrollment",
                value: false);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClientGroupEnrollment",
                value: false);
        }
    }
}
