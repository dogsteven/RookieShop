using MassTransit;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Application.Events;

public class ProductCreatedOrUpdated
{
    public string Sku { get; init; } = null!;
    
    public string Name { get; init; } = null!;
    
    public string Description { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public Guid PrimaryImageId { get; init; }
}

public class UpdateSemanticVectorConsumer : IConsumer<ProductCreatedOrUpdated>
{
    private readonly ProductCatalogDbContext _dbContext;
    private readonly ISemanticEncoder _semanticEncoder;

    public UpdateSemanticVectorConsumer(ProductCatalogDbContext dbContext, ISemanticEncoder semanticEncoder)
    {
        _dbContext = dbContext;
        _semanticEncoder = semanticEncoder;
    }
    
    public async Task Consume(ConsumeContext<ProductCreatedOrUpdated> context)
    {
        var message = context.Message;
        var sku = message.Sku;
        var name = message.Name;
        var description = message.Description;
        
        var cancellationToken = context.CancellationToken;

        var semantic = $"{name} {description}";
        var semanticVector = new Vector(await _semanticEncoder.EncodeAsync(semantic, cancellationToken));
        
        var productSemanticVector = await _dbContext.ProductSemanticVectors
            .FirstOrDefaultAsync(productSemanticVector => productSemanticVector.ProductSku == sku, cancellationToken);

        if (productSemanticVector == null)
        {
            productSemanticVector = new ProductSemanticVector(sku);
            
            _dbContext.ProductSemanticVectors.Add(productSemanticVector);
        }
        
        productSemanticVector.SetSemanticVector(semanticVector);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

public class PublishProductCreatedOrUpdatedIntegrationEventConsumer : IConsumer<ProductCreatedOrUpdated>
{
    public async Task Consume(ConsumeContext<ProductCreatedOrUpdated> context)
    {
        var cancellationToken = context.CancellationToken;
        
        var integrationEvent = new Contracts.Events.ProductCreatedOrUpdated
        {
            Sku = context.Message.Sku,
            Name = context.Message.Name,
            Description = context.Message.Description,
            Price = context.Message.Price,
            PrimaryImageId = context.Message.PrimaryImageId,
        };
        
        await context.Publish(integrationEvent, cancellationToken);
    }
}