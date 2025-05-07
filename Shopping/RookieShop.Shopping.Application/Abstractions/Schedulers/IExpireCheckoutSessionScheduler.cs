namespace RookieShop.Shopping.Application.Abstractions.Schedulers;

public interface IExpireCheckoutSessionScheduler
{
    public void EnqueueSchedule(Guid id, DateTimeOffset scheduledTime);
    public void EnqueueUnschedule(Guid id);
}