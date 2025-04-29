using System.ComponentModel.DataAnnotations;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Events;
using RookieShop.ProductCatalog.Infrastructure.Persistence;

namespace RookieShop.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Produces("application/problem+json")]
public class SeedController : ControllerBase
{
    private readonly IScopedMediator _scopedMediator;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ProductCatalogDbContextImpl _dbContext;

    public SeedController(IScopedMediator scopedMediator, IPublishEndpoint publishEndpoint, ProductCatalogDbContextImpl dbContext)
    {
        _scopedMediator = scopedMediator;
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;
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
        
            [Required]
            public Guid PrimaryImageId { get; set; }
        
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
                PrimaryImageId = product.PrimaryImageId,
                IsFeatured = product.IsFeatured
            }, cancellationToken);
        }
    }

    [HttpGet("seed-products")]
    public async Task SeedSemanticVectorsAsync(CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products.AsNoTracking()
            .Select(product => new { Sku = product.Sku, Name = product.Name, Description = product.Description, Price = product.Price, PrimaryImageId = product.PrimaryImageId })
            .ToListAsync(cancellationToken);

        foreach (var product in products)
        {
            await _publishEndpoint.Publish(new ProductCreatedOrUpdated
            {
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                PrimaryImageId = product.PrimaryImageId,
            }, cancellationToken);
        }
    }
}