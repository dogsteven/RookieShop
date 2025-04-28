using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductCatalog
{
    /// <inheritdoc />
    public partial class AddSemanticVectorForProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSemanticVectors",
                schema: "ProductCatalog",
                columns: table => new
                {
                    ProductSku = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    SemanticVector = table.Column<Vector>(type: "vector", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSemanticVectors", x => x.ProductSku);
                    table.ForeignKey(
                        name: "FK_ProductSemanticVectors_Products_ProductSku",
                        column: x => x.ProductSku,
                        principalSchema: "ProductCatalog",
                        principalTable: "Products",
                        principalColumn: "Sku",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSemanticVectors",
                schema: "ProductCatalog");
        }
    }
}
