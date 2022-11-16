using DataBus;
using DataBus.Requests;
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

            builder.Services.RegisterConsumer<CreateOrderRequest, CreateOrderConsumer>();

            var app = builder.Build();

            app.MapGrpcService<Services.OrderService>();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

            app.Run();
        }
    }
}