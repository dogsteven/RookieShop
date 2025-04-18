using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.WebApi.ExceptionHandlers;

public class ProductCatalogExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ProductCatalogExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        if (exception is RequestException && exception.InnerException is not null)
        {
            exception = exception.InnerException;
        }

        switch (exception)
        {
            case ProductNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Product not found",
                    Detail = exception.Message
                };
                break;
            
            case ProductAlreadyExistsException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Product already exists",
                    Detail = exception.Message
                };
                break;
            
            case CategoryNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Category not found",
                    Detail = exception.Message
                };
                break;
            
            case CategoryAlreadyExistsException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Category already exists",
                    Detail = exception.Message
                };
                break;
                
            default:
                return false;
        }

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}