using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RookieShop.Shared.Domain;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class MessageDispatcher : IDomainEventPublisher
{
    private readonly IServiceProvider _provider;
    
    public MessageDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task SendAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var consumerType = typeof(ICommandConsumer<>).MakeGenericType(messageType);
        
        var consumer = _provider.GetRequiredService(consumerType);
        
        var consumeAsync = consumerType.GetMethod("ConsumeAsync")!;
        
        return (Task)consumeAsync.Invoke(consumer, [message, cancellationToken])!;
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var consumerType = typeof(IEventConsumer<>).MakeGenericType(messageType);

        var consumers = _provider.GetServices(consumerType);

        foreach (var consumer in consumers)
        {
            var consumeAsync = consumerType.GetMethod("ConsumeAsync")!;
            
            await (Task)consumeAsync.Invoke(consumer, [message, cancellationToken])!;
        }
        
    }

    public async Task PublishAsync(DomainEventSource source, CancellationToken cancellationToken = default)
    {
        var domainEvents = source.DomainEvents.ToList();
        
        source.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            await PublishAsync(domainEvent, cancellationToken);
        }
    }
}