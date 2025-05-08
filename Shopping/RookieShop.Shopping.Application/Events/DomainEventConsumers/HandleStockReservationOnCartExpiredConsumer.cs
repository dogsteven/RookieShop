using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Commands.StockItems;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnCartExpiredConsumer : IEventConsumer<CartExpired>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public HandleStockReservationOnCartExpiredConsumer(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(CartExpired message, CancellationToken cancellationToken = default)
    {
        foreach (var item in message.Items)
        {
            _externalMessageDispatcher.EnqueuePublish(new ReleaseStockReservation
            {
                Sku = item.Sku,
                Quantity = item.Quantity
            });
        }

        return Task.CompletedTask;
    }
}