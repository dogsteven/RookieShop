namespace RookieShop.ProductCatalog.Application.Exceptions;

public class ProductAlreadyExistsException : Exception
{
    public ProductAlreadyExistsException(string sku) : base($"Product {sku} already exists.") {}
}