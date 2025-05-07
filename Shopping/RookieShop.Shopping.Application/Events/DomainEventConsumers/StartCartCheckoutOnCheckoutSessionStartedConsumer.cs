using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands.Carts;
using RookieShop.Shopping.Domain.CheckoutSessions.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class StartCartCheckoutOnCheckoutSessionStartedConsumer : IEventConsumer<CheckoutSessionStarted>
{
    private readonly IMessageDispatcher _messageDispatcher;

    public StartCartCheckoutOnCheckoutSessionStartedConsumer(IMessageDispatcher messageDispatcher)
    {
        _messageDispatcher = messageDispatcher;
    }
    
    public Task ConsumeAsync(CheckoutSessionStarted message, CancellationToken cancellationToken = default)
    {
        return _messageDispatcher.SendAsync(new StartCartCheckout
        {
            Id = message.Id,
        }, cancellationToken);
    }
}