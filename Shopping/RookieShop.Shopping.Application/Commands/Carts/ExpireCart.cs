using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.Carts;

public class ExpireCart
{
    public Guid Id { get; set; }
}

public class ExpireCartConsumer : ICommandConsumer<ExpireCart>, IConsumer<ExpireCart>
{
    private readonly ICartRepository _cartRepository;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public ExpireCartConsumer(ICartRepository cartRepository, DomainEventPublisher domainEventPublisher,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _domainEventPublisher = domainEventPublisher;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ConsumeAsync(ExpireCart message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByIdAsync(message.Id, cancellationToken);

        if (cart == null)
        {
            return;
        }
        
        cart.Expire();
        
        _cartRepository.Save(cart);

        await _domainEventPublisher.PublishAsync(cart, cancellationToken);
    }
    
    public async Task Consume(ConsumeContext<ExpireCart> context)
    {
        var message = context.Message;

        var cancellationToken = context.CancellationToken;

        await ConsumeAsync(message, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}