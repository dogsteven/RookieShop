using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductCatalog
{
    /// <inheritdoc />
    public partial class FixTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchase",
                schema: "ProductCatalog",
                table: "Purchase");

            migrationBuilder.RenameTable(
                name: "Purchase",
                schema: "ProductCatalog",
                newName: "Purchases",
                newSchema: "ProductCatalog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchases",
                schema: "ProductCatalog",
                table: "Purchases",
                columns: new[] { "CustomerId", "ProductSku" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchases",
                schema: "ProductCatalog",
                table: "Purchases");

            migrationBuilder.RenameTable(
                name: "Purchases",
                schema: "ProductCatalog",
                newName: "Purchase",
                newSchema: "ProductCatalog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchase",
                schema: "ProductCatalog",
                table: "Purchase",
                columns: new[] { "CustomerId", "ProductSku" });
        }
    }
}
