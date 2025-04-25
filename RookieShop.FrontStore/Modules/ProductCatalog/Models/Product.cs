namespace RookieShop.FrontStore.Modules.ProductCatalog.Models;

public class Product
{
    public string Sku { get; init; } = null!;

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public int CategoryId { get; init; }

    public string CategoryName { get; init; } = null!;

    public Guid PrimaryImageId { get; init; }

    public ISet<Guid> SupportingImageIds { get; init; } = null!;
    
    public bool IsFeatured { get; init; }
    
    public DateTime CreatedDate { get; init; }
    
    public DateTime UpdatedDate { get; init; }

    public Rating Rating { get; init; } = null!;
}

public class Rating
{
    public double Score { get; init; }
    
    public int OneCount { get; init; }
    
    public int TwoCount { get; init; }
    
    public int ThreeCount { get; init; }
    
    public int FourCount { get; init; }
    
    public int FiveCount { get; init; }
    
    public int Count => OneCount + TwoCount + ThreeCount + FourCount + FiveCount;
}