namespace RookieShop.Shopping.Domain.Carts.Events;

public class CartExpirationDateExtended
{
    public Guid Id { get; init; }
    
    public DateTimeOffset ExpirationDate { get; init; }
}