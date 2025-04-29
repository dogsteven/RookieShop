using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Domain;
using RookieShop.Shopping.Infrastructure.Persistence.EntityConfigurations;
using RookieShop.Shopping.Infrastructure.Persistence.Interceptors;

namespace RookieShop.Shopping.Infrastructure.Persistence;

public class ShoppingDbContext : DbContext, ICartRepository, IStockItemRepository
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<StockItem> StockItems { get; set; }
    
    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : base(options.WithInterceptor(new UpdateVersionInterceptor())) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CartEntityConfiguration());
        modelBuilder.ApplyConfiguration(new StockItemEntityConfiguration());
    }

    public Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Carts.FirstOrDefaultAsync(cart => cart.Id == id, cancellationToken); 
    }

    public void Save(Cart cart)
    {
        var entry = Entry(cart);

        if (entry.State == EntityState.Detached)
        {
            Carts.Add(cart);
        }
        else
        {
            Carts.Update(cart);
        }
    }

    public void Remove(Cart cart)
    {
        Carts.Remove(cart);
    }

    public Task<StockItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return StockItems.FirstOrDefaultAsync(stockItem => stockItem.Sku == sku, cancellationToken);
    }

    public void Save(StockItem stockItem)
    {
        var entry = Entry(stockItem);

        if (entry.State == EntityState.Detached)
        {
            StockItems.Add(stockItem);
        }
        else
        {
            StockItems.Update(stockItem);
        }
    }

    public void Remove(StockItem stockItem)
    {
        StockItems.Remove(stockItem);
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