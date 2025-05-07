using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands.Carts;
using RookieShop.Shopping.Domain.CheckoutSessions.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class FailCartCheckoutOnCheckoutSessionExpiredConsumer : IEventConsumer<CheckoutSessionExpired>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public FailCartCheckoutOnCheckoutSessionExpiredConsumer(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(CheckoutSessionExpired message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueuePublish(new FailCartCheckout
        {
            Id = message.Id,
        });
        
        return Task.CompletedTask;
    }
}