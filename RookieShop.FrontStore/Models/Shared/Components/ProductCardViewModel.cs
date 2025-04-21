using RookieShop.FrontStore.Modules.ProductCatalog.Models;

namespace RookieShop.FrontStore.Models.Shared.Components;

public class ProductCardViewModel
{
    public Product Product { get; set; } = null!;
    public bool ShowFeaturedTag { get; set; }
}