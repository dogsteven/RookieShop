using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductCatalog
{
    /// <inheritdoc />
    public partial class AddIndexForIsFeaturedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_IsFeatured",
                schema: "ProductCatalog",
                table: "Products",
                column: "IsFeatured");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_IsFeatured",
                schema: "ProductCatalog",
                table: "Products");
        }
    }
}
