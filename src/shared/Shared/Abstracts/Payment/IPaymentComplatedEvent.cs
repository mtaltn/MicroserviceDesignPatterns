using MassTransit;

namespace Shared;

public interface IPaymentCompletedEvent : CorrelatedBy<Guid>
{

}
