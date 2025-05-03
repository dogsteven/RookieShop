using RookieShop.Shopping.ViewModels;

namespace RookieShop.FrontStore.Modules.Shopping.Abstractions;

public record QuantityAdjustment(string Sku, int NewQuantity);

public interface ICartService
{
    public Task<CartDto> GetCartAsync(CancellationToken cancellationToken = default);
    public Task AddItemToCartAsync(string sku, int quantity, CancellationToken cancellationToken = default);
    public Task AdjustItemQuantityAsync(IEnumerable<QuantityAdjustment> adjustments, CancellationToken cancellationToken = default);
    public Task RemoveItemFromCartAsync(string sku, CancellationToken cancellationToken = default);
}