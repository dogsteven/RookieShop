using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Infrastructure.Messages;
using RookieShop.Shopping.Infrastructure.Persistence;

namespace RookieShop.Shopping.Infrastructure.UnitOfWork;

public class EntityFrameworkCoreUnitOfWork : IUnitOfWork
{
    private readonly ShoppingDbContext _context;
    private readonly ExternalMessageDispatcher _externalMessageDispatcher;

    public EntityFrameworkCoreUnitOfWork(ShoppingDbContext context, ExternalMessageDispatcher externalMessageDispatcher)
    {
        _context = context;
        _externalMessageDispatcher = externalMessageDispatcher;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        await _externalMessageDispatcher.DispatchAsync(cancellationToken);
    }
}