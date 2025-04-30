namespace RookieShop.Shopping.Application.Abstractions.Messages;

public interface IMessageConsumer<in TMessage>
{
    public Task ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}