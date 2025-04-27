using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RookieShop.ImageGallery.Application.Abstractions;
using RookieShop.ImageGallery.Application.Queries;
using RookieShop.ImageGallery.Infrastructure.Configurations;
using RookieShop.ImageGallery.Infrastructure.Persistence;

namespace RookieShop.ImageGallery.Test.Utilities;

public class ImageGalleryServiceCollection : ServiceCollection
{
    public readonly Guid Id;
    
    public ImageGalleryServiceCollection()
    {
        Id = Guid.NewGuid();
        
        this.AddMassTransitTestHarness(bus =>
        {
            bus.AddDelayedMessageScheduler();

            bus.AddImageGalleryConsumers();

            bus.AddMediator(mediator =>
            {
                mediator.AddImageGalleryConsumers();
            });
            
            bus.UsingInMemory((context, inMemoryBus) =>
            {
                inMemoryBus.UseDelayedMessageScheduler();
                
                inMemoryBus.ConfigureEndpoints(context);
            });
        });

        this.AddSingleton<Mock<ITemporaryStorage>>(_ => new Mock<ITemporaryStorage>());
        this.AddSingleton<ITemporaryStorage>(provider => provider.GetRequiredService<Mock<ITemporaryStorage>>().Object);

        this.AddSingleton<Mock<IPersistentStorage>>(_ => new Mock<IPersistentStorage>());
        this.AddSingleton<IPersistentStorage>(provider => provider.GetRequiredService<Mock<IPersistentStorage>>().Object);

        this.AddDbContext<ImageGalleryDbContextImpl>(options =>
        {
            options.UseInMemoryDatabase(Id.ToString());
        });

        this.AddScoped<ImageGalleryDbContext, ImageGalleryDbContextImpl>();

        this.AddScoped<ImageQueryService>();
    }
}