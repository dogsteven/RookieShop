using RookieShop.FrontStore.Modules.ProductCatalog.Models;

namespace RookieShop.FrontStore.Models.Shared.Components;

public class AddToCartButtonViewModel
{
    public string? RedirectUrl { get; set; }

    public Product Product { get; set; } = null!;
}