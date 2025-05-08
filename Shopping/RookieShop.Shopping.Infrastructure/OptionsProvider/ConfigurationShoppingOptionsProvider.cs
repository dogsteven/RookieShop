using Microsoft.Extensions.Configuration;
using RookieShop.Shopping.Application.Abstractions;

namespace RookieShop.Shopping.Infrastructure.OptionsProvider;

public class ConfigurationShoppingOptionsProvider : IShoppingOptionsProvider
{
    private readonly int _cartLifeTimeInMinutes;
    private readonly int _checkoutSessionDurationInMinutes;
    public int CartLifeTimeInMinutes => _cartLifeTimeInMinutes;
    public int CheckoutSessionDurationInMinutes => _checkoutSessionDurationInMinutes;

    public ConfigurationShoppingOptionsProvider(IConfiguration configuration)
    {
        var cartLifeTimeInMinutesString = configuration["Shopping:Cart:LifeTimeInMinutes"];
        var checkoutSessionDurationInMinutesString = configuration["Shopping:Cart:CheckoutSessionDurationInMinutes"];

        if (cartLifeTimeInMinutesString == null || !int.TryParse(cartLifeTimeInMinutesString, out _cartLifeTimeInMinutes))
        {
            _cartLifeTimeInMinutes = 60;
        }

        if (checkoutSessionDurationInMinutesString == null || !int.TryParse(checkoutSessionDurationInMinutesString, out _checkoutSessionDurationInMinutes))
        {
            _checkoutSessionDurationInMinutes = 5;
        }
    }
}