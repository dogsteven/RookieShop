using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RookieShop.WebApi.Migrations.ImageGallery
{
    /// <inheritdoc />
    public partial class RenameImageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsUploaded",
                schema: "ImageGallery",
                table: "Images",
                newName: "IsSynced");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSynced",
                schema: "ImageGallery",
                table: "Images",
                newName: "IsUploaded");
        }
    }
}
