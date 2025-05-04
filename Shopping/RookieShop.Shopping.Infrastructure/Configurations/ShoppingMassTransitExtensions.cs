using MassTransit;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Events;
using RookieShop.Shopping.Application.Events.DomainEventConsumers;
using RookieShop.Shopping.Application.Events.IntegrationEventConsumers;

namespace RookieShop.Shopping.Infrastructure.Configurations;

public static class ShoppingMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddShoppingConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<ReleaseStockReservationConsumer, ReleaseStockReservationConsumerDefinition>();
        bus.AddConsumer<ClearCartConsumer, ClearCartConsumerDefinition>();
        bus.AddConsumer<ScheduleClearCartOnExpirationConsumer, ScheduleClearCartOnExpirationConsumerDefinition>();
        
        bus.AddConsumer<ProductCreatedOrUpdatedConsumer, ProductCreatedOrUpdatedConsumerDefinition>();
        bus.AddConsumer<ProductDeletedConsumer, ProductDeletedConsumerDefinition>();
        
        return bus;
    }
}

public class ReleaseStockReservationConsumerDefinition : ConsumerDefinition<ReleaseStockReservationConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ReleaseStockReservationConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry  => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class ClearCartConsumerDefinition : ConsumerDefinition<ClearCartConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ClearCartConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(500)));
    }
}

public class ScheduleClearCartOnExpirationConsumerDefinition : ConsumerDefinition<ScheduleClearCartOnExpirationConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ScheduleClearCartOnExpirationConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(2000)));
    }
}

public class ProductCreatedOrUpdatedConsumerDefinition : ConsumerDefinition<ProductCreatedOrUpdatedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ProductCreatedOrUpdatedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class ProductDeletedConsumerDefinition : ConsumerDefinition<ProductDeletedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ProductDeletedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}