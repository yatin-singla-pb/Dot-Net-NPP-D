using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class EnsureCustomerAccountExtendedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "MemberAccounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ClientGroupEnrollment",
                table: "MemberAccounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ClientGroupNumber",
                table: "MemberAccounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EntegraGPONumber",
                table: "MemberAccounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EntegraIdNumber",
                table: "MemberAccounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "MemberAccounts",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "LopDate",
                table: "MemberAccounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentMemberAccountNumber",
                table: "MemberAccounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PayType",
                table: "MemberAccounts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesforceAccountName",
                table: "MemberAccounts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VMAPNumber",
                table: "MemberAccounts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VMSupplierName",
                table: "MemberAccounts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VMSupplierSite",
                table: "MemberAccounts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Association",
                table: "CustomerAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "CustomerAccounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CombinedUniqueID",
                table: "CustomerAccounts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "DSO",
                table: "CustomerAccounts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateToEntegra",
                table: "CustomerAccounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "CustomerAccounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "CustomerAccounts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Markup",
                table: "CustomerAccounts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesRep",
                table: "CustomerAccounts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "CustomerAccounts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TRACSAccess",
                table: "CustomerAccounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ToEntegra",
                table: "CustomerAccounts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Association", "AuditDate", "CombinedUniqueID", "DSO", "DateToEntegra", "EndDate", "InternalNotes", "Markup", "SalesRep", "StartDate", "TRACSAccess", "ToEntegra" },
                values: new object[] { 1, null, null, null, null, null, null, null, null, null, false, false });

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Association", "AuditDate", "CombinedUniqueID", "DSO", "DateToEntegra", "EndDate", "InternalNotes", "Markup", "SalesRep", "StartDate", "TRACSAccess", "ToEntegra" },
                values: new object[] { 1, null, null, null, null, null, null, null, null, null, false, false });

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Association", "AuditDate", "CombinedUniqueID", "DSO", "DateToEntegra", "EndDate", "InternalNotes", "Markup", "SalesRep", "StartDate", "TRACSAccess", "ToEntegra" },
                values: new object[] { 1, null, null, null, null, null, null, null, null, null, false, false });

            migrationBuilder.UpdateData(
                table: "CustomerAccounts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Association", "AuditDate", "CombinedUniqueID", "DSO", "DateToEntegra", "EndDate", "InternalNotes", "Markup", "SalesRep", "StartDate", "TRACSAccess", "ToEntegra" },
                values: new object[] { 1, null, null, null, null, null, null, null, null, null, false, false });

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AuditDate", "ClientGroupEnrollment", "ClientGroupNumber", "EntegraGPONumber", "EntegraIdNumber", "InternalNotes", "LopDate", "ParentMemberAccountNumber", "PayType", "SalesforceAccountName", "VMAPNumber", "VMSupplierName", "VMSupplierSite" },
                values: new object[] { null, false, null, null, null, null, null, null, 1, "", null, null, null });

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AuditDate", "ClientGroupEnrollment", "ClientGroupNumber", "EntegraGPONumber", "EntegraIdNumber", "InternalNotes", "LopDate", "ParentMemberAccountNumber", "PayType", "SalesforceAccountName", "VMAPNumber", "VMSupplierName", "VMSupplierSite" },
                values: new object[] { null, false, null, null, null, null, null, null, 1, "", null, null, null });

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AuditDate", "ClientGroupEnrollment", "ClientGroupNumber", "EntegraGPONumber", "EntegraIdNumber", "InternalNotes", "LopDate", "ParentMemberAccountNumber", "PayType", "SalesforceAccountName", "VMAPNumber", "VMSupplierName", "VMSupplierSite" },
                values: new object[] { null, false, null, null, null, null, null, null, 1, "", null, null, null });

            migrationBuilder.UpdateData(
                table: "MemberAccounts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AuditDate", "ClientGroupEnrollment", "ClientGroupNumber", "EntegraGPONumber", "EntegraIdNumber", "InternalNotes", "LopDate", "ParentMemberAccountNumber", "PayType", "SalesforceAccountName", "VMAPNumber", "VMSupplierName", "VMSupplierSite" },
                values: new object[] { null, false, null, null, null, null, null, null, 1, "", null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "ClientGroupEnrollment",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "ClientGroupNumber",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "EntegraGPONumber",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "EntegraIdNumber",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "LopDate",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "ParentMemberAccountNumber",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "PayType",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "SalesforceAccountName",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "VMAPNumber",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "VMSupplierName",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "VMSupplierSite",
                table: "MemberAccounts");

            migrationBuilder.DropColumn(
                name: "Association",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "CombinedUniqueID",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "DSO",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "DateToEntegra",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "InternalNotes",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "Markup",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "SalesRep",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "TRACSAccess",
                table: "CustomerAccounts");

            migrationBuilder.DropColumn(
                name: "ToEntegra",
                table: "CustomerAccounts");
        }
    }
}
