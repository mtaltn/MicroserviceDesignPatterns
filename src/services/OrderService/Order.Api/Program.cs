using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Order.Api.Consumers;
using Order.Api.Models;
using Shared.RabbitMqSettings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order API",
        Version = "v1",
        Description = "Order API",
        Contact = new OpenApiContact
        {
            Name = "Mehmet Tekin ALTUN",
            Email = "mehmettekinaltun@gmail.com",
            Url = new Uri("https://github.com/mtaltn")
        }
    }));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentComplatedEventConsumer>();
    x.AddConsumer<PaymentFailedEventConsumer>();
    x.AddConsumer<StockNotReservedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ReceiveEndpoint(RabbitMqConst.OrderPaymentComplatedEventQueueName , e =>
        {
            e.ConfigureConsumer<PaymentComplatedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMqConst.OrderPaymentFailedEventQueueName , e =>
        {
            e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMqConst.StockNotReservedEventQueueName, e =>
        {
            e.ConfigureConsumer<StockNotReservedEventConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
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

app.Run();