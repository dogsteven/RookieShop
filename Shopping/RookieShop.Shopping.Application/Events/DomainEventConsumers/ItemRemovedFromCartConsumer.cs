using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Contracts.Events;
using RookieShop.Shopping.Domain.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class ItemRemovedFromCartConsumer : IMessageConsumer<ItemRemovedFromCart>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public ItemRemovedFromCartConsumer(IStockItemRepository stockItemRepository, IIntegrationEventPublisher integrationEventPublisher)
    {
        _stockItemRepository = stockItemRepository;
        _integrationEventPublisher = integrationEventPublisher;
    }
    
    public async Task ConsumeAsync(ItemRemovedFromCart message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        stockItem.ReleaseReservation(message.Quantity);
        
        _stockItemRepository.Save(stockItem);
        
        _integrationEventPublisher.Enqueue(new StockLevelUpdated
        {
            Sku = stockItem.Sku,
            AvailableQuantity = stockItem.AvailableQuantity
        });
    }
}