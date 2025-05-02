using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class RemoveItemFromCart
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = null!;
}

public class RemoveItemFromCartConsumer : ICommandConsumer<RemoveItemFromCart>
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly ICartRepository _cartRepository;
    private readonly TimeProvider _timeProvider;
    private readonly DomainEventPublisher _domainEventPublisher;

    public RemoveItemFromCartConsumer(CartRepositoryHelper cartRepositoryHelper, ICartRepository cartRepository,
        TimeProvider timeProvider, DomainEventPublisher domainEventPublisher)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _cartRepository = cartRepository;
        _timeProvider = timeProvider;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(RemoveItemFromCart message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        cart.RemoveItem(message.Sku);
        cart.ExtendExpiration(_timeProvider);

        _cartRepository.Save(cart);

        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
    }
}