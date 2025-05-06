namespace RookieShop.Shopping.Domain.Carts.Events;

public class CartExpirationTimeExtended
{
    public Guid Id { get; init; }
    
    public DateTimeOffset ExpirationDate { get; init; }
}