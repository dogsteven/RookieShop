using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Application.Entities;

namespace RookieShop.ImageGallery.Application.Abstractions;

public abstract class ImageGalleryDbContext : DbContext
{
    public DbSet<Image> Images { get; set; }
    
    protected ImageGalleryDbContext(DbContextOptions options) : base(options) {}
}