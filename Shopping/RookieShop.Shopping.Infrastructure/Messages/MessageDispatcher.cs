using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class MessageDispatcher : IMessageDispatcher
{
    private readonly IServiceProvider _provider;
    private readonly ConsumeMethodRegistry _consumeMethodRegistry;
    private readonly MessageDispatcherInstrumentation _instrumentation;
    private readonly ILogger<MessageDispatcher> _logger;
    
    public MessageDispatcher(IServiceProvider provider, ConsumeMethodRegistry consumeMethodRegistry, MessageDispatcherInstrumentation instrumentation, ILogger<MessageDispatcher> logger)
    {
        _provider = provider;
        _consumeMethodRegistry = consumeMethodRegistry;
        _instrumentation = instrumentation;
        _logger = logger;
    }

    private async Task ConsumeMessageAsync(string operation, object consumer, object message, Delegate consumeMethod,
        CancellationToken cancellationToken = default)
    {
        using var activity = _instrumentation.MessageDispatcherActivitySource.StartActivity($"{consumer.GetType().Name} {operation}");

        await (Task)consumeMethod.DynamicInvoke(consumer, message, cancellationToken)!;
    }

    public async Task SendAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var consumerType = typeof(ICommandConsumer<>).MakeGenericType(messageType);
        
        var consumer = _provider.GetRequiredService(consumerType);

        var consumeMethod = _consumeMethodRegistry.GetConsumeMethod(messageType);

        await ConsumeMessageAsync("send", consumer, message, consumeMethod, cancellationToken);
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType();
        var consumerType = typeof(IEventConsumer<>).MakeGenericType(messageType);

        var consumers = _provider.GetServices(consumerType);

        foreach (var consumer in consumers)
        {
            var consumeMethod = _consumeMethodRegistry.GetConsumeMethod(messageType);
            
            await ConsumeMessageAsync("publish", consumer!, message, consumeMethod, cancellationToken);
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

public class MessageDispatcherInstrumentation : IDisposable
{
    internal readonly ActivitySource MessageDispatcherActivitySource;

    public MessageDispatcherInstrumentation()
    {
        MessageDispatcherActivitySource = new ActivitySource("Shopping.MessageDispatcher");
    }

    public void Dispose()
    {
        MessageDispatcherActivitySource.Dispose();
    }
}