using Shared;
using Shared.Models;

namespace Shared;

public class OrderCreatedEvent : IOrderCreatedEvent
{
    public OrderCreatedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    public List<OrderItemMessageDto> OrderItemMessages { get; set; }

    public Guid CorrelationId { get; }
}
