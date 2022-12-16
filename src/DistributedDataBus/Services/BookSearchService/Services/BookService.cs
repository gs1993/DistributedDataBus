using BookSearchService.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Nest;
using static BookProtoService;

namespace BookSearchService.Services
{
    public class BookService : BookProtoServiceBase
    {
        private readonly ILogger<BookService> _logger;
        private readonly ElasticClient _elasticClient;

        public BookService(ILogger<BookService> logger, ElasticClient elasticClient)
        {
            _logger = logger;
            _elasticClient = elasticClient;
        }

        public override async Task<BookDetails> Get(GetBookRequest request, ServerCallContext context)
        {
            var getResponse = _elasticClient.Search<Book>(s => s.Query(q => q
                .Match(m => m.Field(f => f.Isbn)
                .Query(request.Isbn))));
            if (!getResponse.IsValid)
                throw new Exception(getResponse.ServerError.ToString());

            var book = getResponse.Documents.FirstOrDefault();
            var dto = book != null
                ? new BookDetails
                {
                    Isbn = book.Isbn,
                    Title = book.Title,
                    PublishDate = book.PublishDate?.ToShortDateString() ?? string.Empty,
                    PageCount = book.PageCount ?? 0,
                } : null;

            if (book?.Authors != null)
            {
                var authorsDto = book.Authors.Select(x => new AuthorDetailsDto
                {
                    FullName = x.FullName,
                    Bio = x.Bio ?? string.Empty
                }).ToList();
                dto.Authors.AddRange(authorsDto);
            }
            
            return dto;
        }

        public override async Task<BookCountResult> GetCount(Empty request, ServerCallContext context)
        {
            var countResponse = await _elasticClient.CountAsync<Book>();
            if (!countResponse.IsValid)
                throw new Exception(countResponse.ServerError.ToString());

            return new BookCountResult
            {
                Count = (int)countResponse.Count
            };
        }
    }
}
