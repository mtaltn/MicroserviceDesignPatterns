using MassTransit;
using Shared.Models;

namespace Shared;

public interface IStockReservedRequestPayment : CorrelatedBy<Guid>
{
    public PaymentMessageDto PaymentMessage { get; set; }
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
    public string BuyerId { get; set; }
}
