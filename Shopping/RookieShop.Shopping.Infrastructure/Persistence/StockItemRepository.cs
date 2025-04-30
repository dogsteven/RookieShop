using Microsoft.EntityFrameworkCore;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Domain;

namespace RookieShop.Shopping.Infrastructure.Persistence;

public class StockItemRepository : IStockItemRepository
{
    private readonly ShoppingDbContext _context;

    public StockItemRepository(ShoppingDbContext context)
    {
        _context = context;
    }
    
    public Task<StockItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return _context.StockItems.FirstOrDefaultAsync(stockItem => stockItem.Sku == sku, cancellationToken); 
    }

    public void Save(StockItem stockItem)
    {
        var entry = _context.Entry(stockItem);

        if (entry.State == EntityState.Detached)
        {
            _context.StockItems.Add(stockItem);
        }
        else if (entry.State == EntityState.Unchanged)
        {
            _context.StockItems.Update(stockItem);
        }
    }

    public void Remove(StockItem stockItem)
    {
        _context.StockItems.Remove(stockItem);
    }
}