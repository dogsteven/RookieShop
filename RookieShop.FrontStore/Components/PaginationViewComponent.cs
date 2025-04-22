using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models.Shared.Components;

namespace RookieShop.FrontStore.Components;

[ViewComponent(Name = "Pagination")]
public class PaginationViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(long count, int pageNumber, int pageSize)
    {
        return View(new PaginationViewModel
        {
            Count = count,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }
}