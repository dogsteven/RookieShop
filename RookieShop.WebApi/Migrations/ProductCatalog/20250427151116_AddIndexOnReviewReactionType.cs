using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductCatalog
{
    /// <inheritdoc />
    public partial class AddIndexOnReviewReactionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewReactions",
                schema: "ProductCatalog",
                table: "ReviewReactions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewReactions",
                schema: "ProductCatalog",
                table: "ReviewReactions",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewReactions",
                schema: "ProductCatalog",
                table: "ReviewReactions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewReactions",
                schema: "ProductCatalog",
                table: "ReviewReactions",
                columns: new[] { "ReactorId", "WriterId", "ProductSku" });
        }
    }
}
