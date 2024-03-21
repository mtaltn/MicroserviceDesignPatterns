using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.RabbitMqSettings;
using Stock.Api.Models;

namespace Stock.Api.Consumers;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderCreatedEventConsumer(AppDbContext appDbContext, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
    {
        _appDbContext = appDbContext;
        _logger = logger;
        _sendEndpointProvider = sendEndpointProvider;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        //var stockResults = new List<bool>();

        //foreach (var item in context.Message.OrderItemMessages)
        //{
        //    stockResults.Add(await _appDbContext.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count >= item.Count));
        //}

        var stockResults = await Task
            .WhenAll(context.Message.OrderItemMessages
            .Select(async item =>await _appDbContext.Stocks
            .AnyAsync(x => x.ProductId == item.ProductId && x.Count >= item.Count)));

        if (stockResults.All(x => x))
        {
            foreach (var item in context.Message.OrderItemMessages)
            {
                var stock = await _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                if (stock != null)
                {
                    stock.Count -= item.Count;
                }
            }

            await _appDbContext.SaveChangesAsync();

            _logger.LogInformation($"Stock was reserved for Buyer Id: {context.Message.BuyerId}");

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMqConst.StockReservedEventQueueName}"));

            var stockReservedEvent = new StockReservedEvent
            {
                PaymentMessage = context.Message.PaymentMessage,
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                OrderItemMessages = context.Message.OrderItemMessages,
            };

            await sendEndpoint.Send(stockReservedEvent);
        }
        else
        {
            await _publishEndpoint.Publish(new StockNotReservedEvent
            {
                OrderId = context.Message.OrderId,
                Message = "Stock was not enough."
            });
            _logger.LogInformation($"Stock was not enough for Buyer Id: {context.Message.BuyerId}");
        }
    }
}
