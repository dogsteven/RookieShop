using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ImageGallery.Exceptions;

namespace RookieShop.WebApi.ExceptionHandlers;

public class ImageGalleryExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ImageGalleryExceptionHandler(IProblemDetailsService problemDetailsService)
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
            case ImageNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Image was not found",
                    Detail = exception.Message,
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