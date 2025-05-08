using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Domain.CheckoutSessions.Events;

namespace RookieShop.Shopping.Application.Events.DomainEventConsumers;

public class UnscheduleExpireCheckoutSessionOnCheckoutSessionCompletedConsumer : IEventConsumer<CheckoutSessionCompleted>
{
    private readonly IExpireCheckoutSessionScheduler _expireCheckoutSessionScheduler;

    public UnscheduleExpireCheckoutSessionOnCheckoutSessionCompletedConsumer(
        IExpireCheckoutSessionScheduler expireCheckoutSessionScheduler)
    {
        _expireCheckoutSessionScheduler = expireCheckoutSessionScheduler;
    }
    
    public Task ConsumeAsync(CheckoutSessionCompleted message, CancellationToken cancellationToken = default)
    {
        _expireCheckoutSessionScheduler.EnqueueUnschedule(message.Id);
        
        return Task.CompletedTask;
    }
}