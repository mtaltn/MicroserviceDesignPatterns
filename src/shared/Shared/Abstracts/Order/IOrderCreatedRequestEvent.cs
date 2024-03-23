using Shared.Models;

namespace Shared;

public interface IOrderCreatedRequestEvent
{
    public int OrderId { get; set; }
    public string BuyerId { get; set; }
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
    public PaymentMessageDto PaymentMessage { get; set; }
}
