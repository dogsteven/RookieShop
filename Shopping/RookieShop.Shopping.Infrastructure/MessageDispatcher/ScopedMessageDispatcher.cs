using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Shared.Domain;
using RookieShop.Shopping.Application.Abstractions;

namespace RookieShop.Shopping.Infrastructure.MessageDispatcher;

public class ScopedMessageDispatcher : IDomainEventPublisher
{
    private readonly IServiceProvider _provider;
    
    public ScopedMessageDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task DispatchAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var consumerType = typeof(IMessageConsumer<>).MakeGenericType(messageType);
        
        dynamic consumer = _provider.GetRequiredService(consumerType);
        
        await consumer.ConsumeAsync(message, cancellationToken);
    }

    public async Task PublishAsync(DomainEventSource source, CancellationToken cancellationToken = default)
    {
        var domainEvents = source.DomainEvents.ToList();
        
        source.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }
    }
}