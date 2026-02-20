using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class _2025_10_02_Create_Proposals_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "amendment_actions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amendment_actions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "price_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_types", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "product_proposal_statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_proposal_statuses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_batch_jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JobType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Total = table.Column<int>(type: "int", nullable: false),
                    Processed = table.Column<int>(type: "int", nullable: false),
                    Errors = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_batch_jobs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_statuses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_types", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProposalTypeId = table.Column<int>(type: "int", nullable: false),
                    ProposalStatusId = table.Column<int>(type: "int", nullable: false),
                    ManufacturerId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    InternalNotes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proposals_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_proposals_proposal_statuses_ProposalStatusId",
                        column: x => x.ProposalStatusId,
                        principalTable: "proposal_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposals_proposal_types_ProposalTypeId",
                        column: x => x.ProposalTypeId,
                        principalTable: "proposal_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_distributors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProposalId = table.Column<int>(type: "int", nullable: false),
                    DistributorId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_distributors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proposal_distributors_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalTable: "Distributors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposal_distributors_proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_industries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProposalId = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_industries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proposal_industries_Industries_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Industries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposal_industries_proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_opcos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProposalId = table.Column<int>(type: "int", nullable: false),
                    OpCoId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_opcos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proposal_opcos_OpCos_OpCoId",
                        column: x => x.OpCoId,
                        principalTable: "OpCos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposal_opcos_proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProposalId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PriceTypeId = table.Column<int>(type: "int", nullable: true),
                    ProposedPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    PackingList = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetaJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductProposalStatusId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proposal_products_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposal_products_price_types_PriceTypeId",
                        column: x => x.PriceTypeId,
                        principalTable: "price_types",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_proposal_products_product_proposal_statuses_ProductProposalS~",
                        column: x => x.ProductProposalStatusId,
                        principalTable: "product_proposal_statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_proposal_products_proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_status_history",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProposalId = table.Column<int>(type: "int", nullable: false),
                    FromStatusId = table.Column<int>(type: "int", nullable: false),
                    ToStatusId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChangedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ChangedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_status_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proposal_status_history_proposal_statuses_FromStatusId",
                        column: x => x.FromStatusId,
                        principalTable: "proposal_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposal_status_history_proposal_statuses_ToStatusId",
                        column: x => x.ToStatusId,
                        principalTable: "proposal_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposal_status_history_proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "proposal_product_history",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProposalProductId = table.Column<int>(type: "int", nullable: false),
                    ChangeType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PreviousJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrentJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChangedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ChangedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal_product_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proposal_product_history_proposal_products_ProposalProductId",
                        column: x => x.ProposalProductId,
                        principalTable: "proposal_products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_distributors_DistributorId",
                table: "proposal_distributors",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_distributors_ProposalId_DistributorId",
                table: "proposal_distributors",
                columns: new[] { "ProposalId", "DistributorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proposal_industries_IndustryId",
                table: "proposal_industries",
                column: "IndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_industries_ProposalId_IndustryId",
                table: "proposal_industries",
                columns: new[] { "ProposalId", "IndustryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proposal_opcos_OpCoId",
                table: "proposal_opcos",
                column: "OpCoId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_opcos_ProposalId_OpCoId",
                table: "proposal_opcos",
                columns: new[] { "ProposalId", "OpCoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proposal_product_history_ProposalProductId",
                table: "proposal_product_history",
                column: "ProposalProductId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_products_PriceTypeId",
                table: "proposal_products",
                column: "PriceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_products_ProductId",
                table: "proposal_products",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_products_ProductProposalStatusId",
                table: "proposal_products",
                column: "ProductProposalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_products_ProposalId_ProductId",
                table: "proposal_products",
                columns: new[] { "ProposalId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proposal_status_history_FromStatusId",
                table: "proposal_status_history",
                column: "FromStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_status_history_ProposalId",
                table: "proposal_status_history",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_status_history_ToStatusId",
                table: "proposal_status_history",
                column: "ToStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_proposals_ManufacturerId",
                table: "proposals",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_proposals_ProposalStatusId",
                table: "proposals",
                column: "ProposalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_proposals_ProposalTypeId_ProposalStatusId",
                table: "proposals",
                columns: new[] { "ProposalTypeId", "ProposalStatusId" });

            // Idempotent seeds for lookup tables
            migrationBuilder.Sql(@"INSERT INTO proposal_statuses (Id, Name, IsActive) VALUES
                (1,'Requested',1),(2,'Pending',1),(3,'Saved',1),(4,'Submitted',1),(5,'Completed',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");

            migrationBuilder.Sql(@"INSERT INTO proposal_types (Id, Name, IsActive) VALUES
                (1,'NewContract',1),(2,'Amendment',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");

            migrationBuilder.Sql(@"INSERT INTO price_types (Id, Name, IsActive) VALUES
                (1,'Commercial',1),(2,'Commodity',1),(3,'FFS',1),(4,'NOI',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");

            migrationBuilder.Sql(@"INSERT INTO product_proposal_statuses (Id, Name, IsActive) VALUES
                (1,'Pending',1),(2,'Accepted',1),(3,'Rejected',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");

            migrationBuilder.Sql(@"INSERT INTO amendment_actions (Id, Name, IsActive) VALUES
                (1,'Add',1),(2,'Update',1),(3,'Remove',1)
                ON DUPLICATE KEY UPDATE Name=VALUES(Name), IsActive=VALUES(IsActive);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "amendment_actions");

            migrationBuilder.DropTable(
                name: "proposal_batch_jobs");

            migrationBuilder.DropTable(
                name: "proposal_distributors");

            migrationBuilder.DropTable(
                name: "proposal_industries");

            migrationBuilder.DropTable(
                name: "proposal_opcos");

            migrationBuilder.DropTable(
                name: "proposal_product_history");

            migrationBuilder.DropTable(
                name: "proposal_status_history");

            migrationBuilder.DropTable(
                name: "proposal_products");

            migrationBuilder.DropTable(
                name: "price_types");

            migrationBuilder.DropTable(
                name: "product_proposal_statuses");

            migrationBuilder.DropTable(
                name: "proposals");

            migrationBuilder.DropTable(
                name: "proposal_statuses");

            migrationBuilder.DropTable(
                name: "proposal_types");
        }
    }
}
