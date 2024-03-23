using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.RabbitMqSettings;
using Stock.Api.Consumers;
using Stock.Api.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Stock API",
        Version = "v1",
        Description = "Stock API",
        Contact = new OpenApiContact
        {
            Name = "Mehmet Tekin ALTUN",
            Email = "mehmettekinaltun@gmail.com",
            Url = new Uri("https://github.com/mtaltn")
        }
    }));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<StockRollBackMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));

        cfg.ReceiveEndpoint(RabbitMqConst.StockOrderCreatedEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMqConst.StockRollBackMessageQueueName, e =>
        {
            e.ConfigureConsumer<StockRollBackMessageConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("StockDb");
});


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<AppDbContext>();

    context.Stocks.AddRange(
        new Stock.Api.Models.Stock { Id = 1, ProductId = 1, Count = 100 },
        new Stock.Api.Models.Stock { Id = 2, ProductId = 2, Count = 100 },
        new Stock.Api.Models.Stock { Id = 3, ProductId = 3, Count = 100 },
        new Stock.Api.Models.Stock { Id = 4, ProductId = 4, Count = 100 },
        new Stock.Api.Models.Stock { Id = 5, ProductId = 5, Count = 100 },
        new Stock.Api.Models.Stock { Id = 6, ProductId = 6, Count = 100 },
        new Stock.Api.Models.Stock { Id = 7, ProductId = 7, Count = 100 },
        new Stock.Api.Models.Stock { Id = 8, ProductId = 8, Count = 100 },
        new Stock.Api.Models.Stock { Id = 9, ProductId = 9, Count = 100 },
        new Stock.Api.Models.Stock { Id = 10, ProductId = 10, Count = 100 }
    );

    context.SaveChanges();
}

app.Run();