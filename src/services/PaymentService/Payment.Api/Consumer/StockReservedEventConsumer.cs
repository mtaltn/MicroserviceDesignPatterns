using MassTransit;
using Shared;

namespace Payment.Api.Consumer;

public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
{
    private readonly ILogger<StockReservedEventConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var balance = 3000m; // m eki decimal olduğunu belirtmek için

        if (balance > context.Message.PaymentMessage.TotalPrice)
        {
            _logger.LogInformation($"Payment successful: {context.Message.PaymentMessage.TotalPrice} TL was withdrawn from credit card for user id = {context.Message.BuyerId}");
            await _publishEndpoint.Publish(new PaymentCompletedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId });
        }
        else
        {
            _logger.LogError($"Payment failed: {context.Message.PaymentMessage.TotalPrice} TL was not withdrawn from credit card for user id = {context.Message.BuyerId}");
            await _publishEndpoint.Publish(new PaymentFailedEvent { OrderId = context.Message.OrderId, BuyerId = context.Message.BuyerId, Message = "not enough balance" , OrderItemMessages = context.Message.OrderItemMessages});
        }
    }
}
