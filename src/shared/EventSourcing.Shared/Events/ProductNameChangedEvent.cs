namespace EventSourcing.Shared;

public class ProductNameChangedEvent : ProductEventBase
{
    public string ChangedName { get; set; }
}