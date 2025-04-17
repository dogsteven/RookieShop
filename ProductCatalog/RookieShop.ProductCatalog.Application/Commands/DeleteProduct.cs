using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.ProductCatalog.Application.Commands;

public class DeleteProduct
{
    public string Sku { get; set; } = null!;
}

public class DeleteProductConsumer : IConsumer<DeleteProduct>
{
    private readonly ProductCatalogDbContext _dbContext;

    public DeleteProductConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<DeleteProduct> context)
    {
        var sku = context.Message.Sku;
        
        var cancellationToken = context.CancellationToken;
        
        var product = await _dbContext.Products.Include(product => product.Rating)
            .FirstOrDefaultAsync(product => product.Sku == sku, cancellationToken);

        if (product == null)
        {
            throw new ProductNotFoundException(sku);
        }
        
        _dbContext.Products.Remove(product);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}