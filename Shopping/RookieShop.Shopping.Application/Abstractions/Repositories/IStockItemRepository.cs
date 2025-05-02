using RookieShop.Shopping.Domain;
using RookieShop.Shopping.Domain.StockItems;

namespace RookieShop.Shopping.Application.Abstractions.Repositories;

public interface IStockItemRepository
{
    public Task<StockItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    public void Save(StockItem stockItem);
    public void Remove(StockItem stockItem);
}