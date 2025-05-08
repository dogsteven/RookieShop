using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Commands.Carts;
using RookieShop.Shopping.Application.Commands.StockItems;
using RookieShop.Shopping.Application.Events.DomainEventConsumers;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Abstractions;
using RookieShop.Shopping.Domain.Carts.Events;
using RookieShop.Shopping.Domain.Services;
using RookieShop.Shopping.Domain.StockItems.Events;

namespace RookieShop.Shopping.Application.Test.Utilities;

public class ShoppingServiceCollection : ServiceCollection
{
    public ShoppingServiceCollection()
    {
        
        this.AddScoped<Mock<IExternalMessageDispatcher>>();
        this.AddScoped<Mock<IMessageDispatcher>>();
        this.AddScoped<Mock<ICartRepository>>();
        this.AddScoped<Mock<IStockItemRepository>>();
        this.AddScoped<Mock<IUnitOfWork>>();
        
        this.AddSingleton<Mock<IShoppingOptionsProvider>>();
        this.AddSingleton<Mock<IExpireCartScheduler>>();
        this.AddSingleton<Mock<TimeProvider>>();
        
        this.AddScoped<IExternalMessageDispatcher>(provider => provider.GetRequiredService<Mock<IExternalMessageDispatcher>>().Object);
        this.AddScoped<IMessageDispatcher>(provider => provider.GetRequiredService<Mock<IMessageDispatcher>>().Object);
        this.AddScoped<ICartRepository>(provider => provider.GetRequiredService<Mock<ICartRepository>>().Object);
        this.AddScoped<IStockItemRepository>(provider => provider.GetRequiredService<Mock<IStockItemRepository>>().Object);
        this.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<Mock<IUnitOfWork>>().Object);
        
        this.AddSingleton<IShoppingOptionsProvider>(provider => provider.GetRequiredService<Mock<IShoppingOptionsProvider>>().Object);
        this.AddSingleton<IExpireCartScheduler>(provider => provider.GetRequiredService<Mock<IExpireCartScheduler>>().Object);
        this.AddSingleton<TimeProvider>(provider => provider.GetRequiredService<Mock<TimeProvider>>().Object);

        this.AddScoped<AddItemToCartConsumer>();
        this.AddScoped<AdjustItemQuantityConsumer>();
        this.AddScoped<RemoveItemFromCartConsumer>();
        this.AddScoped<ExpireCartConsumer>();

        this.AddScoped<IncreaseStockConsumer>();
        this.AddScoped<ReleaseStockReservationConsumer>();

        this.AddScoped<PublishIntegrationEventOnStockLevelChangedConsumer>();
        this.AddScoped<HandleStockReservationOnCartExpiredConsumer>();
        
        this.AddScoped<ICommandConsumer<AddItemToCart>>(provider => provider.GetRequiredService<AddItemToCartConsumer>());
        this.AddScoped<ICommandConsumer<AdjustItemQuantity>>(provider => provider.GetRequiredService<AdjustItemQuantityConsumer>());
        this.AddScoped<ICommandConsumer<RemoveItemFromCart>>(provider => provider.GetRequiredService<RemoveItemFromCartConsumer>());
        this.AddScoped<ICommandConsumer<ExpireCart>>(provider => provider.GetRequiredService<ExpireCartConsumer>());
        
        this.AddScoped<ICommandConsumer<IncreaseStock>>(provider => provider.GetRequiredService<IncreaseStockConsumer>());
        this.AddScoped<ICommandConsumer<ReleaseStockReservation>>(provider => provider.GetRequiredService<ReleaseStockReservationConsumer>());
        
        this.AddScoped<IEventConsumer<StockLevelChanged>>(provider => provider.GetRequiredService<PublishIntegrationEventOnStockLevelChangedConsumer>());
        this.AddScoped<IEventConsumer<CartExpired>>(provider => provider.GetRequiredService<HandleStockReservationOnCartExpiredConsumer>());

        this.AddSingleton<CartService>();
        this.AddScoped<CartRepositoryHelper>();
        this.AddScoped<DomainEventPublisher>();
    }
}