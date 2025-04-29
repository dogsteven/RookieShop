using System.Data;
using Microsoft.EntityFrameworkCore;
using RookieShop.Shopping.Infrastructure.Persistence;

namespace RookieShop.Shopping.Infrastructure.MessageDispatcher;

public class PessimisticScopedMessageDispatcher
{
    private readonly ScopedMessageDispatcher _dispatcher;
    private readonly ShoppingDbContext _context;

    public PessimisticScopedMessageDispatcher(ScopedMessageDispatcher dispatcher, ShoppingDbContext context)
    {
        _dispatcher = dispatcher;
        _context = context;
    }

    public async Task DispatchAsync(object message, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        
        await _dispatcher.DispatchAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
    } 
}