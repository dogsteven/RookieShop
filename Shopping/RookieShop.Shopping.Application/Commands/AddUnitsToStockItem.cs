using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;

namespace RookieShop.Shopping.Application.Commands;

public class AddUnitsToStockItem
{
    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class AddUnitsToStockItemConsumer : ICommandConsumer<AddUnitsToStockItem>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public AddUnitsToStockItemConsumer(IStockItemRepository stockItemRepository, IDomainEventPublisher domainEventPublisher)
    {
        _stockItemRepository = stockItemRepository;
        _domainEventPublisher = domainEventPublisher;
    }
        
    public async Task ConsumeAsync(AddUnitsToStockItem message, CancellationToken cancellationToken = default)
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