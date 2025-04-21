using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductCatalog
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProductCatalog");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "ProductCatalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "ProductCatalog",
                columns: table => new
                {
                    WriterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductSku = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    WriterName = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => new { x.WriterId, x.ProductSku });
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "ProductCatalog",
                columns: table => new
                {
                    Sku = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    PrimaryImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Sku);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "ProductCatalog",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                schema: "ProductCatalog",
                columns: table => new
                {
                    ReactorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WriterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductSku = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => new { x.ReactorId, x.WriterId, x.ProductSku });
                    table.ForeignKey(
                        name: "FK_Reactions_Reviews_WriterId_ProductSku",
                        columns: x => new { x.WriterId, x.ProductSku },
                        principalSchema: "ProductCatalog",
                        principalTable: "Reviews",
                        principalColumns: new[] { "WriterId", "ProductSku" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                schema: "ProductCatalog",
                columns: table => new
                {
                    Sku = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    OneCount = table.Column<int>(type: "integer", nullable: false),
                    TwoCount = table.Column<int>(type: "integer", nullable: false),
                    ThreeCount = table.Column<int>(type: "integer", nullable: false),
                    FourCount = table.Column<int>(type: "integer", nullable: false),
                    FiveCount = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Sku);
                    table.ForeignKey(
                        name: "FK_Ratings_Products_Sku",
                        column: x => x.Sku,
                        principalSchema: "ProductCatalog",
                        principalTable: "Products",
                        principalColumn: "Sku",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "ProductCatalog",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                schema: "ProductCatalog",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_WriterId_ProductSku",
                schema: "ProductCatalog",
                table: "Reactions",
                columns: new[] { "WriterId", "ProductSku" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductSku",
                schema: "ProductCatalog",
                table: "Reviews",
                column: "ProductSku");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ratings",
                schema: "ProductCatalog");

            migrationBuilder.DropTable(
                name: "Reactions",
                schema: "ProductCatalog");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "ProductCatalog");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "ProductCatalog");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "ProductCatalog");
        }
    }
}
