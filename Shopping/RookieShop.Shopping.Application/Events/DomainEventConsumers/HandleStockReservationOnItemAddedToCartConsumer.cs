using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class HandleStockReservationOnItemAddedToCartConsumer : IEventConsumer<ItemAddedToCart>
{
    private readonly IMessageDispatcher _messageDispatcher;

    public HandleStockReservationOnItemAddedToCartConsumer(IMessageDispatcher messageDispatcher)
    {
        _messageDispatcher = messageDispatcher;
    }
    
    public Task ConsumeAsync(ItemAddedToCart message, CancellationToken cancellationToken = default)
    {
        return _messageDispatcher.SendAsync(new ReserveStock
        {
            Sku = message.Sku,
            Quantity = message.Quantity
        }, cancellationToken);
    }
}