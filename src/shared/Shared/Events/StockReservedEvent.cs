﻿using Shared.Models;

namespace Shared;

public class StockReservedEvent
{
    public int OrderId { get; set; }
    public string BuyerId { get; set; }
    public PaymentMessageDto PaymentMessage { get; set; }
    public List<OrderItemMessageDto> OrderItemMessages { get; set; } = new List<OrderItemMessageDto>();
}