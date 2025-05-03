using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Queries;
using RookieShop.Shopping.Infrastructure.Messages;
using RookieShop.Shopping.ViewModels;

namespace RookieShop.WebApi.Shopping.Controllers;

[ApiController]
[Route("/shopping/api/carts")]
[Produces("application/problem+json")]
public class CartsController : ControllerBase
{
    private readonly TransactionalMessageDispatcher _dispatcher;
    private readonly ShoppingQueryService _shoppingQueryService;

    public CartsController(TransactionalMessageDispatcher dispatcher, ShoppingQueryService shoppingQueryService)
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
        
        return Ok(await _shoppingQueryService.GetCartByIdAsync(customerId, cancellationToken));
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

        await _dispatcher.SendAsync(new AddItemToCart
        {
            Id = customerId,
            Sku = body.Sku,
            Quantity = body.Quantity,
        }, cancellationToken);

        return NoContent();
    }

    public class AdjustItemQuantityBody
    {
        public IEnumerable<Adjustment> Adjustments { get; set; }

        public class Adjustment
        {
            [Required, MinLength(1), MaxLength(16)]
            public string Sku { get; set; }
            
            [Required, Range(0, int.MaxValue)]
            public int NewQuantity { get; set; }
            
#pragma warning disable CS8618, CS9264
            public Adjustment() {}
#pragma warning restore CS8618, CS9264
        }
        
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

        await _dispatcher.SendAsync(new AdjustItemQuantity
        {
            Id = customerId,
            Adjustments = body.Adjustments.Select(adjustment => new AdjustItemQuantity.Adjustment
            {
                Sku = adjustment.Sku,
                NewQuantity = adjustment.NewQuantity
            })
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

        await _dispatcher.SendAsync(new RemoveItemFromCart
        {
            Id = customerId,
            Sku = body.Sku,
        }, cancellationToken);

        return NoContent();
    }
}