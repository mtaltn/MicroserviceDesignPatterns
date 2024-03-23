namespace Shared;

public class OrderRequestCompletedEvent : IOrderRequestCompletedEvent
{
    public int OrderId { get; set; }
}
