using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Test.Utilities;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Contracts.Events;
using RookieShop.Shopping.Domain.Carts;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Application.Test;

public class UtilitiesUnitTest
{
    [Fact]
    public async Task Test_GetOrCreateCartAsync_WithNullCart()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        var id = Guid.NewGuid();
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Cart);
        
        var cartRepositoryHelper = scope.ServiceProvider.GetRequiredService<CartRepositoryHelper>();
        
        // Act
        var cart = await cartRepositoryHelper.GetOrCreateCartAsync(id, It.IsAny<CancellationToken>());
        
        // Assert
        Assert.NotNull(cart);
        
        mockCartRepository.Verify(repository => repository.Save(cart), Times.Once);
    }
    
    [Fact]
    public async Task Test_GetOrCreateCartAsync_WithNotNullCart()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();
        
        var mockCartRepository = scope.ServiceProvider.GetRequiredService<Mock<ICartRepository>>();
        
        var id = Guid.NewGuid();

        var cart = new Cart(id);
        
        mockCartRepository.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);
        
        var cartRepositoryHelper = scope.ServiceProvider.GetRequiredService<CartRepositoryHelper>();
        
        // Act
        var resultCart = await cartRepositoryHelper.GetOrCreateCartAsync(id, It.IsAny<CancellationToken>());
        
        // Assert
        Assert.Equal(cart, resultCart);
        
        mockCartRepository.Verify(repository => repository.Save(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task Test_PublishDomainEventAsync()
    {
        // Arrange
        var service = new ShoppingServiceCollection();
        
        var builder = service.BuildServiceProvider();
        
        using var scope = builder.CreateScope();

        var domainEvents = new List<object>([
            new StockLevelChanged
            {
                Sku = "sku3",
                ChangedQuantity = 3
            },
            new CartExpired
            {
                Id = Guid.NewGuid(),
                Items = []
            }
        ]);
        
        var fakeDomainEventSource = new FakeDomainEventSource(domainEvents);

        var domainEventPublisher = scope.ServiceProvider.GetRequiredService<DomainEventPublisher>();

        // Act
        await domainEventPublisher.PublishAsync(fakeDomainEventSource, default);
        
        // Assert
        var mockMessageDispatcher = scope.ServiceProvider.GetRequiredService<Mock<IMessageDispatcher>>();

        foreach (var domainEvent in domainEvents)
        {
            mockMessageDispatcher.Verify(dispatcher => dispatcher.PublishAsync(domainEvent, It.IsAny<CancellationToken>()), Times.Once);
        }
        
        Assert.Empty(fakeDomainEventSource.DomainEvents);
    }
}