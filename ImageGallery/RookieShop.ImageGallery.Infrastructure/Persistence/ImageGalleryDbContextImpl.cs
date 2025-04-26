using Microsoft.EntityFrameworkCore;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Infrastructure.Persistence.EntityConfigurations;

namespace RookieShop.ImageGallery.Infrastructure.Persistence;

public class ImageGalleryDbContextImpl : ImageGalleryDbContext
{
    public ImageGalleryDbContextImpl(DbContextOptions<ImageGalleryDbContextImpl> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ImageEntityConfiguration());
    }
}