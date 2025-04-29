using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Infrastructure.MessageDispatcher;
using RookieShop.Shopping.Infrastructure.Persistence;

namespace RookieShop.Shopping.Infrastructure.UnitOfWork;

public class EntityFrameworkCoreUnitOfWork : IUnitOfWork
{
    private readonly ShoppingDbContext _context;
    private readonly IntegrationEventPublisher _integrationEventPublisher;

    public EntityFrameworkCoreUnitOfWork(ShoppingDbContext context, IntegrationEventPublisher integrationEventPublisher)
    {
        _context = context;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        await _integrationEventPublisher.PublishAllAsync(cancellationToken);
    }
}