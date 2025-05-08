namespace RookieShop.Ordering.Domain.Orders.Events;

public class OrderPlaced
{
    public Guid Id { get; init; }
    
    public Guid CustomerId { get; init; }
}