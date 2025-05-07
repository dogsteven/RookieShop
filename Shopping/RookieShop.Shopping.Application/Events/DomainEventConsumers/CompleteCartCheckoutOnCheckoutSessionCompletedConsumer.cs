using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands.Carts;
using RookieShop.Shopping.Domain.CheckoutSessions.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class CompleteCartCheckoutOnCheckoutSessionCompletedConsumer : IEventConsumer<CheckoutSessionCompleted>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public CompleteCartCheckoutOnCheckoutSessionCompletedConsumer(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(CheckoutSessionCompleted message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueuePublish(new CompleteCartCheckout
        {
            Id = message.Id
        });

        return Task.CompletedTask;
    }
}