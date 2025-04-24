using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RookieShop.FrontStore.Models;

namespace RookieShop.FrontStore.ExceptionFilters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var metadataProvider = new EmptyModelMetadataProvider();
            
        context.Result = new ViewResult
        {
            ViewName = "Error",
            ViewData = new ViewDataDictionary(metadataProvider, context.ModelState)
            {
                Model = new ErrorViewModel
                {
                    RequestId = context.HttpContext.TraceIdentifier,
                    Exception = context.Exception
                }
            }
        };
        
        context.ExceptionHandled = true;
    }
}