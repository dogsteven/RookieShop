using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RookieShop.FrontStore.Models;
using RookieShop.FrontStore.Modules.Shared;

namespace RookieShop.FrontStore.Middlewares;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnException(ExceptionContext context)
    {
        _logger.LogInformation("Exception Type: {0}", context.Exception.GetType().Name);
        
        if (context.Exception is RookieShopHttpClientUnauthorizedException)
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { redirectUrl = context.HttpContext.Request.Path });
        }
        else
        {
            context.Result = new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                {
                    Model = new ErrorViewModel
                    {
                        RequestId = context.HttpContext.TraceIdentifier,
                        Exception = context.Exception
                    }
                }
            };
        }

        context.ExceptionHandled = true;
    }
}