using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models.Shared.Components;
using RookieShop.ProductCatalog.ViewModels;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "AddToCartButton")]
public class AddToCartButtonViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ProductDto product, string? continueUrl)
    {
        return View(new AddToCartButtonViewModel
        {
            Product = product,
            ContinueUrl = continueUrl
        });
    }
}