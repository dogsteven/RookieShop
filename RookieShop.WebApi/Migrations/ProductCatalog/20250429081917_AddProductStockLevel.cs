using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductCatalog
{
    /// <inheritdoc />
    public partial class AddProductStockLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Version",
                schema: "ProductCatalog",
                table: "ProductSemanticVectors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ProductStockLevels",
                schema: "ProductCatalog",
                columns: table => new
                {
                    ProductSku = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    AvailableQuantity = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStockLevels", x => x.ProductSku);
                    table.ForeignKey(
                        name: "FK_ProductStockLevels_Products_ProductSku",
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
                name: "ProductStockLevels",
                schema: "ProductCatalog");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "ProductCatalog",
                table: "ProductSemanticVectors");
        }
    }
}
