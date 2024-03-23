namespace Shared.RabbitMqSettings;

public class RabbitMqConst
{
    //public const string StockReservedEventQueueName = "stock-reserved-queue";
    public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
    //public const string PaymentStockReservedEventQueueName = "payment-stock-reserved-queue";
    public const string OrderRequestCompletedEventQueueName = "order-request-complated-queue";
    public const string OrderRequestFailedEventQueueName = "order-request-failed-queue";
    //public const string OrderPaymentFailedEventQueueName = "order-payment-failed-queue";
    //public const string StockPaymentFailedEventQueueName = "stock-payment-failed-queue";
    public const string StockNotReservedEventQueueName = "order-stock-not-reserved-queue";
    public const string OrderSaga = "order-saga-queue";
    public const string PaymentStockReservedRequestEventQueueName = "payment-stock-reserved-request-queue";
    public const string StockRollBackMessageQueueName = "stock-roll-back-message-queue";
}
