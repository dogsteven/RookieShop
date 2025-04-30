using RookieShop.Shared.Domain;

namespace RookieShop.Shopping.Application.Abstractions.Messages;

public interface IDomainEventPublisher
{
    public Task PublishAsync(DomainEventSource source, CancellationToken cancellationToken = default);
}