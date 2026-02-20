using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailAndContactPersonFromMemberAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "MemberAccounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "MemberAccounts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "MemberAccounts",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContactPerson", "Email" },
                values: new object[] { "Jennifer Adams", "dining@uchicago.edu" });

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ContactPerson", "Email" },
                values: new object[] { "Mary Johnson", "cafeteria@lincoln.edu" });

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContactPerson", "Email" },
                values: new object[] { "Carlos Rodriguez", "manager@quickbite.com" });

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ContactPerson", "Email" },
                values: new object[] { "Dr. Patricia Lee", "food@generalhospital.org" });
        }
    }
}
