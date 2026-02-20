using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NPPContractManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class EnsureAlwaysListNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The original MakeAlwaysListNullable migration was missing its Designer.cs,
            // so EF Core never applied it. This ensures the column is nullable.
            migrationBuilder.Sql(@"ALTER TABLE `Products` MODIFY COLUMN `AlwaysList` tinyint(1) NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
