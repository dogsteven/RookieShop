using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.Shopping.Contracts.Events;

namespace RookieShop.ProductCatalog.Application.Events.IntegrationEventConsumers;

public class UpdateStockLevelConsumer : IConsumer<StockLevelChanged>
{
    private readonly ProductCatalogDbContext _dbContext;

    public UpdateStockLevelConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<StockLevelChanged> context)
    {
        var message = context.Message;
        var sku = message.Sku;
        var changedQuantity = message.ChangedQuantity;
        
        var cancellationToken = context.CancellationToken;
        
        var productStockLevel = await _dbContext.ProductStockLevels
            .FirstOrDefaultAsync(productStockLevel => productStockLevel.ProductSku == sku, cancellationToken);

        if (productStockLevel == null)
        {
            return;
        }
        
        productStockLevel.SetAvailableQuantity(productStockLevel.AvailableQuantity + changedQuantity);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}