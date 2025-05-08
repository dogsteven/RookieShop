using MassTransit;
using RookieShop.Ordering.Domain.Orders.Events;

namespace RookieShop.Ordering.Application.Events.DomainEventConsumers;

public class PublishIntegrationEventOnOrderCancelledConsumer : IConsumer<OrderCancelled>
{
    public Task Consume(ConsumeContext<OrderCancelled> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        return context.Publish(new Contracts.Events.OrderCancelled
        {
            Id = message.Id,
            CustomerId = message.CustomerId,
            Items = message.Items.Select(item => new Contracts.Events.OrderCancelled.Item(item.Sku, item.Quantity))
        }, cancellationToken);
    }
}