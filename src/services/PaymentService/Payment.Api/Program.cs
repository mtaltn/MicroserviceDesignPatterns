using MassTransit;
using Microsoft.OpenApi.Models;
using Payment.Api.Consumer;
using Shared.RabbitMqSettings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment API",
        Version = "v1",
        Description = "Payment API",
        Contact = new OpenApiContact
        {
            Name = "Mehmet Tekin ALTUN",
            Email = "mehmettekinaltun@gmail.com",
            Url = new Uri("https://github.com/mtaltn")
        }
    }));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockReservedRequestPaymentConsumer>();
        
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));

        cfg.ReceiveEndpoint(RabbitMqConst.PaymentStockReservedRequestEventQueueName, e =>
        {
            e.ConfigureConsumer<StockReservedRequestPaymentConsumer>(context);
        });
    });
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