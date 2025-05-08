using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.CheckoutSessions;

public class ExpireCheckoutSession
{
    public Guid Id { get; init; }
}

public class ExpireCheckoutSessionConsumer : ICommandConsumer<ExpireCheckoutSession>, IConsumer<ExpireCheckoutSession>
{
    private readonly CheckoutSessionRepositoryHelper _checkoutSessionRepositoryHelper;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public ExpireCheckoutSessionConsumer(CheckoutSessionRepositoryHelper checkoutSessionRepositoryHelper,
        TimeProvider timeProvider, DomainEventPublisher domainEventPublisher, IUnitOfWork unitOfWork)
    {
        _checkoutSessionRepositoryHelper = checkoutSessionRepositoryHelper;
        _domainEventPublisher = domainEventPublisher;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ConsumeAsync(ExpireCheckoutSession message, CancellationToken cancellationToken = default)
    {
        var checkoutSession = await _checkoutSessionRepositoryHelper.GetOrCreateCheckoutSessionAsync(message.Id, cancellationToken);
        
        checkoutSession.Expire();
        
        await _domainEventPublisher.PublishAsync(checkoutSession, cancellationToken);
    }
    
    public async Task Consume(ConsumeContext<ExpireCheckoutSession> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;
        
        await ConsumeAsync(message, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}