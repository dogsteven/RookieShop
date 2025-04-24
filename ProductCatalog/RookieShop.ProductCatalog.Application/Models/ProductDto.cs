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
    
    public Guid PrimaryImageId { get; init; }
    
    public IEnumerable<Guid> SupportingImageIds { get; init; }
    
    public bool IsFeatured { get; init; }
    
    public DateTime CreatedDate { get; init; }
    
    public DateTime UpdatedDate { get; init; }

    public RatingDto Rating { get; init; }
    
#pragma warning disable CS8618, CS9264
    public ProductDto() {}
#pragma warning restore CS8618, CS9264
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