using Microsoft.EntityFrameworkCore;
using OrderService.Repositories;
using System;

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

            var app = builder.Build();

            app.MapGrpcService<Services.OrderService>();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

            app.Run();
        }
    }
}