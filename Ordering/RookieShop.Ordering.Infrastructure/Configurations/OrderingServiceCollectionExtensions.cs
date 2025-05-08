using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Ordering.Application.Abstractions;
using RookieShop.Ordering.Infrastructure.Persistence;
using RookieShop.Ordering.Infrastructure.UnitOfWorks;

namespace RookieShop.Ordering.Infrastructure.Configurations;

public static class OrderingServiceCollectionExtensions
{
    public static IServiceCollection AddOrdering(this IServiceCollection services,
        Action<OrderingConfigurator> configure)
    {
        var configurator = new OrderingConfigurator();
        configure(configurator);
        
        return configurator.ConfigureServices(services);
    }
}

public class OrderingConfigurator
{
    private Func<IServiceProvider, string>? _databaseConnectionString;
    private string? _migrationAssembly;

    internal OrderingConfigurator()
    {
        _databaseConnectionString = null;
        _migrationAssembly = null;
    }

    public OrderingConfigurator SetDatabaseConnectionString(Func<IServiceProvider, string>? databaseConnectionString)
    {
        _databaseConnectionString = databaseConnectionString;
        return this;
    }

    public OrderingConfigurator SetMigrationAssembly(string? migrationAssembly)
    {
        _migrationAssembly = migrationAssembly;
        return this;
    }

    internal IServiceCollection ConfigureServices(IServiceCollection services)
    {
        if (_databaseConnectionString == null)
        {
            return services;
        }

        services.AddDbContext<OrderingDbContext>((provider, db) =>
        {
            var connectionString = _databaseConnectionString(provider);

            db.UseNpgsql(connectionString, npgsql =>
            {
                if (_migrationAssembly != null)
                {
                    npgsql.MigrationsAssembly(_migrationAssembly);
                }

                npgsql.UseVector();
            });
        });

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, EntityFrameworkCoreUnitOfWork>();

        return services;
    }
}