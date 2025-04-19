using System.ComponentModel.DataAnnotations;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RookieShop.ProductCatalog.Application.Commands;

namespace RookieShop.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Produces("application/problem+json")]
[Authorize(Roles = "admin")]
public class SeedController : ControllerBase
{
    private readonly IScopedMediator _scopedMediator;

    public SeedController(IScopedMediator scopedMediator)
    {
        _scopedMediator = scopedMediator;
    }
    
    public class SeedBody
    {
        public List<Category> Categories { get; set; } = null!;
        public List<Product> Products { get; set; } = null!;
        
        public class Category
        {
            [Required, MinLength(1), MaxLength(100)]
            public string Name { get; set; }
        
            [Required, MinLength(1), MaxLength(250)]
            public string Description { get; set; }
            
#pragma warning disable CS8618, CS9264
            public Category() {}
#pragma warning restore CS8618, CS9264
        }

        public class Product
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
        
            [Required, Url, MinLength(1), MaxLength(500)]
            public string ImageUrl { get; set; }
        
            [Required]
            public bool IsFeatured { get; set; }
            
#pragma warning disable CS8618, CS9264
            public Product() {}
#pragma warning restore CS8618, CS9264
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task SeedAsync([FromBody] SeedBody body, CancellationToken cancellationToken)
    {
        foreach (var category in body.Categories)
        {
            await _scopedMediator.Send(new CreateCategory
            {
                Name = category.Name,
                Description = category.Description
            }, cancellationToken);
        }

        foreach (var product in body.Products)
        {
            await _scopedMediator.Send(new CreateProduct
            {
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                IsFeatured = product.IsFeatured
            }, cancellationToken);
        }
    }
}