using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Exceptions;
using RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.FrontStore.Modules.Shared;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Services;

public class CategoryService : ICategoryService
{
    private readonly RookieShopHttpClient _httpClient;

    public CategoryService(RookieShopHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/product-catalog/api/categories/{id}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<Category>(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/product-catalog/api/categories");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<IEnumerable<Category>>(cancellationToken);
    }
}