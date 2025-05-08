using MassTransit;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Events;
using RookieShop.ProductCatalog.Application.Events.IntegrationEventConsumers;
using RookieShop.ProductReview.Application.Commands;

namespace RookieShop.ProductCatalog.Infrastructure.Configurations;

public static class ProductCatalogMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddProductCatalogConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<ApplyScoreConsumer, ApplyScoreConsumerDefinition>();
        bus.AddConsumer<UpdateSemanticVectorConsumer, UpdateSemanticVectorConsumerDefinition>();
        bus.AddConsumer<PublishProductCreatedOrUpdatedIntegrationEventConsumer, PublishProductCreatedOrUpdatedIntegrationEventConsumerDefinition>();
        bus.AddConsumer<UpdateStockLevelConsumer, UpdateStockLevelConsumerDefinition>();
        bus.AddConsumer<CreatePurchaseConsumer, CreatePurchaseConsumerDefinition>();
        bus.AddConsumer<OrderCompletedConsumer, OrderCompletedConsumerDefinition>();

        return bus;
    }

    public static IMediatorRegistrationConfigurator AddProductCatalogConsumers(
        this IMediatorRegistrationConfigurator mediator)
    {
        mediator.AddConsumer<CreateProductConsumer>();
        mediator.AddConsumer<UpdateProductConsumer>();
        mediator.AddConsumer<DeleteProductConsumer>();
        
        mediator.AddConsumer<CreateCategoryConsumer>();
        mediator.AddConsumer<UpdateCategoryConsumer>();
        mediator.AddConsumer<DeleteCategoryConsumer>();

        mediator.AddConsumer<SubmitReviewConsumer>();
        mediator.AddConsumer<MakeReactionConsumer>();
        
        return mediator;
    }
}

public class ApplyScoreConsumerDefinition : ConsumerDefinition<ApplyScoreConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ApplyScoreConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class UpdateSemanticVectorConsumerDefinition : ConsumerDefinition<UpdateSemanticVectorConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateSemanticVectorConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(20, TimeSpan.FromMilliseconds(500)));
        
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

public class PublishProductCreatedOrUpdatedIntegrationEventConsumerDefinition : ConsumerDefinition<PublishProductCreatedOrUpdatedIntegrationEventConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PublishProductCreatedOrUpdatedIntegrationEventConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class UpdateStockLevelConsumerDefinition : ConsumerDefinition<UpdateStockLevelConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateStockLevelConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class CreatePurchaseConsumerDefinition : ConsumerDefinition<CreatePurchaseConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreatePurchaseConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class OrderCompletedConsumerDefinition : ConsumerDefinition<OrderCompletedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderCompletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}