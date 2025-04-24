using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models.Shared.Components;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "AddToCartButton")]
public class AddToCartButtonViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string? redirectUrl)
    {
        return View(new AddToCartButtonViewModel
        {
            RedirectUrl = redirectUrl
        });
    }
}