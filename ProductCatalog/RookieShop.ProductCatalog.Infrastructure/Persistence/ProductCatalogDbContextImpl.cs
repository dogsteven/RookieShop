using MassTransit.UsageTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;
using RookieShop.ProductCatalog.Infrastructure.Persistence.Interceptors;
using RookieShop.Shared.Models;

namespace RookieShop.ProductCatalog.Infrastructure.Persistence;

public class ProductCatalogDbContextImpl : ProductCatalogDbContext
{
    public ProductCatalogDbContextImpl(DbContextOptions<ProductCatalogDbContextImpl> options)
        : base(options.WithInterceptor(new UpdateVersionInterceptor())) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductRatingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductStockLevelEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductSemanticVectorEntityConfiguration());
                    
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
        
        modelBuilder.ApplyConfiguration(new ReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewReactionEntityConfiguration());
    }

    public override async Task<List<Product>> GetSemanticallyOrderedProductsAsync(Vector semanticVector, int offset, int limit, CancellationToken cancellationToken)
    {
        return await Products
            .Include(product => product.Category)
            .Include(product => product.Rating)
            .Include(product => product.StockLevel)
            .OrderBy(product => semanticVector.L2Distance(product.SemanticVector.SemanticVector))
            .Skip(offset)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
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