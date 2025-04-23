using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Services;

public class ReviewService : IReviewService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<ReviewService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("RookieShop.WebApi");
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    
    public async Task<Pagination<Review>> GetRatingsBySkuAsync(string sku, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageSize"] = $"{pageSize}";
        queries["pageNumber"] = $"{pageNumber}";
        
        var queryString = queries.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Review/{sku}?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var pagination = await response.Content.ReadFromJsonAsync<Pagination<Review>>(cancellationToken: cancellationToken);
        
        ArgumentNullException.ThrowIfNull(pagination);
        
        return pagination;
    }

    public async Task SubmitReviewAsync(string sku, int score, string comment, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);
        
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        
        _logger.LogInformation("Access Token: {0}", accessToken);
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Review/{sku}");

        var body = new
        {
            score = score,
            comment = comment
        };

        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
    
    public async Task MakeReactionAsync(Guid writerId, string sku, bool likeReaction, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);
        
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Review/{writerId}");

        var body = new
        {
            reactionType = likeReaction ? 0 : 1
        };
        
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }
}