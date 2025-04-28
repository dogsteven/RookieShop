using RookieShop.FrontStore.Modules.ProductCatalog.Models;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;

public interface IProductService
{
    public Task<Product> GetProductBySkuAsync(string sku, CancellationToken cancellationToken);
    public Task<Pagination<Product>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<IEnumerable<Product>> GetFeaturedProductsAsync(int maxCount, CancellationToken cancellationToken);
    public Task<Pagination<Product>> GetProductsByCategoryIdAsync(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<Pagination<Product>> GetProductsSemanticAsync(string semantic, int pageNumber, int pageSize, CancellationToken cancellationToken);
}