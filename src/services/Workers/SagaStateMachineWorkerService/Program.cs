using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachineWorkerService;
using SagaStateMachineWorkerService.Models;
using Shared.RabbitMqSettings;
using System.Reflection;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddDbContext<OrderStateDbContext>(options =>
    {
        options.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlCon"), m =>
        {
            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
        });
    });

    services.AddMassTransit(cfg =>
    {
        cfg.UsingRabbitMq((context, configure) =>
        {
            configure.Host(hostContext.Configuration.GetConnectionString("RabbitMQ"));

            configure.ReceiveEndpoint(RabbitMqConst.OrderSaga, e =>
            {
                e.ConfigureSaga<OrderStateInstance>(context);
            });
        });

        //cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
        //{
        //    configure.Host(hostContext.Configuration.GetConnectionString("RabbitMQ"));

        //    configure.ReceiveEndpoint(RabbitMqConst.OrderSaga, e =>
        //    {
        //        e.ConfigureSaga<OrderStateInstance>(provider);
        //    });
        //}));

        cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
        {
            opt.ExistingDbContext<OrderStateDbContext>();
        });
    });

    services.AddHostedService<Worker>();
});

var host = builder.Build();
host.Run();
