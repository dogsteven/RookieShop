namespace RookieShop.ProductCatalog.Application.Events;

public class ProductCreatedOrUpdated
{
    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}