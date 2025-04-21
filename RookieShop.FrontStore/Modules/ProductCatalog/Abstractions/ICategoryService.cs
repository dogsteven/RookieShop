using RookieShop.FrontStore.Modules.ProductCatalog.Models;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;

public interface ICategoryService
{
    public Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
    public Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
}