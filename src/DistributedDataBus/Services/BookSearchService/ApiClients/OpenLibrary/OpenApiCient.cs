using BookSearchService.Models;
using CSharpFunctionalExtensions;
using Polly;
using Refit;
using System.Globalization;
using System.Net;

namespace BookSearchService.ApiClients.OpenLibrary
{
    public interface IOpenApiCient
    {
        Task<Result<Book>> GetBook(string isbn, CancellationToken ct);
    }

    public class OpenApiCient : IOpenApiCient
    {
        private static HttpStatusCode[] HttpStatusCodesWorthRetrying = {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout // 504
        };

        private readonly IOpenLibraryApi _openLibraryApiClient;

        public OpenApiCient(IOpenLibraryApi openLibraryApiClient)
        {
            _openLibraryApiClient = openLibraryApiClient;
        }

        public async Task<Result<Book>> GetBook(string isbn, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(isbn) || isbn.Length != 10)
                Result.Failure<Book>($"Invalid ISBN: '{isbn}'");

            try
            {
                var bookDto = await Policy<BookDto>
                        .Handle<ApiException>(e => HttpStatusCodesWorthRetrying.Contains(e.StatusCode))
                        .RetryAsync(5) //TODO: From appsettings
                        .ExecuteAsync(() => _openLibraryApiClient.GetBook(isbn, ct))
                        .ConfigureAwait(false);

                if (bookDto == null)
                    return Result.Failure<Book>($"Book with isbn: {isbn} not found");

                var authors = await TryGetAuthors(bookDto, ct);

                var publishDate = TryGetPublishDate(bookDto.Publish_date);
                return new Book
                {
                    Title = bookDto.Title,
                    Isbn = isbn,
                    PageCount = bookDto.Number_of_pages,
                    PublishDate = null,
                    Authors = authors.Select(x => new Author
                    {
                        FullName = x.Name,
                        Bio = x.Bio
                    }).ToArray()
                };
            }
            catch (Exception e)
            {
                return Result.Failure<Book>(e.Message);
            }
        }

        private async Task<List<AuthorDetailsDto>> TryGetAuthors(BookDto bookDto, CancellationToken ct)
        {
            if (bookDto.Authors == null || !bookDto.Authors.Any())
                return new List<AuthorDetailsDto>();

            var authors = new List<AuthorDetailsDto>(bookDto.Authors.Count);
            var authorKeys = bookDto.Authors.Select(x => x.Key).ToArray();
            foreach (var authorKey in authorKeys)
            {
                var authorDto = await Policy<AuthorDetailsDto>
                    .Handle<ApiException>(e => HttpStatusCodesWorthRetrying.Contains(e.StatusCode))
                    .RetryAsync(5) //TODO: From appsettings
                    .ExecuteAsync(() => _openLibraryApiClient.GetAuthor(authorKey, ct))
                    .ConfigureAwait(false);

                if (authorDto != null)
                    authors.Add(authorDto);
            }

            return authors;
        }

        private static DateTime? TryGetPublishDate(string publish_date)
        {
            DateTime.TryParseExact(publish_date, "MMMM d, yyyy", null, DateTimeStyles.None, out DateTime result);
            return result;
        }
    }
}
