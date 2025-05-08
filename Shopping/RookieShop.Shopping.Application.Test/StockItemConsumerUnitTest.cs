using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Commands.StockItems;
using RookieShop.Shopping.Application.Events.DomainEventConsumers;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Test.Utilities;
using RookieShop.Shopping.Domain.StockItems;
using RookieShop.Shopping.Domain.StockItems.Events;

namespace RookieShop.Shopping.Application.Test;

public class StockItemConsumerUnitTest
{
    [Fact]
    public async Task Should_IncreaseStock_FailedWithNotFoundStockItem()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();
        
        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as StockItem);

        var increaseStockConsumer = scope.ServiceProvider.GetRequiredService<IncreaseStockConsumer>();

        var command = new IncreaseStock
        {
            Sku = "sku",
            Quantity = 1,
        };
        
        // Act
        var increaseStockAction = async () => await increaseStockConsumer.ConsumeAsync(command, default);
        
        // Arrange
        await Assert.ThrowsAsync<StockItemNotFoundException>(increaseStockAction);
    }
    
    [Fact]
    public async Task Should_IncreaseStock_Success()
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

        var increaseStockConsumer = scope.ServiceProvider.GetRequiredService<IncreaseStockConsumer>();

        var command = new IncreaseStock
        {
            Sku = "sku",
            Quantity = 1,
        };
        
        // Act
        await increaseStockConsumer.ConsumeAsync(command, default);
        
        // Arrange
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();
        
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<StockLevelChanged>(message => message.Sku == "sku" && message.ChangedQuantity == 1), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Should_ReleaseStockReservation_FailedWithNotFoundStockItem()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();
        
        var mockStockItemRepository = scope.ServiceProvider.GetRequiredService<Mock<IStockItemRepository>>();
        
        mockStockItemRepository.Setup(repository => repository.GetBySkuAsync("sku", It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as StockItem);

        var releaseStockReservationConsumer = scope.ServiceProvider.GetRequiredService<ReleaseStockReservationConsumer>();

        var command = new ReleaseStockReservation
        {
            Sku = "sku",
            Quantity = 1
        };
        
        // Act
        var releaseStockReservationAction = async () => await releaseStockReservationConsumer.ConsumeAsync(command, default);
        
        // Arrange
        await Assert.ThrowsAsync<StockItemNotFoundException>(releaseStockReservationAction);
    }

    [Fact]
    public async Task Should_ReleaseStockReservation_Success()
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

        var releaseStockReservationConsumer = scope.ServiceProvider.GetRequiredService<ReleaseStockReservationConsumer>();

        var command = new ReleaseStockReservation
        {
            Sku = "sku",
            Quantity = 2
        };
        
        // Act
        await releaseStockReservationConsumer.ConsumeAsync(command, default);
        
        // Arrange
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();
        
        mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(It.Is<StockLevelChanged>(message => message.Sku == "sku" && message.ChangedQuantity == 2), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Test_PublishIntegrationEventOnStockLevelChanged()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();
        
        var publishIntegrationEventOnStockLevelChangedConsumer = scope.ServiceProvider.GetRequiredService<PublishIntegrationEventOnStockLevelChangedConsumer>();

        var @event = new StockLevelChanged
        {
            Sku = "sku",
            ChangedQuantity = -2,
        };
        
        // Act
        await publishIntegrationEventOnStockLevelChangedConsumer.ConsumeAsync(@event, default);
        
        // Assert
        var mockExternalMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IExternalMessageDispatcher>>();
        
        mockExternalMessageDispatcher.Verify(dispatcher => dispatcher.EnqueuePublish(It.Is<Contracts.Events.StockLevelChanged>(message => message.Sku == "sku" && message.ChangedQuantity == -2)), Times.Once);
    }
}