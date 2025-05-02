namespace RookieShop.Shopping.Domain.Carts.Events;

public class ItemQuantityAdjusted
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = null!;
    public int OldQuantity { get; init; }
    public int NewQuantity { get; init; }
}