using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RevertToInitialState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_ProductCategoryId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "RatingScore",
                table: "Ratings",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "RatingCreatedDate",
                table: "Ratings",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "RatingComment",
                table: "Ratings",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "RatingSku",
                table: "Ratings",
                newName: "Sku");

            migrationBuilder.RenameColumn(
                name: "RatingCustomerId",
                table: "Ratings",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_RatingSku",
                table: "Ratings",
                newName: "IX_Ratings_Sku");

            migrationBuilder.RenameColumn(
                name: "ProductUpdatedDate",
                table: "Products",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductIsFeatured",
                table: "Products",
                newName: "IsFeatured");

            migrationBuilder.RenameColumn(
                name: "ProductImageUrl",
                table: "Products",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "ProductDescription",
                table: "Products",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ProductCreatedDate",
                table: "Products",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "ProductSku",
                table: "Products",
                newName: "Sku");

            migrationBuilder.RenameColumn(
                name: "ProductCategoryId",
                table: "Products",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "Categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CategoryDescription",
                table: "Categories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Ratings",
                newName: "RatingScore");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Ratings",
                newName: "RatingCreatedDate");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Ratings",
                newName: "RatingComment");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "Ratings",
                newName: "RatingSku");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Ratings",
                newName: "RatingCustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_Sku",
                table: "Ratings",
                newName: "IX_Ratings_RatingSku");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Products",
                newName: "ProductUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "ProductPrice");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "IsFeatured",
                table: "Products",
                newName: "ProductIsFeatured");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "ProductImageUrl");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "ProductDescription");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Products",
                newName: "ProductCreatedDate");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "Products",
                newName: "ProductSku");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Products",
                newName: "ProductCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                newName: "IX_Products_ProductCategoryId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categories",
                newName: "CategoryName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Categories",
                newName: "CategoryDescription");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Categories",
                newName: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
