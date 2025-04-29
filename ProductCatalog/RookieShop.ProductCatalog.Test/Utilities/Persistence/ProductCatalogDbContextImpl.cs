using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Infrastructure.Persistence.EntityConfigurations;
using RookieShop.ProductCatalog.Infrastructure.Persistence.Interceptors;

namespace RookieShop.ProductCatalog.Test.Utilities.Persistence;

public class ProductCatalogDbContextImpl : ProductCatalogDbContext
{
    public ProductCatalogDbContextImpl(DbContextOptions<ProductCatalogDbContextImpl> options)
        : base(options.WithInterceptor(new UpdateVersionInterceptor())) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductRatingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductStockLevelEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UnitTestProductSemanticVectorEntityConfiguration());
                    
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
        
        modelBuilder.ApplyConfiguration(new ReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewReactionEntityConfiguration());
    }

    public override Task<List<Product>> GetSemanticallyOrderedProductsAsync(Vector semanticVector, int offset, int limit,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<Product>());
    }
}

public class UnitTestProductSemanticVectorEntityConfiguration : IEntityTypeConfiguration<ProductSemanticVector>
{
    public void Configure(EntityTypeBuilder<ProductSemanticVector> builder)
    {
        builder.ToTable("ProductSemanticVectors", schema: "ProductCatalog");

        builder.HasKey(productSemanticVector => productSemanticVector.ProductSku);
        
        builder.Property(productSemanticVector => productSemanticVector.ProductSku)
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnName("ProductSku");

        builder.Property(productSemanticVector => productSemanticVector.SemanticVector)
            .IsRequired()
            .HasConversion<string>(vector => "", column => new Vector(Array.Empty<float>()))
            .HasColumnName("SemanticVector");
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