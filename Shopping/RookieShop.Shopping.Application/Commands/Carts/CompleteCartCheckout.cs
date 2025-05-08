using MassTransit;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Messages;
using RookieShop.Shopping.Application.Utilities;

namespace RookieShop.Shopping.Application.Commands.Carts;

public class CompleteCartCheckout
{
    public Guid Id { get; init; }
}

public class CompleteCartCheckoutConsumer : ICommandConsumer<CompleteCartCheckout>, IConsumer<CompleteCartCheckout>
{
    private readonly CartRepositoryHelper _cartRepositoryHelper;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteCartCheckoutConsumer(CartRepositoryHelper cartRepositoryHelper, IUnitOfWork unitOfWork)
    {
        _cartRepositoryHelper = cartRepositoryHelper;
        _unitOfWork = unitOfWork;
    }
    
    public async Task ConsumeAsync(CompleteCartCheckout message, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepositoryHelper.GetOrCreateCartAsync(message.Id, cancellationToken);
        
        cart.CompleteCheckout();
    }


    public async Task Consume(ConsumeContext<CompleteCartCheckout> context)
    {
        var message = context.Message;
        
        var cancellationToken = context.CancellationToken;

        await ConsumeAsync(message, cancellationToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}