namespace RookieShop.Shopping.Domain.Abstractions;

public abstract class DomainEventSource
{
    private List<object>? _domainEvents;
    public IEnumerable<object> DomainEvents => _domainEvents ??= [];
    
    public void ClearDomainEvents() => _domainEvents?.Clear();

    protected void AddDomainEvent(object domainEvent)
    {
        _domainEvents ??= [];
        _domainEvents.Add(domainEvent);
    }
}