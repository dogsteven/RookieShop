using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RookieShop.FrontStore.Modules.Shopping.Abstractions;
using RookieShop.FrontStore.Modules.Shopping.Models;

namespace RookieShop.FrontStore.Middlewares;

public class QueryCartActionFilter : IAsyncActionFilter
{
    private readonly ICartService _cartService;
    private readonly ILogger<QueryCartActionFilter> _logger;
    
    private readonly IEnumerable<string> _unsupportedRoutes;

    public QueryCartActionFilter(ICartService cartService, ILogger<QueryCartActionFilter> logger)
    {
        _cartService = cartService;
        _logger = logger;
        
        _unsupportedRoutes = ["/Account/Login", "/Account/Logout"];
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;

        if (request.Method != "GET" || _unsupportedRoutes.Any(route => request.Path.StartsWithSegments(route)))
        {
            await next();
            return;
        }
        
        _logger.LogInformation("Query cart from {route}", request.Path);
        
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