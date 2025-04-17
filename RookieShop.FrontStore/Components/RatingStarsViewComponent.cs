using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models.Shared.Components;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "RatingStars")]
public class RatingStarsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(int stars, bool isBig)
    {
        return View(new RatingStartsViewModel
        {
            Stars = stars,
            IsBig = isBig
        });
    }
}