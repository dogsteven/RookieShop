using Microsoft.Extensions.Configuration;
using RookieShop.Shopping.Application.Abstractions;

namespace RookieShop.Shopping.Infrastructure.CartOptionsProvider;

public class ConfigurationCartOptionsProvider : ICartOptionsProvider
{
    private readonly int _lifeTimeInMinutes;
    public int LifeTimeInMinutes => _lifeTimeInMinutes;

    public ConfigurationCartOptionsProvider(IConfiguration configuration)
    {
        var lifeTimeInMinutesString = configuration["Shopping:Cart:LifeTimeInMinutes"];

        if (lifeTimeInMinutesString == null || !int.TryParse(lifeTimeInMinutesString, out _lifeTimeInMinutes))
        {
            _lifeTimeInMinutes = 60;
        }
    }
}