using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnItemQuantityDecreasedConsumer : IEventConsumer<ItemQuantityDecreased>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public HandleStockReservationOnItemQuantityDecreasedConsumer(
        IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(ItemQuantityDecreased message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueuePublish(new ReleaseStockReservation
        {
            Sku = message.Sku,
            Quantity = message.QuantityDifference
        });
        
        return Task.CompletedTask;
    }
}