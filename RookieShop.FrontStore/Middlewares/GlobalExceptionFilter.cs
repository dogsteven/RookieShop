using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RookieShop.FrontStore.Models;
using RookieShop.FrontStore.Modules.Shared;

namespace RookieShop.FrontStore.Middlewares;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is RookieShopHttpClientUnauthorizedException)
        {
            var redirectUrl = context.HttpContext.Request.Path;

            if (context.HttpContext.Request.Method != "GET")
            {
                redirectUrl = "/";
            }
            
            context.Result = new RedirectToActionResult("Login", "Account", new { redirectUrl });
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