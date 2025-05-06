using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Events.DomainEventConsumers;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Test.Utilities;
using RookieShop.Shopping.Domain.Carts;
using RookieShop.Shopping.Domain.Carts.Events;
using RookieShop.Shopping.Domain.StockItems;
using RookieShop.Shopping.Domain.StockItems.Events;

namespace RookieShop.Shopping.Application.Test;

public class CartConsumerUnitTest
{
    [Fact]
    public async Task Should_AddItemToCart_FailedWithNotFoundStockItem()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as StockItem);
        
        var addItemToCartConsumer = scope.ServiceProvider.GetRequiredService<AddItemToCartConsumer>();

        var command = new AddItemToCart
        {
            Id = Guid.NewGuid(),
            Sku = "sku",
            Quantity = 1,
        };
        
        // Act
        var addItemToCartAction = async () => await addItemToCartConsumer.ConsumeAsync(command, default);
        
        // Assert
        await Assert.ThrowsAsync<StockItemNotFoundException>(addItemToCartAction);
    }
    
    [Fact]
    public async Task Should_AddItemToCart_FailedWithInsufficientStock()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
                stockItem.IncreaseStock(4);
                stockItem.ClearDomainEvents();
                return stockItem;
            });

        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Cart);
        
        var addItemToCartConsumer = scope.ServiceProvider.GetRequiredService<AddItemToCartConsumer>();

        var command = new AddItemToCart
        {
            Id = Guid.NewGuid(),
            Sku = "sku",
            Quantity = 5,
        };
        
        // Act
        var addItemToCartAction = async () => await addItemToCartConsumer.ConsumeAsync(command, default);
        
        // Assert
        await Assert.ThrowsAsync<InsufficientStockException>(addItemToCartAction);
    }
    
    [Fact]
    public async Task Should_AddItemToCart_Success()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
                stockItem.IncreaseStock(4);
                stockItem.ClearDomainEvents();
                
                return stockItem;
            });

        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Cart);
        
        var addItemToCartConsumer = scope.ServiceProvider.GetRequiredService<AddItemToCartConsumer>();

        var command = new AddItemToCart
        {
            Id = Guid.NewGuid(),
            Sku = "sku",
            Quantity = 3,
        };
        
        // Act
        await addItemToCartConsumer.ConsumeAsync(command, default);
        
        // Assert
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();
        
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<ItemAddedToCart>(message => message.Sku == "sku" && message.Quantity == 3), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<StockLevelChanged>(message => message.Sku == "sku" && message.ChangedQuantity == -3), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.IsAny<CartExpirationTimeExtended>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_AdjustItemQuantity_FailedWithNotFoundStockItem()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as StockItem);
        
        var adjustItemQuantityConsumer = scope.ServiceProvider.GetRequiredService<AdjustItemQuantityConsumer>();

        var command = new AdjustItemQuantity
        {
            Id = Guid.NewGuid(),
            Adjustments = [
                new AdjustItemQuantity.Adjustment
                {
                    Sku = "sku",
                    NewQuantity = 10
                }
            ]
        };
        
        // Act
        var addItemToCartAction = async () => await adjustItemQuantityConsumer.ConsumeAsync(command, default);
        
        // Assert
        await Assert.ThrowsAsync<StockItemNotFoundException>(addItemToCartAction);
    }

    [Fact]
    public async Task Should_AdjustItemQuantity_FailedWithInsufficientStock()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
                stockItem.IncreaseStock(4);
                stockItem.Reserve(1);
                stockItem.ClearDomainEvents();
                
                return stockItem;
            });
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var cart = new Cart(Guid.NewGuid());
                cart.AddItem("sku", "name", 100, Guid.NewGuid(), 1);
                cart.ClearDomainEvents();
                
                return cart;
            });
        
        var adjustItemQuantityConsumer = scope.ServiceProvider.GetRequiredService<AdjustItemQuantityConsumer>();

        var command = new AdjustItemQuantity
        {
            Id = Guid.NewGuid(),
            Adjustments = [
                new AdjustItemQuantity.Adjustment
                {
                    Sku = "sku",
                    NewQuantity = 10
                }
            ]
        };
        
        // Act
        var addItemToCartAction = async () => await adjustItemQuantityConsumer.ConsumeAsync(command, default);
        
        // Assert
        await Assert.ThrowsAsync<InsufficientStockException>(addItemToCartAction);
    }
    
    [Fact]
    public async Task Should_AdjustItemQuantity_SuccessWithItemQuantityIncreased()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
                stockItem.IncreaseStock(4);
                stockItem.Reserve(1);
                stockItem.ClearDomainEvents();
                
                return stockItem;
            });
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var cart = new Cart(Guid.NewGuid());
                cart.AddItem("sku", "name", 100, Guid.NewGuid(), 1);
                cart.ClearDomainEvents();
                
                return cart;
            });
        
        var adjustItemQuantityConsumer = scope.ServiceProvider.GetRequiredService<AdjustItemQuantityConsumer>();

        var command = new AdjustItemQuantity
        {
            Id = Guid.NewGuid(),
            Adjustments = [
                new AdjustItemQuantity.Adjustment
                {
                    Sku = "sku",
                    NewQuantity = 3
                }
            ]
        };
        
        // Act
        await adjustItemQuantityConsumer.ConsumeAsync(command, default);
        
        // Assert
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();
        
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<ItemAddedToCart>(message => message.Sku == "sku" && message.Quantity == 2), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<StockLevelChanged>(message => message.Sku == "sku" && message.ChangedQuantity == -2), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.IsAny<CartExpirationTimeExtended>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Should_AdjustItemQuantity_SuccessWithItemQuantityDecreased()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
                stockItem.IncreaseStock(4);
                stockItem.Reserve(3);
                stockItem.ClearDomainEvents();
                
                return stockItem;
            });
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var cart = new Cart(Guid.NewGuid());
                cart.AddItem("sku", "name", 100, Guid.NewGuid(), 3);
                cart.ClearDomainEvents();
                
                return cart;
            });
        
        var adjustItemQuantityConsumer = scope.ServiceProvider.GetRequiredService<AdjustItemQuantityConsumer>();

        var command = new AdjustItemQuantity
        {
            Id = Guid.NewGuid(),
            Adjustments = [
                new AdjustItemQuantity.Adjustment
                {
                    Sku = "sku",
                    NewQuantity = 2
                }
            ]
        };
        
        // Act
        await adjustItemQuantityConsumer.ConsumeAsync(command, default);
        
        // Assert
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();
        
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<ItemRemovedFromCart>(message => message.Sku == "sku" && message.Quantity == 1), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<StockLevelChanged>(message => message.Sku == "sku" && message.ChangedQuantity == 1), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.IsAny<CartExpirationTimeExtended>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_RemoveItemFromCart_FailedWithStockItemNotFound()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as StockItem);
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Cart);
        
        var removeItemFromCartConsumer = scope.ServiceProvider.GetRequiredService<RemoveItemFromCartConsumer>();

        var command = new RemoveItemFromCart
        {
            Id = Guid.NewGuid(),
            Sku = "sku",
        };
        
        // Act
        var removeItemFromCartAction = async () => await removeItemFromCartConsumer.ConsumeAsync(command, default);
        
        // Assert
        await Assert.ThrowsAsync<StockItemNotFoundException>(removeItemFromCartAction);
    }
    
    [Fact]
    public async Task Should_RemoveItemFromCart_Success()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
                stockItem.IncreaseStock(4);
                stockItem.Reserve(3);
                stockItem.ClearDomainEvents();
                
                return stockItem;
            });
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var cart = new Cart(Guid.NewGuid());
                cart.AddItem("sku", "name", 100, Guid.NewGuid(), 1);
                cart.ClearDomainEvents();
                
                return cart;
            });
        
        var removeItemFromCartConsumer = scope.ServiceProvider.GetRequiredService<RemoveItemFromCartConsumer>();

        var command = new RemoveItemFromCart
        {
            Id = Guid.NewGuid(),
            Sku = "sku"
        };
        
        // Act
        await removeItemFromCartConsumer.ConsumeAsync(command, default);
        
        // Assert
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();
        
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<ItemRemovedFromCart>(message => message.Sku == "sku" && message.Quantity == 1), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<StockLevelChanged>(message => message.Sku == "sku" && message.ChangedQuantity == 1), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.IsAny<CartExpirationTimeExtended>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ExpireCart_SuccessWithNothing()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockTimeProvider = scope.ServiceProvider.GetRequiredService<Mock<TimeProvider>>();
        
        mockTimeProvider.Setup(provider => provider.GetUtcNow())
            .Returns(() => new DateTimeOffset(2025, 1, 1, 1, 0, 0, TimeSpan.FromHours(7)));
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var fakeTimeProvider = new Mock<TimeProvider>();

                fakeTimeProvider.Setup(provider => provider.GetUtcNow())
                    .Returns(() => new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.FromHours(7)));
                
                var cart = new Cart(Guid.NewGuid());
                cart.AddItem("sku", "name", 100, Guid.NewGuid(), 1);
                cart.ExtendExpiration(fakeTimeProvider.Object, 120);
                cart.ClearDomainEvents();
                
                return cart;
            });
        
        var expiredCartConsumer = scope.ServiceProvider.GetRequiredService<ExpireCartConsumer>();

        var command = new ExpireCart
        {
            Id = Guid.NewGuid()
        };

        // Act
        await expiredCartConsumer.ConsumeAsync(command, default);
        
        // Assert
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();
        
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.IsAny<CartExpired>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Should_ExpireCart_Success()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var mockTimeProvider = scope.ServiceProvider.GetRequiredService<Mock<TimeProvider>>();
        
        mockTimeProvider.Setup(provider => provider.GetUtcNow())
            .Returns(() => new DateTimeOffset(2025, 1, 1, 1, 0, 0, TimeSpan.FromHours(7)));
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var fakeTimeProvider = new Mock<TimeProvider>();

                fakeTimeProvider.Setup(provider => provider.GetUtcNow())
                    .Returns(() => new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.FromHours(7)));
                
                var cart = new Cart(Guid.NewGuid());
                cart.AddItem("sku1", "name1", 100, Guid.NewGuid(), 2);
                cart.AddItem("sku2", "name2", 200, Guid.NewGuid(), 3);
                cart.ExtendExpiration(fakeTimeProvider.Object, 30);
                cart.ClearDomainEvents();
                
                return cart;
            });
        
        var expiredCartConsumer = scope.ServiceProvider.GetRequiredService<ExpireCartConsumer>();

        var command = new ExpireCart
        {
            Id = Guid.NewGuid()
        };

        // Act
        await expiredCartConsumer.ConsumeAsync(command, default);
        
        // Assert
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();
        
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<CartExpired>(message => message.Items.First(item => item.Sku == "sku1").Quantity == 2), It.IsAny<CancellationToken>()), Times.Once);
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<CartExpired>(message => message.Items.First(item => item.Sku == "sku2").Quantity == 3), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Test_HandleStockReservationOnCartExpired()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();
        
        var handleStockReservationOnCartExpiredConsumer = scope.ServiceProvider.GetRequiredService<HandleStockReservationOnCartExpiredConsumer>();
        
        var id = Guid.NewGuid();

        var @event = new CartExpired
        {
            Id = id,
            Items = [
                new CartExpired.Item
                {
                    Sku = "sku1",
                    Quantity = 2,
                },
                new CartExpired.Item
                {
                    Sku = "sku3",
                    Quantity = 3,
                }
            ]
        };
        
        // Act
        await handleStockReservationOnCartExpiredConsumer.ConsumeAsync(@event, default);
        
        // Assert
        var mockExternalMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IExternalMessageDispatcher>>();

        foreach (var item in @event.Items)
        {
            mockExternalMessageDispatcher.Verify(dispatch => dispatch.EnqueuePublish(It.Is<ReleaseStockReservation>(message => message.Sku == item.Sku && message.Quantity == item.Quantity)), Times.Once);
        }
    }
}