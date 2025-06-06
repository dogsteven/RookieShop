using System.ComponentModel.DataAnnotations;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.ViewModels;
using RookieShop.Shared.Models;

namespace RookieShop.WebApi.ProductCatalog.Controllers;

[ApiController]
[Route("/product-catalog/api/products")]
[Produces("application/problem+json")]
public class ProductsController : ControllerBase
{
    private readonly ProductQueryService _productQueryService;
    private readonly IScopedMediator _scopedMediator;

    public ProductsController(ProductQueryService productQueryService, IScopedMediator scopedMediator)
    {
        _productQueryService = productQueryService;
        _scopedMediator = scopedMediator;
    }

    [HttpGet("{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProductBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        return Ok(await _productQueryService.GetProductBySkuAsync(sku, cancellationToken));
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ProductDto>))]
    public async Task<ActionResult<Pagination<ProductDto>>> GetProductsAsync(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        return Ok(await _productQueryService.GetProductsAsync(pageNumber ?? 1, pageSize ?? 20, cancellationToken));
    }
    
    [HttpGet("featured")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ProductDto>))]
    public async Task<ActionResult<Pagination<ProductDto>>> GetFeaturedProductsAsync(
        [FromQuery] int? maxCount,
        CancellationToken cancellationToken)
    {
        return Ok(await _productQueryService.GetFeaturedProductsAsync(maxCount ?? 12, cancellationToken));
    }
    
    [HttpGet("by-category/{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ProductDto>))]
    public async Task<ActionResult<Pagination<ProductDto>>> GetProductsByCategoryAsync(
        [FromRoute] int categoryId,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        return Ok(await _productQueryService.GetProductsByCategoryAsync(categoryId, pageNumber ?? 1, pageSize ?? 20, cancellationToken));
    }

    [HttpGet("semantic")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<ProductDto>))]
    public async Task<ActionResult<Pagination<ProductDto>>> GetSemanticallyOrderedProductsAsync(
        [FromQuery] string? semantic,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        return Ok(await _productQueryService.GetSemanticallyOrderedProductsAsync(semantic ?? "Nothing", pageNumber ?? 1, pageSize ?? 20, cancellationToken));
    }

    public class CreateProductBody
    {
        [Required, MinLength(1), MaxLength(16)]
        public string Sku { get; set; }
        
        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; }
        
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public Guid PrimaryImageId { get; set; }
        
        [Required]
        public ISet<Guid> SupportingImageIds { get; set; }
        
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
        var createProduct = new CreateProduct
        {
            Sku = body.Sku,
            Name = body.Name,
            Description = body.Description,
            Price = body.Price,
            CategoryId = body.CategoryId,
            PrimaryImageId = body.PrimaryImageId,
            SupportingImageIds = body.SupportingImageIds,
            IsFeatured = body.IsFeatured
        };

        await _scopedMediator.Send(createProduct, cancellationToken);

        return Created();
    }
    
    public class UpdateProductBody
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; }
        
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public Guid PrimaryImageId { get; set; }
        
        [Required]
        public ISet<Guid> SupportingImageIds { get; set; }
        
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
        var updateProduct = new UpdateProduct
        {
            Sku = sku,
            Name = body.Name,
            Description = body.Description,
            Price = body.Price,
            CategoryId = body.CategoryId,
            PrimaryImageId = body.PrimaryImageId,
            SupportingImageIds = body.SupportingImageIds,
            IsFeatured = body.IsFeatured
        };
        
        await _scopedMediator.Send(updateProduct, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{sku}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> DeleteProductAsync([FromRoute] string sku, CancellationToken cancellationToken)
    {
        var deleteProduct = new DeleteProduct
        {
            Sku = sku
        };
        
        await _scopedMediator.Send(deleteProduct, cancellationToken);
        
        return NoContent();
    }
}