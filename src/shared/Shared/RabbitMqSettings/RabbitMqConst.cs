﻿namespace Shared.RabbitMqSettings;

public class RabbitMqConst
{
    public const string StockReservedEventQueueName = "stock-reserved-queue";
    public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
    //public const string PaymentStockReservedEventQueueName = "payment-stock-reserved-queue";
    public const string OrderPaymentComplatedEventQueueName = "order-payment-complated-queue";
}
