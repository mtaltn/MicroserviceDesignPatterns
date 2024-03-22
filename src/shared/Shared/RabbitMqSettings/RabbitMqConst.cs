namespace Shared.RabbitMqSettings;

public class RabbitMqConst
{
    public const string StockReservedEventQueueName = "stock-reserved-queue";
    public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
    //public const string PaymentStockReservedEventQueueName = "payment-stock-reserved-queue";
    public const string OrderPaymentComplatedEventQueueName = "order-payment-complated-queue";
    public const string OrderPaymentFailedEventQueueName = "order-payment-failed-queue";
    public const string StockPaymentFailedEventQueueName = "stock-payment-failed-queue";
    public const string StockNotReservedEventQueueName = "order-stock-not-reserved-queue";



    public const string OrderSaga = "order-saga-queue";
}
