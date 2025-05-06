using RookieShop.Shopping.Domain.Carts;
using RookieShop.Shopping.Domain.Carts.Events;
using RookieShop.Shopping.Domain.Test.Utilities;

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
    public void Should_AdjustItemQuantity_SuccessWithItemRemoved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ClearDomainEvents();
        
        // Act
        cart.AdjustItemQuantity("sku", 0);
        
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

    [Fact]
    public void Should_AdjustItemQuantity_SuccessWithCartItemQuantityIncreased()
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
            var isItemQuantityAdjusted = domainEvent is ItemAddedToCart;

            if (!isItemQuantityAdjusted)
            {
                return false;
            }

            var itemQuantityAdjusted = (ItemAddedToCart)domainEvent;

            return itemQuantityAdjusted is { Sku: "sku", Quantity: 2 };
        });
    }
    
    [Fact]
    public void Should_AdjustItemQuantity_SuccessWithCartItemQuantityDecreased()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 5);
        cart.ClearDomainEvents();
        
        // Act
        cart.AdjustItemQuantity("sku", 1);
        
        // Assert
        var item = cart.Items.FirstOrDefault(item => item.Sku == "sku");
        
        Assert.NotNull(item);
        Assert.Equal(1, item.Quantity);

        Assert.Contains(cart.DomainEvents, domainEvent =>
        {
            var isItemQuantityAdjusted = domainEvent is ItemRemovedFromCart;

            if (!isItemQuantityAdjusted)
            {
                return false;
            }

            var itemQuantityAdjusted = (ItemRemovedFromCart)domainEvent;

            return itemQuantityAdjusted is { Sku: "sku", Quantity: 4 };
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

    [Fact]
    public void Test_ExtendExpirationDate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        
        var fakeTimeProvider = new FakeTimeProvider(new DateTime(2025, 5, 2, 20, 11, 56, DateTimeKind.Utc));
        
        // Act
        cart.ExtendExpiration(fakeTimeProvider, 60);
        
        // Assert
        Assert.Equal(new DateTime(2025, 5, 2, 21, 11, 56, DateTimeKind.Utc), cart.ExpirationTime);
    }

    [Fact]
    public void Should_Expire_SuccessWithNothingHappens()
    {
        // Arrange
        var extendExpirationTimeProvider = new FakeTimeProvider(new DateTime(2025, 5, 2, 20, 11, 56, DateTimeKind.Utc));
        var tryClearTimeProvider = new FakeTimeProvider(new DateTime(2025, 5, 2, 20, 49, 56, DateTimeKind.Utc));
        
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku", "name", 10, Guid.NewGuid(), 3);
        cart.ExtendExpiration(extendExpirationTimeProvider, 60);
        cart.ClearDomainEvents();
        
        // Act
        cart.Expire(tryClearTimeProvider);
        
        // Assert
        Assert.Single(cart.Items);
        Assert.Empty(cart.DomainEvents);
    }
    
    [Fact]
    public void Should_Expire_Success()
    {
        // Arrange
        var extendExpirationTimeProvider = new FakeTimeProvider(new DateTime(2025, 5, 2, 20, 11, 56, DateTimeKind.Utc));
        var tryClearTimeProvider = new FakeTimeProvider(new DateTime(2025, 5, 2, 22, 0, 56, DateTimeKind.Utc));
        
        var id = Guid.NewGuid();
        var cart = new Cart(id);
        cart.AddItem("sku1", "name1", 10, Guid.NewGuid(), 3);
        cart.AddItem("sku2", "name2", 10, Guid.NewGuid(), 2);
        cart.ExtendExpiration(extendExpirationTimeProvider, 60);
        cart.ClearDomainEvents();
        
        // Act
        cart.Expire(tryClearTimeProvider);
        
        // Assert
        Assert.Empty(cart.Items);

        var domainEvents = cart.DomainEvents.ToList();
        
        Assert.Contains(domainEvents, domainEvent =>
        {
            var isCartExpired = domainEvent is CartExpired;

            if (!isCartExpired)
            {
                return false;
            }

            var cartExpired = (CartExpired)domainEvent;

            return cartExpired.Items.Count() == 2;
        });
    }
}