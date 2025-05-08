using RookieShop.Ordering.Application.Abstractions;
using RookieShop.Ordering.Infrastructure.Persistence;

namespace RookieShop.Ordering.Infrastructure.UnitOfWorks;

public class EntityFrameworkCoreUnitOfWork : IUnitOfWork
{
    private readonly OrderingDbContext _context;

    public EntityFrameworkCoreUnitOfWork(OrderingDbContext context)
    {
        _context = context;
    }
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}