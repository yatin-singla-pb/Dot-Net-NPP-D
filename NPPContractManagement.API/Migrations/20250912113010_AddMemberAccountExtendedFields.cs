using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    public partial class AddMemberAccountExtendedFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LopDate",
                table: "MemberAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "MemberAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ClientGroupEnrollment",
                table: "MemberAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SalesforceAccountName",
                table: "MemberAccounts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VMAPNumber",
                table: "MemberAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VMSupplierName",
                table: "MemberAccounts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VMSupplierSite",
                table: "MemberAccounts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PayType",
                table: "MemberAccounts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentMemberAccountNumber",
                table: "MemberAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntegraGPONumber",
                table: "MemberAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientGroupNumber",
                table: "MemberAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntegraIdNumber",
                table: "MemberAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "MemberAccounts",
                type: "datetime2",
                nullable: true);

            // Seed defaults for existing rows
            migrationBuilder.Sql(@"
                UPDATE MA SET 
                    SalesforceAccountName = CASE WHEN ISNULL(SalesforceAccountName, '') = '' THEN FacilityName ELSE SalesforceAccountName END,
                    LopDate = ISNULL(LopDate, SYSUTCDATETIME()),
                    AuditDate = SYSUTCDATETIME(),
                    ClientGroupEnrollment = ISNULL(ClientGroupEnrollment, 0),
                    PayType = ISNULL(PayType, 1)
                FROM MemberAccounts MA
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LopDate", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "InternalNotes", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "ClientGroupEnrollment", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "SalesforceAccountName", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "VMAPNumber", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "VMSupplierName", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "VMSupplierSite", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "PayType", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "ParentMemberAccountNumber", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "EntegraGPONumber", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "ClientGroupNumber", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "EntegraIdNumber", table: "MemberAccounts");
            migrationBuilder.DropColumn(name: "AuditDate", table: "MemberAccounts");
        }
    }
}

