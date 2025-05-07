using MassTransit;
using Quartz;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Abstractions.Schedulers;
using RookieShop.Shopping.Application.Commands.CheckoutSessions;

namespace RookieShop.Shopping.Infrastructure.Schedulers;

public class QuartzExpireCheckoutSessionScheduler : IExpireCheckoutSessionScheduler
{
    private readonly IExternalMessageDispatcher _externalMessageDispatcher;

    public QuartzExpireCheckoutSessionScheduler(IExternalMessageDispatcher externalMessageDispatcher)
    {
        _externalMessageDispatcher = externalMessageDispatcher;
    }

    public void EnqueueSchedule(Guid id, DateTimeOffset scheduledTime)
    {
        _externalMessageDispatcher.EnqueuePublish(new ScheduleExpireCheckoutSession
        {
            Id = id,
            ScheduledTime = scheduledTime
        });
    }

    public void EnqueueUnschedule(Guid id)
    {
        _externalMessageDispatcher.EnqueuePublish(new UnscheduleExpireCheckoutSession
        {
            Id = id
        });
    }
} 

public class ScheduleExpireCheckoutSession
{
    public Guid Id { get; init; }
    
    public DateTimeOffset ScheduledTime { get; init; }
}

public class UnscheduleExpireCheckoutSession
{
    public Guid Id { get; init; }
}

public class ScheduleExpireCheckoutSessionConsumer : IConsumer<ScheduleExpireCheckoutSession>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public ScheduleExpireCheckoutSessionConsumer(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }
    
    public async Task Consume(ConsumeContext<ScheduleExpireCheckoutSession> context)
    {
        var message = context.Message;
        var id = message.Id;
        var scheduledTime = message.ScheduledTime;
        
        var cancellationToken = context.CancellationToken;
        
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var jobKey = new JobKey("ExpireCheckoutSession", "Shopping");
        var triggerKey = new TriggerKey($"ExpireCheckoutSession-{id}", "Shopping");

        if (await scheduler.CheckExists(triggerKey, cancellationToken))
        {
            await scheduler.UnscheduleJob(triggerKey, cancellationToken);
        }

        if (!await scheduler.CheckExists(jobKey, cancellationToken))
        {
            var jobDetail = JobBuilder.Create<ExpireCheckoutSessionJob>()
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

public class UnscheduleExpireCheckoutSessionConsumer : IConsumer<UnscheduleExpireCheckoutSession>
{
    private readonly ISchedulerFactory _schedulerFactory;

    public UnscheduleExpireCheckoutSessionConsumer(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }
    
    public async Task Consume(ConsumeContext<UnscheduleExpireCheckoutSession> context)
    {
        var message = context.Message;
        var id = message.Id;
        
        var cancellationToken = context.CancellationToken;
        
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var triggerKey = new TriggerKey($"ExpireCheckoutSession-{id}", "Shopping");

        if (await scheduler.CheckExists(triggerKey, cancellationToken))
        {
            await scheduler.UnscheduleJob(triggerKey, cancellationToken);
        }
    }
}

public class ExpireCheckoutSessionJob : IJob
{
    private readonly IBusTopology _busTopology;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly TimeProvider _timeProvider;

    public ExpireCheckoutSessionJob(IBus bus, ISendEndpointProvider sendEndpointProvider, TimeProvider timeProvider)
    {
        _busTopology = bus.Topology;
        _sendEndpointProvider = sendEndpointProvider;
        _timeProvider = timeProvider;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var id = new Guid(context.Trigger.JobDataMap.GetString("Id")!);
        
        if (!_busTopology.TryGetPublishAddress(typeof(ExpireCheckoutSession), out var sendAddress))
        {
            return;
        }

        try
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(sendAddress);

            await sendEndpoint.Send(new ExpireCheckoutSession
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