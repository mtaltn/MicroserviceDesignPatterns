using Shared.Models;

namespace Shared.Events;

public class OrderCreatedRequestEvent : IOrderCreatedRequestEvent
{
    public int OrderId { get; set; }
    public string BuyerId { get; set; }
    public List<OrderItemMessageDto> OrderItemMessages { get; set; } = new List<OrderItemMessageDto>();
    public PaymentMessageDto PaymentMessage { get; set; }
}
