using Microsoft.Extensions.DependencyInjection;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Test.Utilities.Persistence;

namespace RookieShop.ProductCatalog.Test.Utilities;

public class ProductCatalogDatabaseSeeder
{
    private readonly IServiceProvider _provider;
    public readonly Guid CustomerId;

    public ProductCatalogDatabaseSeeder(IServiceProvider provider)
    {
        _provider = provider;
        CustomerId = Guid.NewGuid();
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ProductCatalogDbContextImpl>();

        var categories = new[]
        {
            new Category
            {
                Id = 1,
                Name = "Reflecting Telescopes",
                Description = "Use mirrors to reflect light, forming images. Ideal for deep-sky observing, free of chromatic aberration. Larger apertures at lower costs than refractors. Require regular collimation."
            },
            new Category
            {
                Id = 2,
                Name = "Refracting Telescopes",
                Description = "Use lenses to bend light, focusing images. Known for high-contrast, sharp views, ideal for planets and astrophotography. Prone to chromatic aberration in lower-end models, higher cost for large apertures."
            }
        };
        
        await context.Categories.AddRangeAsync(categories, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);

        var products = new[]
        {
            new Product
            {
                Sku = "CELE114LCM",
                Name = "Celestron 114LCM Newtonian",
                Description = "Computerized Newtonian reflector with 114mm aperture. Ideal for beginners, includes motorized mount, two eyepieces, and astronomy software. Great for viewing planets and deep-sky objects with crisp, aberration-free images.",
                Price = decimal.Parse("349.99"),
                Category = categories[0],
                PrimaryImageId = Guid.NewGuid(),
                SupportingImageIds = [],
                IsFeatured = true,
                CreatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10)),
                UpdatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(9)),
                Rating = new ProductRating("CELE114LCM"),
                StockLevel = new ProductStockLevel("CELE114LCM")
            },
            new Product
            {
                Sku = "SKYWATCH12DOB",
                Name = "Sky-Watcher 12\" Dobsonian",
                Description = "12-inch reflector with 1500mm focal length. Large aperture for faint deep-sky objects like galaxies. Simple rocker-box mount, ideal for visual observers.",
                Price = decimal.Parse("799.99"),
                Category = categories[0],
                PrimaryImageId = Guid.NewGuid(),
                SupportingImageIds = [],
                IsFeatured = false,
                CreatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10)),
                UpdatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                Rating = new ProductRating("SKYWATCH12DOB"),
                StockLevel = new ProductStockLevel("SKYWATCH12DOB")
            },
            new Product
            {
                Sku = "BRESSARCT60",
                Name = "Bresser Arcturus 60/700",
                Description = "60mm achromatic refractor with 700mm focal length. Azimuthal mount, includes case and accessories. Ideal for kids and beginners, great for Moon and star clusters.",
                Price = decimal.Parse("149.99"),
                Category = categories[1],
                PrimaryImageId = Guid.NewGuid(),
                SupportingImageIds = [],
                IsFeatured = false,
                CreatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(8)),
                UpdatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                Rating = new ProductRating("BRESSARCT60"),
                StockLevel = new ProductStockLevel("BRESSARCT60")
            },
            new Product
            {
                Sku = "ASKAR160APO",
                Name = "Askar 203 APO Triplet",
                Description = "8-inch apochromatic refractor with 1400mm focal length. Triplet ED glass reduces chromatic aberration, ideal for astrophotography. Compact, premium build for deep-sky imaging.",
                Price = decimal.Parse("1999.99"),
                Category = categories[1],
                PrimaryImageId = Guid.NewGuid(),
                SupportingImageIds = [],
                IsFeatured = true,
                CreatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(7)),
                UpdatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(5)),
                Rating = new ProductRating("ASKAR160APO"),
                StockLevel = new ProductStockLevel("ASKAR160APO")
            }
        };
        
        products[0].Rating.ApplyScore(5);
        products[0].Rating.ApplyScore(3);
        
        products[3].Rating.ApplyScore(4);
        
        context.Products.AddRange(products);
        
        await context.SaveChangesAsync(cancellationToken);

        var reviews = new[]
        {
            new Review
            {
                WriterId = CustomerId,
                ProductSku = "CELE114LCM",
                WriterName = "Khoa",
                Score = 5,
                Comment = "Extremely good",
                CreatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(7)),
            },
            new Review
            {
                WriterId = Guid.NewGuid(),
                ProductSku = "CELE114LCM",
                WriterName = "An",
                Score = 3,
                Comment = "Not very special",
                CreatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(7)),
            },
            new Review
            {
                WriterId = CustomerId,
                ProductSku = "ASKAR160APO",
                WriterName = "Khoa",
                Score = 4,
                Comment = "Pretty good",
                CreatedDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(7)),
            }
        };
        
        context.Reviews.AddRange(reviews);
        
        await context.SaveChangesAsync(cancellationToken);
    }
}