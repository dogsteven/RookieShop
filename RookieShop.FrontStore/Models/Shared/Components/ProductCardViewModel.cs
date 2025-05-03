using RookieShop.ProductCatalog.ViewModels;

namespace RookieShop.FrontStore.Models.Shared.Components;

public class ProductCardViewModel
{
    public ProductDto Product { get; set; } = null!;
    public bool ShowFeaturedTag { get; set; }
}