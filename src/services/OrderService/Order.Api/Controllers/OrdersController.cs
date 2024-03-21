using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Models;
using Shared;
using Shared.Models;

namespace Order.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrdersController(AppDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateDto orderCreate)
    {
        // bu kodların hepsi service klasörü oluşturup onun altına taşınacak

        var newOrder = new Models.Order
        {
            BuyerId = orderCreate.BuyerId,
            Status = OrderStatu.Suspend,
            Address = new Address { Line = orderCreate.Address.Line, Provience = orderCreate.Address.Provience, District = orderCreate.Address.District},
            CreatedDate = DateTime.Now,
            FailMessage = "null",
        };               

        // burada Parallel.ForEach kullanımına bir örnek verildi
        Parallel.ForEach(orderCreate.OrderItems, item =>
        {
            var newOrderItem = new OrderItem
            {
                Price = item.Price,
                ProductId = item.ProductId,
                Count = item.Count
            };

            lock (newOrder.Items)
            {
                newOrder.Items.Add(newOrderItem);
            }
        });

        await _context.AddAsync(newOrder);
        await _context.SaveChangesAsync();

        var orderCreatedEvent = new OrderCreatedEvent()
        {
            BuyerId = orderCreate.BuyerId,
            OrderId = newOrder.Id,
            // burada mapping işlemi manuel yapıldı automapper yada mapster kullanarak bunu yazmaktan kurtulabilirsin
            PaymentMessage = new PaymentMessageDto(
                CardName: orderCreate.Payment.CardName,
                CardNumber: orderCreate.Payment.CardNumber,
                Expiration: orderCreate.Payment.Expiration,
                CVV: orderCreate.Payment.CVV,
                TotalPrice: orderCreate.OrderItems.Sum(x => x.Price * x.Count)
            )
        };

        // burada ForEach in eski kullanımına bir örnek verildi
        orderCreate.OrderItems.ForEach(item =>
        {
            // gözünü seveyim burayı da unutma mapping yaparken
            orderCreatedEvent.OrderItemMessages.Add(new OrderItemMessageDto(
                ProductId: item.ProductId,
                Count: item.Count
            ));
        });

        await _publishEndpoint.Publish(orderCreatedEvent);

        return Ok();
    }
}
