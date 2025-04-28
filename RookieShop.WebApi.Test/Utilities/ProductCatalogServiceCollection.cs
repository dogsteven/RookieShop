using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ProductCatalog.Application.Abstractions;
using RookieShop.ProductCatalog.Application.Queries;
using RookieShop.WebApi.Controllers;
using RookieShop.WebApi.Test.Utilities.Persistence;

namespace RookieShop.WebApi.Test.Utilities;

public class ProductCatalogServiceCollection : ServiceCollection
{
    private readonly Guid Id;
    
    public ProductCatalogServiceCollection()
    {
        Id = Guid.NewGuid();
        
        this.AddDbContext<ProductCatalogDbContextImpl>((options) =>
        {
            options.UseInMemoryDatabase(Id.ToString());
        });

        this.AddScoped<ProductCatalogDbContext, ProductCatalogDbContextImpl>();
        
        this.AddScoped<Mock<IScopedMediator>>(_ => new Mock<IScopedMediator>());
        this.AddScoped<IScopedMediator>(provider => provider.GetRequiredService<Mock<IScopedMediator>>().Object);

        this.AddScoped<Mock<ISemanticEncoder>>(_ => new Mock<ISemanticEncoder>());
        this.AddScoped<ISemanticEncoder>(provider => provider.GetRequiredService<Mock<ISemanticEncoder>>().Object);
        
        this.AddScoped<Mock<ProductQueryService>>(provider =>
        {
            var productCatalogDbContext = provider.GetRequiredService<ProductCatalogDbContext>();
            var semanticEncoder = provider.GetRequiredService<ISemanticEncoder>();
            
            return new Mock<ProductQueryService>(productCatalogDbContext, semanticEncoder);
        });
        
        this.AddScoped<ProductQueryService>(provider => provider.GetRequiredService<Mock<ProductQueryService>>().Object);

        this.AddScoped<Mock<CategoryQueryService>>(provider =>
        {
            var productCatalogDbContext = provider.GetRequiredService<ProductCatalogDbContext>();
            
            return new Mock<CategoryQueryService>(productCatalogDbContext);
        });
        this.AddScoped<CategoryQueryService>(provider => provider.GetRequiredService<Mock<CategoryQueryService>>().Object);
        
        this.AddScoped<Mock<ReviewQueryService>>(provider =>
        {
            var productCatalogDbContext = provider.GetRequiredService<ProductCatalogDbContext>();
            
            return new Mock<ReviewQueryService>(productCatalogDbContext);
        });
        
        this.AddScoped<ReviewQueryService>(provider => provider.GetRequiredService<Mock<ReviewQueryService>>().Object);

        this.AddScoped<ProductController>();
        this.AddScoped<CategoryController>();
        this.AddScoped<ReviewController>();
    }
}