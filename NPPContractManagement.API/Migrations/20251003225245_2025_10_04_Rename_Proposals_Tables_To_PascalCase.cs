using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class _2025_10_04_Rename_Proposals_Tables_To_PascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_proposal_distributors_Distributors_DistributorId",
                table: "proposal_distributors");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_distributors_proposals_ProposalId",
                table: "proposal_distributors");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_industries_Industries_IndustryId",
                table: "proposal_industries");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_industries_proposals_ProposalId",
                table: "proposal_industries");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_opcos_OpCos_OpCoId",
                table: "proposal_opcos");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_opcos_proposals_ProposalId",
                table: "proposal_opcos");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_product_history_proposal_products_ProposalProductId",
                table: "proposal_product_history");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_products_Products_ProductId",
                table: "proposal_products");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_products_price_types_PriceTypeId",
                table: "proposal_products");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_products_product_proposal_statuses_ProductProposalS~",
                table: "proposal_products");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_products_proposals_ProposalId",
                table: "proposal_products");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_status_history_proposal_statuses_FromStatusId",
                table: "proposal_status_history");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_status_history_proposal_statuses_ToStatusId",
                table: "proposal_status_history");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_status_history_proposals_ProposalId",
                table: "proposal_status_history");

            migrationBuilder.DropForeignKey(
                name: "FK_proposals_Manufacturers_ManufacturerId",
                table: "proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_proposals_proposal_statuses_ProposalStatusId",
                table: "proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_proposals_proposal_types_ProposalTypeId",
                table: "proposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposals",
                table: "proposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_types",
                table: "proposal_types");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_statuses",
                table: "proposal_statuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_status_history",
                table: "proposal_status_history");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_products",
                table: "proposal_products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_product_history",
                table: "proposal_product_history");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_opcos",
                table: "proposal_opcos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_industries",
                table: "proposal_industries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_distributors",
                table: "proposal_distributors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proposal_batch_jobs",
                table: "proposal_batch_jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_product_proposal_statuses",
                table: "product_proposal_statuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_price_types",
                table: "price_types");

            migrationBuilder.DropPrimaryKey(
                name: "PK_amendment_actions",
                table: "amendment_actions");

            migrationBuilder.RenameTable(
                name: "proposals",
                newName: "Proposals");

            migrationBuilder.RenameTable(
                name: "proposal_types",
                newName: "ProposalTypes");

            migrationBuilder.RenameTable(
                name: "proposal_statuses",
                newName: "ProposalStatuses");

            migrationBuilder.RenameTable(
                name: "proposal_status_history",
                newName: "ProposalStatusHistory");

            migrationBuilder.RenameTable(
                name: "proposal_products",
                newName: "ProposalProducts");

            migrationBuilder.RenameTable(
                name: "proposal_product_history",
                newName: "ProposalProductHistory");

            migrationBuilder.RenameTable(
                name: "proposal_opcos",
                newName: "ProposalOpCos");

            migrationBuilder.RenameTable(
                name: "proposal_industries",
                newName: "ProposalIndustries");

            migrationBuilder.RenameTable(
                name: "proposal_distributors",
                newName: "ProposalDistributors");

            migrationBuilder.RenameTable(
                name: "proposal_batch_jobs",
                newName: "ProposalBatchJobs");

            migrationBuilder.RenameTable(
                name: "product_proposal_statuses",
                newName: "ProductProposalStatuses");

            migrationBuilder.RenameTable(
                name: "price_types",
                newName: "PriceTypes");

            migrationBuilder.RenameTable(
                name: "amendment_actions",
                newName: "AmendmentActions");

            migrationBuilder.RenameIndex(
                name: "IX_proposals_ProposalTypeId_ProposalStatusId",
                table: "Proposals",
                newName: "IX_Proposals_ProposalTypeId_ProposalStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_proposals_ProposalStatusId",
                table: "Proposals",
                newName: "IX_Proposals_ProposalStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_proposals_ManufacturerId",
                table: "Proposals",
                newName: "IX_Proposals_ManufacturerId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_status_history_ToStatusId",
                table: "ProposalStatusHistory",
                newName: "IX_ProposalStatusHistory_ToStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_status_history_ProposalId",
                table: "ProposalStatusHistory",
                newName: "IX_ProposalStatusHistory_ProposalId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_status_history_FromStatusId",
                table: "ProposalStatusHistory",
                newName: "IX_ProposalStatusHistory_FromStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_products_ProposalId_ProductId",
                table: "ProposalProducts",
                newName: "IX_ProposalProducts_ProposalId_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_products_ProductProposalStatusId",
                table: "ProposalProducts",
                newName: "IX_ProposalProducts_ProductProposalStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_products_ProductId",
                table: "ProposalProducts",
                newName: "IX_ProposalProducts_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_products_PriceTypeId",
                table: "ProposalProducts",
                newName: "IX_ProposalProducts_PriceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_product_history_ProposalProductId",
                table: "ProposalProductHistory",
                newName: "IX_ProposalProductHistory_ProposalProductId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_opcos_ProposalId_OpCoId",
                table: "ProposalOpCos",
                newName: "IX_ProposalOpCos_ProposalId_OpCoId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_opcos_OpCoId",
                table: "ProposalOpCos",
                newName: "IX_ProposalOpCos_OpCoId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_industries_ProposalId_IndustryId",
                table: "ProposalIndustries",
                newName: "IX_ProposalIndustries_ProposalId_IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_industries_IndustryId",
                table: "ProposalIndustries",
                newName: "IX_ProposalIndustries_IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_distributors_ProposalId_DistributorId",
                table: "ProposalDistributors",
                newName: "IX_ProposalDistributors_ProposalId_DistributorId");

            migrationBuilder.RenameIndex(
                name: "IX_proposal_distributors_DistributorId",
                table: "ProposalDistributors",
                newName: "IX_ProposalDistributors_DistributorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalTypes",
                table: "ProposalTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalStatuses",
                table: "ProposalStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalStatusHistory",
                table: "ProposalStatusHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalProducts",
                table: "ProposalProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalProductHistory",
                table: "ProposalProductHistory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalOpCos",
                table: "ProposalOpCos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalIndustries",
                table: "ProposalIndustries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalDistributors",
                table: "ProposalDistributors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProposalBatchJobs",
                table: "ProposalBatchJobs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductProposalStatuses",
                table: "ProductProposalStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceTypes",
                table: "PriceTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AmendmentActions",
                table: "AmendmentActions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalDistributors_Distributors_DistributorId",
                table: "ProposalDistributors",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalDistributors_Proposals_ProposalId",
                table: "ProposalDistributors",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalIndustries_Industries_IndustryId",
                table: "ProposalIndustries",
                column: "IndustryId",
                principalTable: "Industries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalIndustries_Proposals_ProposalId",
                table: "ProposalIndustries",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalOpCos_OpCos_OpCoId",
                table: "ProposalOpCos",
                column: "OpCoId",
                principalTable: "OpCos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalOpCos_Proposals_ProposalId",
                table: "ProposalOpCos",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalProductHistory_ProposalProducts_ProposalProductId",
                table: "ProposalProductHistory",
                column: "ProposalProductId",
                principalTable: "ProposalProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalProducts_PriceTypes_PriceTypeId",
                table: "ProposalProducts",
                column: "PriceTypeId",
                principalTable: "PriceTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalProducts_ProductProposalStatuses_ProductProposalStat~",
                table: "ProposalProducts",
                column: "ProductProposalStatusId",
                principalTable: "ProductProposalStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalProducts_Products_ProductId",
                table: "ProposalProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalProducts_Proposals_ProposalId",
                table: "ProposalProducts",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Manufacturers_ManufacturerId",
                table: "Proposals",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_ProposalStatuses_ProposalStatusId",
                table: "Proposals",
                column: "ProposalStatusId",
                principalTable: "ProposalStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_ProposalTypes_ProposalTypeId",
                table: "Proposals",
                column: "ProposalTypeId",
                principalTable: "ProposalTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalStatusHistory_ProposalStatuses_FromStatusId",
                table: "ProposalStatusHistory",
                column: "FromStatusId",
                principalTable: "ProposalStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalStatusHistory_ProposalStatuses_ToStatusId",
                table: "ProposalStatusHistory",
                column: "ToStatusId",
                principalTable: "ProposalStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalStatusHistory_Proposals_ProposalId",
                table: "ProposalStatusHistory",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProposalDistributors_Distributors_DistributorId",
                table: "ProposalDistributors");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalDistributors_Proposals_ProposalId",
                table: "ProposalDistributors");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalIndustries_Industries_IndustryId",
                table: "ProposalIndustries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalIndustries_Proposals_ProposalId",
                table: "ProposalIndustries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalOpCos_OpCos_OpCoId",
                table: "ProposalOpCos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalOpCos_Proposals_ProposalId",
                table: "ProposalOpCos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalProductHistory_ProposalProducts_ProposalProductId",
                table: "ProposalProductHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalProducts_PriceTypes_PriceTypeId",
                table: "ProposalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalProducts_ProductProposalStatuses_ProductProposalStat~",
                table: "ProposalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalProducts_Products_ProductId",
                table: "ProposalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalProducts_Proposals_ProposalId",
                table: "ProposalProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Manufacturers_ManufacturerId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_ProposalStatuses_ProposalStatusId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_ProposalTypes_ProposalTypeId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalStatusHistory_ProposalStatuses_FromStatusId",
                table: "ProposalStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalStatusHistory_ProposalStatuses_ToStatusId",
                table: "ProposalStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalStatusHistory_Proposals_ProposalId",
                table: "ProposalStatusHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalTypes",
                table: "ProposalTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalStatusHistory",
                table: "ProposalStatusHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalStatuses",
                table: "ProposalStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalProducts",
                table: "ProposalProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalProductHistory",
                table: "ProposalProductHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalOpCos",
                table: "ProposalOpCos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalIndustries",
                table: "ProposalIndustries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalDistributors",
                table: "ProposalDistributors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProposalBatchJobs",
                table: "ProposalBatchJobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductProposalStatuses",
                table: "ProductProposalStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceTypes",
                table: "PriceTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AmendmentActions",
                table: "AmendmentActions");

            migrationBuilder.RenameTable(
                name: "Proposals",
                newName: "proposals");

            migrationBuilder.RenameTable(
                name: "ProposalTypes",
                newName: "proposal_types");

            migrationBuilder.RenameTable(
                name: "ProposalStatusHistory",
                newName: "proposal_status_history");

            migrationBuilder.RenameTable(
                name: "ProposalStatuses",
                newName: "proposal_statuses");

            migrationBuilder.RenameTable(
                name: "ProposalProducts",
                newName: "proposal_products");

            migrationBuilder.RenameTable(
                name: "ProposalProductHistory",
                newName: "proposal_product_history");

            migrationBuilder.RenameTable(
                name: "ProposalOpCos",
                newName: "proposal_opcos");

            migrationBuilder.RenameTable(
                name: "ProposalIndustries",
                newName: "proposal_industries");

            migrationBuilder.RenameTable(
                name: "ProposalDistributors",
                newName: "proposal_distributors");

            migrationBuilder.RenameTable(
                name: "ProposalBatchJobs",
                newName: "proposal_batch_jobs");

            migrationBuilder.RenameTable(
                name: "ProductProposalStatuses",
                newName: "product_proposal_statuses");

            migrationBuilder.RenameTable(
                name: "PriceTypes",
                newName: "price_types");

            migrationBuilder.RenameTable(
                name: "AmendmentActions",
                newName: "amendment_actions");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_ProposalTypeId_ProposalStatusId",
                table: "proposals",
                newName: "IX_proposals_ProposalTypeId_ProposalStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_ProposalStatusId",
                table: "proposals",
                newName: "IX_proposals_ProposalStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_ManufacturerId",
                table: "proposals",
                newName: "IX_proposals_ManufacturerId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalStatusHistory_ToStatusId",
                table: "proposal_status_history",
                newName: "IX_proposal_status_history_ToStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalStatusHistory_ProposalId",
                table: "proposal_status_history",
                newName: "IX_proposal_status_history_ProposalId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalStatusHistory_FromStatusId",
                table: "proposal_status_history",
                newName: "IX_proposal_status_history_FromStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalProducts_ProposalId_ProductId",
                table: "proposal_products",
                newName: "IX_proposal_products_ProposalId_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalProducts_ProductProposalStatusId",
                table: "proposal_products",
                newName: "IX_proposal_products_ProductProposalStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalProducts_ProductId",
                table: "proposal_products",
                newName: "IX_proposal_products_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalProducts_PriceTypeId",
                table: "proposal_products",
                newName: "IX_proposal_products_PriceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalProductHistory_ProposalProductId",
                table: "proposal_product_history",
                newName: "IX_proposal_product_history_ProposalProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalOpCos_ProposalId_OpCoId",
                table: "proposal_opcos",
                newName: "IX_proposal_opcos_ProposalId_OpCoId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalOpCos_OpCoId",
                table: "proposal_opcos",
                newName: "IX_proposal_opcos_OpCoId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalIndustries_ProposalId_IndustryId",
                table: "proposal_industries",
                newName: "IX_proposal_industries_ProposalId_IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalIndustries_IndustryId",
                table: "proposal_industries",
                newName: "IX_proposal_industries_IndustryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalDistributors_ProposalId_DistributorId",
                table: "proposal_distributors",
                newName: "IX_proposal_distributors_ProposalId_DistributorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProposalDistributors_DistributorId",
                table: "proposal_distributors",
                newName: "IX_proposal_distributors_DistributorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposals",
                table: "proposals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_types",
                table: "proposal_types",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_status_history",
                table: "proposal_status_history",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_statuses",
                table: "proposal_statuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_products",
                table: "proposal_products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_product_history",
                table: "proposal_product_history",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_opcos",
                table: "proposal_opcos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_industries",
                table: "proposal_industries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_distributors",
                table: "proposal_distributors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proposal_batch_jobs",
                table: "proposal_batch_jobs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_proposal_statuses",
                table: "product_proposal_statuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_price_types",
                table: "price_types",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_amendment_actions",
                table: "amendment_actions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_distributors_Distributors_DistributorId",
                table: "proposal_distributors",
                column: "DistributorId",
                principalTable: "Distributors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_distributors_proposals_ProposalId",
                table: "proposal_distributors",
                column: "ProposalId",
                principalTable: "proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_industries_Industries_IndustryId",
                table: "proposal_industries",
                column: "IndustryId",
                principalTable: "Industries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_industries_proposals_ProposalId",
                table: "proposal_industries",
                column: "ProposalId",
                principalTable: "proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_opcos_OpCos_OpCoId",
                table: "proposal_opcos",
                column: "OpCoId",
                principalTable: "OpCos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_opcos_proposals_ProposalId",
                table: "proposal_opcos",
                column: "ProposalId",
                principalTable: "proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_product_history_proposal_products_ProposalProductId",
                table: "proposal_product_history",
                column: "ProposalProductId",
                principalTable: "proposal_products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_products_Products_ProductId",
                table: "proposal_products",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_products_price_types_PriceTypeId",
                table: "proposal_products",
                column: "PriceTypeId",
                principalTable: "price_types",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_products_product_proposal_statuses_ProductProposalS~",
                table: "proposal_products",
                column: "ProductProposalStatusId",
                principalTable: "product_proposal_statuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_products_proposals_ProposalId",
                table: "proposal_products",
                column: "ProposalId",
                principalTable: "proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_status_history_proposal_statuses_FromStatusId",
                table: "proposal_status_history",
                column: "FromStatusId",
                principalTable: "proposal_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_status_history_proposal_statuses_ToStatusId",
                table: "proposal_status_history",
                column: "ToStatusId",
                principalTable: "proposal_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_status_history_proposals_ProposalId",
                table: "proposal_status_history",
                column: "ProposalId",
                principalTable: "proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposals_Manufacturers_ManufacturerId",
                table: "proposals",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_proposals_proposal_statuses_ProposalStatusId",
                table: "proposals",
                column: "ProposalStatusId",
                principalTable: "proposal_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposals_proposal_types_ProposalTypeId",
                table: "proposals",
                column: "ProposalTypeId",
                principalTable: "proposal_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
