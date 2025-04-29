namespace RookieShop.Shopping.Application.Models;

public class StockItemDto
{
    public string Sku { get; init; } = null!;

    public string Name { get; init; } = null!;

    public decimal Price { get; init; }
    
    public Guid ImageId { get; init; }
    
    public int AvailableQuantity { get; init; }
    
    public int ReservedQuantity { get; init; }
}