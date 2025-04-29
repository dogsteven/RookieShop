namespace RookieShop.Shopping.Application.Abstractions;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken = default);
}