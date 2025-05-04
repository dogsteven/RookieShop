using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class ReserveStock
{
    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class ReserveStockConsumer : ICommandConsumer<ReserveStock>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly DomainEventPublisher _domainEventPublisher;

    public ReserveStockConsumer(IStockItemRepository stockItemRepository, DomainEventPublisher domainEventPublisher)
    {
        _stockItemRepository = stockItemRepository;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(ReserveStock message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        stockItem.Reserve(message.Quantity);
        
        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
    }
}