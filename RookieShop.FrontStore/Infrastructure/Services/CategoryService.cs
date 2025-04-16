using RookieShop.FrontStore.Abstractions;
using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("RookieShop.WebApi");
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Category/{id}");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var categoryDto = await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(categoryDto);
        
        return categoryDto;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Category");
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        response.EnsureSuccessStatusCode();
        
        var categoryDtos = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(categoryDtos);
        
        return categoryDtos;
    }
}