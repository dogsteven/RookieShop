using RookieShop.Shopping.Domain.StockItems;
using RookieShop.Shopping.Domain.StockItems.Events;

namespace RookieShop.Shopping.Domain.Test;

public class StockItemAggregateUnitTest
{
    [Fact]
    public void Test_StockItemCreation()
    {
        // Arrange
        var sku = "sku";
        var name = "name";
        var price = 100;
        var imageId = Guid.NewGuid();
        
        // Act
        var stockItem = new StockItem(sku, name, price, imageId);
        
        // Assert
        Assert.Equal(sku, stockItem.Sku);
        Assert.Equal(name, stockItem.Name);
        Assert.Equal(price, stockItem.Price);
        Assert.Equal(imageId, stockItem.ImageId);
        Assert.Equal(0, stockItem.AvailableQuantity);
        Assert.Equal(0, stockItem.ReservedQuantity);
    }

    [Fact]
    public void Test_UpdateInfo()
    {
        // Arrange
        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        var newName = "newName";
        var newPrice = 200;
        var newImageId = Guid.NewGuid();
        
        // Act
        stockItem.UpdateInfo(newName, newPrice, newImageId);
        
        // Assert
        Assert.Equal(newName, stockItem.Name);
        Assert.Equal(newPrice, stockItem.Price);
        Assert.Equal(newImageId, stockItem.ImageId);
    }

    [Fact]
    public void Test_AddUnits()
    {
        // Arrange
        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(20);
        stockItem.ClearDomainEvents();
        
        // Act
        stockItem.IncreaseStock(10);
        
        // Assert
        Assert.Equal(30, stockItem.AvailableQuantity);

        Assert.Contains(stockItem.DomainEvents, domainEvent =>
        {
            var isStockLevelChanged = domainEvent is StockLevelChanged;

            if (!isStockLevelChanged)
            {
                return false;
            }
            
            var stockLevelChanged = (StockLevelChanged)domainEvent;

            return stockLevelChanged is { Sku: "sku", ChangedQuantity: 10 };
        });
    }

    [Fact]
    public void Should_Reserve_FailedWithNotEnoughStock()
    {
        // Arrange
        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(5);
        stockItem.Reserve(3);
        stockItem.ClearDomainEvents();
        
        // Act
        var reserveAction = () => stockItem.Reserve(4);
        
        // Assert
        Assert.Throws<InsufficientStockException>(reserveAction);
    }

    [Fact]
    public void Should_Reserve_Success()
    {
        // Arrange
        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(5);
        stockItem.Reserve(2);
        stockItem.ClearDomainEvents();
        
        // Act
        stockItem.Reserve(2);
        
        // Assert
        Assert.Equal(1, stockItem.AvailableQuantity);
        Assert.Equal(4, stockItem.ReservedQuantity);
        
        Assert.Contains(stockItem.DomainEvents, domainEvent =>
        {
            var isStockLevelChanged = domainEvent is StockLevelChanged;

            if (!isStockLevelChanged)
            {
                return false;
            }
            
            var stockLevelChanged = (StockLevelChanged)domainEvent;

            return stockLevelChanged is { Sku: "sku", ChangedQuantity: -2 };
        });
    }

    [Fact]
    public void Should_ConfirmReservation_Failed()
    {
        // Arrange
        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(5);
        stockItem.Reserve(2);
        stockItem.ClearDomainEvents();
        
        // Act
        var confirmReservationAction = () => stockItem.ConfirmReservation(3);
        
        // Assert
        Assert.Throws<InvalidOperationException>(confirmReservationAction);
    }
    
    [Fact]
    public void Should_ConfirmReservation_Success()
    {
        // Arrange
        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(5);
        stockItem.Reserve(4);
        stockItem.ClearDomainEvents();
        
        // Act
        stockItem.ConfirmReservation(3);
        
        // Assert
        Assert.Equal(1, stockItem.AvailableQuantity);
        Assert.Equal(1, stockItem.ReservedQuantity);
        
        Assert.Empty(stockItem.DomainEvents);
    }

    [Fact]
    public void Should_ReleaseReservation_Failed()
    {
        // Arrange
        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(5);
        stockItem.Reserve(2);
        stockItem.ClearDomainEvents();
        
        // Act
        var releaseReservationAction = () => stockItem.ReleaseReservation(3);
        
        // Assert
        Assert.Throws<InvalidOperationException>(releaseReservationAction);
    }
    
    [Fact]
    public void Should_ReleaseReservation_Success()
    {
        // Arrange
        var stockItem = new StockItem("sku", "name", 100, Guid.NewGuid());
        stockItem.IncreaseStock(5);
        stockItem.Reserve(3);
        stockItem.ClearDomainEvents();
        
        // Act
        stockItem.ReleaseReservation(2);
        
        // Assert
        Assert.Equal(4, stockItem.AvailableQuantity);
        Assert.Equal(1, stockItem.ReservedQuantity);
        
        Assert.Contains(stockItem.DomainEvents, domainEvent =>
        {
            var isStockLevelChanged = domainEvent is StockLevelChanged;

            if (!isStockLevelChanged)
            {
                return false;
            }
            
            var stockLevelChanged = (StockLevelChanged)domainEvent;

            return stockLevelChanged is { Sku: "sku", ChangedQuantity: 2 };
        });
    }
}