namespace RookieShop.Shopping.Application.Abstractions;

public interface IShoppingOptionsProvider
{
    public int CartLifeTimeInMinutes { get; }
    public int CheckoutSessionDurationInMinutes { get; }
}