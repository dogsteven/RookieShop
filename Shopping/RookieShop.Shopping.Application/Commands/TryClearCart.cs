using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class TryClearCart
{
    public Guid Id { get; set; }
}

public class TryClearCartConsumer : IConsumer<TryClearCart>
{
    private readonly ICartRepository _cartRepository;
    private readonly TimeProvider _timeProvider;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public TryClearCartConsumer(ICartRepository cartRepository, TimeProvider timeProvider,
        DomainEventPublisher domainEventPublisher, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _timeProvider = timeProvider;
        _domainEventPublisher = domainEventPublisher;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<TryClearCart> context)
    {
        var message = context.Message;

        var cancellationToken = context.CancellationToken;

        var cart = await _cartRepository.GetByIdAsync(message.Id, cancellationToken);

        if (cart == null)
        {
            return;
        }
        
        cart.TryClear(_timeProvider);

        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}