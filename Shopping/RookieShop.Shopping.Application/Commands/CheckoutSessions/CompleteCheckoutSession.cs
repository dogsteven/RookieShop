using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.CheckoutSessions;

public class CompleteCheckoutSession
{
    public Guid Id { get; set; }
}

public class CompleteCheckoutSessionConsumer : ICommandConsumer<CompleteCheckoutSession>
{
    private readonly CheckoutSessionRepositoryHelper _checkoutSessionRepositoryHelper;
    private readonly DomainEventPublisher _domainEventPublisher;

    public CompleteCheckoutSessionConsumer(CheckoutSessionRepositoryHelper checkoutSessionRepositoryHelper, DomainEventPublisher domainEventPublisher)
    {
        _checkoutSessionRepositoryHelper = checkoutSessionRepositoryHelper;
        _domainEventPublisher = domainEventPublisher;
    }
    
    public async Task ConsumeAsync(CompleteCheckoutSession message, CancellationToken cancellationToken = default)
    {
        var checkoutSession = await _checkoutSessionRepositoryHelper.GetOrCreateCheckoutSessionAsync(message.Id, cancellationToken);
        
        checkoutSession.Complete();

        await _domainEventPublisher.PublishAsync(checkoutSession, cancellationToken);
    }
}