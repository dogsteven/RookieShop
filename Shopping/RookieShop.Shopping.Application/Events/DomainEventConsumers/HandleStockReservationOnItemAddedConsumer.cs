using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnItemAddedConsumer : IEventConsumer<ItemAddedToCart>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly DomainEventPublisher _domainEventPublisher;

    public HandleStockReservationOnItemAddedConsumer(IStockItemRepository stockItemRepository, DomainEventPublisher domainEventPublisher)
    {
        _stockItemRepository = stockItemRepository;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(ItemAddedToCart message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        stockItem.Reserve(message.Quantity);
        
        _stockItemRepository.Save(stockItem);
        
        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
    }
}