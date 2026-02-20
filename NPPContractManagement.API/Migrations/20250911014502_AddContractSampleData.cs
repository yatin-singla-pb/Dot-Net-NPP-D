using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContractSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Contracts",
                columns: new[] { "Id", "ContractNumber", "CreatedBy", "CreatedDate", "CurrentVersionNumber", "Description", "DistributorId", "EndDate", "IsSuspended", "ManufacturerId", "ModifiedBy", "ModifiedDate", "Notes", "SendToPerformance", "StartDate", "Status", "Terms", "Title", "TotalValue" },
                values: new object[,]
                {
                    { 1, "CNT-2025-001", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Comprehensive food service contract for university dining facilities", null, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, null, null, null, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, null, "University Food Service Contract 2025", 500000.00m },
                    { 2, "CNT-2025-002", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Food service agreement for K-12 school district cafeterias", null, new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2, null, null, null, true, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, null, "K-12 School District Agreement", 750000.00m },
                    { 3, "CNT-2025-003", "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Food service contract for healthcare facilities and hospitals", null, new DateTime(2025, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 3, null, null, null, false, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, null, "Healthcare Facilities Contract", 300000.00m }
                });

            migrationBuilder.InsertData(
                table: "ContractDistributors",
                columns: new[] { "Id", "ContractId", "CreatedBy", "CreatedDate", "DistributorId", "IsActive" },
                values: new object[,]
                {
                    { 1, 1, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true },
                    { 2, 1, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true },
                    { 3, 2, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true },
                    { 4, 3, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true }
                });

            migrationBuilder.InsertData(
                table: "ContractIndustries",
                columns: new[] { "Id", "ContractId", "CreatedBy", "CreatedDate", "IndustryId", "IsActive" },
                values: new object[,]
                {
                    { 1, 1, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true },
                    { 2, 2, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true },
                    { 3, 3, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true }
                });

            migrationBuilder.InsertData(
                table: "ContractOpCos",
                columns: new[] { "Id", "ContractId", "CreatedBy", "CreatedDate", "IsActive", "OpCoId" },
                values: new object[,]
                {
                    { 1, 1, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1 },
                    { 2, 1, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4 },
                    { 3, 2, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2 },
                    { 4, 3, "System", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ContractDistributors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ContractDistributors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ContractDistributors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ContractDistributors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ContractIndustries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ContractIndustries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ContractIndustries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ContractOpCos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ContractOpCos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ContractOpCos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ContractOpCos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
