using MassTransit;
using Shared.Models;

namespace Shared;

public interface IPaymentFailedEvent : CorrelatedBy<Guid>
{
    public string Reason { get; set; }
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
}
