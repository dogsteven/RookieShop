using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Services;

namespace RookieShop.Shopping.Application.Commands;

public class RemoveItemFromCart
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = null!;
}

public class RemoveItemFromCartConsumer : ICommandConsumer<RemoveItemFromCart>
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly IStockItemRepository _stockItemRepository;
    private readonly CartService _cartService;
    private readonly TimeProvider _timeProvider;
    private readonly ICartOptionsProvider _cartOptionsProvider;
    private readonly DomainEventPublisher _domainEventPublisher;

    public RemoveItemFromCartConsumer(CartRepositoryHelper cartRepositoryHelper,
        IStockItemRepository stockItemRepository, CartService cartService, TimeProvider timeProvider,
        ICartOptionsProvider cartOptionsProvider, DomainEventPublisher domainEventPublisher)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _stockItemRepository = stockItemRepository;
        _cartService = cartService;
        _timeProvider = timeProvider;
        _cartOptionsProvider = cartOptionsProvider;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(RemoveItemFromCart message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }

        _cartService.RemoveItemFromCart(cart, stockItem);
        cart.ExtendExpiration(_timeProvider, _cartOptionsProvider.LifeTimeInMinutes);

        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
        await _domainEventPublisher.PublishAsync(stockItem, cancellationToken);
    }
}