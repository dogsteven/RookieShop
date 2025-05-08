using MassTransit;
using RookieShop.Ordering.Domain.Orders.Events;

namespace RookieShop.Ordering.Application.Events.DomainEventConsumers;

public class PublishIntegrationEventOnOrderCompletedConsumer : IConsumer<OrderCompleted>
{
    public Task Consume(ConsumeContext<OrderCompleted> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        return context.Publish(new Contracts.Events.OrderCompleted
        {
            Id = message.Id,
            CustomerId = message.CustomerId,
            Items = message.Items.Select(item => new Contracts.Events.OrderCompleted.Item(item.Sku, item.Quantity))
        }, cancellationToken);
    }
}