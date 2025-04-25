namespace RookieShop.ProductCatalog.Application.Exceptions;

public class ProductAlreadyExistsException : Exception
{
    public readonly string Sku;

    public ProductAlreadyExistsException(string sku) : base($"Product {sku} already exists.")
    {
        Sku = sku;
    }
}