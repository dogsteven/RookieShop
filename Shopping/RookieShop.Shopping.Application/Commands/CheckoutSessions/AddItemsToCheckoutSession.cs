using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Utilities;
using RookieShop.Shopping.Domain.Shared;

namespace RookieShop.Shopping.Application.Commands.CheckoutSessions;

public class AddItemsToCheckoutSession
{
    public Guid Id { get; init; }

    public IEnumerable<CheckoutItem> Items { get; init; } = null!;
}

public class AddItemsToCheckoutSessionConsumer : ICommandConsumer<AddItemsToCheckoutSession>
{
    private readonly CheckoutSessionRepositoryHelper _checkoutSessionRepositoryHelper;

    public AddItemsToCheckoutSessionConsumer(CheckoutSessionRepositoryHelper checkoutSessionRepositoryHelper)
    {
        _checkoutSessionRepositoryHelper = checkoutSessionRepositoryHelper;
    }
    
    public async Task ConsumeAsync(AddItemsToCheckoutSession message, CancellationToken cancellationToken = default)
    {
        var checkoutSession = await _checkoutSessionRepositoryHelper.GetOrCreateCheckoutSessionAsync(message.Id, cancellationToken);
        
        checkoutSession.AddItems(message.Items);
    }
}