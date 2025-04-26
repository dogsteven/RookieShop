using MassTransit;
using RookieShop.ImageGallery.Application.Commands;
using RookieShop.ImageGallery.Application.Events;

namespace RookieShop.ImageGallery.Infrastructure.Configurations;

public static class ImageGalleryMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddImageGalleryConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<SyncTemporaryEntryToPersistentStorageConsumer>((_, consumer) =>
        {
            consumer.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(1000)));
        });
        
        bus.AddConsumer<CleanUpTemporaryStorageOnSyncedConsumer>((_, consumer) =>
        {
            consumer.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(1000)));
        });
        
        bus.AddConsumer<CleanUpPersistentStorageOnDeletedConsumer>((_, consumer) =>
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