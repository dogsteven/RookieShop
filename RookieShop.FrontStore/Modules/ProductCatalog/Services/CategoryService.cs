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
        
        response.EnsureSuccessStatusCode();
        
        var category = await response.Content.ReadFromJsonAsync<Category>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(category);
        
        return category;
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Category");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var categories = await response.Content.ReadFromJsonAsync<IEnumerable<Category>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(categories);
        
        return categories;
    }
}