using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Abstractions;

public interface ICategoryService
{
    public Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
    public Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
}