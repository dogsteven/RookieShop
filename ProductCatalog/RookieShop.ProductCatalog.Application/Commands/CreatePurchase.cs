using MassTransit;
using Microsoft.EntityFrameworkCore;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Entities;

namespace RookieShop.ProductCatalog.Application.Commands;

public class CreatePurchase
{
    public Guid CustomerId { get; set; }

    public string ProductSku { get; set; } = null!;
}

public class CreatePurchaseConsumer : IConsumer<CreatePurchase>
{
    private readonly ProductCatalogDbContext _dbContext;

    public CreatePurchaseConsumer(ProductCatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Consume(ConsumeContext<CreatePurchase> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;
        
        var exists = await _dbContext.Purchases.AnyAsync(purchase => purchase.CustomerId == message.CustomerId && purchase.ProductSku == message.ProductSku, cancellationToken);

        if (exists)
        {
            return;
        }

        var purchase = new Purchase
        {
            CustomerId = message.CustomerId,
            ProductSku = message.ProductSku,
        };
        
        _dbContext.Purchases.Add(purchase);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}