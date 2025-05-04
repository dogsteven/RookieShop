namespace RookieShop.Shopping.Application.Abstractions.Messages;

public interface IMessageDispatcher
{
    public Task SendAsync(object message, CancellationToken cancellationToken = default);
    public Task PublishAsync(object message, CancellationToken cancellationToken = default);
}