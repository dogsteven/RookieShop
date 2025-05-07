using MassTransit;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Commands.Carts;
using RookieShop.Shopping.Application.Commands.CheckoutSessions;
using RookieShop.Shopping.Application.Commands.StockItems;
using RookieShop.Shopping.Application.Events.IntegrationEventConsumers;
using RookieShop.Shopping.Infrastructure.Schedulers;

namespace RookieShop.Shopping.Infrastructure.Configurations;

public static class ShoppingMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddShoppingConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<ExpireCartConsumer, ExpireCardConsumerDefinition>();
        bus.AddConsumer<CompleteCartCheckoutConsumer, CompleteCartCheckoutConsumerDefinition>();
        bus.AddConsumer<FailCartCheckoutConsumer, FailCartCheckoutConsumerDefinition>();
        
        bus.AddConsumer<ReleaseStockReservationConsumer, ReleaseStockReservationConsumerDefinition>();
        bus.AddConsumer<ConfirmStockReservationConsumer, ConfirmStockReservationConsumerDefinition>();
        
        bus.AddConsumer<ExpireCheckoutSessionConsumer, ExpireCheckoutSessionConsumerDefinition>();
        
        bus.AddConsumer<ProductCreatedOrUpdatedConsumer, ProductCreatedOrUpdatedConsumerDefinition>();
        bus.AddConsumer<ProductDeletedConsumer, ProductDeletedConsumerDefinition>();
        
        bus.AddConsumer<ScheduleExpireCartConsumer, ScheduleExpireCartConsumerDefinition>();
        bus.AddConsumer<UnscheduleExpireCartConsumer, UnscheduleExpireCartConsumerDefinition>();
        bus.AddConsumer<ScheduleExpireCheckoutSessionConsumer, ScheduleExpireCheckoutSessionConsumerDefinition>();
        bus.AddConsumer<UnscheduleExpireCheckoutSessionConsumer, UnscheduleExpireCheckoutSessionConsumerDefinition>();
        
        return bus;
    }
}

public class ExpireCardConsumerDefinition : ConsumerDefinition<ExpireCartConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ExpireCartConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(500)));
    }
}

public class CompleteCartCheckoutConsumerDefinition : ConsumerDefinition<CompleteCartCheckoutConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CompleteCartCheckoutConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class FailCartCheckoutConsumerDefinition : ConsumerDefinition<FailCartCheckoutConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<FailCartCheckoutConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
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

public class ConfirmStockReservationConsumerDefinition : ConsumerDefinition<ConfirmStockReservationConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ConfirmStockReservationConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class ExpireCheckoutSessionConsumerDefinition : ConsumerDefinition<ExpireCheckoutSessionConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ExpireCheckoutSessionConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
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

public class ScheduleExpireCartConsumerDefinition : ConsumerDefinition<ScheduleExpireCartConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ScheduleExpireCartConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromSeconds(250)));
    }
}

public class UnscheduleExpireCartConsumerDefinition : ConsumerDefinition<UnscheduleExpireCartConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UnscheduleExpireCartConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class ScheduleExpireCheckoutSessionConsumerDefinition : ConsumerDefinition<ScheduleExpireCheckoutSessionConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ScheduleExpireCheckoutSessionConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}

public class UnscheduleExpireCheckoutSessionConsumerDefinition : ConsumerDefinition<UnscheduleExpireCheckoutSessionConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UnscheduleExpireCheckoutSessionConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
    }
}