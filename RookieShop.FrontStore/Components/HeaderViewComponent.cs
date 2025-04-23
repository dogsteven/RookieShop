using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models.Shared.Components;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "Header")]
public class HeaderViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
    {
        return View(new HeaderViewModel
        {
        });
    }
}