using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Domain;
using RookieShop.Shopping.Domain.Carts;
using RookieShop.Shopping.Domain.StockItems;

namespace RookieShop.WebApi.Shopping.ExceptionHandlers;

public class ShoppingExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ShoppingExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            case CartItemNotFoundException cartItemNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Cart item not found",
                    Detail = $"Cart item {cartItemNotFoundException.Sku} was not found in your cart.",
                    Extensions = new Dictionary<string, object?>
                    {
                        { "Id", cartItemNotFoundException.Id },
                        { "Sku", cartItemNotFoundException.Sku }
                    }
                };
                break;
            
            case InsufficientStockException notEnoughUnitsToReserveException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Not enough unit to reserve",
                    Detail = exception.Message,
                    Extensions = new Dictionary<string, object?>
                    {
                        { "Sku", notEnoughUnitsToReserveException.Sku },
                        { "Quantity", notEnoughUnitsToReserveException.Quantity }
                    }
                };
                break;
            
            case StockItemNotFoundException stockItemNotFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Stock item not found",
                    Detail = exception.Message,
                    Extensions = new Dictionary<string, object?>
                    {
                        { "Sku", stockItemNotFoundException.Sku }
                    }
                };
                break;
            
            default:
                return false;
        }

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        });
    }
}