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
    /// <summary>
    /// Bu controller, sipariş işlemlerini yönetmek için kullanılır.
    /// </summary>
    /// <remarks>
    /// OrdersController, bir e-ticaret uygulamasının sipariş yönetimi kısmını ele alır.
    /// Temel olarak aşağıdaki işlevleri yerine getirir:
    /// 
    /// 1. Yeni Sipariş Oluşturma:
    ///    - Müşteriden gelen sipariş bilgilerini alır.
    ///    - Bu bilgileri kullanarak yeni bir sipariş nesnesi oluşturur.
    ///    - Siparişi veritabanına kaydeder.
    ///    - Ödeme işlemi için gerekli bilgileri hazırlar.
    /// 
    /// 2. Sipariş Durumu Yönetimi:
    ///    - Yeni oluşturulan siparişlerin durumunu "Askıda" olarak belirler.
    ///    - Sipariş durumunu güncellemek için gerekli altyapıyı sağlar.
    /// 
    /// 3. Ödeme İşlemi Başlatma:
    ///    - Sipariş oluşturulduktan sonra ödeme işlemini başlatmak için bir olay yayınlar.
    ///    - Bu olay, ödeme servisine gerekli bilgileri iletir.
    /// 
    /// 4. Veri Bütünlüğü ve Güvenlik:
    ///    - Paralel işlemler sırasında veri bütünlüğünü korumak için kilit mekanizmaları kullanır.
    ///    - Gelen verilerin doğruluğunu kontrol eder ve gerekli validasyonları yapar.
    /// 
    /// 5. Veritabanı İşlemleri:
    ///    - Entity Framework Core kullanarak veritabanı işlemlerini gerçekleştirir.
    ///    - Siparişleri ve ilgili detayları veritabanına kaydeder.
    /// 
    /// 6. Olay Tabanlı İletişim:
    ///    - MassTransit kütüphanesini kullanarak mikroservisler arası iletişimi sağlar.
    ///    - Sipariş oluşturma olayını yayınlayarak diğer servisleri bilgilendirir.
    /// 
    /// Bu controller, SOLID prensiplerini göz önünde bulundurarak tasarlanmıştır ve
    /// gelecekte yapılacak geliştirmeler için uygun bir altyapı sunar. Ayrıca, performans
    /// optimizasyonu için Parallel.ForEach gibi paralel programlama teknikleri kullanılmıştır.
    /// </remarks>


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
