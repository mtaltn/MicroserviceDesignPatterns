using MassTransit;
using Order.Api.Models;
using Shared;

namespace Order.Api.Consumers;

public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
{
    //bu kodlar her yerde aynı bunları ortak bir yere al 3 consumer içinde kopyala yapıştır yakışmaz 
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<StockNotReservedEventConsumer> _logger;

    public StockNotReservedEventConsumer(AppDbContext appDbContext, ILogger<StockNotReservedEventConsumer> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
    {
        var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);

        if (order is not null)
        {
            order.Status = OrderStatu.Fail;
            order.FailMessage = context.Message.Message;
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
