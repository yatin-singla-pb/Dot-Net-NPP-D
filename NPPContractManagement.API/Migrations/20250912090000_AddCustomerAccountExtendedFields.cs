using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    public partial class AddCustomerAccountExtendedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesRep",
                table: "CustomerAccounts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DSO",
                table: "CustomerAccounts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "CustomerAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "CustomerAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TRACSAccess",
                table: "CustomerAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Markup",
                table: "CustomerAccounts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "CustomerAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ToEntegra",
                table: "CustomerAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateToEntegra",
                table: "CustomerAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CombinedUniqueID",
                table: "CustomerAccounts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "CustomerAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Association",
                table: "CustomerAccounts",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesRep",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "DSO",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "TRACSAccess",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "Markup",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "ToEntegra",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "DateToEntegra",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "CombinedUniqueID",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "Association",
                table: "CustomerAccounts");
        }
    }
}

