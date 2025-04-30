using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class AdjustItemQuantity
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = null!;
    public int NewQuantity { get; init; }
}

public class AdjustItemQuantityConsumer : IMessageConsumer<AdjustItemQuantity>
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly ICartRepository _cartRepository;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public AdjustItemQuantityConsumer(CartRepositoryHelper cartRepositoryHelper, ICartRepository cartRepository,
        IDomainEventPublisher domainEventPublisher)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _cartRepository = cartRepository;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(AdjustItemQuantity message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        cart.AdjustItemQuantity(message.Sku, message.NewQuantity);
        
        _cartRepository.Save(cart);

        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
    }
}