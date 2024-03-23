using Shared.Models;

namespace Shared;

public class StockReservedRequestPayment : IStockReservedRequestPayment
{
    public StockReservedRequestPayment(Guid gorrelationId)
    {
        CorrelationId = gorrelationId;
    }
    public PaymentMessageDto PaymentMessage { get; set; }
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
    //public string BuyerId { get; set; }

    public Guid CorrelationId { get; }
    public string BuyerId { get; set; }
}
