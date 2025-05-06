using RookieShop.Shopping.Domain.Abstractions;

namespace RookieShop.Shopping.Application.Test.Utilities;

public class FakeDomainEventSource : DomainEventSource
{
    public FakeDomainEventSource(IEnumerable<object> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            AddDomainEvent(domainEvent);
        }
    }
}