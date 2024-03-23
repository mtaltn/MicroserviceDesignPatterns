using MassTransit;

namespace Shared;

public interface IStockNotReservedEvent : CorrelatedBy<Guid>
{
    public string Reason { get; set; }
}
