namespace RookieShop.ProductCatalog.Application.Exceptions;

public class ProductSkuHasAlreadyBeenTakenException : Exception
{
    public readonly string Sku;

    public ProductSkuHasAlreadyBeenTakenException(string sku) : base($"Product {sku} has already been taken.")
    {
        Sku = sku;
    }
}