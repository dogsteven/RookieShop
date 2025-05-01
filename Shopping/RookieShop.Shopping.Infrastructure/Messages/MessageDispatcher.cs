using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Shared.Domain;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class MessageDispatcher : IDomainEventPublisher
{
    private readonly IServiceProvider _provider;
    private readonly ConsumeMethodRegistry _consumeMethodRegistry;
    
    public MessageDispatcher(IServiceProvider provider)
    {
        _provider = provider;
        _consumeMethodRegistry = _provider.GetRequiredService<ConsumeMethodRegistry>();
    }

    public Task SendAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var consumerType = typeof(ICommandConsumer<>).MakeGenericType(messageType);
        
        var consumer = _provider.GetRequiredService(consumerType);

        var consumeAsync = _consumeMethodRegistry.GetConsumeMethod(consumerType);
        
        return (Task)consumeAsync.Invoke(consumer, [message, cancellationToken])!;
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var consumerType = typeof(IEventConsumer<>).MakeGenericType(messageType);

        var consumers = _provider.GetServices(consumerType);

        foreach (var consumer in consumers)
        {
            var consumeAsync = _consumeMethodRegistry.GetConsumeMethod(consumerType);
            
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

    public class ConsumeMethodRegistry
    {
        private readonly Dictionary<Type, MethodInfo> _consumeMethods;

        public ConsumeMethodRegistry()
        {
            _consumeMethods = [];
        }

        public MethodInfo GetConsumeMethod(Type consumerType)
        {
            return _consumeMethods[consumerType];
        }

        public void Add(Type consumerType)
        {
            var consumeMethods = consumerType.GetInterfaces()
                .FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))!
                .GetMethod("ConsumeAsync")!;
            
            _consumeMethods[consumerType] = consumeMethods;
        }

        public void AddCommandConsumer<TMessage>()
        {
            Add(typeof(ICommandConsumer<>).MakeGenericType(typeof(TMessage)));
        }

        public void AddEventConsumer<TMessage>()
        {
            Add(typeof(IEventConsumer<>).MakeGenericType(typeof(TMessage)));
        }
    }
}