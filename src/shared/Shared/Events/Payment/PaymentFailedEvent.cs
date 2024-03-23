using Shared.Models;

namespace Shared;

public class PaymentFailedEvent : IPaymentFailedEvent
{
    public PaymentFailedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
    public string Reason { get; set; }

    public Guid CorrelationId { get; }
}
