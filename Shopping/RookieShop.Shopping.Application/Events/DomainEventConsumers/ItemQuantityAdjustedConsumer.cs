using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Contracts.Events;
using RookieShop.Shopping.Domain.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class ItemQuantityAdjustedConsumer : IMessageConsumer<ItemQuantityAdjusted>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public ItemQuantityAdjustedConsumer(IStockItemRepository stockItemRepository, IIntegrationEventPublisher integrationEventPublisher)
    {
        _stockItemRepository = stockItemRepository;
        _integrationEventPublisher = integrationEventPublisher;
    }
    
    public async Task ConsumeAsync(ItemQuantityAdjusted message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }

        if (message.NewQuantity < message.OldQuantity)
        {
            stockItem.ReleaseReservation(message.OldQuantity - message.NewQuantity);
        }
        else
        {
            stockItem.Reserve(message.NewQuantity - message.OldQuantity);
        }
        
        _stockItemRepository.Save(stockItem);
        
        _integrationEventPublisher.Enqueue(new StockLevelUpdated
        {
            Sku = stockItem.Sku,
            AvailableQuantity = stockItem.AvailableQuantity
        });
    }
}