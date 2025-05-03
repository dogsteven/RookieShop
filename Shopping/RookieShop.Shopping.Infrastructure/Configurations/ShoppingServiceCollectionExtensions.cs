using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Events.DomainEventConsumers;
using RookieShop.Shopping.Application.Queries;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Carts.Events;
using RookieShop.Shopping.Domain.StockItems.Events;
using RookieShop.Shopping.Infrastructure.ClearCartScheduler;
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

                npgsql.UseVector();
            });
        });
        
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IStockItemRepository, StockItemRepository>();
        
        services.AddScoped<IUnitOfWork, EntityFrameworkCoreUnitOfWork>();
        
        services.AddScoped<MassTransitMessageDispatcher>();
        services.AddScoped<IExternalMessageDispatcher>(provider => provider.GetRequiredService<MassTransitMessageDispatcher>());

        services.AddScoped<MessageDispatcher>();
        services.AddScoped<IMessageDispatcher>(provider => provider.GetRequiredService<MessageDispatcher>());
        services.AddScoped<DomainEventPublisher>();
        services.AddScoped<TransactionalMessageDispatcher>();

        services.AddSingleton<IClearCartScheduler, QuartzClearCartScheduler>();

        services.AddScoped<ICommandConsumer<IncreaseStock>, IncreaseStockConsumer>();
        services.AddScoped<ICommandConsumer<AddItemToCart>, AddItemToCartConsumer>();
        services.AddScoped<ICommandConsumer<AdjustItemQuantity>, AdjustItemQuantityConsumer>();
        services.AddScoped<ICommandConsumer<RemoveItemFromCart>, RemoveItemFromCartConsumer>();
        
        services.AddScoped<IEventConsumer<ItemAddedToCart>, HandleStockReservationOnItemAddedConsumer>();
        services.AddScoped<IEventConsumer<ItemQuantityAdjusted>, HandleStockReservationOnQuantityAdjustedConsumer>();
        services.AddScoped<IEventConsumer<ItemRemovedFromCart>, HandleStockReservationOnItemRemovedConsumer>();
        services.AddScoped<IEventConsumer<StockLevelChanged>, PublishIntegrationEventOnStockLevelChangedConsumer>();
        services.AddScoped<IEventConsumer<CartExpirationDateExtended>, ScheduleClearCartOnExpirationConsumer>();
        
        services.AddScoped<CartRepositoryHelper>();

        services.AddScoped<ShoppingQueryService>();

        services.AddSingleton(TimeProvider.System);

        services.AddSingleton<MessageDispatcher.ConsumeMethodRegistry>(_ =>
        {
            var consumeMethodRegistry = new MessageDispatcher.ConsumeMethodRegistry();
            consumeMethodRegistry.Add<IncreaseStock>();
            consumeMethodRegistry.Add<AddItemToCart>();
            consumeMethodRegistry.Add<AdjustItemQuantity>();
            consumeMethodRegistry.Add<RemoveItemFromCart>();
        
            consumeMethodRegistry.Add<ItemAddedToCart>();
            consumeMethodRegistry.Add<ItemQuantityAdjusted>();
            consumeMethodRegistry.Add<ItemRemovedFromCart>();
            consumeMethodRegistry.Add<StockLevelChanged>();
            consumeMethodRegistry.Add<CartExpirationDateExtended>();
            
            return consumeMethodRegistry;
        });
        
        return services;
    }
}