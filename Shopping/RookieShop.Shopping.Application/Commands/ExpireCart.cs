using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands;

public class ExpireCart
{
    public Guid Id { get; set; }
}

public class ExpireCartConsumer : IConsumer<ExpireCart>
{
    private readonly ICartRepository _cartRepository;
    private readonly TimeProvider _timeProvider;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public ExpireCartConsumer(ICartRepository cartRepository, TimeProvider timeProvider,
        DomainEventPublisher domainEventPublisher, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _timeProvider = timeProvider;
        _domainEventPublisher = domainEventPublisher;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<ExpireCart> context)
    {
        var message = context.Message;

        var cancellationToken = context.CancellationToken;

        var cart = await _cartRepository.GetByIdAsync(message.Id, cancellationToken);

        if (cart == null)
        {
            return;
        }
        
        cart.Expire(_timeProvider);
        
        _cartRepository.Save(cart);

        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}