using RookieShop.Shopping.Infrastructure.Persistence;

namespace RookieShop.Shopping.Infrastructure.MessageDispatcher;

public class OptimisticScopedMessageDispatcher
{
    private readonly ScopedMessageDispatcher _dispatcher;
    private readonly ShoppingDbContext _context;

    public OptimisticScopedMessageDispatcher(ScopedMessageDispatcher dispatcher, ShoppingDbContext context)
    {
        _dispatcher = dispatcher;
        _context = context;
    }

    public async Task DispatchAsync(object message, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}