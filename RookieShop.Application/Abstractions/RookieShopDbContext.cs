using Microsoft.EntityFrameworkCore;
using RookieShop.Application.Entities;

namespace RookieShop.Application.Abstractions;

public abstract class RookieShopDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    
    protected RookieShopDbContext(DbContextOptions options) : base(options) {}
}