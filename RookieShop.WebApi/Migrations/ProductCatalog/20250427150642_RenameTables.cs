using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductCatalog
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Products_Sku",
                schema: "ProductCatalog",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_Reviews_WriterId_ProductSku",
                schema: "ProductCatalog",
                table: "Reactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reactions",
                schema: "ProductCatalog",
                table: "Reactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                schema: "ProductCatalog",
                table: "Ratings");

            migrationBuilder.RenameTable(
                name: "Reactions",
                schema: "ProductCatalog",
                newName: "ReviewReactions",
                newSchema: "ProductCatalog");

            migrationBuilder.RenameTable(
                name: "Ratings",
                schema: "ProductCatalog",
                newName: "ProductRatings",
                newSchema: "ProductCatalog");

            migrationBuilder.RenameIndex(
                name: "IX_Reactions_WriterId_ProductSku",
                schema: "ProductCatalog",
                table: "ReviewReactions",
                newName: "IX_ReviewReactions_WriterId_ProductSku");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewReactions",
                schema: "ProductCatalog",
                table: "ReviewReactions",
                columns: new[] { "ReactorId", "WriterId", "ProductSku" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductRatings",
                schema: "ProductCatalog",
                table: "ProductRatings",
                column: "Sku");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRatings_Products_Sku",
                schema: "ProductCatalog",
                table: "ProductRatings",
                column: "Sku",
                principalSchema: "ProductCatalog",
                principalTable: "Products",
                principalColumn: "Sku",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewReactions_Reviews_WriterId_ProductSku",
                schema: "ProductCatalog",
                table: "ReviewReactions",
                columns: new[] { "WriterId", "ProductSku" },
                principalSchema: "ProductCatalog",
                principalTable: "Reviews",
                principalColumns: new[] { "WriterId", "ProductSku" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductRatings_Products_Sku",
                schema: "ProductCatalog",
                table: "ProductRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewReactions_Reviews_WriterId_ProductSku",
                schema: "ProductCatalog",
                table: "ReviewReactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewReactions",
                schema: "ProductCatalog",
                table: "ReviewReactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductRatings",
                schema: "ProductCatalog",
                table: "ProductRatings");

            migrationBuilder.RenameTable(
                name: "ReviewReactions",
                schema: "ProductCatalog",
                newName: "Reactions",
                newSchema: "ProductCatalog");

            migrationBuilder.RenameTable(
                name: "ProductRatings",
                schema: "ProductCatalog",
                newName: "Ratings",
                newSchema: "ProductCatalog");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewReactions_WriterId_ProductSku",
                schema: "ProductCatalog",
                table: "Reactions",
                newName: "IX_Reactions_WriterId_ProductSku");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reactions",
                schema: "ProductCatalog",
                table: "Reactions",
                columns: new[] { "ReactorId", "WriterId", "ProductSku" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                schema: "ProductCatalog",
                table: "Ratings",
                column: "Sku");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Products_Sku",
                schema: "ProductCatalog",
                table: "Ratings",
                column: "Sku",
                principalSchema: "ProductCatalog",
                principalTable: "Products",
                principalColumn: "Sku",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_Reviews_WriterId_ProductSku",
                schema: "ProductCatalog",
                table: "Reactions",
                columns: new[] { "WriterId", "ProductSku" },
                principalSchema: "ProductCatalog",
                principalTable: "Reviews",
                principalColumns: new[] { "WriterId", "ProductSku" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
