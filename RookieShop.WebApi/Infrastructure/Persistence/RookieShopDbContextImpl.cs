using Microsoft.EntityFrameworkCore;
using RookieShop.Application.Abstractions;
using RookieShop.WebApi.Infrastructure.Persistence.EntityConfigurations;

namespace RookieShop.WebApi.Infrastructure.Persistence;

public class RookieShopDbContextImpl : RookieShopDbContext, IPurchaseChecker
{
    public RookieShopDbContextImpl(DbContextOptions<RookieShopDbContextImpl> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RatingEntityConfiguration());
    }

    public ValueTask<bool> CheckIfCustomerHasPurchasedProductAsync(Guid customerId, string sku, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(true);
    }
}