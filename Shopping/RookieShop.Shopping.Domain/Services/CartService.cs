using RookieShop.Shared.Models;
using RookieShop.Shopping.Domain.Abstractions;
using RookieShop.Shopping.Domain.Carts;
using RookieShop.Shopping.Domain.StockItems;

namespace RookieShop.Shopping.Domain.Services;

public class CartService
{
    public void AddItemToCart(Cart cart, StockItem stockItem, int quantity)
    {
        stockItem.Reserve(quantity);
        
        cart.AddItem(stockItem.Sku, stockItem.Name, stockItem.Price, stockItem.ImageId, quantity);
    }

    public void AdjustItemQuantity(Cart cart, StockItem stockItem, int newQuantity)
    {
        var oldQuantity = cart.GetItemQuantity(stockItem.Sku);
        
        var quantityDifference = newQuantity - oldQuantity;

        if (quantityDifference > 0)
        {
            stockItem.Reserve(quantityDifference);
        }
        else
        {
            stockItem.ReleaseReservation(-quantityDifference);
        }
        
        cart.AdjustItemQuantity(stockItem.Sku, newQuantity);
    }

    public void RemoveItemFromCart(Cart cart, StockItem stockItem)
    {
        var quantity = cart.GetItemQuantity(stockItem.Sku);
        
        stockItem.ReleaseReservation(quantity);
        
        cart.RemoveItem(stockItem.Sku);
    }
}
