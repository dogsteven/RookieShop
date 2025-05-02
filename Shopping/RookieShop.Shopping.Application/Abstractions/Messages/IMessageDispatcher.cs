namespace RookieShop.Shopping.Application.Abstractions.Messages;

public interface IMessageDispatcher
{
    public Task PublishAsync(object message, CancellationToken cancellationToken = default);
}