using RookieShop.ProductCatalog.ViewModels;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;

public interface ICategoryService
{
    public Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
    public Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
}