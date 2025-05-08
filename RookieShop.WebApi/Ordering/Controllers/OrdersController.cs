using System.Security.Claims;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Ordering.Application.Commands;

namespace RookieShop.WebApi.Ordering.Controllers;

[ApiController]
[Route("/ordering/api/orders")]
[Produces("application/problem+json")]
public class OrdersController : ControllerBase
{
    private readonly IScopedMediator _scopedMediator;

    public OrdersController(IScopedMediator scopedMediator)
    {
        _scopedMediator = scopedMediator;
    }

    [HttpPut("{id:guid}/cancel")]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> CancelAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        await _scopedMediator.Send(new CancelOrder
        {
            Id = id,
            UserId = userId,
        }, cancellationToken);

        return NoContent();
    }
    
    [HttpPut("{id:guid}/complete")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> CompleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        await _scopedMediator.Send(new CompleteOrder 
        {
            Id = id,
        }, cancellationToken);

        return NoContent();
    }
}