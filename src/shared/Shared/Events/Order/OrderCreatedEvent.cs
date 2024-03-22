﻿using Shared.Models;

namespace Shared;

public class OrderCreatedEvent : IOrderCreatedEvent
{
    public List<OrderItemMessageDto> OrderItemMessages { get; set; }
}
