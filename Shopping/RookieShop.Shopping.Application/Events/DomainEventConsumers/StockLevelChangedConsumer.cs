using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Domain.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class StockLevelChangedConsumer : IMessageConsumer<StockLevelChanged>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public StockLevelChangedConsumer(IIntegrationEventPublisher integrationEventPublisher)
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