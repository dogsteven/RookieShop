using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductReview
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProductReview");

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "ProductReview",
                columns: table => new
                {
                    WriterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductSku = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => new { x.WriterId, x.ProductSku });
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                schema: "ProductReview",
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
                        principalSchema: "ProductReview",
                        principalTable: "Reviews",
                        principalColumns: new[] { "WriterId", "ProductSku" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_WriterId_ProductSku",
                schema: "ProductReview",
                table: "Reactions",
                columns: new[] { "WriterId", "ProductSku" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductSku",
                schema: "ProductReview",
                table: "Reviews",
                column: "ProductSku");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reactions",
                schema: "ProductReview");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "ProductReview");
        }
    }
}
