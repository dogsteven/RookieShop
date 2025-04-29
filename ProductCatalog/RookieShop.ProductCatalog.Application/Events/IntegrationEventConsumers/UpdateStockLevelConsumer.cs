using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.Shopping.Contracts.Events;

namespace RookieShop.ProductCatalog.Application.Events.IntegrationEventConsumers;

public class UpdateStockLevelConsumer : IConsumer<StockLevelUpdated>
{
    private readonly ProductCatalogDbContext _dbContext;

    public UpdateStockLevelConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<StockLevelUpdated> context)
    {
        var message = context.Message;
        var sku = message.Sku;
        var availableQuantity = message.AvailableQuantity;
        
        var cancellationToken = context.CancellationToken;
        
        var productStockLevel = await _dbContext.ProductStockLevels
            .FirstOrDefaultAsync(productStockLevel => productStockLevel.ProductSku == sku, cancellationToken);

        if (productStockLevel == null)
        {
            return;
        }
        
        productStockLevel.SetAvailableQuantity(availableQuantity);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}