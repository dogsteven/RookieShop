using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Models;
using RookieShop.FrontStore.Models.Products;
using RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;

namespace RookieShop.FrontStore.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IReviewService _reviewService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ICategoryService categoryService, IReviewService reviewService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _reviewService = reviewService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int? pageNumber, CancellationToken cancellationToken)
    {
        var productPage = await _productService.GetProductsAsync(int.Max(pageNumber ?? 1, 1), 8, cancellationToken);

        return View(new ProductsViewModel
        {
            ProductPage = productPage
        });
    }
    
    public async Task<IActionResult> ProductsByCategory(int id, int? pageNumber, CancellationToken cancellationToken)
    {
        var categoryTask = _categoryService.GetCategoryByIdAsync(id, cancellationToken);
        var productPageTask = _productService.GetProductsByCategoryIdAsync(id, int.Max(pageNumber ?? 1, 1), 12, cancellationToken);
        
        await Task.WhenAll(categoryTask, productPageTask);
        
        var category = categoryTask.Result;
        var productPage = productPageTask.Result;

        return View(new ProductsByCategoryViewModel
        {
            Category = category,
            ProductPage = productPage
        });
    }

    public async Task<IActionResult> ProductDetails(string id, int? pageNumber,
        CancellationToken cancellationToken)
    {
        var productTask = _productService.GetProductBySkuAsync(id, cancellationToken);
        var reviewPageTask = _reviewService.GetRatingsBySkuAsync(id, int.Max(pageNumber ?? 1, 1), 10, cancellationToken);
        
        await Task.WhenAll(productTask, reviewPageTask);
        
        var product = productTask.Result;
        var reviewPage = reviewPageTask.Result;

        return View(new ProductDetailsViewModel
        {
            Product = product,
            ReviewPage = reviewPage,
            Score = 1,
            Comment = "" 
        });
    }

    public async Task<IActionResult> ProductsSemantic(string? semantic, int? pageNumber, int? pageSize,
        CancellationToken cancellationToken)
    {
        semantic ??= "";
        var productPage = await _productService.GetProductsSemanticAsync(semantic, pageNumber ?? 1, pageSize ?? 12, cancellationToken);

        return View(new ProductsSemanticViewModel
        {
            Semantic = semantic,
            ProductPage = productPage
        });
    }

    public class SubmitReviewModel
    {
        [Required, Range(1, 5)]
        public int Score { get; set; }
    
        [Required, MinLength(1), MaxLength(250)]
        public string Comment { get; set; } = string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReview(string id, [FromForm] SubmitReviewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var productTask = _productService.GetProductBySkuAsync(id, cancellationToken);
            var reviewPageTask = _reviewService.GetRatingsBySkuAsync(id, 1, 10, cancellationToken);
        
            await Task.WhenAll(productTask, reviewPageTask);
        
            var product = productTask.Result;
            var reviewPage = reviewPageTask.Result;

            return View("ProductDetails", new ProductDetailsViewModel
            {
                Product = product,
                ReviewPage = reviewPage,
                Score = model.Score,
                Comment = model.Comment,
            });
        }
        
        await _reviewService.SubmitReviewAsync(id, model.Score, model.Comment, cancellationToken);
        
        return RedirectToAction("ProductDetails", new { id = id });
    }
}