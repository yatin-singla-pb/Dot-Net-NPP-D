using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    public partial class UpdateContractsSchema_AddNameInternalNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns if they don't exist
            try
            {
                migrationBuilder.AddColumn<string>(
                    name: "Name",
                    table: "Contracts",
                    type: "varchar(200)",
                    maxLength: 200,
                    nullable: false,
                    defaultValue: "");
            }
            catch { }

            try
            {
                migrationBuilder.AddColumn<string>(
                    name: "InternalNotes",
                    table: "Contracts",
                    type: "longtext",
                    nullable: true);
            }
            catch { }

            try
            {
                migrationBuilder.AddColumn<string>(
                    name: "ForeignContractId",
                    table: "Contracts",
                    type: "varchar(100)",
                    maxLength: 100,
                    nullable: true);
            }
            catch { }

            // Backfill data from legacy columns
            migrationBuilder.Sql(@"
                UPDATE Contracts
                SET Name = CASE
                    WHEN Name IS NULL OR Name = '' THEN COALESCE(Title, '')
                    ELSE Name END,
                    InternalNotes = CASE
                    WHEN (InternalNotes IS NULL OR InternalNotes = '') AND Notes IS NOT NULL THEN Notes
                    ELSE InternalNotes END,
                    ForeignContractId = CASE
                    WHEN (ForeignContractId IS NULL OR ForeignContractId = '') AND ForeignContractID IS NOT NULL THEN ForeignContractID
                    ELSE ForeignContractId END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Best-effort revert: keep legacy data; do not drop legacy columns
            try { migrationBuilder.DropColumn(name: "ForeignContractId", table: "Contracts"); } catch { }
            try { migrationBuilder.DropColumn(name: "InternalNotes", table: "Contracts"); } catch { }
            try { migrationBuilder.DropColumn(name: "Name", table: "Contracts"); } catch { }
        }
    }
}

