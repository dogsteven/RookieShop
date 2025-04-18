using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ProductReview.Application.Exceptions;

namespace RookieShop.WebApi.ExceptionHandlers;

public class ProductReviewExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ProductReviewExceptionHandler(IProblemDetailsService problemDetailsService)
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
            case ProfaneCommentException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Profane comment detected",
                    Detail = exception.Message
                };
                break;
            
            case CustomerHasNotWrittenReviewException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Customer has not written review",
                    Detail = exception.Message
                };
                break;
            
            case MakeReactionToOwnReviewException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Make reaction to own review",
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