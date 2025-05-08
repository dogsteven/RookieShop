using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RookieShop.Ordering.Domain.Orders;
using RookieShop.Ordering.Infrastructure.Persistence.EntityConfigurations;
using RookieShop.Ordering.Infrastructure.Persistence.Interceptors;

namespace RookieShop.Ordering.Infrastructure.Persistence;

public class OrderingDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options.WithInterceptor(new UpdateVersionInterceptor())) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
    }
}

internal static class OrderingDbContextExtensions
{
    internal static DbContextOptions WithInterceptor(this DbContextOptions options,
        ISaveChangesInterceptor interceptor)
    {
        var builder = new DbContextOptionsBuilder(options);

        builder.AddInterceptors(interceptor);

        return builder.Options;
    }
}