using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Domain.CheckoutSessions.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class PublishIntegrationEventOnCheckoutSessionCompletedConsumer : IEventConsumer<CheckoutSessionCompleted>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public PublishIntegrationEventOnCheckoutSessionCompletedConsumer(
        IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(CheckoutSessionCompleted message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueuePublish(new Contracts.Events.CheckoutSessionCompleted
        {
            Id = message.Id,
            SessionId = message.SessionId,
            BillingAddress = message.BillingAddress,
            ShippingAddress = message.ShippingAddress,
            Items = message.Items.Select(item => new Contracts.Events.CheckoutSessionCompleted.CheckoutItem(item.Sku, item.Name, item.Price, item.Quantity))
        });
        
        return Task.CompletedTask;
    }
}