using MassTransit;
using Microsoft.Extensions.Logging;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class ExternalMessageDispatcher : IExternalMessageDispatcher
{
    private readonly IMessageScheduler _messageScheduler;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IBusTopology _busTopology;
    private readonly ILogger<ExternalMessageDispatcher> _logger;

    private readonly List<(object, DateTimeOffset)> _scheduleSends = [];
    private readonly List<object> _publishes = [];

    public ExternalMessageDispatcher(IMessageScheduler messageScheduler, IPublishEndpoint publishEndpoint,
        IBus bus, ILogger<ExternalMessageDispatcher> logger)
    {
        _messageScheduler = messageScheduler;
        _publishEndpoint = publishEndpoint;
        _busTopology = bus.Topology;
        _logger = logger;
    }

    public void EnqueueScheduleSend(object message, DateTimeOffset scheduledTime)
    {
        _scheduleSends.Add((message, scheduledTime));
    }

    public void EnqueuePublish(object message)
    {
        _publishes.Add(message);
    }

    public async Task DispatchAsync(CancellationToken cancellationToken = default)
    {
        foreach (var (message, scheduledTime) in _scheduleSends)
        {
            if (!_busTopology.TryGetPublishAddress(message.GetType(), out var destinationAddress))
            {
                continue;
            }
            
            await _messageScheduler.ScheduleSend(destinationAddress, scheduledTime.UtcDateTime, message, cancellationToken);
        }
        
        foreach (var message in _publishes)
        {
            await _publishEndpoint.Publish(message, cancellationToken);
        }
        
        _publishes.Clear();
    }
}