namespace EventSourcing.Shared;

public class ProductDeletedEvent : IEvent
{
    public Guid Id { get; set; }
}