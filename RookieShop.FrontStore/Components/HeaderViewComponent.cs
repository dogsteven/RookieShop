using Microsoft.AspNetCore.Mvc;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "Header")]
public class HeaderViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}