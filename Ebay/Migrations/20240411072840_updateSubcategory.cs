using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ebay.Migrations
{
    /// <inheritdoc />
    public partial class updateSubcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SubCategories",
                newName: "SId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SId",
                table: "SubCategories",
                newName: "Id");
        }
    }
}
