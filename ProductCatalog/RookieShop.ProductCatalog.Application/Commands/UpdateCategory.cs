using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Exceptions;

namespace RookieShop.ProductCatalog.Application.Commands;

public class UpdateCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}

public class UpdateCategoryConsumer : IConsumer<UpdateCategory>
{
    private readonly ProductCatalogDbContext _dbContext;

    public UpdateCategoryConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<UpdateCategory> context)
    {
        var message = context.Message;
        var id = message.Id;
        var name = message.Name;
        var description = message.Description;
        
        var cancellationToken = context.CancellationToken;

        var alreadyExists = await _dbContext.Categories.AnyAsync(category => category.Name == name, cancellationToken);

        if (alreadyExists)
        {
            throw new CategoryAlreadyExistsException(name);
        }
        
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException(id);
        }
        
        category.Name = name;
        category.Description = description;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}