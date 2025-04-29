using RookieShop.Shared.Domain;

namespace RookieShop.Shopping.Application.Abstractions;

public interface IDomainEventPublisher
{
    public Task PublishAsync(DomainEventSource source, CancellationToken cancellationToken = default);
}