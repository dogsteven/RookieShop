using RookieShop.Shopping.Domain.CheckoutSessions;

namespace RookieShop.Shopping.Application.Abstractions.Repositories;

public interface ICheckoutSessionRepository
{
    public Task<CheckoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public void Add(CheckoutSession checkoutSession);
}