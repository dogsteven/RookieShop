using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class UnscheduleExpireCartOnCartCheckoutStartedConsumer : IEventConsumer<CartCheckoutStarted>
{
    private readonly IExpireCartScheduler _expireCartScheduler;

    public UnscheduleExpireCartOnCartCheckoutStartedConsumer(IExpireCartScheduler expireCartScheduler)
    {
        _expireCartScheduler = expireCartScheduler;
    }
    
    public Task ConsumeAsync(CartCheckoutStarted message, CancellationToken cancellationToken = default)
    {
        _expireCartScheduler.EnqueueUnschedule(message.Id);
        
        return Task.CompletedTask;
    }
}