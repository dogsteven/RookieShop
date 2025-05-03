using System.Diagnostics;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Infrastructure.Messages;
using RookieShop.Shopping.Infrastructure.Persistence;

namespace RookieShop.Shopping.Infrastructure.UnitOfWork;

public class EntityFrameworkCoreUnitOfWork : IUnitOfWork
{
    private readonly ShoppingDbContext _context;
    private readonly MassTransitMessageDispatcher _massTransitMessageDispatcher;

    public EntityFrameworkCoreUnitOfWork(ShoppingDbContext context, MassTransitMessageDispatcher massTransitMessageDispatcher)
    {
        _context = context;
        _massTransitMessageDispatcher = massTransitMessageDispatcher;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        await _massTransitMessageDispatcher.DispatchAsync(cancellationToken);
    }
}