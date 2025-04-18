using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ProductReview.Application.Commands;
using RookieShop.ProductReview.Application.Entities;
using RookieShop.ProductReview.Application.Models;
using RookieShop.ProductReview.Application.Queries;

namespace RookieShop.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Produces("application/problem+json")]
public class ReviewController : ControllerBase
{
    private readonly ReviewQueryService _reviewQueryService;
    private readonly IScopedMediator _scopedMediator;

    public ReviewController(ReviewQueryService reviewQueryService, IScopedMediator scopedMediator)
    {
        _reviewQueryService = reviewQueryService;
        _scopedMediator = scopedMediator;
    }
    
    [HttpGet("{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ReviewDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Pagination<ReviewDto>>> GetRatings(
        [FromRoute] string sku,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _reviewQueryService.GetReviewsByProductSku(sku, pageNumber ?? 1, pageSize ?? 5, cancellationToken));
    }
    
    public class WriteReviewBody
    {
        [Required, Range(1, 5)]
        public int Score { get; set; }
        
        [Required, MinLength(1), MaxLength(500)]
        public string Comment { get; set; }
        
#pragma warning disable CS8618, CS9264
        public WriteReviewBody() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPost("{sku}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> WriteReviewAsync(
        [FromRoute] string sku,
        [FromBody] WriteReviewBody body,
        CancellationToken cancellationToken)
    {
        var customerId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var writeReview = new WriteReview
        {
            WriterId = customerId,
            ProductSku = sku,
            Score = body.Score,
            Comment = body.Comment,
        };
        
        await _scopedMediator.Send(writeReview, cancellationToken);

        return NoContent();
    }

    public class MakeReactionBody
    {
        [Required]
        public ReactionType Type { get; set; }
    }

    [HttpPost("{sku}/{writerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> MakeReactionAsync(
        [FromRoute] string sku,
        [FromRoute] Guid writerId,
        [FromBody] MakeReactionBody body,
        CancellationToken cancellationToken)
    {
        var customerId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var makeReaction = new MakeReaction
        {
            ReactorId = customerId,
            WriterId = writerId,
            ProductSku = sku,
            ReactionType = body.Type,
        };
        
        await _scopedMediator.Send(makeReaction, cancellationToken);
        
        return NoContent();
    }
}