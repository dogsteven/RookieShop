namespace RookieShop.Shopping.Application.Exceptions;

public class StockItemNotFoundException : Exception
{
    public readonly string Sku;
    
    public StockItemNotFoundException(string sku) : base($"The stock item {sku} was not found.")
    {
        Sku = sku;
    }
}