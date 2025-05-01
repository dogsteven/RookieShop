using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using RookieShop.FrontStore.Exceptions;
using RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.FrontStore.Modules.Shared;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Services;

public class ReviewService : IReviewService
{
    private readonly RookieShopHttpClient _httpClient;

    public ReviewService(RookieShopHttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Pagination<Review>> GetReviewsBySkuAsync(string sku, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageSize"] = $"{pageSize}";
        queries["pageNumber"] = $"{pageNumber}";
        
        var queryString = queries.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/product-catalog/api/reviews/{sku}?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var pagination = await response.Content.ReadFromJsonAsync<Pagination<Review>>(cancellationToken: cancellationToken);
        
        ArgumentNullException.ThrowIfNull(pagination);
        
        return pagination;
    }

    public async Task SubmitReviewAsync(string sku, int score, string comment, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/product-catalog/api/reviews/{sku}");

        var body = new { score, comment };
        
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SecurelySendAsync(request, cancellationToken);

        await response.EnsureSuccess(cancellationToken);
    }
    
    public async Task MakeReactionAsync(Guid writerId, string sku, bool likeReaction, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/product-catalog/api/reviews/{writerId}");

        var body = new { reactionType = likeReaction ? 0 : 1 };
        
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SecurelySendAsync(request, cancellationToken);
        
        await response.EnsureSuccess(cancellationToken);
    }
}