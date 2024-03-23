using Shared.Models;

namespace Shared;

public class StockReservedEvent : IStockReservedEvent
{
    public StockReservedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }

    public Guid CorrelationId { get; }
}
