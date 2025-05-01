using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Infrastructure.Messages;

namespace RookieShop.WebApi.Shopping.Controllers;

[ApiController]
[Route("shopping/api/stock-items")]
[Produces("application/problem+json")]
public class StockItemsController : ControllerBase
{
    private readonly TransactionalMessageDispatcher _dispatcher;

    public StockItemsController(TransactionalMessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public class AddUnitsToStockItemBody
    {
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        public AddUnitsToStockItemBody() {}
    }

    [HttpPut("{sku}/add-units")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> AddUnitsToStockItemAsync(
        [FromRoute] string sku,
        [FromBody] AddUnitsToStockItemBody body,
        CancellationToken cancellationToken)
    {
        await _dispatcher.SendAsync(new AddUnitsToStockItem
        {
            Sku = sku,
            Quantity = body.Quantity
        }, cancellationToken);
        
        return NoContent();
    } 
}