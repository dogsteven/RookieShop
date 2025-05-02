namespace RookieShop.Shopping.Application.Abstractions.Messages;

public interface IExternalMessageDispatcher
{
    public void EnqueueScheduleSend(object message, DateTimeOffset scheduledTime);
    public void EnqueuePublish(object message);
}