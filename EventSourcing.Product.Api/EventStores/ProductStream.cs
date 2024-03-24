using EventSourcing.Product.Api.Models;
using EventSourcing.Shared;
using EventStore.ClientAPI;

namespace EventSourcing.Product.Api;

public class ProductStream(IEventStoreConnection eventStoreConnection) : AbstractStream(StreamName, eventStoreConnection)
{
    public static readonly string StreamName = "ProductStream";
    public static readonly string GroupName = "ProductStreamGroupOne";

    public void Created(CreateProductDto createProductDto)
    {
        Events.AddLast(new ProductCreatedEvent
        {
            Id = Guid.NewGuid(),
            Name = createProductDto.Name,
            Price = createProductDto.Price,
            Stock = createProductDto.Stock,
            UserId = createProductDto.UserId
        });
    }

    public void NameChanged(ChangeProductNameDto changeProductNameDto)
    {
        Events.AddLast(new ProductNameChangedEvent
        {
            ChangedName = changeProductNameDto.Name,
            Id = changeProductNameDto.Id
        });
    }

    public void PriceChanged(ChangeProductPriceDto changeProductPriceDto)
    {
        Events.AddLast(new ProductPriceChangedEvent
        {
            ChangedPrice = changeProductPriceDto.Price,
            Id = changeProductPriceDto.Id
        });
    }

    public void Deleted(Guid id)
    {
        Events.AddLast(new ProductDeletedEvent { Id = id });
    }
}

