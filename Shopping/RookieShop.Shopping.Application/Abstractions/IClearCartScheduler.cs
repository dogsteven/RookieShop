namespace RookieShop.Shopping.Application.Abstractions;

public interface IClearCartScheduler
{
    public Task ScheduleAsync(Guid id, DateTimeOffset scheduledTime, CancellationToken cancellationToken = default);
}