using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Application.Abstractions;

public abstract class ProductCatalogDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    
    protected ProductCatalogDbContext(DbContextOptions options) : base(options) {}
}