using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;

namespace RookieShop.Shopping.Application.Commands;

public class ConfirmStockReservation
{
    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class ConfirmStockReservationConsumer : ICommandConsumer<ConfirmStockReservation>
{
    private readonly IStockItemRepository _stockItemRepository;

    public ConfirmStockReservationConsumer(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }
    
    public async Task ConsumeAsync(ConfirmStockReservation message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        stockItem.ConfirmReservation(message.Quantity);
    }
} 