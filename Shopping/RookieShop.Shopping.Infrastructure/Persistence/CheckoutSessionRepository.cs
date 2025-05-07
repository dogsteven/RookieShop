using Microsoft.EntityFrameworkCore;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Domain.CheckoutSessions;

namespace RookieShop.Shopping.Infrastructure.Persistence;

public class CheckoutSessionRepository : ICheckoutSessionRepository
{
    private readonly ShoppingDbContext _dbContext;

    public CheckoutSessionRepository(ShoppingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<CheckoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CheckoutSessions.FindAsync([id], cancellationToken: cancellationToken); 
    }

    public void Add(CheckoutSession checkoutSession)
    {
        _dbContext.CheckoutSessions.Add(checkoutSession);
    }
}