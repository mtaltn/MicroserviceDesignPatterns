using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.RabbitMqSettings;
using Stock.Api.Models;

namespace Stock.Api.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrderCreatedEvent>
    {

        private readonly AppDbContext _appDbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext appDbContext, ILogger<OrderCreatedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
            var stockResults = await Task
                .WhenAll(context.Message.OrderItemMessages
                .Select(async item => await _appDbContext.Stocks
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

                _logger.LogInformation($"Stock was reserved for Correlation Id: {context.Message.CorrelationId}");

                var stockReservedEvent = new StockReservedEvent(context.Message.CorrelationId)
                {                    
                    OrderItemMessages = context.Message.OrderItemMessages,
                };

                await _publishEndpoint.Publish(stockReservedEvent);
            }
            else
            {
                await _publishEndpoint.Publish(new StockNotReservedEvent(context.Message.CorrelationId)
                {
                    Reason = "Stock was not enough."
                });
                _logger.LogError($"Stock was not enough for Correlation Id: {context.Message.CorrelationId}");
            }
        }
    }
}
