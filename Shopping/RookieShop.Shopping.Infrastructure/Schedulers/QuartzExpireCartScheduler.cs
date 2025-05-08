using MassTransit;
using Quartz;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Application.Commands;
using RookieShop.Shopping.Application.Commands.Carts;
using RookieShop.Shopping.Infrastructure.Messages;

namespace RookieShop.Shopping.Infrastructure.Schedulers;

public class QuartzExpireCartScheduler : IExpireCartScheduler
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public QuartzExpireCartScheduler(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }

    public void EnqueueSchedule(Guid id, DateTimeOffset scheduledTime)
    {
        _externalMessageDispatcher.EnqueuePublish(new ScheduleExpireCart
        {
            Id = id,
            ScheduledTime = scheduledTime
        });
    }

    public void EnqueueUnschedule(Guid id)
    {
        _externalMessageDispatcher.EnqueuePublish(new UnscheduleExpireCart
        {
            Id = id
        });
    }
}

public class ScheduleExpireCart
{
    public Guid Id { get; init; }
    
    public DateTimeOffset ScheduledTime { get; init; }
}

public class UnscheduleExpireCart
{
    public Guid Id { get; init; }
}

public class ScheduleExpireCartConsumer : IConsumer<ScheduleExpireCart>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public ScheduleExpireCartConsumer(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }
    
    public async Task Consume(ConsumeContext<ScheduleExpireCart> context)
    {
        var message = context.Message;
        var id = message.Id;
        var scheduledTime = message.ScheduledTime;
        
        var cancellationToken = context.CancellationToken;
        
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var jobKey = new JobKey("ExpireCart", "Shopping");
        var triggerKey = new TriggerKey($"ExpireCart-{id}", "Shopping");

        if (await scheduler.CheckExists(triggerKey, cancellationToken))
        {
            await scheduler.UnscheduleJob(triggerKey, cancellationToken);
        }

        if (!await scheduler.CheckExists(jobKey, cancellationToken))
        {
            var jobDetail = JobBuilder.Create<ExpireCartJob>()
                .WithIdentity(jobKey)
                .StoreDurably()
                .Build();
            
            await scheduler.AddJob(jobDetail, true, cancellationToken);
        }

        var trigger = TriggerBuilder.Create()
            .WithIdentity(triggerKey)
            .ForJob(jobKey)
            .UsingJobData("Id", id.ToString())
            .StartAt(scheduledTime)
            .Build();

        await scheduler.ScheduleJob(trigger, cancellationToken);
    }
}

public class UnscheduleExpireCartConsumer : IConsumer<UnscheduleExpireCart>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public UnscheduleExpireCartConsumer(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }
    
    public async Task Consume(ConsumeContext<UnscheduleExpireCart> context)
    {
        var message = context.Message;
        var id = message.Id;
        
        var cancellationToken = context.CancellationToken;
        
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var triggerKey = new TriggerKey($"ExpireCart-{id}", "Shopping");

        if (await scheduler.CheckExists(triggerKey, cancellationToken))
        {
            await scheduler.UnscheduleJob(triggerKey, cancellationToken);
        }
    }
}

public class ExpireCartJob : IJob
{
    private readonly IBusTopology _busTopology;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly TimeProvider _timeProvider;

    public ExpireCartJob(IBus bus, ISendEndpointProvider sendEndpointProvider, TimeProvider timeProvider)
    {
        _busTopology = bus.Topology;
        _sendEndpointProvider = sendEndpointProvider;
        _timeProvider = timeProvider;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var id = new Guid(context.Trigger.JobDataMap.GetString("Id")!);
        
        if (!_busTopology.TryGetPublishAddress(typeof(ExpireCart), out var sendAddress))
        {
            return;
        }

        try
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(sendAddress);

            await sendEndpoint.Send(new ExpireCart
            {
                Id = id
            }, context.CancellationToken);
        }
        catch (Exception exception)
        {
            var newTrigger = TriggerBuilder.Create()
                .WithIdentity(context.Trigger.Key)
                .ForJob(context.JobDetail)
                .UsingJobData("Id", id.ToString())
                .StartAt(_timeProvider.GetUtcNow().AddMinutes(1))
                .Build();
            
            await context.Scheduler.RescheduleJob(context.Trigger.Key, newTrigger, context.CancellationToken);
            
            throw new JobExecutionException(exception, refireImmediately: false);
        }
    }
}