using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RookieShop.Shared.Domain;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class MessageDispatcher : IDomainEventPublisher
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<MessageDispatcher> _logger;
    
    public MessageDispatcher(IServiceProvider provider, ILogger<MessageDispatcher> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task DispatchAsync(object message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching message {MessageType}", message.GetType());
        
        var messageType = message.GetType();
        var consumerType = typeof(IMessageConsumer<>).MakeGenericType(messageType);
        
        var consumer = _provider.GetRequiredService(consumerType);

        var consumeAsync = consumerType.GetMethod("ConsumeAsync")!;
        
        await (Task)consumeAsync.Invoke(consumer, [message, cancellationToken])!;
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