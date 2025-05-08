using MassTransit;
using RookieShop.Ordering.Application.Abstractions;
using RookieShop.Ordering.Application.Exceptions;

namespace RookieShop.Ordering.Application.Commands;

public class CancelOrder
{
    public Guid Id { get; init; }
}

public class CancelOrderConsumer : IConsumer<CancelOrder>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public CancelOrderConsumer(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<CancelOrder> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;
        
        var order = await _orderRepository.GetByIdAsync(message.Id, cancellationToken);

        if (order == null)
        {
            throw new OrderNotFoundException(message.Id);
        }
        
        order.Cancel();
        
        await _unitOfWork.CommitAsync(cancellationToken);
        
        foreach (var domainEvent in order.DomainEvents)
        {
            await _publishEndpoint.Publish(domainEvent, cancellationToken); 
        }
        
        order.ClearDomainEvents();
    }
}