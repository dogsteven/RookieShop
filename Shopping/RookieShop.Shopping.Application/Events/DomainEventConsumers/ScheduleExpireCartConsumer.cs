using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class ScheduleExpireCartConsumer : IEventConsumer<CartExpirationTimeExtended>, IConsumer<CartExpirationTimeExtended>
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;
    private readonly IExpireCartScheduler _expireCartScheduler;

    public ScheduleExpireCartConsumer(IExternalMessageDispatcher externalMessageDispatcher, IExpireCartScheduler expireCartScheduler)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
        _expireCartScheduler = expireCartScheduler;
    }
    
    public Task ConsumeAsync(CartExpirationTimeExtended message, CancellationToken cancellationToken = default)
    {
        _externalMessageDispatcher.EnqueuePublish(message);
        
        return Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<CartExpirationTimeExtended> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;
        
        await _expireCartScheduler.ScheduleAsync(message.Id, message.ExpirationDate, cancellationToken);
    }
}