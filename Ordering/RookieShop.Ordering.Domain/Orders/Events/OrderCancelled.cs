namespace RookieShop.Ordering.Domain.Orders.Events;

public class OrderCancelled
{
    public Guid Id { get; init; }
    
    public Guid CustomerId { get; init; }

    public IEnumerable<OrderItem> Items { get; init; } = null!;
}