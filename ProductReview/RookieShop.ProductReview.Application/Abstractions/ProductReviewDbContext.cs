using Microsoft.EntityFrameworkCore;
using RookieShop.ProductReview.Application.Entities;

namespace RookieShop.ProductReview.Application.Abstractions;

public abstract class ProductReviewDbContext : DbContext
{
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    
    protected ProductReviewDbContext(DbContextOptions options) : base(options) {}
}