using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models.Shared.Components;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "Header")]
public class HeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(new HeaderViewModel
        {
            Cart = ViewBag.Cart
        });
    }
}