using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContractVersionsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeReason",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "Terms",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "ContractVersions");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ContractVersions",
                newName: "Name");


            migrationBuilder.DropColumn(
                name: "IsCurrentVersion",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ContractVersions");

            migrationBuilder.AddColumn<bool>(
                name: "SendToPerformance",
                table: "ContractVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "ContractVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ForeignContractId",
                table: "ContractVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "ContractVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedDate",
                table: "ContractVersions",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForeignContractId",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "SuspendedDate",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "SendToPerformance",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "ContractVersions");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ContractVersions",
                newName: "Title");

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrentVersion",
                table: "ContractVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ContractVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeReason",
                table: "ContractVersions",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ContractVersions",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ContractVersions",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Terms",
                table: "ContractVersions",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "ContractVersions",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
