using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Commands.Carts;
using RookieShop.Shopping.Application.Commands.CheckoutSessions;
using RookieShop.Shopping.Application.Commands.StockItems;
using RookieShop.Shopping.Application.Events.DomainEventConsumers;
using RookieShop.Shopping.Application.Queries;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Abstractions;
using RookieShop.Shopping.Domain.Carts.Events;
using RookieShop.Shopping.Domain.CheckoutSessions;
using RookieShop.Shopping.Domain.CheckoutSessions.Events;
using RookieShop.Shopping.Domain.Services;
using RookieShop.Shopping.Domain.StockItems.Events;
using RookieShop.Shopping.Infrastructure.Messages;
using RookieShop.Shopping.Infrastructure.OptionsProvider;
using RookieShop.Shopping.Infrastructure.Persistence;
using RookieShop.Shopping.Infrastructure.Schedulers;
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

        services.AddSingleton<CartService>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IShoppingOptionsProvider, ConfigurationShoppingOptionsProvider>();
        
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICheckoutSessionRepository, CheckoutSessionRepository>();
        services.AddScoped<IStockItemRepository, StockItemRepository>();
        
        services.AddSingleton<MessageDispatcherInstrumentation>();
        services.AddSingleton<MassTransitMessageDispatcherInstrumentation>();
        
        services.AddScoped<IUnitOfWork, EntityFrameworkCoreUnitOfWork>();
        
        services.AddScoped<MassTransitMessageDispatcher>();
        services.AddScoped<IExternalMessageDispatcher>(provider => provider.GetRequiredService<MassTransitMessageDispatcher>());
        
        services.AddScoped<MessageDispatcher>();
        services.AddScoped<IMessageDispatcher>(provider => provider.GetRequiredService<MessageDispatcher>());
        services.AddScoped<DomainEventPublisher>();
        services.AddScoped<TransactionalMessageDispatcher>();
        
        services.AddScoped<IExpireCartScheduler, QuartzExpireCartScheduler>();
        services.AddScoped<IExpireCheckoutSessionScheduler, QuartzExpireCheckoutSessionScheduler>();
        
        // Cart
        services.AddScoped<ICommandConsumer<AddItemToCart>, AddItemToCartConsumer>();
        services.AddScoped<ICommandConsumer<AdjustItemQuantity>, AdjustItemQuantityConsumer>();
        services.AddScoped<ICommandConsumer<RemoveItemFromCart>, RemoveItemFromCartConsumer>();
        services.AddScoped<ICommandConsumer<StartCartCheckout>, StartCartCheckoutConsumer>();
        
        services.AddScoped<IEventConsumer<CartExpired>, HandleStockReservationOnCartExpiredConsumer>();
        services.AddScoped<IEventConsumer<CartCheckoutStarted>, AddItemsToCheckoutSessionOnCartCheckoutStartedConsumer>();
        services.AddScoped<IEventConsumer<CartCheckoutStarted>, UnscheduleExpireCartOnCartCheckoutStartedConsumer>();
        
        // Checkout session
        services.AddScoped<ICommandConsumer<StartCheckoutSession>, StartCheckoutSessionConsumer>();
        services.AddScoped<ICommandConsumer<AddItemsToCheckoutSession>, AddItemsToCheckoutSessionConsumer>();
        services.AddScoped<ICommandConsumer<SetCheckoutSessionAddresses>, SetCheckoutSessionAddressesConsumer>();
        services.AddScoped<ICommandConsumer<CompleteCheckoutSession>, CompleteCheckoutSessionConsumer>();
        
        services.AddScoped<IEventConsumer<CheckoutSessionStarted>, StartCartCheckoutOnCheckoutSessionStartedConsumer>();
        services.AddScoped<IEventConsumer<CheckoutSessionCompleted>, CompleteCartCheckoutOnCheckoutSessionCompletedConsumer>();
        services.AddScoped<IEventConsumer<CheckoutSessionCompleted>, PublishIntegrationEventOnCheckoutSessionCompletedConsumer>();
        services.AddScoped<IEventConsumer<CheckoutSessionExpired>, FailCartCheckoutOnCheckoutSessionExpiredConsumer>();
        
        // Stock item
        services.AddScoped<ICommandConsumer<IncreaseStock>, IncreaseStockConsumer>();
        
        services.AddScoped<IEventConsumer<StockLevelChanged>, PublishIntegrationEventOnStockLevelChangedConsumer>();
        
        services.AddScoped<CartRepositoryHelper>();
        services.AddScoped<CheckoutSessionRepositoryHelper>();
        services.AddScoped<ShoppingQueryService>();

        services.AddSingleton<MessageDispatcher.ConsumeMethodRegistry>(_ =>
        {
            var consumeMethodRegistry = new MessageDispatcher.ConsumeMethodRegistry();
            
            // Cart
            consumeMethodRegistry.Add<AddItemToCart>();
            consumeMethodRegistry.Add<AdjustItemQuantity>();
            consumeMethodRegistry.Add<RemoveItemFromCart>();
            consumeMethodRegistry.Add<StartCartCheckout>();
            
            consumeMethodRegistry.Add<CartExpired>();
            consumeMethodRegistry.Add<CartCheckoutStarted>();
            
            // Stock item
            consumeMethodRegistry.Add<IncreaseStock>();
            
            consumeMethodRegistry.Add<StockLevelChanged>();
            
            // Checkout session
            consumeMethodRegistry.Add<StartCheckoutSession>();
            consumeMethodRegistry.Add<AddItemsToCheckoutSession>();
            consumeMethodRegistry.Add<SetCheckoutSessionAddresses>();
            consumeMethodRegistry.Add<CompleteCheckoutSession>();
            
            consumeMethodRegistry.Add<CheckoutSessionStarted>();
            consumeMethodRegistry.Add<CheckoutSessionCompleted>();
            consumeMethodRegistry.Add<CheckoutSessionExpired>();
            
            return consumeMethodRegistry;
        });
        
        return services;
    }
}