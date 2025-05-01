using System.Net;
using Microsoft.AspNetCore.Authentication;

namespace RookieShop.FrontStore.Modules.Shared;

public class RookieShopHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RookieShopHttpClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient("RookieShop.WebApi");
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new RookieShopHttpClientUnauthorizedException();
        }

        return response;
    }

    public async Task<HttpResponseMessage> SecurelySendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);
        
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        return await SendAsync(request, cancellationToken);
    }
}

public class RookieShopHttpClientUnauthorizedException : Exception
{
    public RookieShopHttpClientUnauthorizedException() : base("Unauthorized") {}
}