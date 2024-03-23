namespace EventSourcing.Shared;

public class ProductCreatedEvent : IEvent
{
    public Guid Id { get; set; } //eğer select isteği atacağımız veritabanı çökerse buradan son hallerini çekmek için kendimiz ekliyoruz
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int UserId { get; set; }
}