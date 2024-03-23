using MassTransit;
using Shared;

namespace Payment.Api.Consumer;

public class StockReservedRequestPaymentConsumer : IConsumer<IStockReservedRequestPayment>
{
    private readonly ILogger<StockReservedRequestPaymentConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public StockReservedRequestPaymentConsumer(ILogger<StockReservedRequestPaymentConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IStockReservedRequestPayment> context)
    {
        var balance = 3000m; // m eki decimal olduğunu belirtmek için

        if (balance > context.Message.PaymentMessage.TotalPrice)
        {
            _logger.LogInformation($"Payment successful: {context.Message.PaymentMessage.TotalPrice} TL was withdrawn from credit card for user id = {context.Message.BuyerId}");
            await _publishEndpoint.Publish(new PaymentCompletedEvent(context.Message.CorrelationId));
        }
        else
        {
            _logger.LogError($"Payment failed: {context.Message.PaymentMessage.TotalPrice} TL was not withdrawn from credit card for user id = {context.Message.BuyerId}");
            await _publishEndpoint.Publish(new PaymentFailedEvent(context.Message.CorrelationId) { Reason = "not enough balance", OrderItemMessages = context.Message.OrderItemMessages });
        }
    }
}

