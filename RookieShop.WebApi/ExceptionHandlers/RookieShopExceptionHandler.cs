using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Application.Exceptions;

namespace RookieShop.WebApi.ExceptionHandlers;

public class RookieShopExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public RookieShopExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            case ProductNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Product was not found",
                    Detail = exception.Message,
                    Type = nameof(ProductNotFoundException)
                };

                break;
            
            case CategoryNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Category was not found",
                    Detail = exception.Message,
                    Type = nameof(CategoryNotFoundException)
                };

                break;
            
            case ProfanityDetectedException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Profanity detected",
                    Detail = exception.Message,
                    Type = nameof(ProfanityDetectedException)
                };

                break;
            
            case CustomerHasNotPurchasedException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Customer has not purchased product yet",
                    Detail = exception.Message,
                    Type = nameof(CustomerHasNotPurchasedException)
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