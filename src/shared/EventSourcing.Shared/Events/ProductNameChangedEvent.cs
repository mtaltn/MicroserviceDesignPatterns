namespace EventSourcing.Shared;

public class ProductNameChangedEvent : IEvent
{
    public Guid Id { get; set; }
    public string ChangedName { get; set; }
}