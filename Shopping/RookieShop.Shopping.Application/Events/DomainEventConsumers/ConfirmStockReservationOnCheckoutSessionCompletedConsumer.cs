using MassTransit;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands.StockItems;
using RookieShop.Shopping.Domain.CheckoutSessions.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class ConfirmStockReservationOnCheckoutSessionCompletedConsumer : IEventConsumer<CheckoutSessionCompleted>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public ConfirmStockReservationOnCheckoutSessionCompletedConsumer(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }

    public Task ConsumeAsync(CheckoutSessionCompleted message, CancellationToken cancellationToken = default)
    {
        foreach (var item in message.Items)
        {
            _externalMessageDispatcher.EnqueuePublish(new ConfirmStockReservation
            {
                Sku = item.Sku,
                Quantity = item.Quantity,
            });
        }

        return Task.CompletedTask;
    }
}