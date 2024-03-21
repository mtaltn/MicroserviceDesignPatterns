using Shared.Models;

namespace Shared;

public class PaymentFailedEvent
{
    public int OrderId { get; set; }
    public string BuyerId { get; set; }
    public string Message { get; set; }
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
}
