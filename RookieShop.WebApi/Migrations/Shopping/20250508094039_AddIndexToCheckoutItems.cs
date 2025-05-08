using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.Shopping
{
    /// <inheritdoc />
    public partial class AddIndexToCheckoutItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CheckoutSessionCheckoutItems_CheckoutSessionId",
                schema: "Shopping",
                table: "CheckoutSessionCheckoutItems",
                column: "CheckoutSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CheckoutSessionCheckoutItems_CheckoutSessionId",
                schema: "Shopping",
                table: "CheckoutSessionCheckoutItems");
        }
    }
}
