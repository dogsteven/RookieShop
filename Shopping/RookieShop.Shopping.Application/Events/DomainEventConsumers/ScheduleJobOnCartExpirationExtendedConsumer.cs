using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class ScheduleJobOnCartExpirationExtendedConsumer : IEventConsumer<CartExpirationDateExtended>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public ScheduleJobOnCartExpirationExtendedConsumer(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }
    
    public Task ConsumeAsync(CartExpirationDateExtended message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueueScheduleSend(new TryClearCart
        {
            Id = message.Id
        }, message.ExpirationDate);
        
        return Task.CompletedTask;
    }
}