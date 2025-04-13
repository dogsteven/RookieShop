using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.Application.Models;
using RookieShop.Application.Services;

namespace RookieShop.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Produces("application/problem+json")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetProductBySkuAsync(sku, cancellationToken));
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ProductDto>))]
    public async Task<ActionResult<Pagination<ProductDto>>> GetProductsAsync(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetProductsAsync(pageNumber ?? 1, pageSize ?? 20, cancellationToken));
    }
    
    [HttpGet("featured")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ProductDto>))]
    public async Task<ActionResult<Pagination<ProductDto>>> GetFeaturedProductsAsync(
        [FromQuery] int? maxCount,
        CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetFeaturedProductsAsync(maxCount ?? 12, cancellationToken));
    }
    
    [HttpGet("by-category/{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ProductDto>))]
    public async Task<ActionResult<Pagination<ProductDto>>> GetProductsByCategoryAsync(
        [FromRoute] int categoryId,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetProductsByCategoryAsync(categoryId, pageNumber ?? 1, pageSize ?? 20, cancellationToken));
    }

    public class CreateProductBody
    {
        [Required, MinLength(1), MaxLength(16)]
        public string Sku { get; set; }
        
        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; }
        
        [Required, MaxLength(500)]
        public string Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required, Url, MinLength(1), MaxLength(250)]
        public string ImageUrl { get; set; }
        
        [Required]
        public bool IsFeatured { get; set; }
        
#pragma warning disable CS8618, CS9264
        public CreateProductBody() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> CreateProductAsync([FromBody] CreateProductBody body,
        CancellationToken cancellationToken)
    {
        await _productService.CreateProductAsync(body.Sku, body.Name, body.Description, body.Price,
            body.CategoryId, body.ImageUrl, body.IsFeatured, cancellationToken);

        return Created();
    }
    
    public class UpdateProductBody
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; }
        
        [Required, MaxLength(500)]
        public string Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required, Url, MinLength(1), MaxLength(250)]
        public string ImageUrl { get; set; }
        
        [Required]
        public bool IsFeatured { get; set; }
        
#pragma warning disable CS8618, CS9264
        public UpdateProductBody() {}
#pragma warning restore CS8618, CS9264
    }

    [HttpPut("{sku}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateProductAsync([FromRoute] string sku, [FromBody] UpdateProductBody body,
        CancellationToken cancellationToken)
    {
        await _productService.UpdateProductAsync(sku, body.Name, body.Description, body.Price, body.CategoryId,
            body.ImageUrl, body.IsFeatured, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{sku}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteProductAsync([FromRoute] string sku, CancellationToken cancellationToken)
    {
        await _productService.DeleteProductAsync(sku, cancellationToken);
        
        return NoContent();
    }
}