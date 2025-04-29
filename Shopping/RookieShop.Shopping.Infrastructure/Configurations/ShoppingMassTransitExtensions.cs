using MassTransit;
using RookieShop.Shopping.Application.Events;
using RookieShop.Shopping.Application.Events.IntegrationEventConsumers;

namespace RookieShop.Shopping.Infrastructure.Configurations;

public static class ShoppingMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddShoppingConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<ProductCreatedOrUpdatedConsumer, ProductCreatedOrUpdatedConsumerDefinition>();
        bus.AddConsumer<ProductDeletedConsumer, ProductDeletedConsumerDefinition>();
        
        return bus;
    }
}

internal class ProductCreatedOrUpdatedConsumerDefinition : ConsumerDefinition<ProductCreatedOrUpdatedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ProductCreatedOrUpdatedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

internal class ProductDeletedConsumerDefinition : ConsumerDefinition<ProductDeletedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ProductDeletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}