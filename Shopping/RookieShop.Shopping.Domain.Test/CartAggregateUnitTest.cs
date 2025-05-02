using RookieShop.Shopping.Domain.Carts;
using RookieShop.Shopping.Domain.Carts.Events;

namespace RookieShop.Shopping.Domain.Test;

public class CartAggregateUnitTest
{
    [Fact]
    public void Test_CartCreation()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        var cart = new Cart(id);
        
        // Assert
        Assert.Equal(id, cart.Id);
        Assert.Empty(cart.Items);
    }

    [Fact]
    public void Should_AddItem_SuccessWithNewItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("abc", "ABC", 20, Guid.NewGuid(), 1);
        cart.ClearDomainEvents();
        
        // Act
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        
        // Assert
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.NotNull(item);
        Assert.Equal("name", item.Name);
        Assert.Equal(10, item.Price);
        Assert.Equal(3, item.Quantity);

        Assert.Contains(cart.DomainEvents, domainEvent =>
        {
            var isItemAddedToCart = domainEvent is ItemAddedToCart;

            if (!isItemAddedToCart)
            {
                return false;
            }

            var itemAddedToCart = (ItemAddedToCart)domainEvent;

            return itemAddedToCart is { Sku: "sku", Quantity: 3 };
        });
    }
    
    [Fact]
    public void Should_AddItem_SuccessWithAddedQuantityToExistingItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ClearDomainEvents();
        
        // Act
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 2);
        
        // Assert
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.NotNull(item);
        Assert.Equal("name", item.Name);
        Assert.Equal(10, item.Price);
        Assert.Equal(5, item.Quantity);

        Assert.Contains(cart.DomainEvents, domainEvent =>
        {
            var isItemAddedToCart = domainEvent is ItemAddedToCart;

            if (!isItemAddedToCart)
            {
                return false;
            }

            var itemAddedToCart = (ItemAddedToCart)domainEvent;

            return itemAddedToCart is { Sku: "sku", Quantity: 2 };
        });
    }
    
    [Fact]
    public void Should_AddItem_SuccessWithUnchangedItemInfo()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ClearDomainEvents();
        
        // Act
        cart.AddItem("sku", "name name", 100, Guid.NewGuid(), 2);
        
        // Assert
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.NotNull(item);
        Assert.Equal("name", item.Name);
        Assert.Equal(10, item.Price);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void Should_AdjustItemQuantity_FailedWithNotFoundItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ClearDomainEvents();
        
        // Act
        var adjustItemQuantityAction = () => cart.AdjustItemQuantity("abc", 4);
        
        // Assert
        Assert.Throws<CartItemNotFoundException>(adjustItemQuantityAction);
    }

    [Fact]
    public void Should_AdjustItemQuantity_SuccessWithNothingHappens()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ClearDomainEvents();
        
        // Act
        cart.AdjustItemQuantity("sku", 3);
        
        // Assert
        Assert.Empty(cart.DomainEvents);
    }

    [Fact]
    public void Should_AdjustItemQuantity_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ClearDomainEvents();
        
        // Act
        cart.AdjustItemQuantity("sku", 5);
        
        // Assert
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.NotNull(item);
        Assert.Equal(5, item.Quantity);

        Assert.Contains(cart.DomainEvents, domainEvent =>
        {
            var isItemQuantityAdjusted = domainEvent is ItemQuantityAdjusted;

            if (!isItemQuantityAdjusted)
            {
                return false;
            }

            var itemQuantityAdjusted = (ItemQuantityAdjusted)domainEvent;

            return itemQuantityAdjusted is { Sku: "sku", OldQuantity: 3, NewQuantity: 5 };
        });
    }

    [Fact]
    public void Should_RemoveItem_FailedWithNotFoundItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ClearDomainEvents();
        
        // Act
        var removeItemAction = () => cart.RemoveItem("abc");
        
        // Assert
        Assert.Throws<CartItemNotFoundException>(removeItemAction);
    }

    [Fact]
    public void Should_RemoveItem_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ClearDomainEvents();
        
        // Act
        cart.RemoveItem("sku");
        
        // Assert
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.Null(item);

        Assert.Contains(cart.DomainEvents, domainEvent =>
        {
            var isItemRemovedFromCart = domainEvent is ItemRemovedFromCart;

            if (!isItemRemovedFromCart)
            {
                return false;
            }

            var itemRemovedFromCart = (ItemRemovedFromCart)domainEvent;

            return itemRemovedFromCart is { Sku: "sku", Quantity: 3 };
        });
    }
}