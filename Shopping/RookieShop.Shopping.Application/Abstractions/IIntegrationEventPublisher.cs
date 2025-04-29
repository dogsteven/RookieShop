namespace RookieShop.Shopping.Application.Abstractions;

public interface IIntegrationEventPublisher
{
    public void Enqueue(object integrationEvent);
}