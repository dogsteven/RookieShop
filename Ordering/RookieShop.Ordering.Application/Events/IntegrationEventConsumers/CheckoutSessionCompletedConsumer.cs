using MassTransit;
using RookieShop.Ordering.Application.Commands;
using RookieShop.Ordering.Domain.Orders;
using RookieShop.Shopping.Contracts.Events;

namespace RookieShop.Ordering.Application.Events.IntegrationEventConsumers;

public class CheckoutSessionCompletedConsumer : IConsumer<CheckoutSessionCompleted>
{
    public Task Consume(ConsumeContext<CheckoutSessionCompleted> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        return context.Publish(new PlaceOrder
        {
            Id = message.SessionId,
            CustomerId = message.Id,
            BillingAddress = message.BillingAddress,
            ShippingAddress = message.ShippingAddress,
            Items = message.Items.Select(item => new OrderItem(item.Sku, item.Name, item.Price, item.Quantity))
        }, cancellationToken);
    }
}