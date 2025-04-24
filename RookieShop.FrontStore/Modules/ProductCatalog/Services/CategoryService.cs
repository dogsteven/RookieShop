using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Exceptions;
using RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Services;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("RookieShop.WebApi");
    }

    public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Category/{id}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<Category>(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Category");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<IEnumerable<Category>>(cancellationToken);
    }
}