using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Domain.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class PublishIntegrationEventOnStockLevelChangedConsumer : IEventConsumer<StockLevelChanged>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public PublishIntegrationEventOnStockLevelChangedConsumer(IIntegrationEventPublisher integrationEventPublisher)
    {
        _integrationEventPublisher = integrationEventPublisher;
    }
    
    public Task ConsumeAsync(StockLevelChanged message, CancellationToken cancellationToken = default)
    {
        _integrationEventPublisher.Enqueue(new Contracts.Events.StockLevelChanged
        {
            Sku = message.Sku,
            ChangedQuantity = message.ChangedQuantity
        });
        
        return Task.CompletedTask;
    }
}