using RookieShop.ProductCatalog.ViewModels;

namespace RookieShop.FrontStore.Models.Shared.Components;

public class AddToCartButtonViewModel
{
    public string? ContinueUrl { get; set; }

    public ProductDto Product { get; set; } = null!;
}