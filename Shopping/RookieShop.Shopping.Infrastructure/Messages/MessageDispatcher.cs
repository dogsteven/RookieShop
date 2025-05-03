using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class MessageDispatcher : IMessageDispatcher
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

        var consumeMethod = _consumeMethodRegistry.GetConsumeMethod(messageType);
        
        return (Task)consumeMethod.DynamicInvoke(consumer, message, cancellationToken)!;
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var consumerType = typeof(IEventConsumer<>).MakeGenericType(messageType);

        var consumers = _provider.GetServices(consumerType);

        foreach (var consumer in consumers)
        {
            var consumeMethod = _consumeMethodRegistry.GetConsumeMethod(messageType);
            
            await (Task)consumeMethod.DynamicInvoke(consumer, message, cancellationToken)!;
        }
    }

    public class ConsumeMethodRegistry
    {
        private readonly Dictionary<Type, Delegate> _consumeDelegates;

        public ConsumeMethodRegistry()
        {
            _consumeDelegates = [];
        }

        public Delegate GetConsumeMethod(Type consumerType)
        {
            return _consumeDelegates[consumerType];
        }

        public void Add(Type messageType)
        {
            var consumerType = typeof(IMessageConsumer<>).MakeGenericType(messageType);

            var consumerMethod = consumerType.GetMethod("ConsumeAsync", BindingFlags.Public | BindingFlags.Instance)!;
            
            var consumeDelegate = Delegate.CreateDelegate(typeof(Func<,,,>).MakeGenericType(consumerType, messageType, typeof(CancellationToken), typeof(Task)), consumerMethod);
            
            _consumeDelegates[messageType] = consumeDelegate;
        }

        public void Add<TMessage>()
        {
            Add(typeof(TMessage));
        }
    }
}