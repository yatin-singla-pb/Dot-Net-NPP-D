using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContractsSchema_20250923 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ForeignContractID",
                table: "Contracts",
                newName: "ForeignContractId");



            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "Contracts",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contracts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "InternalNotes", "Name" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "InternalNotes", "Name" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "InternalNotes", "Name" },
                values: new object[] { null, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "ForeignContractId",
                table: "Contracts",
                newName: "ForeignContractID");
        }
    }
}
