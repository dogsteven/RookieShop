using MassTransit;
using RookieShop.Ordering.Domain.Orders.Events;

namespace RookieShop.Ordering.Application.Events.DomainEventConsumers;

public class PublishIntegrationEventOnOrderPlacedConsumer : IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        return context.Publish(new Contracts.Events.OrderPlaced
        {
            Id = message.Id,
            CustomerId = message.CustomerId
        }, cancellationToken);
    }
}