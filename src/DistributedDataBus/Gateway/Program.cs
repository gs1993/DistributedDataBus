using Contracts.Utils;
using DataBus;
using DataBus.Requests.Book;
using DataBus.Requests.Order;
using Gateway.Utils;
using MassTransit;

namespace Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();

        services.AddOptions();

        var grpcSettings = configuration.GetConfigSection<GrpcSettings>();
        RegisterGrpcClients(services, grpcSettings);

        var rabbitMqSettings = configuration.GetConfigSection<RabbitMqSettings>();
        RegisterRabbitMq(services, rabbitMqSettings);

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();
    }

    private static void RegisterGrpcClients(IServiceCollection services, GrpcSettings grpcSettings)
    {
        services.AddGrpcClient<OrderProtoService.OrderProtoServiceClient>(o =>
        {
            o.Address = new Uri(grpcSettings.GetUrl("OrderService"));
        });
        services.AddGrpcClient<BookProtoService.BookProtoServiceClient>(o =>
        {
            o.Address = new Uri(grpcSettings.GetUrl("BookService"));
        });
    }

    private static void RegisterRabbitMq(IServiceCollection services, RabbitMqSettings rabbitMqSettings)
    {
        services.AddMassTransit(mt =>
        {
            mt.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMqSettings.UserName);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.RegisterProducer<CreateOrderRequest>();
                cfg.RegisterProducer<CancelOrderRequest>();
                cfg.RegisterProducer<ImportBookRequest>();
            });
        });
    }
}