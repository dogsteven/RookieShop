using RookieShop.Shopping.Domain.Shared;

namespace RookieShop.Shopping.Domain.Carts.Events;

public class CartCheckoutStarted
{
    public Guid Id { get; init; }

    public IEnumerable<CheckoutItem> Items { get; init; } = null!;
}