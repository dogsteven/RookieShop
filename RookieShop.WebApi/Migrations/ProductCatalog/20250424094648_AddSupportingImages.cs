using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.ProductCatalog
{
    /// <inheritdoc />
    public partial class AddSupportingImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ISet<Guid>>(
                name: "SupportingImageIds",
                schema: "ProductCatalog",
                table: "Products",
                type: "uuid[]",
                nullable: false,
                defaultValueSql: "array[]::uuid[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportingImageIds",
                schema: "ProductCatalog",
                table: "Products");
        }
    }
}
