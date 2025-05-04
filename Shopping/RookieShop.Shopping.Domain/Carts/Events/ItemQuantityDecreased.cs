namespace RookieShop.Shopping.Domain.Carts.Events;

public class ItemQuantityDecreased
{
    public Guid Id { get; init; }

    public string Sku { get; init; } = null!;
    
    public int QuantityDifference { get; init; }
}