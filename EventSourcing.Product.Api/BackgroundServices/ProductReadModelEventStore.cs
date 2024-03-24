using EventSourcing.Product.Api.Models;
using EventSourcing.Shared;
using EventStore.ClientAPI;
using System.Text;
using System.Text.Json;

namespace EventSourcing.Product.Api.BackgroundServices;

public class ProductReadModelEventStore(IEventStoreConnection eventStoreConnection, ILogger<ProductReadModelEventStore> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IEventStoreConnection _eventStoreConnection = eventStoreConnection;
    private readonly ILogger<ProductReadModelEventStore> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override Task StartAsync(CancellationToken cancellationToken) => base.StartAsync(cancellationToken);
    public override Task StopAsync(CancellationToken cancellationToken) => base.StopAsync(cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(ProductStream.StreamName, ProductStream.GroupName, EventAppeared, autoAck: false);
        // true:  EventAppeared exception firlamadı ise event gönderildi sayar.
        // false : manuel kontrol eder arg1.Acknowledge(arg2.Event.EventId) bunu gönderirsem tamamlandı sayar
        //throw new NotImplementedException();
    }

    private async Task EventAppeared(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2)
    {
        var type = Type.GetType($"{Encoding.UTF8.GetString(arg2.Event.Metadata)}, EventSourcing.Shared");
        _logger.LogInformation($"The Message processing... : {type.ToString()}");
        var eventData = Encoding.UTF8.GetString(arg2.Event.Data);

        var @event = JsonSerializer.Deserialize(eventData, type);
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Models.Product newProduct = null;

        switch (@event)
        {
            case ProductCreatedEvent productCreatedEvent:
                newProduct = new Models.Product()
                {
                    Name = productCreatedEvent.Name,
                    Id = productCreatedEvent.Id,
                    Price = productCreatedEvent.Price,
                    Stock = productCreatedEvent.Stock,
                    UserId = productCreatedEvent.UserId
                };
                //context.Products.Add(newProduct);
                // eğer bu işlemi asenkron olarak ilerletmek istersen aşağıdaki kodu yok ben senkron istiyorum diyorsan üstteki kodu kullan veritabanı işlemi olduğu için ben asenkron seçtim
                _ = await context.Products.AddAsync(newProduct);
                break;

            #region Check Here for Alternative Code
            ///bu kod içerisinde veri tabanı bağlantıları ortak olduğu için toparlayıp yazmakta fayda gördüm eğer bunu kullanmak isterseniz GetProductId methodunu silebilirsiniz
            /*case ProductNameChangedEvent productNameChangedEvent:

                newProduct = context.Products.Find(productNameChangedEvent.Id);
                if (newProduct is not null)
                {
                    newProduct.Name = productNameChangedEvent.ChangedName;
                }
                break;

            case ProductPriceChangedEvent productPriceChangedEvent:
                newProduct = context.Products.Find(productPriceChangedEvent.Id);
                if (newProduct is not null)
                {
                    newProduct.Price = productPriceChangedEvent.ChangedPrice;
                }
                break;

            case ProductDeletedEvent productDeletedEvent:
                newProduct = context.Products.Find(productDeletedEvent.Id);
                if (newProduct is not null)
                {
                    context.Products.Remove(newProduct);
                }
                break;
        }*/
            #endregion
            case ProductNameChangedEvent productNameChangedEvent:
            case ProductPriceChangedEvent productPriceChangedEvent:
            case ProductDeletedEvent productDeletedEvent:
                ///burada id yi asenkron olarak çekmeye çalıştım fakat süreç ilerlemedi karmaşıklığı arttığını düşünerek tekrardan senkron olan haline çevirdim. 
                ///asenkronu daha sonra tekrar test etmek istersen FindAsync kullan başına await ekleyip GetProductId kısmınında Guid geri dönüş tipini async Task<Guid> yap
                var existingProduct = context.Products.Find(GetProductId(@event));
                if (existingProduct is not null)
                {
                    //bu şekilde hem daha temiz bir kod bloğuna sahip oldu hemde okunabilirlikten uzaklaşmamış olduk if yapısında tek satır varsa süslü paranteze zorunlu değil
                    if (@event is ProductNameChangedEvent nameChangedEvent)
                        existingProduct.Name = nameChangedEvent.ChangedName;

                    else if (@event is ProductPriceChangedEvent priceChangedEvent)
                        existingProduct.Price = priceChangedEvent.ChangedPrice;

                    else if (@event is ProductDeletedEvent)
                        context.Products.Remove(existingProduct);

                    #region Check Here for Alternative Code
                    ///------------------///

                    ///bu if bloklarını tek satıra indirger fakat okumayı biraz zorlaştırıyor bence

                    //if (@event is ProductNameChangedEvent nameChangedEvent) existingProduct.Name = nameChangedEvent.ChangedName;
                    //else if (@event is ProductPriceChangedEvent priceChangedEvent) existingProduct.Price = priceChangedEvent.ChangedPrice;
                    //else if (@event is ProductDeletedEvent) context.Products.Remove(existingProduct);

                    ///------------------///

                    ///bu hali tam açılmış halidir

                    //if (@event is ProductNameChangedEvent nameChangedEvent)
                    //{
                    //    existingProduct.Name = nameChangedEvent.ChangedName;
                    //}
                    //else if (@event is ProductPriceChangedEvent priceChangedEvent)
                    //{
                    //    existingProduct.Price = priceChangedEvent.ChangedPrice;
                    //}
                    //else if (@event is ProductDeletedEvent)
                    //{
                    //    context.Products.Remove(existingProduct);
                    //}
                    #endregion
                }
                break;
        }

        Guid GetProductId(object eventObj)
        {
            return eventObj switch
            {
                ProductNameChangedEvent nameChangedEvent => nameChangedEvent.Id,
                ProductPriceChangedEvent priceChangedEvent => priceChangedEvent.Id,
                ProductDeletedEvent deletedEvent => deletedEvent.Id,
                _ => Guid.Empty
            };
        }

        await context.SaveChangesAsync();

        arg1.Acknowledge(arg2.Event.EventId);
    }
}