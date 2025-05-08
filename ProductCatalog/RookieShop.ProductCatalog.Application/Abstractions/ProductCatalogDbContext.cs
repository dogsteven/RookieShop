using Microsoft.EntityFrameworkCore;
using Pgvector;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.Shared.Models;

namespace RookieShop.ProductCatalog.Application.Abstractions;

public abstract class ProductCatalogDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductRating> ProductRatings { get; set; }
    public DbSet<ProductStockLevel> ProductStockLevels { get; set; }
    public DbSet<ProductSemanticVector> ProductSemanticVectors { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReviewReaction> ReviewReactions { get; set; }
    
    public DbSet<Purchase> Purchases { get; set; }

    public abstract Task<List<Product>> GetSemanticallyOrderedProductsAsync(Vector semanticVector, int offset, int limit, CancellationToken cancellationToken);
    
    protected ProductCatalogDbContext(DbContextOptions options) : base(options) {}
}