using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnItemQuantityAdjustedConsumer : IEventConsumer<ItemQuantityAdjusted>
{
    private readonly IMessageDispatcher _messageDispatcher;
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public HandleStockReservationOnItemQuantityAdjustedConsumer(IMessageDispatcher messageDispatcher,
        IExternalMessageDispatcher externalMessageDispatcher)
    {
        _messageDispatcher = messageDispatcher;
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(ItemQuantityAdjusted message, CancellationToken cancellationToken = default)
    {
        if (message.NewQuantity < message.OldQuantity)
        {
            _externalMessageDispatcher.EnqueuePublish(new ReleaseStockReservation
            {
                Sku = message.Sku,
                Quantity = message.OldQuantity - message.NewQuantity
            });
            
            return Task.CompletedTask;
        }
        else
        {
            return _messageDispatcher.SendAsync(new ReserveStock
            {
                Sku = message.Sku,
                Quantity = message.NewQuantity - message.OldQuantity
            }, cancellationToken);
        }
    }
}