namespace RookieShop.Shopping.Application.Abstractions.Schedulers;

public interface IExpireCartScheduler
{
    public void EnqueueSchedule(Guid id, DateTimeOffset scheduledTime);
    public void EnqueueUnschedule(Guid id);
}