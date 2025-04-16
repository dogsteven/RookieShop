using System.Web;
using RookieShop.FrontStore.Abstractions;
using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("RookieShop.WebApi");
    }
    
    public async Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/{sku}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var productDto = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(productDto);
        
        return productDto;
    }

    public async Task<Pagination<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageNumber"] = $"{pageNumber}";
        queries["pageSize"] = $"{pageSize}";
        
        var queryString = queries.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/all?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var pagination = await response.Content.ReadFromJsonAsync<Pagination<ProductDto>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(pagination);
        
        return pagination;
    }

    public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int maxCount, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["maxCount"] = $"{maxCount}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/featured?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var productDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(productDtos);
        
        return productDtos;
    }

    public async Task<Pagination<ProductDto>> GetProductsByCategoryIdAsync(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageSize"] = $"{pageSize}";
        queries["pageNumber"] = $"{pageNumber}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/by-category/{categoryId}?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var pagination = await response.Content.ReadFromJsonAsync<Pagination<ProductDto>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(pagination);
        
        return pagination;
    }
}