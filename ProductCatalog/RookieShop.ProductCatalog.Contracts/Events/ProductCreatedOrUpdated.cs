namespace RookieShop.ProductCatalog.Contracts.Events;

public class ProductCreatedOrUpdated
{
    public string Sku { get; init; } = null!;
    
    public string Name { get; init; } = null!;
    
    public string Description { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public Guid PrimaryImageId { get; init; }
}