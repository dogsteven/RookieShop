using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class IncreaseStock
{
    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class IncreaseStockConsumer : ICommandConsumer<IncreaseStock>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly DomainEventPublisher _domainEventPublisher;

    public IncreaseStockConsumer(IStockItemRepository stockItemRepository, DomainEventPublisher domainEventPublisher)
    {
        _stockItemRepository = stockItemRepository;
        _domainEventPublisher = domainEventPublisher;
    }
        
    public async Task ConsumeAsync(IncreaseStock message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        stockItem.AddUnits(message.Quantity);
        
        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
    }
}