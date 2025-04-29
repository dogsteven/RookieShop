namespace RookieShop.Shopping.Application.Abstractions;

public interface IMessageConsumer<in TMessage>
{
    public Task ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}