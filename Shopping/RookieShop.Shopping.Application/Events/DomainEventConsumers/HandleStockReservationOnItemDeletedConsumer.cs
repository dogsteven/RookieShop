using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Domain.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnItemDeletedConsumer : IEventConsumer<ItemRemovedFromCart>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public HandleStockReservationOnItemDeletedConsumer(IStockItemRepository stockItemRepository, IDomainEventPublisher domainEventPublisher)
    {
        _stockItemRepository = stockItemRepository;
        _domainEventPublisher = domainEventPublisher;
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

        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
    }
}