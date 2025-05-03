using System.Text;
using System.Text.Json;
using RookieShop.FrontStore.Exceptions;
using RookieShop.FrontStore.Modules.Shared;
using RookieShop.FrontStore.Modules.Shopping.Abstractions;
using RookieShop.Shopping.ViewModels;

namespace RookieShop.FrontStore.Modules.Shopping.Services;

public class CartService : ICartService
{
    private readonly RookieShopHttpClient _httpClient;

    public CartService(RookieShopHttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<CartDto> GetCartAsync(CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/shopping/api/carts");
        
        var response = await _httpClient.SecurelySendAsync(request, cancellationToken);

        return await response.ReadFromJsonAsync<CartDto>(cancellationToken);
    }

    public async Task AddItemToCartAsync(string sku, int quantity, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, "/shopping/api/carts/add-item");

        var body = new { sku, quantity };
        
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SecurelySendAsync(request, cancellationToken);

        await response.EnsureSuccess(cancellationToken);
    }

    public async Task AdjustItemQuantityAsync(IEnumerable<QuantityAdjustment> adjustments, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, "/shopping/api/carts/adjust-item-quantity");

        var body = new
        {
            adjustments = adjustments.Select(adjustment => new { sku = adjustment.Sku, newQuantity = adjustment.NewQuantity })
        };
        
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SecurelySendAsync(request, cancellationToken);

        await response.EnsureSuccess(cancellationToken);
    }

    public async Task RemoveItemFromCartAsync(string sku, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, "/shopping/api/carts/remove-item");

        var body = new { sku };
        
        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.SecurelySendAsync(request, cancellationToken);

        await response.EnsureSuccess(cancellationToken);
    }
}