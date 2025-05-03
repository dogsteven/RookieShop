using System.Diagnostics;
using MassTransit;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class MassTransitMessageDispatcher : IExternalMessageDispatcher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly MassTransitMessageDispatcherInstrumentation _instrumentation;

    private readonly List<object> _publishes = [];

    public MassTransitMessageDispatcher(IPublishEndpoint publishEndpoint, MassTransitMessageDispatcherInstrumentation instrumentation)
    {
        _publishEndpoint = publishEndpoint;
        _instrumentation = instrumentation;
    }

    public void EnqueuePublish(object message)
    {
        using var activity = _instrumentation.MassTransitMessageDispatcherActivitySource.StartActivity($"{message.GetType().Name} enqueue");
        
        _publishes.Add(message);
    }

    public async Task DispatchAsync(CancellationToken cancellationToken = default)
    {
        foreach (var message in _publishes)
        {
            using var activity = _instrumentation.MassTransitMessageDispatcherActivitySource.StartActivity($"{message.GetType().Name} publish");
            
            await _publishEndpoint.Publish(message, cancellationToken);
        }
        
        _publishes.Clear();
    }
}

public class MassTransitMessageDispatcherInstrumentation : IDisposable
{
    internal readonly ActivitySource MassTransitMessageDispatcherActivitySource;

    public MassTransitMessageDispatcherInstrumentation()
    {
        MassTransitMessageDispatcherActivitySource = new ActivitySource("Shopping.MassTransitMessageDispatcher");
    }

    public void Dispose()
    {
        MassTransitMessageDispatcherActivitySource.Dispose();
    }
}