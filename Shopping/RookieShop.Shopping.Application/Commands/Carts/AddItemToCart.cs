using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Services;

namespace RookieShop.Shopping.Application.Commands.Carts;

public class AddItemToCart
{
    public Guid Id { get; init; }

    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class AddItemToCartConsumer : ICommandConsumer<AddItemToCart>
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly IStockItemRepository _stockItemRepository;
    private readonly CartService _cartService;
    private readonly TimeProvider _timeProvider;
    private readonly IShoppingOptionsProvider _shoppingOptionsProvider;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IExpireCartScheduler _expireCartScheduler;

    public AddItemToCartConsumer(CartRepositoryHelper cartRepositoryHelper, IStockItemRepository stockItemRepository,
        CartService cartService, TimeProvider timeProvider, IShoppingOptionsProvider shoppingOptionsProvider,
        DomainEventPublisher domainEventPublisher, IExpireCartScheduler expireCartScheduler)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _stockItemRepository = stockItemRepository;
        _cartService = cartService;
        _timeProvider = timeProvider;
        _shoppingOptionsProvider = shoppingOptionsProvider;
        _domainEventPublisher = domainEventPublisher;
        _expireCartScheduler = expireCartScheduler;
    }
    
    public async Task ConsumeAsync(AddItemToCart message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        _cartService.AddItemToCart(cart, stockItem, message.Quantity);
        
        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
        
        _expireCartScheduler.EnqueueSchedule(message.Id, _timeProvider.GetUtcNow().AddMinutes(_shoppingOptionsProvider.CartLifeTimeInMinutes));
    }
}