namespace RookieShop.ProductCatalog.ViewModels;

public class CategoryDto
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;
    
    public int ProductCount { get; init; }
}