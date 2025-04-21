using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Entities;

namespace RookieShop.ImageGallery.Abstractions;

public abstract class ImageGalleryDbContext : DbContext
{
    public DbSet<Image> Images { get; set; }
    
    protected ImageGalleryDbContext(DbContextOptions options) : base(options) {}
}