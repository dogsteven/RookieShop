using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.Shopping
{
    /// <inheritdoc />
    public partial class AddCheckoutSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                schema: "Shopping",
                table: "Carts");

            migrationBuilder.AddColumn<bool>(
                name: "IsClosedForCheckout",
                schema: "Shopping",
                table: "Carts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CheckoutSessions",
                schema: "Shopping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckoutSessionBillingAddresses",
                schema: "Shopping",
                columns: table => new
                {
                    CheckoutSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutSessionBillingAddresses", x => x.CheckoutSessionId);
                    table.ForeignKey(
                        name: "FK_CheckoutSessionBillingAddresses_CheckoutSessions_CheckoutSe~",
                        column: x => x.CheckoutSessionId,
                        principalSchema: "Shopping",
                        principalTable: "CheckoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckoutSessionCheckoutItems",
                schema: "Shopping",
                columns: table => new
                {
                    CheckoutSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sku = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutSessionCheckoutItems", x => new { x.CheckoutSessionId, x.Sku });
                    table.ForeignKey(
                        name: "FK_CheckoutSessionCheckoutItems_CheckoutSessions_CheckoutSessi~",
                        column: x => x.CheckoutSessionId,
                        principalSchema: "Shopping",
                        principalTable: "CheckoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckoutSessionShippingAddresses",
                schema: "Shopping",
                columns: table => new
                {
                    CheckoutSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutSessionShippingAddresses", x => x.CheckoutSessionId);
                    table.ForeignKey(
                        name: "FK_CheckoutSessionShippingAddresses_CheckoutSessions_CheckoutS~",
                        column: x => x.CheckoutSessionId,
                        principalSchema: "Shopping",
                        principalTable: "CheckoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckoutSessionBillingAddresses",
                schema: "Shopping");

            migrationBuilder.DropTable(
                name: "CheckoutSessionCheckoutItems",
                schema: "Shopping");

            migrationBuilder.DropTable(
                name: "CheckoutSessionShippingAddresses",
                schema: "Shopping");

            migrationBuilder.DropTable(
                name: "CheckoutSessions",
                schema: "Shopping");

            migrationBuilder.DropColumn(
                name: "IsClosedForCheckout",
                schema: "Shopping",
                table: "Carts");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpirationTime",
                schema: "Shopping",
                table: "Carts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
