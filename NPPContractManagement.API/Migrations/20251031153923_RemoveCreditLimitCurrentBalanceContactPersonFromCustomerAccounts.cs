using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreditLimitCurrentBalanceContactPersonFromCustomerAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "CreditLimit",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "CustomerAccounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "CustomerAccounts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "CreditLimit",
                table: "CustomerAccounts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBalance",
                table: "CustomerAccounts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContactPerson", "CreditLimit", "CurrentBalance" },
                values: new object[] { "Jennifer Adams", 50000.00m, 12500.00m });

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ContactPerson", "CreditLimit", "CurrentBalance" },
                values: new object[] { "Mary Johnson", 25000.00m, 5000.00m });

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContactPerson", "CreditLimit", "CurrentBalance" },
                values: new object[] { "Carlos Rodriguez", 15000.00m, 3200.00m });

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ContactPerson", "CreditLimit", "CurrentBalance" },
                values: new object[] { "Dr. Patricia Lee", 75000.00m, 18750.00m });
        }
    }
}
