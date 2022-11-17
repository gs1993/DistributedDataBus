﻿using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace DataBus
{
    public static class Extensions
    {
        public static void RegisterProducer<TMessage>(this IServiceCollection services, RabbitMqSettings settings) where TMessage : class
        {
            services.AddMassTransit(mt =>
            {
                mt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(settings.Host, settings.VirtualHost, h =>
                    {
                        h.Username(settings.UserName);
                        h.Password(settings.Password);
                    });

                    string tMessageName = typeof(TMessage).Name;
                    cfg.Message<TMessage>(e => e.SetEntityName($"ex_{tMessageName}"));
                    cfg.Publish<TMessage>(e => e.ExchangeType = ExchangeType.Direct);
                    cfg.Send<TMessage>(e =>
                    {
                        e.UseRoutingKeyFormatter(context => $"rk_{tMessageName}");
                    });
                });
            });
        }

        public static void RegisterConsumer<TMessage, TConsumer>(this IServiceCollection services, RabbitMqSettings settings)
            where TMessage : class
            where TConsumer : class, IConsumer<TMessage>
        {
            services.AddMassTransit(mt =>
            {
                mt.AddConsumer<TConsumer>();

                mt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(settings.Host, settings.VirtualHost, h =>
                    {
                        h.Username(settings.UserName);
                        h.Password(settings.Password);
                    });

                    string tMessageName = typeof(TMessage).Name;
                    cfg.ReceiveEndpoint($"q_{tMessageName}", re =>
                    {
                        re.ConfigureConsumeTopology = false;
                        re.SetQuorumQueue();
                        re.SetQueueArgument("declare", "lazy");

                        re.Bind($"ex_{tMessageName}", e =>
                        {
                            e.RoutingKey = $"rk_{tMessageName}";
                            e.ExchangeType = ExchangeType.Direct;
                        });

                        re.ConfigureConsumer<TConsumer>(context);
                    });
                });
            });
        }
    }
}
