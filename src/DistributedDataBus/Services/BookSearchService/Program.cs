using BookSearchService.Consumers;
using BookSearchService.Models;
using DataBus;
using DataBus.Requests.Book;
using Elasticsearch.Net;
using MassTransit;
using Nest;
using Refit;
using BookSearchService.ApiClients.OpenLibrary;

namespace BookSearchService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

                    cfg.RegisterConsumer<ImportBookRequest, ImportBookConsumer>(context);
                });
            });

            var elasticClient = CreateElasticsearchClient();
            builder.Services.AddSingleton(elasticClient);

            builder.Services
                .AddRefitClient<IOpenLibraryApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://openlibrary.org/"));
            builder.Services.AddTransient<IOpenApiCient, OpenApiCient>();

            var app = builder.Build();

            app.MapGrpcService<Services.BookService>();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

            app.Run();
        }

        private static ElasticClient CreateElasticsearchClient()
        {
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200")); //TODO: from appsettings.json
            var settings = new ConnectionSettings(pool)
                .DefaultIndex("book");
            var elasticClient = new ElasticClient(settings);
            var createIndexResponse = elasticClient.Indices.Create("book", c => c
                .Map<Book>(b =>  b.Properties(p => p
                    .Keyword(f => f.Name(n => n.Isbn))
                    .Number(f => f.Name(n => n.PageCount))
                    .Text(f => f.Name(n => n.Title))
                    .Date(f => f.Name(n => n.PublishDate))
                    .Nested<Author>(a => a
                        .Name(n => n.Authors)
                            .Properties(ap => ap
                            .Keyword(f => f.Name(n => n.FullName))
                            .Text(f => f.Name(n => n.Bio))))
            )));

            return elasticClient;
        }
    }
}