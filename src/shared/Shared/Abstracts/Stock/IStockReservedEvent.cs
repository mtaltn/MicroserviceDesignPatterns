using MassTransit;
using Shared.Models;

namespace Shared;

public interface IStockReservedEvent : CorrelatedBy<Guid>
{
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
}
