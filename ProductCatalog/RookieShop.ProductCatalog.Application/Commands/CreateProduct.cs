using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.ProductCatalog.Application.Commands;

public class CreateProduct
{
    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public decimal Price { get; set; }
    
    public int CategoryId { get; set; }

    public Guid PrimaryImageId { get; set; }
    
    public bool IsFeatured { get; set; }
}

public class CreateProductConsumer : IConsumer<CreateProduct>
{
    private readonly ProductCatalogDbContext _dbContext;

    public CreateProductConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<CreateProduct> context)
    {
        var message = context.Message;
        var sku = message.Sku;
        var name = message.Name;
        var description = message.Description;
        var price = message.Price;
        var categoryId = message.CategoryId;
        var primaryImageId = message.PrimaryImageId;
        var isFeatured = message.IsFeatured;
        
        var cancellationToken = context.CancellationToken;
        
        var alreadyExists = await _dbContext.Products.AnyAsync(product => product.Sku == sku, cancellationToken);

        if (alreadyExists)
        {
            throw new ProductAlreadyExistsException(sku);
        }
        
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException(categoryId);
        }

        var now = DateTime.UtcNow;

        var productRating = new Rating(sku);

        var product = new Product
        {
            Sku = sku,
            Name = name,
            Description = description,
            Price = price,
            Category = category,
            PrimaryImageId = primaryImageId,
            IsFeatured = isFeatured,
            CreatedDate = now,
            UpdatedDate = now,
            Rating = productRating
        };
        
        _dbContext.Products.Add(product);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}