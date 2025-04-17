using MassTransit;
using RookieShop.ProductReview.Application.Commands;

namespace RookieShop.ProductReview.Infrastructure.Configurations;

public static class MassTransitConfigurationExtensions
{
    public static IMediatorRegistrationConfigurator AddProductReviewConsumers(
        this IMediatorRegistrationConfigurator mediator)
    {
        mediator.AddConsumer<WriteReviewConsumer>();
        mediator.AddConsumer<MakeReactionConsumer>();
        
        return mediator;
    }
}