using RookieShop.Shopping.Application.Abstractions;

namespace RookieShop.Shopping.Infrastructure.Messages;

public class TransactionalMessageDispatcher
{
    private readonly MessageDispatcher _dispatcher;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionalMessageDispatcher(MessageDispatcher dispatcher, IUnitOfWork unitOfWork)
    {
        _dispatcher = dispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task DispatchAsync(object message, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(message, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}