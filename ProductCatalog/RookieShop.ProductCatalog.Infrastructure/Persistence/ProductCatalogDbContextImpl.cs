using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;
using RookieShop.ProductCatalog.Infrastructure.Persistence.Interceptors;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence;

public class ProductCatalogDbContextImpl : ProductCatalogDbContext
{
    public ProductCatalogDbContextImpl(DbContextOptions<ProductCatalogDbContextImpl> options)
        : base(options.WithInterceptor(new UpdateVersionInterceptor())) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RatingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
    }
}

internal static class ProductCatalogDbContextExtensions
{
    internal static DbContextOptions WithInterceptor(this DbContextOptions options,
        ISaveChangesInterceptor interceptor)
    {
        var builder = new DbContextOptionsBuilder(options);

        builder.AddInterceptors(interceptor);

        return builder.Options;
    }
}