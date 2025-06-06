using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RookieShop.Ordering.Domain.Orders;

namespace RookieShop.Ordering.Infrastructure.Persistence.Interceptors;

public class UpdateVersionInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var context = eventData.Context;

        if (context == null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
        
        var entries = context.ChangeTracker.Entries<Order>().Cast<EntityEntry>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
        
        foreach (var entry in entries)
        {
            entry.Property("Version").CurrentValue = DateTime.UtcNow;
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}