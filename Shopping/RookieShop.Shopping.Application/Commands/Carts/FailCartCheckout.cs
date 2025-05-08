using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.Carts;

public class FailCartCheckout
{
    public Guid Id { get; init; }
}

public class FailCartCheckoutConsumer : ICommandConsumer<FailCartCheckout>, IConsumer<FailCartCheckout>
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public FailCartCheckoutConsumer(CartRepositoryHelper cartRepositoryHelper, DomainEventPublisher domainEventPublisher, IUnitOfWork unitOfWork)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _domainEventPublisher = domainEventPublisher;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ConsumeAsync(FailCartCheckout message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        cart.FailCheckout();
        
        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
    }

    public async Task Consume(ConsumeContext<FailCartCheckout> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;
        
        await ConsumeAsync(message, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}