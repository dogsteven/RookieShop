using MassTransit;
using RookieShop.Ordering.Contracts.Events;
using RookieShop.Shopping.Application.Commands.StockItems;

namespace RookieShop.Shopping.Application.Events.IntegrationEventConsumers;

public class OrderCompletedConsumer : IConsumer<OrderCompleted>
{
    public async Task Consume(ConsumeContext<OrderCompleted> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        foreach (var item in message.Items)
        {
            await context.Publish(new ConfirmStockReservation
            {
                Sku = item.Sku,
                Quantity = item.Quantity,
            }, cancellationToken);
        }
    }
}