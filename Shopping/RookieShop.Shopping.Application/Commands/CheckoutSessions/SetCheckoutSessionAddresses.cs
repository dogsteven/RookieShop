using RookieShop.Shared.Models;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.CheckoutSessions;

public class SetCheckoutSessionAddresses
{
    public Guid Id { get; init; }

    public Address BillingAddress { get; init; } = null!;

    public Address ShippingAddress { get; init; } = null!;
}

public class SetCheckoutSessionAddressesConsumer : ICommandConsumer<SetCheckoutSessionAddresses>
{
    private readonly CheckoutSessionRepositoryHelper _checkoutSessionRepositoryHelper;
    private readonly TimeProvider _timeProvider;
    private readonly IShoppingOptionsProvider _shoppingOptionsProvider;
    private readonly IExpireCheckoutSessionScheduler _expireCheckoutSessionScheduler;

    public SetCheckoutSessionAddressesConsumer(CheckoutSessionRepositoryHelper checkoutSessionRepositoryHelper,
        TimeProvider timeProvider, IShoppingOptionsProvider shoppingOptionsProvider,
        IExpireCheckoutSessionScheduler expireCheckoutSessionScheduler)
    {
        _checkoutSessionRepositoryHelper = checkoutSessionRepositoryHelper;
        _timeProvider = timeProvider;
        _shoppingOptionsProvider = shoppingOptionsProvider;
        _expireCheckoutSessionScheduler = expireCheckoutSessionScheduler;
    }
    
    public async Task ConsumeAsync(SetCheckoutSessionAddresses message, CancellationToken cancellationToken = default)
    {
        var checkoutSession = await _checkoutSessionRepositoryHelper.GetOrCreateCheckoutSessionAsync(message.Id, cancellationToken);
        
        checkoutSession.SetAddresses(message.BillingAddress, message.ShippingAddress);
        
        _expireCheckoutSessionScheduler.EnqueueSchedule(message.Id, _timeProvider.GetUtcNow().AddMinutes(_shoppingOptionsProvider.CheckoutSessionDurationInMinutes));
    }
}