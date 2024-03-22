using Shared.Models;

namespace Shared;

public interface IOrderCreatedEvent
{
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
}
