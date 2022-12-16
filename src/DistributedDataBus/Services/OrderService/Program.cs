using DataBus;
using DataBus.Requests.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Consumers;
using OrderService.Repositories;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseInMemoryDatabase("OrderDb"));
            builder.Services.AddTransient<IOrderRepository, OrderRepository>();

            builder.Services.AddGrpc();

            RabbitMqSettings rabbitMqSettings = new();
            builder.Configuration.GetSection("RabbitMqSettings").Bind(rabbitMqSettings);

            builder.Services.AddMassTransit(mt =>
            {
                mt.AddConsumers(typeof(Program).Assembly);

                mt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.UserName);
                        h.Password(rabbitMqSettings.Password);
                    });

                    cfg.RegisterConsumer<CreateOrderRequest, CreateOrderConsumer>(context);
                    cfg.RegisterConsumer<CancelOrderRequest, CancelOrderConsumer>(context);
                });
            });


            var app = builder.Build();

            app.MapGrpcService<Services.OrderService>();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

            app.Run();
        }
    }
}