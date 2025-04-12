namespace RookieShop.Application.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(string sku) : base($"Product {sku} was not found.") {}
}