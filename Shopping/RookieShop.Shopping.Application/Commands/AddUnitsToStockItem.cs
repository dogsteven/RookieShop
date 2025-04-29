using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Exceptions;

namespace RookieShop.Shopping.Application.Commands;

public class AddUnitsToStockItem
{
    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class AddUnitsToStockItemConsumer : IMessageConsumer<AddUnitsToStockItem>
{
    private readonly IStockItemRepository _stockItemRepository;

    public AddUnitsToStockItemConsumer(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }
        
    public async Task ConsumeAsync(AddUnitsToStockItem message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        stockItem.AddUnits(message.Quantity);
    }
}