using MassTransit;
using Quartz;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Commands;

namespace RookieShop.Shopping.Infrastructure.ClearCartScheduler;

public class QuartzClearCartScheduler : IClearCartScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzClearCartScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task ScheduleAsync(Guid id, DateTimeOffset scheduledTime, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var jobKey = new JobKey("ClearCart", "Shopping");
        var triggerKey = new TriggerKey($"ClearCart-{id}", "Shopping");

        if (await scheduler.CheckExists(triggerKey, cancellationToken))
        {
            await scheduler.UnscheduleJob(triggerKey, cancellationToken);
        }

        if (!await scheduler.CheckExists(jobKey, cancellationToken))
        {
            var jobDetail = JobBuilder.Create<ClearCartJob>()
                .WithIdentity(jobKey)
                .StoreDurably()
                .Build();
            
            await scheduler.AddJob(jobDetail, true, cancellationToken);
        }

        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .ForJob(jobKey)
            .UsingJobData("CartId", id.ToString())
            .StartAt(scheduledTime)
            .Build();

        await scheduler.ScheduleJob(trigger, cancellationToken);
    }
}

public class ClearCartJob : IJob
{
    private readonly IBusTopology _busTopology;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly TimeProvider _timeProvider;

    public ClearCartJob(IBus bus, ISendEndpointProvider sendEndpointProvider, TimeProvider timeProvider)
    {
        _busTopology = bus.Topology;
        _sendEndpointProvider = sendEndpointProvider;
        _timeProvider = timeProvider;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var id = new Guid(context.Trigger.JobDataMap.GetString("CartId")!);
        
        if (!_busTopology.TryGetPublishAddress(typeof(ClearCart), out var sendAddress))
        {
            return;
        }

        try
        {

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(sendAddress);

            await sendEndpoint.Send(new ClearCart
            {
                Id = id
            }, context.CancellationToken);
        }
        catch (Exception exception)
        {
            var newTrigger = TriggerBuilder.Create()
                .ForJob(context.JobDetail)
                .UsingJobData("CartId", id.ToString())
                .StartAt(_timeProvider.GetUtcNow().AddMinutes(1))
                .Build();
            
            await context.Scheduler.RescheduleJob(context.Trigger.Key, newTrigger, context.CancellationToken);
            
            throw new JobExecutionException(exception, refireImmediately: false);
        }
    }
}