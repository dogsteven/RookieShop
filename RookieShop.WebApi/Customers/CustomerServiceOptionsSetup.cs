using Microsoft.Extensions.Options;

namespace RookieShop.WebApi.Customers;

public class CustomerServiceOptionsSetup : IConfigureOptions<CustomerServiceOptions> 
{
    private readonly IConfiguration _configuration;

    public CustomerServiceOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(CustomerServiceOptions options)
    {
        options.Address = _configuration["Keycloak:AuthSettings:Address"]!;
        options.Realm = _configuration["Keycloak:AuthSettings:Realm"]!;
        options.ClientId = _configuration["Keycloak:ServiceAccount:ClientId"]!;
        options.ClientSecret = _configuration["Keycloak:ServiceAccount:ClientSecret"]!;
        options.CustomersGroupId = _configuration["Keycloak:Customers:GroupId"]!;
    }
}