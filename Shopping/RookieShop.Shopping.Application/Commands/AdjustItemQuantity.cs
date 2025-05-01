using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class AdjustItemQuantity
{
    public Guid Id { get; init; }

    public IEnumerable<Adjustment> Adjustments { get; init; } = null!;

    public class Adjustment
    {
        public string Sku { get; init; } = null!;
        
        public int NewQuantity { get; init; }
    }
}

public class AdjustItemQuantityConsumer : ICommandConsumer<AdjustItemQuantity>
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

        foreach (var adjustment in message.Adjustments)
        {
            cart.AdjustItemQuantity(adjustment.Sku, adjustment.NewQuantity);
        }
        
        _cartRepository.Save(cart);

        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
    }
}