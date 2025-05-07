using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.CheckoutSessions;

public class StartCheckoutSession
{
    public Guid Id { get; init; }
}

public class StartCheckoutSessionConsumer : ICommandConsumer<StartCheckoutSession>
{
    private readonly CheckoutSessionRepositoryHelper _checkoutSessionRepositoryHelper;
    private readonly TimeProvider _timeProvider;
    private readonly IShoppingOptionsProvider _shoppingOptionsProvider;
    private readonly DomainEventPublisher _domainEventPublisher;
    private readonly IExpireCheckoutSessionScheduler _expireCheckoutSessionScheduler;

    public StartCheckoutSessionConsumer(CheckoutSessionRepositoryHelper checkoutSessionRepositoryHelper,
        TimeProvider timeProvider, IShoppingOptionsProvider shoppingOptionsProvider,
        DomainEventPublisher domainEventPublisher, IExternalMessageDispatcher externalMessageDispatcher,
        IExpireCheckoutSessionScheduler expireCheckoutSessionScheduler)
    {
        _checkoutSessionRepositoryHelper = checkoutSessionRepositoryHelper;
        _timeProvider = timeProvider;
        _shoppingOptionsProvider = shoppingOptionsProvider;
        _domainEventPublisher = domainEventPublisher;
        _expireCheckoutSessionScheduler = expireCheckoutSessionScheduler;
    }
    
    public async Task ConsumeAsync(StartCheckoutSession message, CancellationToken cancellationToken = default)
    {
        var checkoutSession = await _checkoutSessionRepositoryHelper.GetOrCreateCheckoutSessionAsync(message.Id, cancellationToken);
        
        checkoutSession.Start();
        
        await _domainEventPublisher.PublishAsync(checkoutSession, cancellationToken);
        
        _expireCheckoutSessionScheduler.EnqueueSchedule(message.Id, _timeProvider.GetUtcNow().AddMinutes(_shoppingOptionsProvider.CheckoutSessionDurationInMinutes));
    }
}