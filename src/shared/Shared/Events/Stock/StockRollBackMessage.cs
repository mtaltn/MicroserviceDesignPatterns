using Shared.Models;

namespace Shared;

public class StockRollBackMessage : IStockRollBackMessage
{
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
}
