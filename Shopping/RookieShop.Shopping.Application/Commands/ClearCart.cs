using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class ClearCart
{
    public Guid Id { get; set; }
}

public class ClearCartConsumer : IConsumer<ClearCart>
{
    private readonly ICartRepository _cartRepository;
    private readonly TimeProvider _timeProvider;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public ClearCartConsumer(ICartRepository cartRepository, TimeProvider timeProvider,
        DomainEventPublisher domainEventPublisher, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _timeProvider = timeProvider;
        _domainEventPublisher = domainEventPublisher;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<ClearCart> context)
    {
        var message = context.Message;

        var cancellationToken = context.CancellationToken;

        var cart = await _cartRepository.GetByIdAsync(message.Id, cancellationToken);

        if (cart == null)
        {
            return;
        }
        
        cart.Clear(_timeProvider);
        
        _cartRepository.Save(cart);

        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}