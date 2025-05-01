using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RookieShop.FrontStore.Modules.Shopping.Abstractions;
using RookieShop.FrontStore.Modules.Shopping.Models;

namespace RookieShop.FrontStore.Middlewares;

public class QueryCartActionFilter : IAsyncActionFilter
{
    private readonly ICartService _cartService;

    public QueryCartActionFilter(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Cart? cart = null;

        if (context.HttpContext.User.Identity is { IsAuthenticated: true })
        {
            cart = await _cartService.GetCartAsync();
        }

        if (context.Controller is Controller controller)
        {
            controller.ViewBag.Cart = cart;
        }

        await next();
    }
}