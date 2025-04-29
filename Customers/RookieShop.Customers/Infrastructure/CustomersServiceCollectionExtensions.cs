using Microsoft.Extensions.DependencyInjection;

namespace RookieShop.Customers.Infrastructure;

public static class CustomersServiceCollectionExtensions
{
    public static IServiceCollection AddCustomers(this IServiceCollection services,
        Action<CustomersConfigurator> configure)
    {
        var configurator = new CustomersConfigurator();
        configure(configurator);
        
        return configurator.ConfigureServices(services);
    }
}

public class CustomersConfigurator
{
    private Func<IServiceProvider, HttpClient>? _httpClientFactory;
    private Func<IServiceProvider, KeycloakCustomerServiceOptions>? _keycloakCustomerServiceOptions;

    internal CustomersConfigurator()
    {
        _httpClientFactory = null;
        _keycloakCustomerServiceOptions = null;
    }

    public CustomersConfigurator SetHttpClient(Func<IServiceProvider, HttpClient> httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        return this;
    }
    
    public CustomersConfigurator SetKeycloakOptions(Func<IServiceProvider, KeycloakCustomerServiceOptions>? keycloakCustomerServiceOptions)
    {
        _keycloakCustomerServiceOptions = keycloakCustomerServiceOptions;
        return this;
    }

    internal IServiceCollection ConfigureServices(IServiceCollection services)
    {
        if (_keycloakCustomerServiceOptions == null || _httpClientFactory == null)
        {
            return services;
        }
        
        services.AddSingleton<ICustomerService>(provider =>
        {
            var httpClient = _httpClientFactory(provider);
            var options = _keycloakCustomerServiceOptions(provider);
            
            return new KeycloakCustomerService(httpClient, options);
        });

        return services;
    }
}