using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Domain.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class ItemQuantityAdjustedConsumer : IMessageConsumer<ItemQuantityAdjusted>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public ItemQuantityAdjustedConsumer(IStockItemRepository stockItemRepository, IDomainEventPublisher domainEventPublisher)
    {
        _stockItemRepository = stockItemRepository;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(ItemQuantityAdjusted message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }

        stockItem.ReleaseReservation(message.OldQuantity);
        stockItem.Reserve(message.NewQuantity);
        
        _stockItemRepository.Save(stockItem);
        
        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
    }
}