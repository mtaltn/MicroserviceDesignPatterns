using MassTransit;
using Shared.Models;

namespace Shared;

public interface IOrderCreatedEvent : CorrelatedBy<Guid>
{
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
}
