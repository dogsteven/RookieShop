using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Domain;
using RookieShop.Shopping.Infrastructure.Persistence.EntityConfigurations;
using RookieShop.Shopping.Infrastructure.Persistence.Interceptors;

namespace RookieShop.Shopping.Infrastructure.Persistence;

public class ShoppingDbContext : DbContext
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    
    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : base(options.WithInterceptor(new UpdateVersionInterceptor())) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CartEntityConfiguration());
        modelBuilder.ApplyConfiguration(new StockItemEntityConfiguration());
    }
}

internal static class ShoppingDbContextExtensions
{
    internal static DbContextOptions WithInterceptor(this DbContextOptions options,
        ISaveChangesInterceptor interceptor)
    {
        var builder = new DbContextOptionsBuilder(options);

        builder.AddInterceptors(interceptor);

        return builder.Options;
    }
}