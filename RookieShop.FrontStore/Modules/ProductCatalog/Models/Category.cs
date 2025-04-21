namespace RookieShop.FrontStore.Modules.ProductCatalog.Models;

public class Category
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;
    
    public int ProductCount { get; init; }
}