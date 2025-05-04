using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class ScheduleExpireCartConsumer : IEventConsumer<CartExpirationDateExtended>, IConsumer<CartExpirationDateExtended>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;
    private readonly IClearCartScheduler _clearCartScheduler;

    public ScheduleExpireCartConsumer(IExternalMessageDispatcher externalMessageDispatcher, IClearCartScheduler clearCartScheduler)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
        _clearCartScheduler = clearCartScheduler;
    }
    
    public Task ConsumeAsync(CartExpirationDateExtended message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueuePublish(message);
        
        return Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<CartExpirationDateExtended> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;
        
        await _clearCartScheduler.ScheduleAsync(message.Id, message.ExpirationDate, cancellationToken);
    }
}