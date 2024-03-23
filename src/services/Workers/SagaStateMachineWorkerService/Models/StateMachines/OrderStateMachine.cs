using MassTransit;
using Shared;
using Shared.Models;
using Shared.RabbitMqSettings;

namespace SagaStateMachineWorkerService.Models;

public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
{
    public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; }
    public Event<IStockReservedEvent> StockReservedEvent { get; }
    public Event<IStockNotReservedEvent> StockNotReservedEvent { get; }
    public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; }
    public Event<IPaymentFailedEvent> PaymentFailedEvent { get; }

    public State OrderCreated { get; }
    public State StockReserved { get; }
    public State StockNotReserved { get; }
    public State PaymentCompleted { get; }
    public State PaymentFailed { get; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreatedRequestEvent, x => x.CorrelateBy<int>(ctx => ctx.OrderId, y => y.Message.OrderId).SelectId(context => NewId.NextGuid()));

        Event(() => StockReservedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => StockNotReservedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => PaymentCompletedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

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
                .Publish(context => new OrderCreatedEvent(context.Instance.CorrelationId) { OrderItemMessages = context.Data.OrderItemMessages })
                .TransitionTo(OrderCreated)
                .Then(context => { Console.WriteLine($"OrderCreatedRequestEvent before : {context.Instance}"); }));

        During(OrderCreated,
            When(StockReservedEvent)
                .TransitionTo(StockReserved)
                .Publish(context => new StockReservedRequestPayment(context.Instance.CorrelationId)
                {
                    OrderItemMessages = context.Data.OrderItemMessages,
                    //automapper kullanılacak
                    PaymentMessage = new PaymentMessageDto()
                    {
                        CardName = context.Instance.CardName,
                        CardNumber = context.Instance.CardNumber,
                        CVV = context.Instance.CVV,
                        Expiration = context.Instance.Expiration,
                        TotalPrice = context.Instance.TotalPrice
                    },
                    BuyerId = context.Instance.BuyerId
                })
                .Then(context => { Console.WriteLine($"StockReservedEvent After : {context.Instance}"); }),
            When(StockNotReservedEvent)
                .TransitionTo(StockNotReserved)
                .Publish(context => new OrderRequestFailedEvent() { OrderId = context.Instance.OrderId, Reason = context.Data.Reason })
                .Then(context => { Console.WriteLine($"StockReservedEvent After : {context.Instance}"); }));
        During(StockReserved,
            When(PaymentCompletedEvent)
                .TransitionTo(PaymentCompleted)
                .Publish(context => new OrderRequestCompletedEvent() { OrderId = context.Instance.OrderId })
                .Then(context => { Console.WriteLine($"PaymentCompletedEvent After : {context.Data}"); })
                .Finalize(),
            When(PaymentFailedEvent)
                .TransitionTo(PaymentFailed)
                .Publish(context => new OrderRequestFailedEvent() { OrderId = context.Instance.OrderId, Reason = context.Data.Reason })
                .Send(new Uri($"queue:{RabbitMqConst.StockRollBackMessageQueueName}"), context => new StockRollBackMessage() { OrderItemMessages = context.Message.OrderItemMessages })
                .TransitionTo(PaymentFailed)
                .Then(context => { Console.WriteLine($"PaymentFailedEvent After : {context.Data}"); })/*** devam etmek istersen buradan devam edeceksin ***/);
        SetCompletedWhenFinalized();
    }
}
