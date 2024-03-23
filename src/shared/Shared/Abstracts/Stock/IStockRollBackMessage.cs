using Shared.Models;

namespace Shared;

public interface IStockRollBackMessage
{
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
}
