namespace RookieShop.Shopping.Contracts.Events;

public class StockLevelChanged
{
    public string Sku { get; init; } = null!;
    
    public int ChangedQuantity { get; init; }
}