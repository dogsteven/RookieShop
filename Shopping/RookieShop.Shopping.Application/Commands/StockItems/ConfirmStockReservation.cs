using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;

namespace RookieShop.Shopping.Application.Commands.StockItems;

public class ConfirmStockReservation
{
    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class ConfirmStockReservationConsumer : ICommandConsumer<ConfirmStockReservation>, IConsumer<ConfirmStockReservation>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmStockReservationConsumer(IStockItemRepository stockItemRepository, IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
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

    public async Task Consume(ConsumeContext<ConfirmStockReservation> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        await ConsumeAsync(message, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
} 