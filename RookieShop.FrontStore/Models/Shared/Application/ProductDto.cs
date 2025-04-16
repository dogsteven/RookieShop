using System.Text.Json.Serialization;

namespace RookieShop.FrontStore.Models.Shared.Application;

public class ProductDto
{
    public string Sku { get; init; } = null!;
    
    public string Name { get; init; } = null!;
    
    public string Description { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public int CategoryId { get; init; }
    
    public string CategoryName { get; init; } = null!;
    
    public string ImageUrl { get; init; } = null!;
    
    public bool IsFeatured { get; init; }
    
    public DateTime CreatedDate { get; init; }
    
    public DateTime UpdatedDate { get; init; }
    
    public float RatingScore { get; init; }
    
    public int RatingCount { get; init; }
}