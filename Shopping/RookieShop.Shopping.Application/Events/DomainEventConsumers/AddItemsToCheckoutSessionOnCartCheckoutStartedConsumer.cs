using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands.CheckoutSessions;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class AddItemsToCheckoutSessionOnCartCheckoutStartedConsumer : IEventConsumer<CartCheckoutStarted>
{
    private readonly IMessageDispatcher _messageDispatcher;

    public AddItemsToCheckoutSessionOnCartCheckoutStartedConsumer(IMessageDispatcher messageDispatcher)
    {
        _messageDispatcher = messageDispatcher;
    }
    
    public Task ConsumeAsync(CartCheckoutStarted message, CancellationToken cancellationToken = default)
    {
        return _messageDispatcher.SendAsync(new AddItemsToCheckoutSession
        {
            Id = message.Id,
            Items = message.Items
        }, cancellationToken);
    }
}