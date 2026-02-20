using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContractsIndustryForeignSuspended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Contracts",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ForeignContractID",
                table: "Contracts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "IndustryId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedDate",
                table: "Contracts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ForeignContractID", "IndustryId", "SuspendedDate" },
                values: new object[] { null, 1, null });

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ForeignContractID", "IndustryId", "SuspendedDate" },
                values: new object[] { null, 2, null });

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ForeignContractID", "IndustryId", "SuspendedDate" },
                values: new object[] { null, 4, null });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_IndustryId",
                table: "Contracts",
                column: "IndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Industries_IndustryId",
                table: "Contracts",
                column: "IndustryId",
                principalTable: "Industries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Industries_IndustryId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_IndustryId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ForeignContractID",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IndustryId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SuspendedDate",
                table: "Contracts");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Contracts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
