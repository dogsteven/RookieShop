namespace RookieShop.Shopping.Application.Abstractions;

public interface IExpireCartScheduler
{
    public Task ScheduleAsync(Guid id, DateTimeOffset scheduledTime, CancellationToken cancellationToken = default);
}