using MassTransit;
using RookieShop.ProductCatalog.Contracts.Events;
using RookieShop.Shopping.Application.Abstractions;
using RookieShop.Shopping.Application.Abstractions.Repositories;
using RookieShop.Shopping.Domain.StockItems;

namespace RookieShop.Shopping.Application.Events.IntegrationEventConsumers;

public class ProductCreatedOrUpdatedConsumer : IConsumer<ProductCreatedOrUpdated>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductCreatedOrUpdatedConsumer(IStockItemRepository stockItemRepository, IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Consume(ConsumeContext<ProductCreatedOrUpdated> context)
    {
        var message = context.Message;
        var sku = message.Sku;
        var name = message.Name;
        var price = message.Price;
        var imageId = message.PrimaryImageId;
        
        var cancellationToken = context.CancellationToken;
        
        var stockItem = await _stockItemRepository.GetBySkuAsync(sku, cancellationToken);

        if (stockItem == null)
        {
            stockItem = new StockItem(sku, name, price, imageId);
            
            _stockItemRepository.Save(stockItem);
        }
        else
        {
            stockItem.UpdateInfo(name, price, imageId);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}