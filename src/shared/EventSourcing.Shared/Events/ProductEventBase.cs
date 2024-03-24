namespace EventSourcing.Shared;

public class ProductEventBase : IEvent
{
    public Guid Id { get; set; }
}
