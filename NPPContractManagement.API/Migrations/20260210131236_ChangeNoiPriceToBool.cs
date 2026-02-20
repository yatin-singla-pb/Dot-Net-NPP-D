using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNoiPriceToBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "NoiPrice",
                table: "ProposalProducts",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "W9",
                table: "MemberAccounts",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DSO",
                table: "CustomerAccounts",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "DSO",
                value: null);

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "DSO",
                value: null);

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "DSO",
                value: null);

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "DSO",
                value: null);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "W9",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "W9",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "W9",
                value: "Not Submitted");

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "W9",
                value: "Submitted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "NoiPrice",
                table: "ProposalProducts",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "W9",
                table: "MemberAccounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "DSO",
                table: "CustomerAccounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "DSO",
                value: null);

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "DSO",
                value: null);

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "DSO",
                value: null);

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "DSO",
                value: null);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "W9",
                value: true);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "W9",
                value: true);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "W9",
                value: false);

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4,
                column: "W9",
                value: true);
        }
    }
}
