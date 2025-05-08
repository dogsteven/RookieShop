namespace RookieShop.Ordering.Application.Abstractions;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default);
}