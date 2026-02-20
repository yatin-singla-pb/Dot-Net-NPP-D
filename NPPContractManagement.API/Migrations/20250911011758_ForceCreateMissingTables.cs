using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class ForceCreateMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedDate",
                table: "ContractOpCos",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                table: "ContractOpCos",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "AssignedDate",
                table: "ContractIndustries",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                table: "ContractIndustries",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "AssignedDate",
                table: "ContractDistributors",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "AssignedBy",
                table: "ContractDistributors",
                newName: "CreatedBy");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(4511));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(5978));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(5983));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(5985));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 433, DateTimeKind.Utc).AddTicks(5986));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(4428));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(5934));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(5938));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(5939));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 1, 17, 56, 432, DateTimeKind.Utc).AddTicks(5940));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ContractOpCos",
                newName: "AssignedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ContractOpCos",
                newName: "AssignedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ContractIndustries",
                newName: "AssignedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ContractIndustries",
                newName: "AssignedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ContractDistributors",
                newName: "AssignedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ContractDistributors",
                newName: "AssignedBy");

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 853, DateTimeKind.Utc).AddTicks(4701));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 853, DateTimeKind.Utc).AddTicks(6150));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 853, DateTimeKind.Utc).AddTicks(6155));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 853, DateTimeKind.Utc).AddTicks(6157));

            migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 853, DateTimeKind.Utc).AddTicks(6158));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 852, DateTimeKind.Utc).AddTicks(5088));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 852, DateTimeKind.Utc).AddTicks(6269));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 852, DateTimeKind.Utc).AddTicks(6272));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 852, DateTimeKind.Utc).AddTicks(6273));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 0, 40, 15, 852, DateTimeKind.Utc).AddTicks(6274));
        }
    }
}
