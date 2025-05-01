using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Events.DomainEventConsumers;
using RookieShop.Shopping.Application.Queries;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Events;
using RookieShop.Shopping.Infrastructure.Messages;
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
        
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IStockItemRepository, StockItemRepository>();
        
        services.AddScoped<IUnitOfWork, EntityFrameworkCoreUnitOfWork>();
        
        services.AddScoped<IntegrationEventPublisher>();
        services.AddScoped<IIntegrationEventPublisher>(provider => provider.GetRequiredService<IntegrationEventPublisher>());

        services.AddScoped<MessageDispatcher>();
        services.AddScoped<TransactionalMessageDispatcher>();
        services.AddScoped<IDomainEventPublisher>(provider => provider.GetRequiredService<MessageDispatcher>());

        services.AddScoped<ICommandConsumer<AddUnitsToStockItem>, AddUnitsToStockItemConsumer>();
        services.AddScoped<ICommandConsumer<AddItemToCart>, AddItemToCartConsumer>();
        services.AddScoped<ICommandConsumer<AdjustItemQuantity>, AdjustItemQuantityConsumer>();
        services.AddScoped<ICommandConsumer<RemoveItemFromCart>, RemoveItemFromCartConsumer>();
        
        services.AddScoped<IEventConsumer<ItemAddedToCart>, HandleStockReservationOnItemAddedConsumer>();
        services.AddScoped<IEventConsumer<ItemQuantityAdjusted>, HandleStockReservationOnQuantityAdjustedConsumer>();
        services.AddScoped<IEventConsumer<ItemRemovedFromCart>, HandleStockReservationOnItemDeletedConsumer>();
        services.AddScoped<IEventConsumer<StockLevelChanged>, PublishIntegrationEventOnStockLevelChangedConsumer>();
        
        services.AddScoped<CartRepositoryHelper>();

        services.AddScoped<ShoppingQueryService>();
        
        return services;
    }
}