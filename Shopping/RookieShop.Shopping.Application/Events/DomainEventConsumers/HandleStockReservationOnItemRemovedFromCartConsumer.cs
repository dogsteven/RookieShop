using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnItemRemovedFromCartConsumer : IEventConsumer<ItemRemovedFromCart>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public HandleStockReservationOnItemRemovedFromCartConsumer(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(ItemRemovedFromCart message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueuePublish(new ReleaseStockReservation
        {
            Sku = message.Sku,
            Quantity = message.Quantity
        });
        
        return Task.CompletedTask;
    }
}