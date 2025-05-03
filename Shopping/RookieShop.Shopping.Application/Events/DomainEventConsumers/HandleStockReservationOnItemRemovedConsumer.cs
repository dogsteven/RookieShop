using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnItemRemovedConsumer : IEventConsumer<ItemRemovedFromCart>, IConsumer<ItemRemovedFromCart>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public HandleStockReservationOnItemRemovedConsumer(IStockItemRepository stockItemRepository, DomainEventPublisher domainEventPublisher, IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _domainEventPublisher = domainEventPublisher;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ConsumeAsync(ItemRemovedFromCart message, CancellationToken cancellationToken = default)
    {
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        stockItem.ReleaseReservation(message.Quantity);

        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
    }

    public async Task Consume(ConsumeContext<ItemRemovedFromCart> context)
    {
        await ConsumeAsync(context.Message, context.CancellationToken);
        
        await _unitOfWork.CommitAsync(context.CancellationToken);
    }
}