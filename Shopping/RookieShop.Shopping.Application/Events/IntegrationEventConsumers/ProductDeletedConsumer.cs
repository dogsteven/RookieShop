using MassTransit;
using RookieShop.ProductCatalog.Contracts.Events;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Application.Exceptions;

namespace RookieShop.Shopping.Application.Events.IntegrationEventConsumers;

public class ProductDeletedConsumer : IConsumer<ProductDeleted>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductDeletedConsumer(IStockItemRepository stockItemRepository, IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<ProductDeleted> context)
    {
        var message = context.Message;
        var sku = message.Sku;
        
        var cancellationToken = context.CancellationToken;
        
        var stockItem = await _stockItemRepository.GetBySkuAsync(sku, cancellationToken);

        if (stockItem == null)
        {
            throw new StockItemNotFoundException(sku);
        }
        
        _stockItemRepository.Remove(stockItem);
        
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}