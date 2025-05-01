using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models.Shared.Components;
using RookieShop.FrontStore.Modules.ProductCatalog.Models;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "AddToCartButton")]
public class AddToCartButtonViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Product product, string? redirectUrl)
    {
        return View(new AddToCartButtonViewModel
        {
            Product = product,
            RedirectUrl = redirectUrl
        });
    }
}