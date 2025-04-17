using Microsoft.EntityFrameworkCore;
using RookieShop.ProductReview.Application.Abstractions;
using RookieShop.ProductReview.Infrastructure.Persistence.EntityConfigurations;

namespace RookieShop.ProductReview.Infrastructure.Persistence;

public class ProductReviewDbContextImpl : ProductReviewDbContext
{
    public ProductReviewDbContextImpl(DbContextOptions<ProductReviewDbContextImpl> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ReviewEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ReactionEntityConfiguration());
    }
}