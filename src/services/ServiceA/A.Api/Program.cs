using A.Api.Services;
using Microsoft.OpenApi.Models;
using Polly.Extensions.Http;
using Polly;
using System.Diagnostics;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "A API",
        Version = "v1",
        Description = "A API",
        Contact = new OpenApiContact
        {
            Name = "Mehmet Tekin ALTUN",
            Email = "mehmettekinaltun@gmail.com",
            Url = new Uri("https://github.com/mtaltn")
        }
    }));

builder.Services.AddHttpClient<ProductService>(opt =>
{
    opt.BaseAddress = new Uri("http://localhost:5001/api/Products/");
})
.AddPolicyHandler(GetAdvanceCircuitBreakerPolicy());
//.AddPolicyHandler(GetCircuitBreakerPolicy());
//.AddPolicyHandler(GetRetryPolicy());
///hangisini kullanmak istiyorsan onu seçebilirsin þuan GetAdvanceCircuitBreakerPolicy a göre çalýþýyor þuan 

// direkt burada url de vererek kullanabilirsiniz ben içerisine girip deðer atamayý daha çok seçiyorum
//builder.Services.AddHttpClient("YourHttpClientName")
//    .AddPolicyHandler(GetCircuitBreakerPolicy())
//    .AddPolicyHandler(GetRetryPolicy());

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

///bunlarý da gözümsün Tekin git baþka bir yerde tanýmla yaz et araþtýr nasýl yapýlýr bul buradan kaldýr bu nedir böyle middleware oluþtur orada yaz
///bu 2 kodda da direkt return olduðu için süslü parantez ve return parametresinden => ile kurtulabilirdik ama okunabilirlik sekteye uðrar diye düþündüm uzun bir kod 
// Define Circuit Breaker policy
IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10),
        onBreak: (exception, timespan) =>
        {
            Debug.WriteLine("Circuit Breaker Status => On Break");
        },
        onReset: () =>
        {
            Debug.WriteLine("Circuit Breaker Status => On Reset");
        },
        onHalfOpen: () =>
        {
            Debug.WriteLine("Circuit Breaker Status => On Half Open");
        });
}
///uzun süre down olmayacak senaryolar için uygundur. misal birisi zabbix ya da docker üzerinden sürekli kontrol ediyordur 
///servisi hata olunca hemen baþlatýyordur ya da otomatize etmiþsindir o senaryolar için uynu
// Define Retry policy
IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound) /// bu elzem deðil burada biz özelleþtirmek istersek içerisine OrResult ile giriyoruz
        .WaitAndRetryAsync(5, retryAttempt =>
        {
            Debug.WriteLine($"Retry Count :{retryAttempt}");
            return TimeSpan.FromSeconds(10);
        },
        onRetryAsync: (result, timespan, context) =>
        {
            Debug.WriteLine($"Request is made again:{timespan.TotalMilliseconds}");
            return Task.CompletedTask;
        });
}

// Define Advanced Circuit Breaker policy
IAsyncPolicy<HttpResponseMessage> GetAdvanceCircuitBreakerPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
       .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(10), 5, TimeSpan.FromSeconds(30),
       onBreak: (exception, timespan) =>
       {
           Debug.WriteLine("Circuit Breaker Status => On Break");
       },
       onReset: () =>
       {
           Debug.WriteLine("Circuit Breaker Status => On Reset");
       },
       onHalfOpen: () =>
       {
           Debug.WriteLine("Circuit Breaker Status => On Half Open");
       });        
}