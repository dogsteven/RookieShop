using MassTransit;
using RookieShop.Ordering.Application.Commands;
using RookieShop.Ordering.Application.Events.DomainEventConsumers;
using RookieShop.Ordering.Application.Events.IntegrationEventConsumers;

namespace RookieShop.Ordering.Infrastructure.Configurations;

public static class OrderingMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddOrderingConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<PlaceOrderConsumer, PlaceOrderConsumerDefinition>();
        
        bus.AddConsumer<PublishIntegrationEventOnOrderPlacedConsumer, PublishIntegrationEventOnOrderPlacedConsumerDefinition>();
        bus.AddConsumer<PublishIntegrationEventOnOrderCancelledConsumer, PublishIntegrationEventOnOrderCancelledConsumerDefinition>();
        bus.AddConsumer<PublishIntegrationEventOnOrderCompletedConsumer, PublishIntegrationEventOnOrderCompletedConsumerDefinition>();

        bus.AddConsumer<CheckoutSessionCompletedConsumer, CheckoutSessionCompletedConsumerDefinition>();
        
        return bus;
    }

    public static IMediatorRegistrationConfigurator AddOrderingConsumers(this IMediatorRegistrationConfigurator mediator)
    {
        mediator.AddConsumer<CancelOrderConsumer>();
        mediator.AddConsumer<CompleteOrderConsumer>();
        
        return mediator;
    }
}

public class PlaceOrderConsumerDefinition : ConsumerDefinition<PlaceOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PlaceOrderConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class PublishIntegrationEventOnOrderPlacedConsumerDefinition : ConsumerDefinition<PublishIntegrationEventOnOrderPlacedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PublishIntegrationEventOnOrderPlacedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class PublishIntegrationEventOnOrderCancelledConsumerDefinition : ConsumerDefinition<PublishIntegrationEventOnOrderCancelledConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PublishIntegrationEventOnOrderCancelledConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class PublishIntegrationEventOnOrderCompletedConsumerDefinition : ConsumerDefinition<PublishIntegrationEventOnOrderCompletedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PublishIntegrationEventOnOrderCompletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class CheckoutSessionCompletedConsumerDefinition : ConsumerDefinition<CheckoutSessionCompletedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CheckoutSessionCompletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}