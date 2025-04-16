using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Application.Models;
using RookieShop.Application.Services;

namespace RookieShop.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Produces("application/problem+json")]
public class RatingController : ControllerBase
{
    private readonly RatingService _ratingService;

    public RatingController(RatingService ratingService)
    {
        _ratingService = ratingService;
    }
    
    [HttpGet("{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<RatingDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Pagination<RatingDto>>> GetRatings(
        [FromRoute] string sku,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _ratingService.GetRatingsBySkuAsync(sku, pageNumber ?? 1, pageSize ?? 10, cancellationToken));
    }

    public class WriteRatingBody
    {
        [Required, Range(0f, 5f)]
        public float Score { get; set; }
        
        [Required, MinLength(1), MaxLength(250)]
        public string Comment { get; set; }
        
#pragma warning disable CS8618, CS9264
        public WriteRatingBody() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPost("{sku}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> WriteRatingAsync(
        [FromRoute] string sku,
        [FromBody] WriteRatingBody body,
        CancellationToken cancellationToken)
    {
        var customerId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        await _ratingService.WriteRatingAsync(customerId, sku, body.Score, body.Comment, cancellationToken);

        return NoContent();
    }
}