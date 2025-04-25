namespace RookieShop.ProductCatalog.Application.Exceptions;

public class ProductNotFoundException : Exception
{
    public readonly string Sku;

    public ProductNotFoundException(string sku) : base($"Product {sku} was not found.")
    {
        Sku = sku;
    }
}