using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using RookieShop.FrontStore.Abstractions;
using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReviewService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient("RookieShop.WebApi");
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<Pagination<RatingDto>> GetRatingsBySkuAsync(string sku, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageSize"] = $"{pageSize}";
        queries["pageNumber"] = $"{pageNumber}";
        
        var queryString = queries.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Review/{sku}?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var pagination = await response.Content.ReadFromJsonAsync<Pagination<RatingDto>>(cancellationToken: cancellationToken);
        
        ArgumentNullException.ThrowIfNull(pagination);
        
        return pagination;
    }

    public async Task WriteRatingAsync(string sku, float score, string comment, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);
        
        var accessToken = _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Review/{sku}");

        var body = new
        {
            Score = score,
            Comment = comment
        };

        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        request.Content = new StringContent(JsonSerializer.Serialize(body));
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
}