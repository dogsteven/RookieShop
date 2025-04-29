using MassTransit;
using RookieShop.Shopping.Application.Abstractions;

namespace RookieShop.Shopping.Infrastructure.MessageDispatcher;

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

    public async Task PublishAllAsync(CancellationToken cancellationToken = default)
    {
        foreach (var integrationEvent in _integrationEvents)
        {
            await _publishEndpoint.Publish(integrationEvent, cancellationToken);
        }
        
        _integrationEvents.Clear();
    }
}