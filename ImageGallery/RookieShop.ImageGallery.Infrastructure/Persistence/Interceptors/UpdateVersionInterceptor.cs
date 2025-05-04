using Microsoft.EntityFrameworkCore.Diagnostics;
using RookieShop.ImageGallery.Application.Entities;

namespace RookieShop.ImageGallery.Infrastructure.Persistence.Interceptors;

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
        
        var entries = context.ChangeTracker.Entries<Image>();

        foreach (var entry in entries)
        {
            entry.Property("Version").CurrentValue = DateTime.UtcNow;
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}