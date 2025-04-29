namespace RookieShop.Shopping.Contracts.Events;

public class StockLevelUpdated
{
    public string Sku { get; init; } = null!;
    
    public int AvailableQuantity { get; init; }
}