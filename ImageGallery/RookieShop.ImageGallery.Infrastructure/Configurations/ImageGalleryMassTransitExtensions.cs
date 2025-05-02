using MassTransit;
using RookieShop.ImageGallery.Application.Commands;
using RookieShop.ImageGallery.Application.Events;

namespace RookieShop.ImageGallery.Infrastructure.Configurations;

public static class ImageGalleryMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddImageGalleryConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<SyncTemporaryEntryToPersistentStorageConsumer, SyncTemporaryEntryToPersistentStorageConsumerDefinition>();
        bus.AddConsumer<CleanUpTemporaryStorageOnSyncedConsumer, CleanUpTemporaryStorageOnSyncedConsumerDefinition>();
        bus.AddConsumer<CleanUpPersistentStorageOnDeletedConsumer, CleanUpPersistentStorageOnDeletedConsumerDefinition>();

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

public class
    SyncTemporaryEntryToPersistentStorageConsumerDefinition : ConsumerDefinition<
    SyncTemporaryEntryToPersistentStorageConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SyncTemporaryEntryToPersistentStorageConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(20, TimeSpan.FromMilliseconds(1000)));
        
        endpointConfigurator.UseKillSwitch(killSwitch =>
        {
            killSwitch
                .SetActivationThreshold(10)
                .SetTripThreshold(0.15)
                .SetRestartTimeout(m: 1)
                .SetTrackingPeriod(m: 1);
        });
    }
}

public class
    CleanUpTemporaryStorageOnSyncedConsumerDefinition : ConsumerDefinition<CleanUpTemporaryStorageOnSyncedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CleanUpTemporaryStorageOnSyncedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(1000)));
    }
}

public class
    CleanUpPersistentStorageOnDeletedConsumerDefinition : ConsumerDefinition<CleanUpPersistentStorageOnDeletedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CleanUpPersistentStorageOnDeletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(20, TimeSpan.FromMilliseconds(1000)));
        
        endpointConfigurator.UseKillSwitch(killSwitch =>
        {
            killSwitch
                .SetActivationThreshold(10)
                .SetTripThreshold(0.15)
                .SetRestartTimeout(m: 1)
                .SetTrackingPeriod(m: 1);
        });
    }
}