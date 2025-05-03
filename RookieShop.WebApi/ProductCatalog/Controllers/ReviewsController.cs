using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.ViewModels;
using RookieShop.ProductReview.Application.Commands;
using RookieShop.Shared.Models;

namespace RookieShop.WebApi.ProductCatalog.Controllers;

[ApiController]
[Route("/product-catalog/api/reviews")]
[Produces("application/problem+json")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewQueryService _reviewQueryService;
    private readonly IScopedMediator _scopedMediator;

    public ReviewsController(ReviewQueryService reviewQueryService, IScopedMediator scopedMediator)
    {
        _reviewQueryService = reviewQueryService;
        _scopedMediator = scopedMediator;
    }
    
    [HttpGet("{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ReviewDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Pagination<ReviewDto>>> GetReviewsByProductSkuAsync(
        [FromRoute] string sku,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _reviewQueryService.GetReviewsByProductSkuAsync(sku, pageNumber ?? 1, pageSize ?? 5, cancellationToken));
    }
    
    public class SubmitReviewBody
    {
        [Required, Range(1, 5)]
        public int Score { get; set; }
        
        [Required, MinLength(1), MaxLength(500)]
        public string Comment { get; set; }
        
#pragma warning disable CS8618, CS9264
        public SubmitReviewBody() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPost("{sku}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult> SubmitReviewAsync(
        [FromRoute] string sku,
        [FromBody] SubmitReviewBody body,
        CancellationToken cancellationToken)
    {
        var customerId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var customerName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Empty";

        var submitReview = new SubmitReview
        {
            WriterId = customerId,
            WriterName = customerName,
            ProductSku = sku,
            Score = body.Score,
            Comment = body.Comment,
        };
        
        await _scopedMediator.Send(submitReview, cancellationToken);

        return Created();
    }

    public class MakeReactionBody
    {
        [Required]
        public ReviewReactionType Type { get; set; }
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
            Type = body.Type,
        };
        
        await _scopedMediator.Send(makeReaction, cancellationToken);
        
        return NoContent();
    }
}