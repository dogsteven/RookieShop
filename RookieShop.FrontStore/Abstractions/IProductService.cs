using RookieShop.FrontStore.Models.Shared.Application;

namespace RookieShop.FrontStore.Abstractions;

public interface IProductService
{
    public Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken cancellationToken);
    public Task<Pagination<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    public Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int maxCount, CancellationToken cancellationToken);
    public Task<Pagination<ProductDto>> GetProductsByCategoryIdAsync(int categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}