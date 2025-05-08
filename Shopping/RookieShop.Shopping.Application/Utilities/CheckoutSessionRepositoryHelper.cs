using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Domain.CheckoutSessions;

namespace RookieShop.Shopping.Application.Utilities;

public class CheckoutSessionRepositoryHelper
{
    private readonly ICheckoutSessionRepository _checkoutSessionRepository;

    public CheckoutSessionRepositoryHelper(ICheckoutSessionRepository checkoutSessionRepository)
    {
        _checkoutSessionRepository = checkoutSessionRepository;
    }

    public async Task<CheckoutSession> GetOrCreateCheckoutSessionAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var checkoutSession = await _checkoutSessionRepository.GetByIdAsync(id, cancellationToken);

        if (checkoutSession != null)
            return checkoutSession;
        
        checkoutSession = new CheckoutSession(id);
        _checkoutSessionRepository.Add(checkoutSession);

        return checkoutSession;
    }
}