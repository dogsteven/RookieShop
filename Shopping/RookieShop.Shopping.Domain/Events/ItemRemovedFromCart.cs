namespace RookieShop.Shopping.Domain.Events;

public class ItemRemovedFromCart
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = null!;
    public int Quantity { get; init; }
}