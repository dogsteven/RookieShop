namespace RookieShop.Shopping.Application.Abstractions.Messages;

public interface IIntegrationEventPublisher
{
    public void Enqueue(object integrationEvent);
}