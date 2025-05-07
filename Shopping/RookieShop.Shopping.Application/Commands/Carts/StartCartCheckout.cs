using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.Carts;

public class StartCartCheckout
{
    public Guid Id { get; init; }
}

public class StartCartCheckoutConsumer : ICommandConsumer<StartCartCheckout>
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly DomainEventPublisher _domainEventPublisher;

    public StartCartCheckoutConsumer(CartRepositoryHelper cartRepositoryHelper,
        DomainEventPublisher domainEventPublisher)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(StartCartCheckout message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        cart.Close();
        
        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
    }
}