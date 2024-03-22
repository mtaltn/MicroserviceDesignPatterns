using MassTransit;
using Shared;
using Shared.Models;

namespace SagaStateMachineWorkerService.Models;

public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
{
    public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; }

    public State OrderCreated { get; }
    public State StockReserved { get; }
    public State StockNotReserved { get; }
    public State PaymentCompleted { get; }
    public State PaymentFailed { get; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreatedRequestEvent, x => x.CorrelateBy<int>(ctx => ctx.OrderId, y => y.Message.OrderId).SelectId(context => NewId.NextGuid()));


        Initially(
            When(OrderCreatedRequestEvent)
                .Then(context =>
                {
                    context.Instance.BuyerId = context.Data.BuyerId;
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.CreatedDate = DateTime.Now;
                    context.Instance.CardName = context.Data.PaymentMessage.CardName;
                    context.Instance.CardNumber = context.Data.PaymentMessage.CardNumber;
                    context.Instance.CVV = context.Data.PaymentMessage.CVV;
                    context.Instance.Expiration = context.Data.PaymentMessage.Expiration;
                    context.Instance.TotalPrice = context.Data.PaymentMessage.TotalPrice;
                })
                .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before : {context.Instance}"); })
                .TransitionTo(OrderCreated)
                .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before : {context.Instance}"); }));


    }
}
