using System.Web;
using Microsoft.AspNetCore.Authentication;
using RookieShop.FrontStore.Exceptions;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.FrontStore.Modules.Shared;
using RookieShop.Shared.Models;
using IProductService = RookieShop.FrontStore.Modules.ProductCatalog.Abstractions.IProductService;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Services;

public class ProductService : IProductService
{
    private readonly RookieShopHttpClient _httpClient;

    public ProductService(RookieShopHttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Product> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/product-catalog/api/products/{sku}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        return await response.ReadFromJsonAsync<Product>(cancellationToken);
    }

    public async Task<Pagination<Product>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageNumber"] = $"{pageNumber}";
        queries["pageSize"] = $"{pageSize}";
        
        var queryString = queries.ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/product-catalog/api/products/all?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<Pagination<Product>>(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int maxCount, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["maxCount"] = $"{maxCount}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/product-catalog/api/products/featured?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<IEnumerable<Product>>(cancellationToken);
    }

    public async Task<Pagination<Product>> GetProductsByCategoryIdAsync(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var queries = HttpUtility.ParseQueryString(string.Empty);
        queries["pageSize"] = $"{pageSize}";
        queries["pageNumber"] = $"{pageNumber}";
        
        var queryString = queries.ToString();
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/product-catalog/api/products/by-category/{categoryId}?{queryString}");
        
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
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/product-catalog/api/products/semantic?{queryString}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<Pagination<Product>>(cancellationToken);
    }
}