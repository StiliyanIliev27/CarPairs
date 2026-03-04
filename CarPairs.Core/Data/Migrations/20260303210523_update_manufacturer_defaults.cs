using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPairs.Core
{
    /// <inheritdoc />
    public partial class update_manufacturer_defaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set sensible defaults for existing seeded rows
            migrationBuilder.Sql("UPDATE Manufacturers SET Country = 'Unknown' WHERE Country IS NULL;");
            migrationBuilder.Sql("UPDATE Manufacturers SET FoundedYear = 1900 WHERE FoundedYear IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // no-op - values can remain
        }
    }
}
