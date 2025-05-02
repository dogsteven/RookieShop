using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Middlewares;
using RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;

namespace RookieShop.FrontStore.Controllers;

[TypeFilter(typeof(QueryCartActionFilter))]
public class ReviewsController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public class SubmitReviewForm
    {
        [Required, MinLength(1), MaxLength(16)]
        public string Sku { get; set; }
        
        [Required, Range(1, 5)]
        public int Score { get; set; }
        
        [Required, MinLength(1), MaxLength(250)]
        public string Comment { get; set; }
        
#pragma warning disable CS8618, CS9264
        public SubmitReviewForm() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> SubmitReview([FromForm] SubmitReviewForm form, CancellationToken cancellationToken)
    {
        await _reviewService.SubmitReviewAsync(form.Sku, form.Score, form.Comment, cancellationToken);
        
        return RedirectToAction("ProductDetails", "Products", new { id = form.Sku });
    }
}