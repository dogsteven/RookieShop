using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Infrastructure.Persistence.EntityConfigurations;
using RookieShop.ImageGallery.Infrastructure.Persistence.Interceptors;

namespace RookieShop.ImageGallery.Infrastructure.Persistence;

public class ImageGalleryDbContextImpl : ImageGalleryDbContext
{
    public ImageGalleryDbContextImpl(DbContextOptions<ImageGalleryDbContextImpl> options) : base(options.WithInterceptor(new UpdateVersionInterceptor())) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ImageEntityConfiguration());
    }
}

internal static class ImageGalleryDbContextExtensions
{
    internal static DbContextOptions WithInterceptor(this DbContextOptions options,
        ISaveChangesInterceptor interceptor)
    {
        var builder = new DbContextOptionsBuilder(options);

        builder.AddInterceptors(interceptor);

        return builder.Options;
    }
}