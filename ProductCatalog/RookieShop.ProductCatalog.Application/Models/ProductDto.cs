using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Application.Models;

public class ProductDto
{
    public string Sku { get; init; }
    
    public string Name { get; init; }
    
    public string Description { get; init; }
    
    public decimal Price { get; init; }
    
    public int CategoryId { get; init; }
    
    public string CategoryName { get; init; }
    
    public string ImageUrl { get; init; }
    
    public bool IsFeatured { get; init; }
    
    public DateTime CreatedDate { get; init; }
    
    public DateTime UpdatedDate { get; init; }

    public RatingDto Rating { get; init; }

    internal ProductDto(Product product)
    {
        Sku = product.Sku;
        Name = product.Name;
        Description = product.Description;
        Price = product.Price;
        CategoryId = product.Category.Id;
        CategoryName = product.Category.Name;
        ImageUrl = product.ImageUrl;
        IsFeatured = product.IsFeatured;
        CreatedDate = product.CreatedDate;
        UpdatedDate = product.UpdatedDate;
        Rating = new RatingDto
        {
            Score = product.Rating.Score,
            OneCount = product.Rating.OneCount,
            TwoCount = product.Rating.TwoCount,
            ThreeCount = product.Rating.ThreeCount,
            FourCount = product.Rating.FourCount,
            FiveCount = product.Rating.FiveCount
        };
    }
}

public class RatingDto
{
    public double Score { get; init; }
    
    public int OneCount { get; init; }
    
    public int TwoCount { get; init; }
    
    public int ThreeCount { get; init; }
    
    public int FourCount { get; init; }
    
    public int FiveCount { get; init; }
}