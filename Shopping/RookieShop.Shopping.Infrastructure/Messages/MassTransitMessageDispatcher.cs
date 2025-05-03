using MassTransit;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class MassTransitMessageDispatcher : IExternalMessageDispatcher
{
    private readonly IPublishEndpoint _publishEndpoint;

    private readonly List<object> _publishes = [];

    public MassTransitMessageDispatcher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public void EnqueuePublish(object message)
    {
        _publishes.Add(message);
    }

    public async Task DispatchAsync(CancellationToken cancellationToken = default)
    {
        foreach (var message in _publishes)
        {
            await _publishEndpoint.Publish(message, cancellationToken);
        }
        
        _publishes.Clear();
    }
}