using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Domain.StockItems.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class PublishIntegrationEventOnStockLevelChangedConsumer : IEventConsumer<StockLevelChanged>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public PublishIntegrationEventOnStockLevelChangedConsumer(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(StockLevelChanged message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueuePublish(new Contracts.Events.StockLevelChanged
        {
            Sku = message.Sku,
            ChangedQuantity = message.ChangedQuantity
        });
        
        return Task.CompletedTask;
    }
}