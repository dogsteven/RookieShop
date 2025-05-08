using RookieShop.Shared.Models;
using RookieShop.Shopping.Domain.Shared;

namespace RookieShop.Shopping.Domain.CheckoutSessions.Events;

public class CheckoutSessionCompleted
{
    public Guid Id { get; init; }
    
    public Guid SessionId { get; init; }

    public Address BillingAddress { get; init; } = null!;

    public Address ShippingAddress { get; init; } = null!;

    public IEnumerable<CheckoutItem> Items { get; init; } = null!;
}