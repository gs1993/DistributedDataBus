using BookSearchService.ApiClients.OpenLibrary;
using DataBus.Requests.Book;
using MassTransit;
using Nest;

namespace BookSearchService.Consumers
{
    public class ImportBookConsumer : IConsumer<ImportBookRequest>
    {
        private readonly ILogger<ImportBookConsumer> _logger;
        private readonly ElasticClient _elasticClient;
        private readonly IOpenApiCient _openApiCient;

        public ImportBookConsumer(ILogger<ImportBookConsumer> logger,
            ElasticClient elasticClient,
            IOpenApiCient openApiCient)
        {
            _logger = logger;
            _elasticClient = elasticClient;
            _openApiCient = openApiCient;
        }

        public async Task Consume(ConsumeContext<ImportBookRequest> context)
        {
            string isbn = context?.Message?.Isbn ?? string.Empty;
            if (string.IsNullOrWhiteSpace(isbn) || isbn.Length != 10)
                throw new ArgumentException("Invalid ISBN", nameof(isbn));
              
            var getBookResult = await _openApiCient.GetBook(isbn, context.CancellationToken);
            if (getBookResult.IsFailure)
            {
                _logger.LogInformation(getBookResult.Error);
                return;
            }

            var indexResult = await _elasticClient.IndexDocumentAsync(getBookResult.Value, context.CancellationToken);
            if(!indexResult.IsValid)
                throw new Exception(indexResult.ServerError.ToString());
        }
    }
}
