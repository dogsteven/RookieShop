using RookieShop.Shopping.Domain.Carts;
using RookieShop.Shopping.Domain.Services;
using RookieShop.Shopping.Domain.StockItems;
using RookieShop.Shopping.Domain.Test.Utilities;

namespace RookieShop.Shopping.Domain.Test;

public class CartServiceUnitTest
{
    [Fact]
    public void Should_AddItemToCart_FailedWithInsufficientStock()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.ClearDomainEvents();

        var sku = "sku";
        var stockItem = new StockItem(sku, "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(2);
        stockItem.ClearDomainEvents();

        var cartService = new CartService();

        // Act
        var addItemToCartAction = () => cartService.AddItemToCart(cart, stockItem, 3);
        
        // Arrange
        Assert.Throws<InsufficientStockException>(addItemToCartAction);
    }
    
    [Fact]
    public void Should_AddItemToCart_WithNewItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("uks", "eman", 99, Guid.NewGuid(), 1);
        cart.ClearDomainEvents();

        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(2);
        stockItem.ClearDomainEvents();
        
        var cartService = new CartService();

        // Act
        cartService.AddItemToCart(cart, stockItem, 1);
        
        // Arrange
        Assert.Equal(2, cart.Items.Count);
        
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.NotNull(item);
        
        Assert.Equal("sku", item.Sku);
        Assert.Equal("name", item.Name);
        Assert.Equal(100, item.Price);
        Assert.Equal(1, item.Quantity);
        
        Assert.Equal(1, stockItem.AvailableQuantity);
        Assert.Equal(1, stockItem.ReservedQuantity);
    }
    
    [Fact]
    public void Should_AddItemToCart_WithQuantityIncreased()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 100, Guid.NewGuid(), 1);
        cart.ClearDomainEvents();

        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(2);
        stockItem.ClearDomainEvents();

        var cartService = new CartService();

        // Act
        cartService.AddItemToCart(cart, stockItem, 1);
        
        // Arrange
        Assert.Single(cart.Items);
        
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.NotNull(item);
        
        Assert.Equal("sku", item.Sku);
        Assert.Equal("name", item.Name);
        Assert.Equal(100, item.Price);
        Assert.Equal(2, item.Quantity);
        
        Assert.Equal(1, stockItem.AvailableQuantity);
        Assert.Equal(1, stockItem.ReservedQuantity);
    }

    [Fact]
    public void Should_AdjustItemQuantity_FailedWithInsufficientStock()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 100, Guid.NewGuid(), 1);
        cart.ClearDomainEvents();

        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(3);
        stockItem.Reserve(1);
        stockItem.ClearDomainEvents();

        var cartService = new CartService();

        // Act
        var adjustItemQuantityAction = () => cartService.AdjustItemQuantity(cart, stockItem, 5);
        
        // Assert
        Assert.Throws<InsufficientStockException>(adjustItemQuantityAction);
    }
    
    [Fact]
    public void Should_AdjustItemQuantity_FailedWithInvalidOperation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 100, Guid.NewGuid(), 1);
        cart.ClearDomainEvents();

        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(3);
        stockItem.ClearDomainEvents();

        var cartService = new CartService();

        // Act
        var adjustItemQuantityAction = () => cartService.AdjustItemQuantity(cart, stockItem, 0);
        
        // Assert
        Assert.Throws<InvalidOperationException>(adjustItemQuantityAction);
    }

    [Fact]
    public void Should_AdjustItemQuantity_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 100, Guid.NewGuid(), 1);
        cart.ClearDomainEvents();

        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(3);
        stockItem.Reserve(1);
        stockItem.ClearDomainEvents();

        var cartService = new CartService();

        // Act
        cartService.AdjustItemQuantity(cart, stockItem, 2);
        
        // Assert
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.NotNull(item);
        Assert.Equal(2, item.Quantity);
        
        Assert.Equal(1, stockItem.AvailableQuantity);
        Assert.Equal(2, stockItem.ReservedQuantity);
    }

    [Fact]
    public void Should_RemoveItemFromCart_FailedWithInvalidOperation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 100, Guid.NewGuid(), 2);
        cart.ClearDomainEvents();

        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(3);
        stockItem.Reserve(2);
        stockItem.ClearDomainEvents();

        var cartService = new CartService();

        // Act
        cartService.RemoveItemFromCart(cart, stockItem);
        
        // Assert
        Assert.Empty(cart.Items);
        
        Assert.Equal(3, stockItem.AvailableQuantity);
        Assert.Equal(0, stockItem.ReservedQuantity);
    }
}