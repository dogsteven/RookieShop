using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Models;
using RookieShop.Shopping.Application.Queries;
using RookieShop.Shopping.Infrastructure.MessageDispatcher;

namespace RookieShop.WebApi.Shopping.Controllers;

[ApiController]
[Route("/shopping/api/cart")]
[Produces("application/problem+json")]
public class CartsController : ControllerBase
{
    private readonly OptimisticScopedMessageDispatcher _dispatcher;
    private readonly ShoppingQueryService _shoppingQueryService;

    public CartsController(OptimisticScopedMessageDispatcher dispatcher, ShoppingQueryService shoppingQueryService)
    {
        _dispatcher = dispatcher;
        _shoppingQueryService = shoppingQueryService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartDto))]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<CartDto>> GetCartAsync(CancellationToken cancellationToken)
    {
        var customerId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        return await _shoppingQueryService.GetCartByIdAsync(customerId, cancellationToken);
    }

    public class AddItemToCartBody
    {
        [Required, MinLength(1), MaxLength(16)]
        public string Sku { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
#pragma warning disable CS8618, CS9264
        public AddItemToCartBody() {}
#pragma warning restore CS8618, CS9264
    }
    
    [HttpPut("add-item")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> AddItemToCartAsync([FromBody] AddItemToCartBody body, CancellationToken cancellationToken)
    {
        var customerId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _dispatcher.DispatchAsync(new AddItemToCart
        {
            Id = customerId,
            Sku = body.Sku,
            Quantity = body.Quantity,
        }, cancellationToken);

        return NoContent();
    }

    public class AdjustItemQuantityBody
    {
        [Required, MinLength(1), MaxLength(16)]
        public string Sku { get; set; }
        
        [Required, Range(1, int.MaxValue)]
        public int NewQuantity { get; set; }
        
#pragma warning disable CS8618, CS9264
        public AdjustItemQuantityBody() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPut("adjust-item-quantity")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> AdjustItemQuantityAsync([FromBody] AdjustItemQuantityBody body,
        CancellationToken cancellationToken)
    {
        var customerId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _dispatcher.DispatchAsync(new AdjustItemQuantity
        {
            Id = customerId,
            Sku = body.Sku,
            NewQuantity = body.NewQuantity,
        }, cancellationToken);
        
        return NoContent();
    }

    public class RemoveItemFromCartBody
    {
        [Required, MinLength(1), MaxLength(16)]
        public string Sku { get; set; }
        
#pragma warning disable CS8618, CS9264
        public RemoveItemFromCartBody() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPut("remove-item")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> RemoveItemFromCartAsync([FromBody] RemoveItemFromCartBody body,
        CancellationToken cancellationToken)
    {
        var customerId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _dispatcher.DispatchAsync(new RemoveItemFromCart
        {
            Id = customerId,
            Sku = body.Sku,
        }, cancellationToken);

        return NoContent();
    }
}