using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.StockItems;

public class ReleaseStockReservation
{
    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class ReleaseStockReservationConsumer : ICommandConsumer<ReleaseStockReservation>, IConsumer<ReleaseStockReservation>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public ReleaseStockReservationConsumer(IStockItemRepository stockItemRepository,
        DomainEventPublisher domainEventPublisher, IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _domainEventPublisher = domainEventPublisher;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ConsumeAsync(ReleaseStockReservation message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        stockItem.ReleaseReservation(message.Quantity);
        
        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
    }

    public async Task Consume(ConsumeContext<ReleaseStockReservation> context)
    {
        await ConsumeAsync(context.Message, context.CancellationToken);
        
        await _unitOfWork.CommitAsync(context.CancellationToken);
    }
}