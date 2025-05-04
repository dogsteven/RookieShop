using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnItemQuantityIncreasedConsumer : IEventConsumer<ItemQuantityIncreased>
{
    private readonly IMessageDispatcher _messageDispatcher;

    public HandleStockReservationOnItemQuantityIncreasedConsumer(IMessageDispatcher messageDispatcher)
    {
        _messageDispatcher = messageDispatcher;
    }
    
    public Task ConsumeAsync(ItemQuantityIncreased message, CancellationToken cancellationToken = default)
    {
        return _messageDispatcher.SendAsync(new ReserveStock
        {
            Sku = message.Sku,
            Quantity = message.QuantityDifference
        }, cancellationToken);
    }
}