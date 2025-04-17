namespace RookieShop.FrontStore.Models.Shared.Application;

public class ProductDto
{
    public string Sku { get; init; } = null!;

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public int CategoryId { get; init; }

    public string CategoryName { get; init; } = null!;
    
    public string ImageUrl { get; init; }
    
    public bool IsFeatured { get; init; }
    
    public DateTime CreatedDate { get; init; }
    
    public DateTime UpdatedDate { get; init; }

    public RatingDto Rating { get; init; } = null!;
}

public class RatingDto
{
    public double Score { get; init; }
    
    public int OneCount { get; init; }
    
    public int TwoCount { get; init; }
    
    public int ThreeCount { get; init; }
    
    public int FourCount { get; init; }
    
    public int FiveCount { get; init; }
    
    public int Count => OneCount + TwoCount + ThreeCount + FourCount + FiveCount;
}