using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Events.DomainEventConsumers;
using RookieShop.Shopping.Application.Queries;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Events;
using RookieShop.Shopping.Infrastructure.MessageDispatcher;
using RookieShop.Shopping.Infrastructure.Persistence;
using RookieShop.Shopping.Infrastructure.UnitOfWork;

namespace RookieShop.Shopping.Infrastructure.Configurations;

public static class ShoppingServiceCollectionExtensions
{
    public static IServiceCollection AddShopping(this IServiceCollection services,
        Action<ShoppingConfigurator> configure)
    {
        var configurator = new ShoppingConfigurator();
        configure(configurator);
        
        return configurator.ConfigureServices(services);
    }
}

public class ShoppingConfigurator
{
    private Func<IServiceProvider, string>? _databaseConnectionString;
    private string? _migrationAssembly;

    internal ShoppingConfigurator()
    {
        _databaseConnectionString = null;
        _migrationAssembly = null;
    }
    
    public ShoppingConfigurator SetDatabaseConnectionString(Func<IServiceProvider, string>? databaseConnectionString)
    {
        _databaseConnectionString = databaseConnectionString;
        return this;
    }

    public ShoppingConfigurator SetMigrationAssembly(string? migrationAssembly)
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

        services.AddDbContext<ShoppingDbContext>((provider, db) =>
        {
            var connectionString = _databaseConnectionString(provider);

            db.UseNpgsql(connectionString, npgsql =>
            {
                if (_migrationAssembly != null)
                {
                    npgsql.MigrationsAssembly(_migrationAssembly);
                }
            });
        });
        
        services.AddScoped<ICartRepository>(provider => provider.GetRequiredService<ShoppingDbContext>());
        services.AddScoped<IStockItemRepository>(provider => provider.GetRequiredService<ShoppingDbContext>());
        
        services.AddScoped<IUnitOfWork, EntityFrameworkCoreUnitOfWork>();
        
        services.AddScoped<IntegrationEventPublisher>();
        services.AddScoped<IIntegrationEventPublisher>(provider => provider.GetRequiredService<IntegrationEventPublisher>());

        services.AddScoped<ScopedMessageDispatcher>();
        services.AddScoped<OptimisticScopedMessageDispatcher>();
        services.AddScoped<PessimisticScopedMessageDispatcher>();
        services.AddScoped<IDomainEventPublisher>(provider => provider.GetRequiredService<ScopedMessageDispatcher>());

        services.AddScoped<IMessageConsumer<AddItemToCart>, AddItemToCartConsumer>();
        services.AddScoped<IMessageConsumer<AdjustItemQuantity>, AdjustItemQuantityConsumer>();
        services.AddScoped<IMessageConsumer<RemoveItemFromCart>, RemoveItemFromCartConsumer>();

        services.AddScoped<IMessageConsumer<ItemAddedToCart>, ItemAddedToCartConsumer>();
        services.AddScoped<IMessageConsumer<ItemQuantityAdjusted>, ItemQuantityAdjustedConsumer>();
        services.AddScoped<IMessageConsumer<ItemRemovedFromCart>, ItemRemovedFromCartConsumer>();

        services.AddScoped<IMessageConsumer<AddUnitsToStockItem>, AddUnitsToStockItemConsumer>();
        
        services.AddScoped<CartRepositoryHelper>();

        services.AddScoped<ShoppingQueryService>();
        
        return services;
    }
}