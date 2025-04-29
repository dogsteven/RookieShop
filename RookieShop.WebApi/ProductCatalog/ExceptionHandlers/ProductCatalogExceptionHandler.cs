using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.WebApi.ProductCatalog.ExceptionHandlers;

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
            case ProductNotFoundException productNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Product not found",
                    Detail = exception.Message,
                    Extensions = new Dictionary<string, object?>
                    {
                        { "Sku", productNotFoundException.Sku }
                    }
                };
                break;
            
            case ProductSkuHasAlreadyBeenTakenException productAlreadyExistsException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Product sku has already been taken",
                    Detail = exception.Message,
                    Extensions = new Dictionary<string, object?>
                    {
                        { "Sku", productAlreadyExistsException.Sku }
                    }
                };
                break;
            
            case CategoryNotFoundException categoryNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Category not found",
                    Detail = exception.Message,
                    Extensions = new Dictionary<string, object?>
                    {
                        { "Id", categoryNotFoundException.Id }
                    }
                };
                break;
            
            case CategoryNameHasAlreadyBeenTakenException categoryAlreadyExistsException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Category name has already been taken",
                    Detail = exception.Message,
                    Extensions = new Dictionary<string, object?>
                    {
                        { "Name", categoryAlreadyExistsException.Name }
                    }
                };
                break;
            
            case CustomerHasAlreadyWrittenReviewException customerHasAlreadyWrittenReviewException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Customer has already written review",
                    Detail = "You have already written review for this product.",
                    Extensions = new Dictionary<string, object?>
                    {
                        { "WriterId", customerHasAlreadyWrittenReviewException.WriterId },
                        { "ProductSku", customerHasAlreadyWrittenReviewException.ProductSku }
                    }
                };
                break;
            
            case ProfaneCommentException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Profane comment detected",
                    Detail = exception.Message
                };
                break;
            
            case CustomerHasNotWrittenReviewException customerHasNotWrittenReviewException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Customer has not written review",
                    Detail = exception.Message,
                    Extensions = new Dictionary<string, object?>
                    {
                        { "WriterId", customerHasNotWrittenReviewException.WriterId },
                        { "ProductSku", customerHasNotWrittenReviewException.ProductSku }
                    }
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