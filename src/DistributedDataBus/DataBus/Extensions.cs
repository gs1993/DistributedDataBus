using MassTransit;
using RabbitMQ.Client;

namespace DataBus
{
    public static class Extensions
    {
        public static void RegisterProducer<TMessage>(this IRabbitMqBusFactoryConfigurator cfg)
            where TMessage : class
        {
            string tMessageName = typeof(TMessage).Name;
            cfg.Message<TMessage>(e => e.SetEntityName($"ex_{tMessageName}"));
            cfg.Publish<TMessage>(e => e.ExchangeType = ExchangeType.Direct);
            cfg.Send<TMessage>(e =>
            {
                e.UseRoutingKeyFormatter(context => $"rk_{tMessageName}");
            });
        }

        public static void RegisterConsumer<TMessage, TConsumer>(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
            where TMessage : class
            where TConsumer : class, IConsumer<TMessage>
        {
            string tMessageName = typeof(TMessage).Name;

            string tConsumerName = typeof(TConsumer).Name;
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
        }
    }
}
