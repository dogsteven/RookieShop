namespace RookieShop.Shopping.Domain.StockItems.Events;

public class StockLevelChanged
{
    public string Sku { get; init; } = null!;
    
    public int ChangedQuantity { get; init; }
}