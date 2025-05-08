using MassTransit;
using RookieShop.Ordering.Contracts.Events;
using RookieShop.Shopping.Application.Commands.StockItems;

namespace RookieShop.Shopping.Application.Events.IntegrationEventConsumers;

public class OrderCancelledConsumer : IConsumer<OrderCancelled>
{
    public async Task Consume(ConsumeContext<OrderCancelled> context)
    {
        var message = context.Message;

        var cancellationToken = context.CancellationToken;

        foreach (var item in message.Items)
        {
            await context.Publish(new ReleaseStockReservation
            {
                Sku = item.Sku,
                Quantity = item.Quantity,
            }, cancellationToken);
        }
    }
}