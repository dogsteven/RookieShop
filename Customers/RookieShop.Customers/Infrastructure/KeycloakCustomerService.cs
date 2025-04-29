using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace RookieShop.Customers.Infrastructure;

public class KeycloakCustomerService : ICustomerService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakCustomerServiceOptions _options;

    public KeycloakCustomerService(HttpClient httpClient, KeycloakCustomerServiceOptions options)
    {
        _httpClient = httpClient;
        _options = options;

        _httpClient.BaseAddress = new Uri(_options.Address);
    }
    
    public async Task<IEnumerable<Customer>> GetCustomersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var accessToken = await GetAccessTokenAsync(cancellationToken);
        
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["first"] = $"{(pageNumber - 1) * pageSize}";
        queries["max"] = $"{pageSize}";
        
        var queryString = queries.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/admin/realms/rookie-shop/groups/{_options.CustomersGroupId}/members?{queryString}");
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var customers = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(customers);
        
        return customers;
    }
    
    private async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/realms/master/protocol/openid-connect/token");
        request.Content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _options.ClientId),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret)
        ]);
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
        
        var authenticationResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(authenticationResponse);
            
        return authenticationResponse.AccessToken;

    }

    private class AuthenticationResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;
    }
}

public class KeycloakCustomerServiceOptions
{
    public string Address { get; set; } = null!;
    public string Realm { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string CustomersGroupId { get; set; } = null!;
}