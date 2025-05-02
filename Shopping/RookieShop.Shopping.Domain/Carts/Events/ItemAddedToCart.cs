namespace RookieShop.Shopping.Domain.Carts.Events;

public class ItemAddedToCart
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = null!;
    public int Quantity { get; init; }
}