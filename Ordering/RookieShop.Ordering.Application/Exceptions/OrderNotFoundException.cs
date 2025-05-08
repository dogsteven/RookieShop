namespace RookieShop.Ordering.Application.Exceptions;

public class OrderNotFoundException : Exception
{
    public readonly Guid Id;

    public OrderNotFoundException(Guid id) : base($"Order {id} was not found.")
    {
        Id = id;
    }
}