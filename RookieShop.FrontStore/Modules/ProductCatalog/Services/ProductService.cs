using System.Web;
using Microsoft.AspNetCore.Authentication;
using RookieShop.FrontStore.Exceptions;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;
using IProductService = RookieShop.FrontStore.Modules.ProductCatalog.Abstractions.IProductService;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly string _imageGalleryBasePath;

    public ProductService(IHttpClientFactory httpClientFactory, string imageGalleryBasePath)
    {
        _httpClient = httpClientFactory.CreateClient("RookieShop.WebApi");
        _imageGalleryBasePath = imageGalleryBasePath;
    }
    
    public async Task<Product> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/{sku}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        return await response.ReadFromJsonAsync<Product>(cancellationToken);
    }

    public async Task<Pagination<Product>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageNumber"] = $"{pageNumber}";
        queries["pageSize"] = $"{pageSize}";
        
        var queryString = queries.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/all?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<Pagination<Product>>(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int maxCount, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["maxCount"] = $"{maxCount}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/featured?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<IEnumerable<Product>>(cancellationToken);
    }

    public async Task<Pagination<Product>> GetProductsByCategoryIdAsync(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageSize"] = $"{pageSize}";
        queries["pageNumber"] = $"{pageNumber}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/by-category/{categoryId}?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<Pagination<Product>>(cancellationToken);
    }

    public async Task<Pagination<Product>> GetProductsSemanticAsync(string semantic, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["semantic"] = semantic;
        queries["pageNumber"] = $"{pageNumber}";
        queries["pageSize"] = $"{pageSize}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Product/semantic?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<Pagination<Product>>(cancellationToken);
    }
}