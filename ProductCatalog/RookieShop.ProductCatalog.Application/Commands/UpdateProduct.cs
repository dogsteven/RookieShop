using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.ProductCatalog.Application.Commands;

public class UpdateProduct
{
    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public decimal Price { get; set; }
    
    public int CategoryId { get; set; }

    public Guid PrimaryImageId { get; set; }

    public ISet<Guid> SupportingImageIds { get; set; } = null!;
    
    public bool IsFeatured { get; set; }
}

public class UpdateProductConsumer : IConsumer<UpdateProduct>
{
    private readonly ProductCatalogDbContext _dbContext;

    public UpdateProductConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<UpdateProduct> context)
    {
        var message = context.Message;
        var sku = message.Sku;
        var name = message.Name;
        var description = message.Description;
        var price = message.Price;
        var categoryId = message.CategoryId;
        var primaryImageId = message.PrimaryImageId;
        var supportingImageIds = message.SupportingImageIds;
        var isFeatured = message.IsFeatured;
        
        var cancellationToken = context.CancellationToken;
        
        var product = await _dbContext.Products.Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.Sku == sku, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(sku);
        }

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.PrimaryImageId = primaryImageId;
        product.SupportingImageIds.Clear();
        product.SupportingImageIds.AddRange(supportingImageIds);
        product.IsFeatured = isFeatured;
        product.UpdatedDate = DateTime.UtcNow;

        if (product.Category.Id != categoryId)
        {
            var newCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken);

            if (newCategory == null)
            {
                throw new CategoryNotFoundException(categoryId);
            }
            
            product.Category = newCategory;
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}