using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RebuildVelocityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VelocityJobRows_VelocityJobs_VelocityJobId",
                table: "VelocityJobRows");

            migrationBuilder.DropForeignKey(
                name: "FK_VelocityJobRows_VelocityShipments_VelocityShipmentId",
                table: "VelocityJobRows");

            migrationBuilder.DropForeignKey(
                name: "FK_VelocityShipments_Distributors_DistributorId",
                table: "VelocityShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_VelocityShipments_VelocityJobs_VelocityJobId",
                table: "VelocityShipments");

            migrationBuilder.DropIndex(
                name: "IX_VelocityShipments_DistributorId",
                table: "VelocityShipments");

            migrationBuilder.DropIndex(
                name: "IX_VelocityJobRows_VelocityShipmentId",
                table: "VelocityJobRows");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "VelocityShipments");

            migrationBuilder.DropColumn(
                name: "DistributorId",
                table: "VelocityShipments");

            migrationBuilder.DropColumn(
                name: "ShipmentId",
                table: "VelocityShipments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "FailedRows",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "ProcessedRows",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "SftpFileUrl",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "SuccessRows",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "TotalRows",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "RawData",
                table: "VelocityJobRows");

            migrationBuilder.DropColumn(
                name: "VelocityShipmentId",
                table: "VelocityJobRows");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "VelocityShipments",
                newName: "sku");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "VelocityShipments",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Origin",
                table: "VelocityShipments",
                newName: "origin");

            migrationBuilder.RenameColumn(
                name: "Destination",
                table: "VelocityShipments",
                newName: "destination");

            migrationBuilder.RenameColumn(
                name: "ShippedAt",
                table: "VelocityShipments",
                newName: "shipped_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VelocityShipments",
                newName: "shipment_id");

            migrationBuilder.RenameColumn(
                name: "VelocityJobId",
                table: "VelocityShipments",
                newName: "job_id");

            migrationBuilder.RenameColumn(
                name: "RowIndex",
                table: "VelocityShipments",
                newName: "file_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "VelocityShipments",
                newName: "ingested_at");

            migrationBuilder.RenameIndex(
                name: "IX_VelocityShipments_VelocityJobId",
                table: "VelocityShipments",
                newName: "IX_VelocityShipments_job_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "VelocityJobs",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "VelocityJobs",
                newName: "started_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "VelocityJobs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VelocityJobs",
                newName: "job_id");

            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "VelocityJobs",
                newName: "finished_at");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "VelocityJobRows",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "RowIndex",
                table: "VelocityJobRows",
                newName: "row_index");

            migrationBuilder.RenameColumn(
                name: "ErrorMessage",
                table: "VelocityJobRows",
                newName: "error_message");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VelocityJobRows",
                newName: "row_id");

            migrationBuilder.RenameColumn(
                name: "VelocityJobId",
                table: "VelocityJobRows",
                newName: "job_id");

            migrationBuilder.RenameColumn(
                name: "ProcessedAt",
                table: "VelocityJobRows",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_VelocityJobRows_VelocityJobId",
                table: "VelocityJobRows",
                newName: "IX_VelocityJobRows_job_id");

            migrationBuilder.AlterColumn<string>(
                name: "sku",
                table: "VelocityShipments",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "VelocityShipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "origin",
                table: "VelocityShipments",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "destination",
                table: "VelocityShipments",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "shipped_at",
                table: "VelocityShipments",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<string>(
                name: "distributor_id",
                table: "VelocityShipments",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "manifest_line",
                table: "VelocityShipments",
                type: "json",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "VelocityJobs",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "distributor_id",
                table: "VelocityJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "file_id",
                table: "VelocityJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "initiated_by",
                table: "VelocityJobs",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "totals",
                table: "VelocityJobs",
                type: "json",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "error_message",
                table: "VelocityJobRows",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "file_id",
                table: "VelocityJobRows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "raw_values",
                table: "VelocityJobRows",
                type: "json",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "IngestedFiles",
                columns: table => new
                {
                    file_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    original_filename = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    uploaded_by = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    source_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    source_details = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content_sha256 = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bytes = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngestedFiles", x => x.file_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VelocityErrors",
                columns: table => new
                {
                    error_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    error_code = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    details = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VelocityErrors", x => x.error_id);
                    table.ForeignKey(
                        name: "FK_VelocityErrors_VelocityJobs_job_id",
                        column: x => x.job_id,
                        principalTable: "VelocityJobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_VelocityShipments_file_id",
                table: "VelocityShipments",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_VelocityJobs_distributor_id",
                table: "VelocityJobs",
                column: "distributor_id");

            migrationBuilder.CreateIndex(
                name: "IX_VelocityJobs_file_id",
                table: "VelocityJobs",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_VelocityJobRows_file_id",
                table: "VelocityJobRows",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_VelocityErrors_job_id",
                table: "VelocityErrors",
                column: "job_id");

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityJobRows_IngestedFiles_file_id",
                table: "VelocityJobRows",
                column: "file_id",
                principalTable: "IngestedFiles",
                principalColumn: "file_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityJobRows_VelocityJobs_job_id",
                table: "VelocityJobRows",
                column: "job_id",
                principalTable: "VelocityJobs",
                principalColumn: "job_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityJobs_Distributors_distributor_id",
                table: "VelocityJobs",
                column: "distributor_id",
                principalTable: "Distributors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityJobs_IngestedFiles_file_id",
                table: "VelocityJobs",
                column: "file_id",
                principalTable: "IngestedFiles",
                principalColumn: "file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityShipments_IngestedFiles_file_id",
                table: "VelocityShipments",
                column: "file_id",
                principalTable: "IngestedFiles",
                principalColumn: "file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityShipments_VelocityJobs_job_id",
                table: "VelocityShipments",
                column: "job_id",
                principalTable: "VelocityJobs",
                principalColumn: "job_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VelocityJobRows_IngestedFiles_file_id",
                table: "VelocityJobRows");

            migrationBuilder.DropForeignKey(
                name: "FK_VelocityJobRows_VelocityJobs_job_id",
                table: "VelocityJobRows");

            migrationBuilder.DropForeignKey(
                name: "FK_VelocityJobs_Distributors_distributor_id",
                table: "VelocityJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_VelocityJobs_IngestedFiles_file_id",
                table: "VelocityJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_VelocityShipments_IngestedFiles_file_id",
                table: "VelocityShipments");

            migrationBuilder.DropForeignKey(
                name: "FK_VelocityShipments_VelocityJobs_job_id",
                table: "VelocityShipments");

            migrationBuilder.DropTable(
                name: "IngestedFiles");

            migrationBuilder.DropTable(
                name: "VelocityErrors");

            migrationBuilder.DropIndex(
                name: "IX_VelocityShipments_file_id",
                table: "VelocityShipments");

            migrationBuilder.DropIndex(
                name: "IX_VelocityJobs_distributor_id",
                table: "VelocityJobs");

            migrationBuilder.DropIndex(
                name: "IX_VelocityJobs_file_id",
                table: "VelocityJobs");

            migrationBuilder.DropIndex(
                name: "IX_VelocityJobRows_file_id",
                table: "VelocityJobRows");

            migrationBuilder.DropColumn(
                name: "distributor_id",
                table: "VelocityShipments");

            migrationBuilder.DropColumn(
                name: "manifest_line",
                table: "VelocityShipments");

            migrationBuilder.DropColumn(
                name: "distributor_id",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "initiated_by",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "totals",
                table: "VelocityJobs");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "VelocityJobRows");

            migrationBuilder.DropColumn(
                name: "raw_values",
                table: "VelocityJobRows");

            migrationBuilder.RenameColumn(
                name: "sku",
                table: "VelocityShipments",
                newName: "Sku");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "VelocityShipments",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "origin",
                table: "VelocityShipments",
                newName: "Origin");

            migrationBuilder.RenameColumn(
                name: "destination",
                table: "VelocityShipments",
                newName: "Destination");

            migrationBuilder.RenameColumn(
                name: "shipped_at",
                table: "VelocityShipments",
                newName: "ShippedAt");

            migrationBuilder.RenameColumn(
                name: "shipment_id",
                table: "VelocityShipments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "job_id",
                table: "VelocityShipments",
                newName: "VelocityJobId");

            migrationBuilder.RenameColumn(
                name: "ingested_at",
                table: "VelocityShipments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "file_id",
                table: "VelocityShipments",
                newName: "RowIndex");

            migrationBuilder.RenameIndex(
                name: "IX_VelocityShipments_job_id",
                table: "VelocityShipments",
                newName: "IX_VelocityShipments_VelocityJobId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "VelocityJobs",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "started_at",
                table: "VelocityJobs",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "VelocityJobs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "job_id",
                table: "VelocityJobs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "finished_at",
                table: "VelocityJobs",
                newName: "CompletedAt");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "VelocityJobRows",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "row_index",
                table: "VelocityJobRows",
                newName: "RowIndex");

            migrationBuilder.RenameColumn(
                name: "error_message",
                table: "VelocityJobRows",
                newName: "ErrorMessage");

            migrationBuilder.RenameColumn(
                name: "row_id",
                table: "VelocityJobRows",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "job_id",
                table: "VelocityJobRows",
                newName: "VelocityJobId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "VelocityJobRows",
                newName: "ProcessedAt");

            migrationBuilder.RenameIndex(
                name: "IX_VelocityJobRows_job_id",
                table: "VelocityJobRows",
                newName: "IX_VelocityJobRows_VelocityJobId");

            migrationBuilder.UpdateData(
                table: "VelocityShipments",
                keyColumn: "Sku",
                keyValue: null,
                column: "Sku",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "VelocityShipments",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "VelocityShipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Origin",
                table: "VelocityShipments",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "VelocityShipments",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShippedAt",
                table: "VelocityShipments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "VelocityShipments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "DistributorId",
                table: "VelocityShipments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShipmentId",
                table: "VelocityShipments",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "VelocityJobs",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "VelocityJobs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "VelocityJobs",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "FailedRows",
                table: "VelocityJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "VelocityJobs",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "VelocityJobs",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ProcessedRows",
                table: "VelocityJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SftpFileUrl",
                table: "VelocityJobs",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SuccessRows",
                table: "VelocityJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRows",
                table: "VelocityJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "VelocityJobRows",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RawData",
                table: "VelocityJobRows",
                type: "varchar(4000)",
                maxLength: 4000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "VelocityShipmentId",
                table: "VelocityJobRows",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VelocityShipments_DistributorId",
                table: "VelocityShipments",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_VelocityJobRows_VelocityShipmentId",
                table: "VelocityJobRows",
                column: "VelocityShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityJobRows_VelocityJobs_VelocityJobId",
                table: "VelocityJobRows",
                column: "VelocityJobId",
                principalTable: "VelocityJobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityJobRows_VelocityShipments_VelocityShipmentId",
                table: "VelocityJobRows",
                column: "VelocityShipmentId",
                principalTable: "VelocityShipments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityShipments_Distributors_DistributorId",
                table: "VelocityShipments",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VelocityShipments_VelocityJobs_VelocityJobId",
                table: "VelocityShipments",
                column: "VelocityJobId",
                principalTable: "VelocityJobs",
                principalColumn: "Id");
        }
    }
}
