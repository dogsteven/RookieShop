using MassTransit;
using RookieShop.Ordering.Application.Abstractions;
using RookieShop.Ordering.Domain.Orders;
using RookieShop.Shared.Models;

namespace RookieShop.Ordering.Application.Commands;

public class PlaceOrder
{
    public Guid Id { get; init; }
    
    public Guid CustomerId { get; init; }

    public Address BillingAddress { get; init; } = null!;

    public Address ShippingAddress { get; init; } = null!;

    public IEnumerable<OrderItem> Items { get; init; } = null!;
}

public class PlaceOrderConsumer : IConsumer<PlaceOrder>
{
    private readonly TimeProvider _timeProvider;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PlaceOrderConsumer(TimeProvider timeProvider, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _timeProvider = timeProvider;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<PlaceOrder> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        var order = new Order(message.Id, message.CustomerId, message.BillingAddress, message.ShippingAddress, message.Items, _timeProvider);
        
        _orderRepository.Add(order);
        
        await _unitOfWork.CommitAsync(cancellationToken);
        
        foreach (var domainEvent in order.DomainEvents)
        {
            await context.Publish(domainEvent, cancellationToken); 
        }
    }
} 