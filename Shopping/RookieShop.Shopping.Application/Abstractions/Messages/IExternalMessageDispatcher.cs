namespace RookieShop.Shopping.Application.Abstractions.Messages;

public interface IExternalMessageDispatcher
{
    public void EnqueuePublish(object message);
}