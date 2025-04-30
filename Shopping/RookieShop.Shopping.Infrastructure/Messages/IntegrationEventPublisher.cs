using MassTransit;
using RookieShop.Shopping.Application.Abstractions.Messages;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class IntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    private readonly List<object> _integrationEvents = [];

    public IntegrationEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public void Enqueue(object integrationEvent)
    {
        _integrationEvents.Add(integrationEvent);
    }

    public async Task PublishAsync(CancellationToken cancellationToken = default)
    {
        foreach (var integrationEvent in _integrationEvents)
        {
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        }
        
        _integrationEvents.Clear();
    }
}