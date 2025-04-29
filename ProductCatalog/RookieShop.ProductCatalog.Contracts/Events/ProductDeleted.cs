namespace RookieShop.ProductCatalog.Contracts.Events;

public class ProductDeleted
{
    public string Sku { get; init; } = null!;
}