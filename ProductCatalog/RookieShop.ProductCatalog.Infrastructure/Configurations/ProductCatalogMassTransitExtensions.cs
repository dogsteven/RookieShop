using MassTransit;
using RookieShop.ProductCatalog.Application.Commands;
using RookieShop.ProductCatalog.Application.Events;
using RookieShop.ProductReview.Application.Commands;

namespace RookieShop.ProductCatalog.Infrastructure.Configurations;

public static class ProductCatalogMassTransitExtensions
{
    public static IBusRegistrationConfigurator AddProductCatalogConsumers(this IBusRegistrationConfigurator bus)
    {
        bus.AddConsumer<ApplyScoreConsumer>((_, consumer) =>
        {
            consumer.UseMessageRetry(retry => retry.Interval(10, TimeSpan.FromMilliseconds(250)));
        });

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