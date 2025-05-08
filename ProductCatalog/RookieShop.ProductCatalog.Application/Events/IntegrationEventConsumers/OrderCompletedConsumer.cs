using MassTransit;
using RookieShop.Ordering.Contracts.Events;
using RookieShop.ProductCatalog.Application.Commands;

namespace RookieShop.ProductCatalog.Application.Events.IntegrationEventConsumers;

public class OrderCompletedConsumer : IConsumer<OrderCompleted>
{
    public async Task Consume(ConsumeContext<OrderCompleted> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        foreach (var item in message.Items)
        {
            await context.Publish(new CreatePurchase
            {
                CustomerId = message.CustomerId,
                ProductSku = item.Sku
            }, cancellationToken);
        }
    }
}