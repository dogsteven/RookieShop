using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Services;

namespace RookieShop.Shopping.Application.Commands.Carts;

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
    private readonly IStockItemRepository _stockItemRepository;
    private readonly CartService _cartService;
    private readonly TimeProvider _timeProvider;
    private readonly IShoppingOptionsProvider _shoppingOptionsProvider;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IExpireCartScheduler _expireCartScheduler;

    public AdjustItemQuantityConsumer(CartRepositoryHelper cartRepositoryHelper,
        IStockItemRepository stockItemRepository, CartService cartService, TimeProvider timeProvider,
        IShoppingOptionsProvider shoppingOptionsProvider, DomainEventPublisher domainEventPublisher,
        IExpireCartScheduler expireCartScheduler)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _stockItemRepository = stockItemRepository;
        _cartService = cartService;
        _timeProvider = timeProvider;
        _shoppingOptionsProvider = shoppingOptionsProvider;
        _domainEventPublisher = domainEventPublisher;
        _expireCartScheduler = expireCartScheduler;
    }
    
    public async Task ConsumeAsync(AdjustItemQuantity message, CancellationToken cancellationToken = default)
    {
        if (!message.Adjustments.Any())
        {
            return;
        }
        
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        foreach (var adjustment in message.Adjustments)
        {
            var stockItem = await _stockItemRepository.GetBySkuAsync(adjustment.Sku, cancellationToken);

            if (stockItem == null)
            {
                throw new StockItemNotFoundException(adjustment.Sku);
            }
            
            _cartService.AdjustItemQuantity(cart, stockItem, adjustment.NewQuantity);
            await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
        }
        
        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
        
        _expireCartScheduler.EnqueueSchedule(message.Id, _timeProvider.GetUtcNow().AddMinutes(_shoppingOptionsProvider.CartLifeTimeInMinutes));
    }
}