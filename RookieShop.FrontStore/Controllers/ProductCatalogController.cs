using Microsoft.AspNetCore.Mvc;
using RookieShop.FrontStore.Abstractions;
using RookieShop.FrontStore.Models;
using RookieShop.FrontStore.Models.ProductCatalog;

namespace RookieShop.FrontStore.Controllers;

public class ProductCatalogController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IReviewService _reviewService;

    public ProductCatalogController(IProductService productService, ICategoryService categoryService, IReviewService reviewService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Index(int? pageNumber, CancellationToken cancellationToken)
    {
        var productPage = await _productService.GetProductsAsync(int.Max(pageNumber ?? 1, 1), 12, cancellationToken);

        return View(new ProductCatalogViewModel
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

    public async Task<IActionResult> ProductDetails(string id, int? pageNumber, CancellationToken cancellationToken)
    {
        var sku = id;
        
        var productTask = _productService.GetProductBySkuAsync(sku, cancellationToken);
        var reviewPageTask = _reviewService.GetRatingsBySkuAsync(sku, int.Max(pageNumber ?? 1, 1), 10, cancellationToken);
        
        await Task.WhenAll(productTask, reviewPageTask);
        
        var product = productTask.Result;
        var reviewPage = reviewPageTask.Result;

        return View(new ProductDetailsViewModel
        {
            Product = product,
            ReviewPage = reviewPage
        });
    }
}