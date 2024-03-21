namespace Order.Api.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string BuyerId { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public OrderStatu Status { get; set; }
    public string FailMessage { get; set; } = "null";
    public Address Address{ get; set; }
}
