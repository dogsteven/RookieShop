using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.ProductCatalog.Infrastructure.Configurations;
using RookieShop.ProductCatalog.Infrastructure.SemanticEncoder;
using RookieShop.ProductCatalog.Test.Utilities.Persistence;

namespace RookieShop.ProductCatalog.Test.Utilities;

public class ProductCatalogServiceCollection : ServiceCollection
{
    public readonly Guid Id;
    
    public ProductCatalogServiceCollection()
    {
        Id = Guid.NewGuid();
        
        this.AddMassTransitTestHarness(bus =>
        {
            bus.AddDelayedMessageScheduler();

            bus.AddProductCatalogConsumers();

            bus.AddMediator(mediator =>
            {
                mediator.AddProductCatalogConsumers();
            });
            
            bus.UsingInMemory((context, inMemoryBus) =>
            {
                inMemoryBus.UseDelayedMessageScheduler();
                
                inMemoryBus.ConfigureEndpoints(context);
            });
        });
        
        this.AddSingleton<Mock<IProfanityChecker>>(_ => new Mock<IProfanityChecker>());
        this.AddSingleton<IProfanityChecker>(provider => provider.GetRequiredService<Mock<IProfanityChecker>>().Object);
        
        this.AddDbContext<ProductCatalogDbContextImpl>((options) =>
        {
            options.UseInMemoryDatabase(Id.ToString());
        });

        this.AddScoped<ProductCatalogDbContext, ProductCatalogDbContextImpl>();

        this.AddSingleton<ISemanticEncoder, LocalSemanticEncoder>();

        this.AddScoped<ProductQueryService>();
        this.AddScoped<CategoryQueryService>();
        this.AddScoped<ReviewQueryService>();
    }
}