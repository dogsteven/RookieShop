using MassTransit;
using RookieShop.ImageGallery.Commands;
using RookieShop.ImageGallery.Events;

namespace RookieShop.ImageGallery.Infrastructure.Configurations;

public static class ImageGalleryMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddImageGalleryConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<UploadImageToStorageConsumer>((_, consumer) =>
        {
            consumer.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(1000)));
        });
        
        bus.AddConsumer<DeleteImageFromStorageConsumer>((_, consumer) =>
        {
            consumer.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(1000)));
        });

        return bus;
    }

    public static IMediatorRegistrationConfigurator AddImageGalleryConsumers(
        this IMediatorRegistrationConfigurator mediator)
    {
        mediator.AddConsumer<UploadImageConsumer>();
        mediator.AddConsumer<DeleteImageConsumer>();
        
        return mediator;
    }
}