namespace EventSourcing.Shared;

public class ProductCreatedEvent : ProductEventBase
{
    //eğer select isteği atacağımız veritabanı çökerse diye buradan son hallerini çekmek için kendimiz ProductEventBase den id yi de ekliyoruz
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int UserId { get; set; }
}