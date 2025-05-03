using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;

public interface IProductService
{
    public Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken cancellationToken);
    public Task<Pagination<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int maxCount, CancellationToken cancellationToken);
    public Task<Pagination<ProductDto>> GetProductsByCategoryIdAsync(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<Pagination<ProductDto>> GetProductsSemanticAsync(string semantic, int pageNumber, int pageSize, CancellationToken cancellationToken);
}