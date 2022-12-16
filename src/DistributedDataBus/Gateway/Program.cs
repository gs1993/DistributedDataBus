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

        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddControllers();

        builder.Services.AddGrpcClient<OrderProtoService.OrderProtoServiceClient>(o =>
        {
            o.Address = new Uri("https://localhost:5001"); //TODO: get address from config
        });
        builder.Services.AddGrpcClient<BookProtoService.BookProtoServiceClient>(o =>
        {
            o.Address = new Uri("https://localhost:5002"); //TODO: get address from config
        });

        RabbitMqSettings rabbitMqSettings = new();
        builder.Configuration.GetSection("RabbitMqSettings").Bind(rabbitMqSettings);

        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("AppSettings"));

        builder.Services.AddMassTransit(mt =>
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
}