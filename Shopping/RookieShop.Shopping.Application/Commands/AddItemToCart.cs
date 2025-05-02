using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class AddItemToCart
{
    public Guid Id { get; init; }

    public string Sku { get; init; } = null!;
    
    public int Quantity { get; init; }
}

public class AddItemToCartConsumer : ICommandConsumer<AddItemToCart>
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly ICartRepository _cartRepository;
    private readonly IStockItemRepository _stockItemRepository;
    private readonly TimeProvider _timeProvider;
    private readonly DomainEventPublisher _domainEventPublisher;

    public AddItemToCartConsumer(CartRepositoryHelper cartRepositoryHelper, ICartRepository cartRepository,
        TimeProvider timeProvider, IStockItemRepository stockItemRepository, DomainEventPublisher domainEventPublisher)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _cartRepository = cartRepository;
        _stockItemRepository = stockItemRepository;
        _timeProvider = timeProvider;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(AddItemToCart message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        var stockItem = await _stockItemRepository.GetBySkuAsync(message.Sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(message.Sku);
        }
        
        cart.AddItem(stockItem.Sku, stockItem.Name, stockItem.Price, stockItem.ImageId, message.Quantity);
        cart.ExtendExpiration(_timeProvider);
        
        _cartRepository.Save(cart);
        
        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
    }
}