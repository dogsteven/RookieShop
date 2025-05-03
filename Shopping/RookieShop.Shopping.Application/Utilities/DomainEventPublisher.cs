using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Domain;

namespace RookieShop.Shopping.Application.Utilities;

public class DomainEventPublisher
{
    private readonly IMessageDispatcher _messageDispatcher;

    public DomainEventPublisher(IMessageDispatcher messageDispatcher)
    {
        _messageDispatcher = messageDispatcher;
    }

    public async Task PublishAsync(DomainEventSource source, CancellationToken cancellationToken = default)
    {
        var domainEvents = source.DomainEvents.ToList();
        
        source.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            await _messageDispatcher.PublishAsync(domainEvent, cancellationToken);
        }
    }
}