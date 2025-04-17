using MassTransit;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Application.Commands;

public class CreateCategory
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}

public class CreateCategoryResponse
{
    public int Id { get; set; }
}

public class CreateCategoryConsumer : IConsumer<CreateCategory>
{
    private readonly ProductCatalogDbContext _dbContext;

    public CreateCategoryConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<CreateCategory> context)
    {
        var message = context.Message;
        var name = message.Name;
        var description = message.Description;
        
        var cancellationToken = context.CancellationToken;
        
        var category = new Category
        {
            Name = name,
            Description = description,
        };
        
        await _dbContext.Categories.AddAsync(category, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        await context.RespondAsync(new CreateCategoryResponse
        {
            Id = category.Id
        });
    }
}