using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models.Shared.Components;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "ProductCard")]
public class ProductCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Product product, bool showFeaturedTag)
    {
        return View(new ProductCardViewModel
        {
            Product = product,
            ShowFeaturedTag = showFeaturedTag
        });
    }
}