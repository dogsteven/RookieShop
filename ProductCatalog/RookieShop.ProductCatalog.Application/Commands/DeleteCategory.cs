using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.ProductCatalog.Application.Commands;

public class DeleteCategory
{
    public int Id { get; set; }
}

public class DeleteCategoryConsumer : IConsumer<DeleteCategory>
{
    private readonly ProductCatalogDbContext _dbContext;

    public DeleteCategoryConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<DeleteCategory> context)
    {
        var message = context.Message;
        var id = message.Id;
        
        var cancellationToken = context.CancellationToken;
        
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException(id);
        }
        
        _dbContext.Categories.Remove(category);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}