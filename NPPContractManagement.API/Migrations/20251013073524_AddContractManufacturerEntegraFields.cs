using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContractManufacturerEntegraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "ContractVersions",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EntegraContractType",
                table: "ContractVersions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EntegraVdaProgram",
                table: "ContractVersions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerBillbackName",
                table: "ContractVersions",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerNotes",
                table: "ContractVersions",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerReferenceNumber",
                table: "ContractVersions",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerTermsAndConditions",
                table: "ContractVersions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Contracts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EntegraContractType",
                table: "Contracts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EntegraVdaProgram",
                table: "Contracts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerBillbackName",
                table: "Contracts",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerNotes",
                table: "Contracts",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerReferenceNumber",
                table: "Contracts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerTermsAndConditions",
                table: "Contracts",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContactPerson", "EntegraContractType", "EntegraVdaProgram", "ManufacturerBillbackName", "ManufacturerNotes", "ManufacturerReferenceNumber", "ManufacturerTermsAndConditions" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ContactPerson", "EntegraContractType", "EntegraVdaProgram", "ManufacturerBillbackName", "ManufacturerNotes", "ManufacturerReferenceNumber", "ManufacturerTermsAndConditions" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContactPerson", "EntegraContractType", "EntegraVdaProgram", "ManufacturerBillbackName", "ManufacturerNotes", "ManufacturerReferenceNumber", "ManufacturerTermsAndConditions" },
                values: new object[] { null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "EntegraContractType",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "EntegraVdaProgram",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "ManufacturerBillbackName",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "ManufacturerNotes",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "ManufacturerReferenceNumber",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "ManufacturerTermsAndConditions",
                table: "ContractVersions");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "EntegraContractType",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "EntegraVdaProgram",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ManufacturerBillbackName",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ManufacturerNotes",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ManufacturerReferenceNumber",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ManufacturerTermsAndConditions",
                table: "Contracts");
        }
    }
}
