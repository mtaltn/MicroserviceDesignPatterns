using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Models;
using Shared;

namespace Order.Api.Consumers;

public class OrderRequestCompletedEventConsumer : IConsumer<IOrderRequestCompletedEvent>
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<OrderRequestCompletedEventConsumer> _logger;

    public OrderRequestCompletedEventConsumer(AppDbContext appDbContext, ILogger<OrderRequestCompletedEventConsumer> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IOrderRequestCompletedEvent> context)
    {        
        var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);

        if (order is not null)
        {
            order.Status = OrderStatu.Complate;
            await _appDbContext.SaveChangesAsync();
            // burada {nameof(order.Status)} kısmı öylesine kullanıldı direkt status de yazılabilirdi nameof kalıbını da kullanmak istedim
            _logger.LogInformation($"Order (Id={context.Message.OrderId}) {nameof(order.Status)} has changed : {order.Status}");
        }
        else
        {
            _logger.LogError($"Order (Id={context.Message.OrderId}) not found");
        }
    }
}
